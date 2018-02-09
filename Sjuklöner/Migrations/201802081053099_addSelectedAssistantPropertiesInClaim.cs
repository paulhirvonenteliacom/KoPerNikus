namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addSelectedAssistantPropertiesInClaim : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Claims", "SelectedRegAssistantId", c => c.Int());
            AddColumn("dbo.Claims", "SelectedSubAssistantId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Claims", "SelectedSubAssistantId");
            DropColumn("dbo.Claims", "SelectedRegAssistantId");
        }
    }
}
