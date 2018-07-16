namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedMoreThan10DayPropertyToClaimClass : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Claims", "MoreThan10SickleavePeriods", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Claims", "MoreThan10SickleavePeriods");
        }
    }
}
