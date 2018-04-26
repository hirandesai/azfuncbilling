using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bellwether.Configuration;
using Bellwether.MpnApi;
using Bellwether.StorageClient;
using Bellwether.StorageClient.MessageFormats;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Store.PartnerCenter.Models;
using Microsoft.Store.PartnerCenter.Models.Subscriptions;

namespace Bellwether.UsageBilling
{
	public static class GetSubscriptionsFunction
	{
		[FunctionName("GetSubscriptions")]
#if DEBUG
		public static async Task RunAync([HttpTrigger(Route = "GetSubscriptions")]HttpRequestMessage req, TraceWriter log)
#else
				public static Task RunAync([QueueTrigger("customers")]CustomerMessage message, TraceWriter log)
#endif

		{
#if DEBUG
			var param = req.GetQueryNameValuePairs().FirstOrDefault(s => s.Key.Equals("customerid"));
			CustomerMessage message = new CustomerMessage()
			{
				CustomerId = param.Value
			};
#endif

			log.Info($"Get subscriptoins function execution started at {DateTime.UtcNow} UTC");
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
				var subscriptions = await mpnClient.GetSubscriptionsAsync(message.CustomerId);
				if (subscriptions != null)
				{
					await ProcessSubscriptions(subscriptions, message.CustomerId, log);
				}
				else
				{
					log.Verbose($"0 subscriptions found");
				}
				log.Info($"Finished processing customers");
			}
			catch (Exception ex)
			{
				log.Error("Some error occured in function - 'GetSubscriptions'", ex);
			}
			log.Info($"Get subscriptoins function execution completed at {DateTime.UtcNow} UTC");
		}
		private async static Task ProcessSubscriptions(ResourceCollection<Subscription> subscriptions, string CustomerId, TraceWriter log)
		{
			log.Verbose($"{subscriptions.TotalCount } subscriptions found");
			SubscriptionsQueueClient queueClient = new SubscriptionsQueueClient(ConfigurationHelper.GetAppSetting(ConfigurationKeys.StorageConnectoinString));
			foreach (var subscription in subscriptions.Items)
			{
				await queueClient.AddMessageAsync(new SubscriptionMessage() { CustomerId = CustomerId, SubscriptionId = subscription.Id });
			}
		}
	}
}
