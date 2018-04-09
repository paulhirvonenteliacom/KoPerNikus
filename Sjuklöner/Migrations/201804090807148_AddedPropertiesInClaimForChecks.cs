namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPropertiesInClaimForChecks : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Claims", "IVOCheckMsg", c => c.String());
            AddColumn("dbo.Claims", "AssistanceCheckMsg", c => c.String());
            AddColumn("dbo.Claims", "ProxyCheckMsg", c => c.String());
            AddColumn("dbo.Claims", "RejectReason", c => c.String());
            DropColumn("dbo.Claims", "IVOCheckResult");
            DropColumn("dbo.Claims", "ProxyCheckResult");
            DropColumn("dbo.Claims", "AssistanceCheckResult");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Claims", "AssistanceCheckResult", c => c.String());
            AddColumn("dbo.Claims", "ProxyCheckResult", c => c.String());
            AddColumn("dbo.Claims", "IVOCheckResult", c => c.String());
            DropColumn("dbo.Claims", "RejectReason");
            DropColumn("dbo.Claims", "ProxyCheckMsg");
            DropColumn("dbo.Claims", "AssistanceCheckMsg");
            DropColumn("dbo.Claims", "IVOCheckMsg");
        }
    }
}
