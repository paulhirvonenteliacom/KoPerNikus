namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedStringPropertiesForRobotInClaimsClass : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Claims", "QualifyingDateAsString", c => c.String());
            AddColumn("dbo.Claims", "LastDayOfSicknessDateAsString", c => c.String());
            AddColumn("dbo.Claims", "SentInDateAsString", c => c.String());
            AddColumn("dbo.Claims", "ClaimedSumAsString", c => c.String());
            AddColumn("dbo.Claims", "ModelSumAsString", c => c.String());
            AddColumn("dbo.Claims", "ApprovedSumAsString", c => c.String());
            AddColumn("dbo.Claims", "RejectedSumAsString", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Claims", "RejectedSumAsString");
            DropColumn("dbo.Claims", "ApprovedSumAsString");
            DropColumn("dbo.Claims", "ModelSumAsString");
            DropColumn("dbo.Claims", "ClaimedSumAsString");
            DropColumn("dbo.Claims", "SentInDateAsString");
            DropColumn("dbo.Claims", "LastDayOfSicknessDateAsString");
            DropColumn("dbo.Claims", "QualifyingDateAsString");
        }
    }
}
