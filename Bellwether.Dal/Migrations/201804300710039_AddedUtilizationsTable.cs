namespace Bellwether.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedUtilizationsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CspUtilizations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomerId = c.String(nullable: false, maxLength: 100),
                        SubscriptionId = c.String(nullable: false, maxLength: 100),
                        ResourceGuid = c.String(nullable: false, maxLength: 100),
                        ResourceName = c.String(maxLength: 255),
                        Category = c.String(maxLength: 100),
                        SubCategory = c.String(maxLength: 100),
                        Region = c.String(maxLength: 50),
                        UsageDateUtc = c.DateTime(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Unit = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CspUtilizations");
        }
    }
}
