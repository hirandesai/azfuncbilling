using Microsoft.Azure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bellwether.Configuration
{
	public class ConfigurationHelper
	{
		public static int GetAppSettingAsInt(string key, int defaultValue = 0)
		{
			var value = GetAppSetting(key);
			if (value == null)
			{
				return defaultValue;
			}

			int result;
			if (int.TryParse(value, out result))
			{
				return result;
			}
			return defaultValue;
		}

		public static bool GetAppSettingAsBool(string key, bool defaultValue = false)
		{
			var value = GetAppSetting(key);
			if (value == null)
			{
				return defaultValue;
			}

			bool result;
			if (bool.TryParse(value, out result))
			{
				return result;
			}

			int result2;
			if (int.TryParse(value, out result2))
			{
				return result2 == 1;
			}
			return defaultValue;
		}

		public static string GetAppSetting(string key)
		{
			return CloudConfigurationManager.GetSetting(key, false);
		}
		public static string GetConnectionString(string key)
		{
			var value = GetAppSetting(key);
			if (string.IsNullOrEmpty(value))
				value = ConfigurationManager.ConnectionStrings[key].ConnectionString;

			return value;
		}
	}
}
