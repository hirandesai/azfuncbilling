using Bellwether.Configuration;
using Microsoft.Store.PartnerCenter;
using Microsoft.Store.PartnerCenter.Enumerators;
using Microsoft.Store.PartnerCenter.Extensions;
using Microsoft.Store.PartnerCenter.Models;
using Microsoft.Store.PartnerCenter.Models.Customers;
using Microsoft.Store.PartnerCenter.Models.Query;
using Microsoft.Store.PartnerCenter.Models.RateCards;
using Microsoft.Store.PartnerCenter.Models.Subscriptions;
using Microsoft.Store.PartnerCenter.Models.Utilizations;
using Microsoft.Store.PartnerCenter.RequestContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bellwether.MpnApi
{
	public class MpnApiClient
	{
		private MpnApiClient() { }
		private IAggregatePartner ApiCaller { get; set; }
		public static async Task<MpnApiClient> CreateAsync(string PartnerServiceApiRoot, string Authority, string ResourceUrl, string ApplicationId, string ApplicationSecret, string ApplicationDomain)
		{
			MpnApiClient client = new MpnApiClient();
			PartnerService.Instance.ApiRootUrl = ConfigurationHelper.GetAppSetting(ConfigurationKeys.MPN.PartnerServiceApiRoot);
			var partnerCredentials = await PartnerCredentials.Instance.GenerateByApplicationCredentialsAsync(ApplicationId, ApplicationSecret, ApplicationDomain, Authority, ResourceUrl);
			client.ApiCaller = PartnerService.Instance.CreatePartnerOperations(partnerCredentials);
			return client;
		}

		public async Task<IResourceCollectionEnumerator<SeekBasedResourceCollection<Customer>>> GetCustomersAsync(int RecordsToFetch = 100)
		{
			IPartner scopedPartnerOperations = ApiCaller.With(RequestContextFactory.Instance.Create(Guid.NewGuid()));
			//var fieldFilter = new SimpleFieldFilter(CustomerSearchField.CompanyName.ToString(), FieldFilterOperation.StartsWith, "Elect");
			//var customersBatch = await scopedPartnerOperations.Customers.QueryAsync(QueryFactory.Instance.BuildIndexedQuery(RecordsToFetch, filter: fieldFilter));
			var customersBatch = await scopedPartnerOperations.Customers.QueryAsync(QueryFactory.Instance.BuildIndexedQuery(RecordsToFetch));
			return scopedPartnerOperations.Enumerators.Customers.Create(customersBatch);
		}

		public async Task<ResourceCollection<Subscription>> GetSubscriptionsAsync(string CustomerId)
		{
			return await ApiCaller.Customers.ById(CustomerId).Subscriptions.GetAsync();
		}

		public async Task<IResourceCollectionEnumerator<ResourceCollection<AzureUtilizationRecord>>> GetUtilizationssAsync(string CustomerId, string SubscriptionId, DateTimeOffset StartTime, DateTimeOffset EndTime)
		{
			var utilizationRecords = await ApiCaller.Customers.ById(CustomerId)
														.Subscriptions.ById(SubscriptionId)
														.Utilization
														.Azure
														.QueryAsync(StartTime, EndTime, AzureUtilizationGranularity.Hourly);

			return ApiCaller.Enumerators.Utilization.Azure.Create(utilizationRecords);
		}
		public async Task<AzureRateCard> GetAzureRateCardAsync(string Currency, string Region)
		{
			return await ApiCaller.RateCards.Azure.GetAsync(Currency, Region);
		}
	}
}
