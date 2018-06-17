namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedSickDayDateStringPropertiesToClaim : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Claims", "QualifyingDayDate", c => c.String());
            AddColumn("dbo.Claims", "Day2OfSicknessDate", c => c.String());
            AddColumn("dbo.Claims", "Day14OfSicknessDate", c => c.String());
            AddColumn("dbo.Claims", "Day15OfSicknessDate", c => c.String());
            AddColumn("dbo.Claims", "LastDayofSicknessDate", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Claims", "LastDayofSicknessDate");
            DropColumn("dbo.Claims", "Day15OfSicknessDate");
            DropColumn("dbo.Claims", "Day14OfSicknessDate");
            DropColumn("dbo.Claims", "Day2OfSicknessDate");
            DropColumn("dbo.Claims", "QualifyingDayDate");
        }
    }
}
