namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newPropertiesForClaimTransferToProcapitaTimeStamps : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Claims", "BasisForDecisionTransferStartTimeStamp", c => c.DateTime(nullable: false));
            AddColumn("dbo.Claims", "BasisForDecisionTransferFinishTimeStamp", c => c.DateTime(nullable: false));
            AddColumn("dbo.Claims", "DecisionTransferTimeStamp", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Claims", "DecisionTransferTimeStamp");
            DropColumn("dbo.Claims", "BasisForDecisionTransferFinishTimeStamp");
            DropColumn("dbo.Claims", "BasisForDecisionTransferStartTimeStamp");
        }
    }
}
