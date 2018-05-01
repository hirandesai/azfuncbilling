using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bellwether.Configuration;
using Bellwether.Dal;
using Bellwether.Dal.Entities;
using Bellwether.MpnApi;
using Bellwether.StorageClient.MessageFormats;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Store.PartnerCenter.Enumerators;
using Microsoft.Store.PartnerCenter.Models;
using Microsoft.Store.PartnerCenter.Models.Utilizations;

namespace Bellwether.UsageBilling
{
	public static class GetUtilizationsFunction
	{
		[FunctionName("GetUtilizations")]
		public static async Task RunAync([QueueTrigger("subscriptions")]SubscriptionMessage message, TraceWriter log)

//#if DEBUG
//		public static async Task RunAync([HttpTrigger(Route = "GetUtilizations")]HttpRequestMessage req, TraceWriter log)
//#else
//		public static async Task RunAync([QueueTrigger("subscriptions")]SubscriptionMessage message, TraceWriter log)
//#endif

		{
//#if DEBUG
//			var customerId = req.GetQueryNameValuePairs().FirstOrDefault(s => s.Key.Equals("customerid"));
//			var subscriptionId = req.GetQueryNameValuePairs().FirstOrDefault(s => s.Key.Equals("subscriptionid"));
//			SubscriptionMessage message = new SubscriptionMessage()
//			{
//				CustomerId = customerId.Value,
//				SubscriptionId = subscriptionId.Value
//			};
//#endif
			log.Info($"Get utilizations function execution started at {DateTime.UtcNow} UTC");
			try
			{
				log.Info($"Database initialization started.");
				DbInitializer.init(ConfigurationHelper.GetConnectionString(ConfigurationKeys.DbConnectoinString));
				log.Info($"Database initialization completed.");

				string partnerServiceApiRoot = ConfigurationHelper.GetAppSetting(ConfigurationKeys.MPN.PartnerServiceApiRoot),
									authority = ConfigurationHelper.GetAppSetting(ConfigurationKeys.MPN.Authority),
									resourceUrl = ConfigurationHelper.GetAppSetting(ConfigurationKeys.MPN.ResourceUrl),
									applicationId = ConfigurationHelper.GetAppSetting(ConfigurationKeys.MPN.ApplicationId),
									applicationSecret = ConfigurationHelper.GetAppSetting(ConfigurationKeys.MPN.ApplicationSecret),
									applicationDomian = ConfigurationHelper.GetAppSetting(ConfigurationKeys.MPN.ApplicationDomain);

				log.Info($"Partner Service Api Root {partnerServiceApiRoot}");
				log.Info($"Authority is {authority}");
				log.Info($"Resource URL is {resourceUrl}");
				log.Info($"Application Id is {applicationId}");
				log.Info($"Application Secret is {new string('*', applicationSecret.Length)}");
				log.Info($"Application Domain is {applicationDomian}");

				log.Info($"Connecting to MPN network");
				MpnApiClient mpnClient = await MpnApiClient.CreateAsync(partnerServiceApiRoot
												, authority
												, resourceUrl
												, applicationId
												, applicationSecret
												, applicationDomian);
				log.Info($"Connected to MPN network");
				DateTimeOffset start = DateTimeOffset.UtcNow.Date.AddDays(-2),
								end = DateTimeOffset.UtcNow.Date.AddDays(-1);

				ResourceCollection<AzureUtilizationRecord> utilizations;
				IResourceCollectionEnumerator<ResourceCollection<AzureUtilizationRecord>> utilizationEnumerator = await mpnClient.GetUtilizationssAsync(message.CustomerId, message.SubscriptionId, start, end);
				if (utilizationEnumerator != null)
				{
					while (utilizationEnumerator.HasValue)
					{
						utilizations = utilizationEnumerator.Current;
						if (utilizations?.Items != null && utilizations?.TotalCount > 0)
						{
							ProcessUtilizations(utilizations, message.CustomerId, message.SubscriptionId, log);
						}
						else
						{
							log.Info($"0 utilizations found");
						}
						log.Verbose($"Fetching next page");
						await utilizationEnumerator.NextAsync();
						log.Verbose($"Next page retrived");
					}
				}
				log.Info($"Finished processing utilizations");
			}
			catch (Exception ex)
			{
				log.Error("Some error occured in function - 'GetSubscriptions'", ex);
			}
			log.Info($"Get utilizations function execution completed at {DateTime.UtcNow} UTC");
		}
		private static void ProcessUtilizations(ResourceCollection<AzureUtilizationRecord> utilizations, string CustomerId, string SubscriptionId, TraceWriter log)
		{
			log.Info($"{utilizations.TotalCount } utilizations found");
			log.Info($"Inserting Utilizaton data into database");
			DumpUtility blkOperation = new DumpUtility(ConfigurationHelper.GetConnectionString(ConfigurationKeys.DbConnectoinString));
			blkOperation.Insert<CspUtilization>(utilizations.Items
												.Select(s => new CspUtilization()
												{
													CustomerId = CustomerId,
													SubscriptionId = SubscriptionId,
													ResourceGuid = s.Resource?.Id,
													ResourceName = s.Resource?.Name,
													Category = s.Resource?.Category,
													SubCategory = s.Resource?.Subcategory,
													Region = s.Resource?.Region,
													UsageDateUtc = s.UsageStartTime.UtcDateTime,
													Quantity = s.Quantity,
													Unit = s.Unit
												}).ToList());
			log.Info($"Database operation completed.");
		}
	}
}
