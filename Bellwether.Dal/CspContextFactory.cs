using Bellwether.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bellwether.Dal
{
	public class CspContextFactory : IDbContextFactory<CspContext>
	{
		public CspContext Create()
		{
			var connectionString = ConfigurationHelper.GetAppSetting(ConfigurationKeys.DbConnectoinString);
			return connectionString != null ? new CspContext(connectionString) : new CspContext();
		}
	}
}
