namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedDecisionContentToClaim : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Claims", "DecisionContent", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Claims", "DecisionContent");
        }
    }
}
