namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedSubAssistantSalarySpec : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Claims", "SalarySpecSubAssistantCheck");
            DropColumn("dbo.Claims", "SalarySpecSubAssistantCheckMsg");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Claims", "SalarySpecSubAssistantCheckMsg", c => c.String());
            AddColumn("dbo.Claims", "SalarySpecSubAssistantCheck", c => c.Boolean(nullable: false));
        }
    }
}
