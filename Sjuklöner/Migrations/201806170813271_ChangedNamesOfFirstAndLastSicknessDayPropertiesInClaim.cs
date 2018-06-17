namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedNamesOfFirstAndLastSicknessDayPropertiesInClaim : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Claims", "FirstClaimDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Claims", "LastClaimDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.Claims", "QualifyingDate");
            DropColumn("dbo.Claims", "LastDayOfSicknessDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Claims", "LastDayOfSicknessDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Claims", "QualifyingDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.Claims", "LastClaimDate");
            DropColumn("dbo.Claims", "FirstClaimDate");
        }
    }
}
