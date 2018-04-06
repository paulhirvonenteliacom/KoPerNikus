namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class activeandpassiveCareCompanies : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CareCompanies", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CareCompanies", "IsActive");
        }
    }
}
