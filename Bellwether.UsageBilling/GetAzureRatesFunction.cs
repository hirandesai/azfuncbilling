using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bellwether.Configuration;
using Bellwether.Dal;
using Bellwether.Dal.Entities;
using Bellwether.MpnApi;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Store.PartnerCenter.Models.RateCards;

namespace Bellwether.UsageBilling
{
	public static class GetAzureRatesFunction
	{
		[FunctionName("GetAzureRates")]

#if DEBUG
		public static async Task RunAync([HttpTrigger(Route = "GetAzureRates")]HttpRequestMessage req, TraceWriter log)
#else
		// CRON expressions format:: {second} {minute} {hour} {day} {month} {day-of-week}
		public static async Task RunAync([TimerTrigger("0 0 10 1/1 * *")]TimerInfo myTimer, TraceWriter log)
#endif
		{
			log.Info($"Get azure rate function execution started at {DateTime.UtcNow} UTC");
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
									applicationDomian = ConfigurationHelper.GetAppSetting(ConfigurationKeys.MPN.ApplicationDomain),
									RateCardCurrancy = ConfigurationHelper.GetAppSetting(ConfigurationKeys.AzureRatesApi.Currancy),
									RateCardRegion = ConfigurationHelper.GetAppSetting(ConfigurationKeys.AzureRatesApi.Region);

				log.Verbose($"Partner Service Api Root {partnerServiceApiRoot}");
				log.Verbose($"Authority is {authority}");
				log.Verbose($"Resource URL is {resourceUrl}");
				log.Verbose($"Application Id is {applicationId}");
				log.Verbose($"Application Secret is {new string('*', applicationSecret.Length)}");
				log.Verbose($"Application Domain is {applicationDomian}");

				log.Info($"Connecting to MPN network");
				MpnApiClient mpnClient = await MpnApiClient.CreateAsync(partnerServiceApiRoot
												, authority
												, resourceUrl
												, applicationId
												, applicationSecret
												, applicationDomian);
				log.Info($"Connected to MPN network");

				AzureRateCard rateCard = await mpnClient.GetAzureRateCardAsync(RateCardCurrancy, RateCardRegion);
				if (rateCard != null)
				{
					ProcessRateCard(rateCard, log);

				}
				log.Info($"Finished processing rate card");
			}
			catch (Exception ex)
			{
				log.Error("Some error occured in function - 'GetAzureRates'", ex);
			}
			log.Info($"Get azure rate function execution completed at {DateTime.UtcNow} UTC");
		}
		private static void ProcessRateCard(AzureRateCard rateCard, TraceWriter log)
		{
			if (rateCard.Meters != null)
			{
				log.Info($"Inserting rate card data into database");
				DumpUtility blkOperation = new DumpUtility(ConfigurationHelper.GetConnectionString(ConfigurationKeys.DbConnectoinString));
				//Sometimes effective data is returned incorrect such as 01/01/0001 which is not a valid date for SQL and throws exception as below
				//System.Data: SqlDateTime overflow. Must be between 1/1/1753 12:00:00 AM and 12/31/9999 11:59:59 PM
				// Hence to support this data, the date is changed to 1/1/1753
				blkOperation.Insert<CspAzureRateCard>(rateCard.Meters.Select(s => new CspAzureRateCard()
				{
					MeterId = s.Id,
					MeterName = s.Name,
					RateKey = string.Join(",", s.Rates.Keys),
					RateValue = string.Join(",", s.Rates.Values),
					Tags = string.Join(",", s.Tags),
					Category = s.Category,
					SubCategory = s.Subcategory,
					Region = s.Region,
					Unit = s.Unit,
					IncludedQuantity = s.IncludedQuantity,
					EffectiveDate = s.EffectiveDate.Year < 1753 ? new DateTime(1753, 1, 1) : s.EffectiveDate
				}).ToList());
				log.Info($"Database operation completed.");
			}
		}
	}
}