namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addFirstAssistanceDateAndLastAssistanceDateInClaim : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Claims", "LastAssistanceDate", c => c.String());
            AddColumn("dbo.Claims", "FirstAssistanceDate", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Claims", "FirstAssistanceDate");
            DropColumn("dbo.Claims", "LastAssistanceDate");
        }
    }
}
