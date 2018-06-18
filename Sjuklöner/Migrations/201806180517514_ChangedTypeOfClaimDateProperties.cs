namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedTypeOfClaimDateProperties : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Claims", "QualifyingDayDate", c => c.DateTime());
            AlterColumn("dbo.Claims", "Day2OfSicknessDate", c => c.DateTime());
            AlterColumn("dbo.Claims", "Day14OfSicknessDate", c => c.DateTime());
            AlterColumn("dbo.Claims", "Day15OfSicknessDate", c => c.DateTime());
            AlterColumn("dbo.Claims", "LastDayofSicknessDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Claims", "LastDayofSicknessDate", c => c.String());
            AlterColumn("dbo.Claims", "Day15OfSicknessDate", c => c.String());
            AlterColumn("dbo.Claims", "Day14OfSicknessDate", c => c.String());
            AlterColumn("dbo.Claims", "Day2OfSicknessDate", c => c.String());
            AlterColumn("dbo.Claims", "QualifyingDayDate", c => c.String());
        }
    }
}
