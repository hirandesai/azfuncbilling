namespace Bellwether.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CspCustomers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TenantId = c.Guid(nullable: false),
                        Domain = c.String(maxLength: 50),
                        CompanyName = c.String(maxLength: 50),
                        Relationship = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.Id, t.TenantId }, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.CspCustomers", new[] { "Id", "TenantId" });
            DropTable("dbo.CspCustomers");
        }
    }
}
