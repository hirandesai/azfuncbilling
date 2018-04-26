using Bellwether.Configuration;
using Bellwether.Dal.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bellwether.Dal
{
	[DbConfigurationType(typeof(CspContextConfiguration))]
	public class CspContext : DbContext
	{
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
			modelBuilder.Entity<CspCustomer>().ToTable("CspCustomers");
			modelBuilder.Entity<CspCustomer>().Property(p => p.Domain).HasMaxLength(50);
			modelBuilder.Entity<CspCustomer>().Property(p => p.CompanyName).HasMaxLength(50);
			modelBuilder.Entity<CspCustomer>().Property(p => p.Relationship).HasMaxLength(50);

			modelBuilder.Entity<CspCustomer>()
				.HasIndex(p => new { p.Id, p.TenantId })
				.IsUnique();

			base.OnModelCreating(modelBuilder);
		}
	}
}
