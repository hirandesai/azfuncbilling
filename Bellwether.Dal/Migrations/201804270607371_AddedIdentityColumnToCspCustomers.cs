namespace Bellwether.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIdentityColumnToCspCustomers : DbMigration
    {
        public override void Up()
        {			
			DropIndex("dbo.CspCustomers", new[] { "Id", "TenantId" });
			DropPrimaryKey("dbo.CspCustomers");
			DropColumn("dbo.CspCustomers", "Id");
			AddColumn("dbo.CspCustomers", "CustomerId", c => c.Guid(nullable: false));
            AddColumn("dbo.CspCustomers", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.CspCustomers", "Id");
            CreateIndex("dbo.CspCustomers", new[] { "CustomerId", "TenantId" }, unique: true, name: "IX_CSPCustomer_CustomerId_TenantId");
			Sql("ALTER INDEX IX_CSPCustomer_CustomerId_TenantId ON dbo.CspCustomers SET (IGNORE_DUP_KEY = ON);");

        }
        
        public override void Down()
        {
            DropIndex("dbo.CspCustomers", "IX_CSPCustomer_CustomerId_TenantId");
            DropPrimaryKey("dbo.CspCustomers");
			DropColumn("dbo.CspCustomers", "Id");
			AddColumn("dbo.CspCustomers", "Id", c => c.Guid(nullable: false));
            DropColumn("dbo.CspCustomers", "CustomerId");
            AddPrimaryKey("dbo.CspCustomers", "Id");
            CreateIndex("dbo.CspCustomers", new[] { "Id", "TenantId" }, unique: true);
        }
    }
}
