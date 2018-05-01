using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bellwether.Configuration
{
	public class ConfigurationKeys
	{
		public const string DbConnectoinString = "DefaultConnection";
		public const string StorageConnectoinString = "AzureWebJobsStorage";
		public class MPN
		{
			public const string PartnerServiceApiRoot = "PartnerServiceApiRoot";
			public const string Authority = "Authority";
			public const string ResourceUrl = "ResourceUrl";
			public const string ApplicationId = "ApplicationId";
			public const string ApplicationSecret = "ApplicationSecret";
			public const string ApplicationDomain = "ApplicationDomain";
		}
		public class AzureRatesApi
		{
			public const string Currancy = "AzureRateCurrancy";
			public const string Region = "AzureRateRegion";
		}
	}
}
