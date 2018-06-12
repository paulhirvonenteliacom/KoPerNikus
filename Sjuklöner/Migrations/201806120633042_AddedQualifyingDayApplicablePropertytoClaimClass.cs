namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedQualifyingDayApplicablePropertytoClaimClass : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Claims", "QualifyingDayApplicable", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Claims", "QualifyingDayApplicable");
        }
    }
}
