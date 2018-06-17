namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedNamesOfQualifyingAndLastDayOfSicknessDateAsStringPropertiesInClaim : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Claims", "FirstClaimDateAsString", c => c.String());
            AddColumn("dbo.Claims", "LastClaimDateAsString", c => c.String());
            DropColumn("dbo.Claims", "QualifyingDateAsString");
            DropColumn("dbo.Claims", "LastDayOfSicknessDateAsString");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Claims", "LastDayOfSicknessDateAsString", c => c.String());
            AddColumn("dbo.Claims", "QualifyingDateAsString", c => c.String());
            DropColumn("dbo.Claims", "LastClaimDateAsString");
            DropColumn("dbo.Claims", "FirstClaimDateAsString");
        }
    }
}
