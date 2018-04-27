using Bellwether.Configuration;
using Bellwether.Dal.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bellwether.Dal
{
	[DbConfigurationType(typeof(CspContextConfiguration))]
	public class CspContext : DbContext
	{
		private static Dictionary<Type, string> tableNames = new Dictionary<Type, string>() {
			{ typeof(CspCustomer),"CspCustomers"}
		};

		public CspContext()
						: base(ConfigurationKeys.DbConnectoinString)
		{
			Database.SetInitializer(new CspContextInitializer());
		}
		public CspContext(string connectionString)
			: base(connectionString)
		{
			Database.SetInitializer(new CspContextInitializer());
		}

		public DbSet<CspCustomer> Customers { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<CspCustomer>().ToTable(CspContext.GetTableName(typeof(CspCustomer)));
			modelBuilder.Entity<CspCustomer>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			modelBuilder.Entity<CspCustomer>().Property(p => p.Domain).HasMaxLength(50);
			modelBuilder.Entity<CspCustomer>().Property(p => p.CompanyName).HasMaxLength(50);
			modelBuilder.Entity<CspCustomer>().Property(p => p.Relationship).HasMaxLength(50);

			modelBuilder.Entity<CspCustomer>()
				.HasIndex(p => new { p.CustomerId, p.TenantId })
				.HasName("IX_CSPCustomer_CustomerId_TenantId")
				.IsUnique();

			base.OnModelCreating(modelBuilder);
		}
		public static string GetTableName(Type tableType)
		{
			var value = string.Empty;
			if (!tableNames.TryGetValue(tableType, out value))
			{
				value = null;
			}
			return value;
		}
	}
}
