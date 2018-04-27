namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedThreePropertiesAndDeletedOnePropertyInClaimClass : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Claims", "CreationDate", c => c.DateTime());
            AddColumn("dbo.Claims", "DecisionDate", c => c.DateTime());
            AddColumn("dbo.Claims", "SentInDate", c => c.DateTime());
            DropColumn("dbo.Claims", "DeadlineDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Claims", "DeadlineDate", c => c.DateTime());
            DropColumn("dbo.Claims", "SentInDate");
            DropColumn("dbo.Claims", "DecisionDate");
            DropColumn("dbo.Claims", "CreationDate");
        }
    }
}
