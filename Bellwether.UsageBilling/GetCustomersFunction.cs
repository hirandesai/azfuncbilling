using System;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Net.Http;
using System.Threading.Tasks;
using Bellwether.Configuration;
using Microsoft.Store.PartnerCenter.Models;
using Microsoft.Store.PartnerCenter.Models.Customers;
using Microsoft.Store.PartnerCenter.Enumerators;
using Bellwether.MpnApi;
using Bellwether.StorageClient;
using Bellwether.StorageClient.MessageFormats;
using Bellwether.Dal;
using Bellwether.Dal.Entities;

namespace Bellwether.UsageBilling
{
	public static class GetCustomersFunction
	{
		[FunctionName("GetCustomers")]
#if DEBUG
		public static async Task RunAync([HttpTrigger(Route = "GetCustomers")]HttpRequestMessage req, TraceWriter log)
#else
		public static async Task RunAync([TimerTrigger("0 0 10 1/1 * *")]TimerInfo myTimer, TraceWriter log)
#endif
		{
			log.Info($"Get customers function execution started at {DateTime.UtcNow} UTC");
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

				SeekBasedResourceCollection<Customer> customers;
				IResourceCollectionEnumerator<SeekBasedResourceCollection<Customer>> enumerator = await mpnClient.GetCustomersAsync();
				if (enumerator != null)
				{
					while (enumerator.HasValue)
					{
						customers = enumerator.Current;
						if (customers?.Items != null && customers?.TotalCount > 0)
						{
							await ProcessCustomers(customers, log);
						}
						else
						{
							log.Info($"0 customers found");
						}
						log.Verbose($"Fetching next page");
						await enumerator.NextAsync();
						log.Verbose($"Next page retrived");
					}
				}
				log.Info($"Finished processing customers");
			}
			catch (Exception ex)
			{
				log.Error("Some error occured in function - 'GetCustomers'", ex);
			}
			log.Info($"Get customers function execution completed at {DateTime.UtcNow} UTC");
		}

		private async static Task ProcessCustomers(SeekBasedResourceCollection<Customer> customers, TraceWriter log)
		{
			log.Info($"{customers.TotalCount } customers found");
			log.Info($"Inserting Customer data into database");
			DumpUtility blkOperation = new DumpUtility(ConfigurationHelper.GetConnectionString(ConfigurationKeys.DbConnectoinString));
			blkOperation.Insert<CspCustomer>(customers.Items
												.Select(s => new CspCustomer()
												{
													CustomerId = s.Id,
													TenantId = s.CompanyProfile != null ? s.CompanyProfile.TenantId : string.Empty,
													CompanyName = s.CompanyProfile?.CompanyName,
													Domain = s.CompanyProfile.Domain,
													Relationship = s.RelationshipToPartner.ToString()
												}).ToList());
			log.Info($"Database operation completed. Adding messages to queue");
			CustomersQueueClient queueClient = new CustomersQueueClient(ConfigurationHelper.GetAppSetting(ConfigurationKeys.StorageConnectoinString));
			foreach (var customer in customers.Items)
			{
				try
				{
					log.Verbose($"Processing customer - {customer.Id }, Relationship {customer.RelationshipToPartner}");
					log.Verbose($"Customer {customer.Id }");
					if (customer.RelationshipToPartner != CustomerPartnerRelationship.Advisor)
					{
						await queueClient.AddMessageAsync(new CustomerMessage() { CustomerId = customer.Id });
					}
				}
				catch (Exception ex)
				{
					log.Error($"Some error occured for customer - {customer.Id}, Relationship {customer.RelationshipToPartner}", ex);
				}
			}
			log.Info($"Message are added to queue.");
		}
	}
}
