using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Net.Http;
using System.Threading.Tasks;
using Bellwether.Configuration;
using Microsoft.Store.PartnerCenter.Models;
using Microsoft.Store.PartnerCenter.Models.Customers;
using Microsoft.Store.PartnerCenter.Enumerators;
using Bellwether.MpnApi;

namespace Bellwether.UsageBilling
{
	public static class GetCustomersFunction
	{
		[FunctionName("GetCustomers")]
#if DEBUG
		public static async Task RunAync([HttpTrigger(Route = "GetCustomers")]HttpRequestMessage req, TraceWriter log)
#else
		public static async Task RunAync([TimerTrigger("0 * 12 1/1 * *")]TimerInfo myTimer, TraceWriter log)
#endif
		{
			log.Info($"Get customers function execution started at {DateTime.UtcNow} UTC");
			try
			{
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

				SeekBasedResourceCollection<Customer> customers;
				IResourceCollectionEnumerator<SeekBasedResourceCollection<Customer>> enumerator = await mpnClient.GetCustomers();
				while (enumerator.HasValue)
				{
					customers = enumerator.Current;
					if (customers?.Items != null && customers?.TotalCount > 0)
					{
						ProcessCustomers(customers, log);
					}
					else
					{
						log.Info($"0 customers found");
					}
					await enumerator.NextAsync();
				}
				log.Info($"Finished processing customers");
			}
			catch (Exception ex)
			{
				log.Error("Some error occured in function - 'GetCustomers'", ex);
			}
			log.Info($"Get customers function execution completed at {DateTime.UtcNow} UTC");
		}

		private static void ProcessCustomers(SeekBasedResourceCollection<Customer> customers, TraceWriter log)
		{
			log.Verbose($"{customers.TotalCount } customers found");
			//Parallel.ForEach(customers.Items, (c) =>
			//{


			//});
		}
	}
}
