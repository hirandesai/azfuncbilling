namespace Bellwether.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAzureRateCardsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CspAzureRateCards",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MeterId = c.String(nullable: false, maxLength: 100),
                        MeterName = c.String(nullable: false, maxLength: 100),
                        RateKey = c.String(maxLength: 500),
                        RateValue = c.String(maxLength: 500),
                        Tags = c.String(maxLength: 255),
                        Category = c.String(maxLength: 100),
                        SubCategory = c.String(maxLength: 100),
                        Region = c.String(maxLength: 50),
                        Unit = c.String(maxLength: 50),
                        IncludedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EffectiveDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.MeterId, t.RateKey, t.RateValue, t.Region, t.EffectiveDate }, unique: true, name: "IX_CspAzureRateCard_MeterId_RateKey_RateVal_Region_EffDate");
			Sql("ALTER INDEX IX_CspAzureRateCard_MeterId_RateKey_RateVal_Region_EffDate ON dbo.CspAzureRateCards SET (IGNORE_DUP_KEY = ON);");

		}

		public override void Down()
        {
            DropIndex("dbo.CspAzureRateCards", "IX_CspAzureRateCard_MeterId_RateKey_RateVal_Region_EffDate");
            DropTable("dbo.CspAzureRateCards");
        }
    }
}
