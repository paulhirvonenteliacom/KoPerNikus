namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTransferPropertiesToClaim : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Claims", "BasisForDecision", c => c.Boolean(nullable: false));
            AddColumn("dbo.Claims", "BasisForDecisionMsg", c => c.String());
            AddColumn("dbo.Claims", "Decision", c => c.Boolean(nullable: false));
            AddColumn("dbo.Claims", "DecisionMsg", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Claims", "DecisionMsg");
            DropColumn("dbo.Claims", "Decision");
            DropColumn("dbo.Claims", "BasisForDecisionMsg");
            DropColumn("dbo.Claims", "BasisForDecision");
        }
    }
}
