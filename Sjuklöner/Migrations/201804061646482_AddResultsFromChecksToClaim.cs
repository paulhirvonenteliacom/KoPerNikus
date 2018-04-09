namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddResultsFromChecksToClaim : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Claims", "IVOCheckResult", c => c.String());
            AddColumn("dbo.Claims", "ProxyCheckResult", c => c.String());
            AddColumn("dbo.Claims", "AssistanceCheckResult", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Claims", "AssistanceCheckResult");
            DropColumn("dbo.Claims", "ProxyCheckResult");
            DropColumn("dbo.Claims", "IVOCheckResult");
        }
    }
}
