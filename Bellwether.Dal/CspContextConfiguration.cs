using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Bellwether.Dal
{
	public class CspContextConfiguration : DbConfiguration
	{
		private const string CallContextKey = "TransactionExecutionStrategy";

		private SqlAzureExecutionStrategy azureExecutionStrategy;

		private DefaultExecutionStrategy defaultExecutionStrategy;

		public CspContextConfiguration()
		{
			SetExecutionStrategy("System.Data.SqlClient", LoadExecutionStrategy);
		}

		private IDbExecutionStrategy LoadExecutionStrategy()
		{
			int? countRanTransactions = (int?)CallContext.LogicalGetData(CallContextKey);
			IDbExecutionStrategy result;
			if (countRanTransactions > 0)
			{
				if (defaultExecutionStrategy == null)
				{
					defaultExecutionStrategy = new DefaultExecutionStrategy();
				}
				result = defaultExecutionStrategy;
			}
			else
			{
				if (azureExecutionStrategy == null)
				{
					azureExecutionStrategy = new SqlAzureExecutionStrategy(3, TimeSpan.FromSeconds(30));
				}
				result = azureExecutionStrategy;
			}
			return result;
		}
	}
}
