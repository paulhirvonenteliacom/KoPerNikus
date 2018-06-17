namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPropertyWellToClaimDays : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClaimDays", "Well", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ClaimDays", "Well");
        }
    }
}
