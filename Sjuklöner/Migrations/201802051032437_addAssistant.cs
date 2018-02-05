namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addAssistant : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CareCompanies", "OmbudId", c => c.Int(nullable: false));
            DropColumn("dbo.CareCompanies", "ReferenceCode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CareCompanies", "ReferenceCode", c => c.String());
            DropColumn("dbo.CareCompanies", "OmbudId");
        }
    }
}
