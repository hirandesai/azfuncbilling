namespace Bellwether.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedSubscriptionsTable : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.CspCustomers", "IX_CSPCustomer_CustomerId_TenantId");
            CreateTable(
                "dbo.CspSubscriptions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SubscriptionId = c.String(nullable: false, maxLength: 100),
                        CustomerId = c.String(nullable: false, maxLength: 100),
                        OfferId = c.String(maxLength: 100),
                        OfferName = c.String(maxLength: 100),
                        FriendlyName = c.String(maxLength: 100),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitType = c.String(maxLength: 20),
                        CreationDateUtc = c.DateTime(nullable: false),
                        EffectiveStartDateUtc = c.DateTime(nullable: false),
                        CommitmentEndDateUtc = c.DateTime(nullable: false),
                        Status = c.String(maxLength: 50),
                        AutoRenewEnabled = c.Boolean(nullable: false),
                        IsTrial = c.Boolean(nullable: false),
                        BillingType = c.String(maxLength: 50),
                        BillingCycle = c.String(maxLength: 50),
                        ContractType = c.String(maxLength: 50),
                        OrderId = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.CustomerId, t.SubscriptionId, t.OfferId, t.Quantity, t.OrderId }, unique: true, name: "IX_CspSubscription_CusId_SubId_OffId_Qty_OrdId");
			Sql("ALTER INDEX IX_CspSubscription_CusId_SubId_OffId_Qty_OrdId ON dbo.CspSubscriptions SET (IGNORE_DUP_KEY = ON);");

			AlterColumn("dbo.CspCustomers", "CustomerId", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.CspCustomers", "TenantId", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.CspCustomers", "Domain", c => c.String(maxLength: 100));
            AlterColumn("dbo.CspCustomers", "CompanyName", c => c.String(maxLength: 100));
            AlterColumn("dbo.CspCustomers", "Relationship", c => c.String(maxLength: 100));
            CreateIndex("dbo.CspCustomers", new[] { "CustomerId", "TenantId" }, unique: true, name: "IX_CSPCustomer_CustomerId_TenantId");
			Sql("ALTER INDEX IX_CSPCustomer_CustomerId_TenantId ON dbo.CspCustomers SET (IGNORE_DUP_KEY = ON);");
		}
        
        public override void Down()
        {
            DropIndex("dbo.CspSubscriptions", "IX_CspSubscription_CusId_SubId_OffId_Qty_OrdId");
            DropIndex("dbo.CspCustomers", "IX_CSPCustomer_CustomerId_TenantId");
            AlterColumn("dbo.CspCustomers", "Relationship", c => c.String(maxLength: 50));
            AlterColumn("dbo.CspCustomers", "CompanyName", c => c.String(maxLength: 50));
            AlterColumn("dbo.CspCustomers", "Domain", c => c.String(maxLength: 50));
            AlterColumn("dbo.CspCustomers", "TenantId", c => c.Guid(nullable: false));
            AlterColumn("dbo.CspCustomers", "CustomerId", c => c.Guid(nullable: false));
            DropTable("dbo.CspSubscriptions");
            CreateIndex("dbo.CspCustomers", new[] { "CustomerId", "TenantId" }, unique: true, name: "IX_CSPCustomer_CustomerId_TenantId");
        }
    }
}
