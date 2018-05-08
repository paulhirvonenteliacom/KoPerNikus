namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdjustedTransferString3 : DbMigration
    {
        public override void Up()
        {
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
        }
    }
}
