using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bellwether.Configuration;
using Bellwether.MpnApi;
using Bellwether.StorageClient.MessageFormats;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace Bellwether.UsageBilling
{
	public static class GetUtilizationsFunction
	{
		[FunctionName("GetUtilizations")]
#if DEBUG
		public static async Task RunAync([HttpTrigger(Route = "GetUtilizations")]HttpRequestMessage req, TraceWriter log)
#else
		public static async Task RunAync([QueueTrigger("subscriptions")]SubscriptionMessage message, TraceWriter log)
#endif
		{
#if DEBUG
			var customerId = req.GetQueryNameValuePairs().FirstOrDefault(s => s.Key.Equals("customerid"));
			var subscriptionId = req.GetQueryNameValuePairs().FirstOrDefault(s => s.Key.Equals("subscriptionid"));
			SubscriptionMessage message = new SubscriptionMessage()
			{
				CustomerId = customerId.Value,
				SubscriptionId = subscriptionId.Value
			};
#endif
			log.Info($"Get utilizations function execution started at {DateTime.UtcNow} UTC");
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

				

				log.Info($"Finished processing customers");
			}
			catch (Exception ex)
			{
				log.Error("Some error occured in function - 'GetSubscriptions'", ex);
			}
			log.Info($"Get utilizations function execution completed at {DateTime.UtcNow} UTC");
		}
	}
}
