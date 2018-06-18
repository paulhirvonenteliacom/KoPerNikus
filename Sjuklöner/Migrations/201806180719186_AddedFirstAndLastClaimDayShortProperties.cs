namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedFirstAndLastClaimDayShortProperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Claims", "FirstClaimDayShort", c => c.String());
            AddColumn("dbo.Claims", "LastClaimDayShort", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Claims", "LastClaimDayShort");
            DropColumn("dbo.Claims", "FirstClaimDayShort");
        }
    }
}
