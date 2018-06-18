namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedDatePropertiesToClaim : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Claims", "QualifyingDayDateAsString", c => c.String());
            AddColumn("dbo.Claims", "Day2OfSicknessDateAsString", c => c.String());
            AddColumn("dbo.Claims", "Day14OfSicknessDateAsString", c => c.String());
            AddColumn("dbo.Claims", "Day15OfSicknessDateAsString", c => c.String());
            AddColumn("dbo.Claims", "LastDayOfSicknessDateAsString", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Claims", "LastDayOfSicknessDateAsString");
            DropColumn("dbo.Claims", "Day15OfSicknessDateAsString");
            DropColumn("dbo.Claims", "Day14OfSicknessDateAsString");
            DropColumn("dbo.Claims", "Day2OfSicknessDateAsString");
            DropColumn("dbo.Claims", "QualifyingDayDateAsString");
        }
    }
}
