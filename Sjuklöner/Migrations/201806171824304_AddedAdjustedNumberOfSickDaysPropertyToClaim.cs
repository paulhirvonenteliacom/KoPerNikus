namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAdjustedNumberOfSickDaysPropertyToClaim : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Claims", "AdjustedNumberOfSickDays", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Claims", "AdjustedNumberOfSickDays");
        }
    }
}
