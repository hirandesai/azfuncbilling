using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Bellwether.Dal
{
	public class CspContextInitializer : IDatabaseInitializer<CspContext>
	{
		public void InitializeDatabase(CspContext context)
		{
			var configuration = new Bellwether.Dal.Migrations.Configuration
			{
				TargetDatabase = new DbConnectionInfo(context.Database.Connection.ConnectionString, "System.Data.SqlClient")
			};
			var dbMigrator = new DbMigrator(configuration);
			dbMigrator.Update();
		}
	}
}
