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
			{ typeof(CspCustomer),"CspCustomers"},
			{ typeof(CspSubscription),"CspSubscriptions"},
			{ typeof(CspUtilization),"CspUtilizations"},
			{ typeof(CspAzureRateCard),"CspAzureRateCards"}
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
			modelBuilder.Entity<CspCustomer>().Property(p => p.CustomerId).HasMaxLength(100).IsRequired();
			modelBuilder.Entity<CspCustomer>().Property(p => p.TenantId).HasMaxLength(100).IsRequired();
			modelBuilder.Entity<CspCustomer>().Property(p => p.Domain).HasMaxLength(100);
			modelBuilder.Entity<CspCustomer>().Property(p => p.CompanyName).HasMaxLength(100);
			modelBuilder.Entity<CspCustomer>().Property(p => p.Relationship).HasMaxLength(100);

			modelBuilder.Entity<CspCustomer>()
				.HasIndex(p => new { p.CustomerId, p.TenantId })
				.HasName("IX_CSPCustomer_CustomerId_TenantId")
				.IsUnique();

			modelBuilder.Entity<CspSubscription>().ToTable(CspContext.GetTableName(typeof(CspSubscription)));
			modelBuilder.Entity<CspSubscription>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			modelBuilder.Entity<CspSubscription>().Property(p => p.SubscriptionId).HasMaxLength(100).IsRequired();
			modelBuilder.Entity<CspSubscription>().Property(p => p.CustomerId).HasMaxLength(100).IsRequired();
			modelBuilder.Entity<CspSubscription>().Property(p => p.OfferId).HasMaxLength(100);
			modelBuilder.Entity<CspSubscription>().Property(p => p.OfferName).HasMaxLength(100);
			modelBuilder.Entity<CspSubscription>().Property(p => p.FriendlyName).HasMaxLength(100);
			modelBuilder.Entity<CspSubscription>().Property(p => p.UnitType).HasMaxLength(20);
			modelBuilder.Entity<CspSubscription>().Property(p => p.Status).HasMaxLength(50);
			modelBuilder.Entity<CspSubscription>().Property(p => p.BillingType).HasMaxLength(50);
			modelBuilder.Entity<CspSubscription>().Property(p => p.BillingCycle).HasMaxLength(50);
			modelBuilder.Entity<CspSubscription>().Property(p => p.ContractType).HasMaxLength(50);
			modelBuilder.Entity<CspSubscription>().Property(p => p.OrderId).HasMaxLength(100);

			modelBuilder.Entity<CspSubscription>()
				.HasIndex(p => new { p.CustomerId, p.SubscriptionId, p.OfferId, p.Quantity, p.OrderId })
				.HasName("IX_CspSubscription_CusId_SubId_OffId_Qty_OrdId")
				.IsUnique();

			modelBuilder.Entity<CspUtilization>().ToTable(CspContext.GetTableName(typeof(CspUtilization)));
			modelBuilder.Entity<CspUtilization>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			modelBuilder.Entity<CspUtilization>().Property(p => p.CustomerId).HasMaxLength(100).IsRequired();
			modelBuilder.Entity<CspUtilization>().Property(p => p.SubscriptionId).HasMaxLength(100).IsRequired();
			modelBuilder.Entity<CspUtilization>().Property(p => p.ResourceGuid).HasMaxLength(100).IsRequired();
			modelBuilder.Entity<CspUtilization>().Property(p => p.ResourceName).HasMaxLength(255);
			modelBuilder.Entity<CspUtilization>().Property(p => p.Category).HasMaxLength(100);
			modelBuilder.Entity<CspUtilization>().Property(p => p.SubCategory).HasMaxLength(100);
			modelBuilder.Entity<CspUtilization>().Property(p => p.Region).HasMaxLength(50);
			modelBuilder.Entity<CspUtilization>().Property(p => p.Unit).HasMaxLength(50);

			modelBuilder.Entity<CspAzureRateCard>().ToTable(CspContext.GetTableName(typeof(CspAzureRateCard)));
			modelBuilder.Entity<CspAzureRateCard>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			modelBuilder.Entity<CspAzureRateCard>().Property(p => p.MeterId).HasMaxLength(100).IsRequired();
			modelBuilder.Entity<CspAzureRateCard>().Property(p => p.MeterName).HasMaxLength(100).IsRequired();
			modelBuilder.Entity<CspAzureRateCard>().Property(p => p.RateKey).HasMaxLength(500);
			modelBuilder.Entity<CspAzureRateCard>().Property(p => p.RateValue).HasMaxLength(500);
			modelBuilder.Entity<CspAzureRateCard>().Property(p => p.Tags).HasMaxLength(255);
			modelBuilder.Entity<CspAzureRateCard>().Property(p => p.Category).HasMaxLength(100);
			modelBuilder.Entity<CspAzureRateCard>().Property(p => p.SubCategory).HasMaxLength(100);
			modelBuilder.Entity<CspAzureRateCard>().Property(p => p.Region).HasMaxLength(50);
			modelBuilder.Entity<CspAzureRateCard>().Property(p => p.Unit).HasMaxLength(50);

			modelBuilder.Entity<CspAzureRateCard>()
				.HasIndex(p => new { p.MeterId, p.RateKey,p.RateValue, p.Region, p.EffectiveDate })
				.HasName("IX_CspAzureRateCard_MeterId_RateKey_RateVal_Region_EffDate")
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
