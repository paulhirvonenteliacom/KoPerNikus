namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addCareCompanyKeyToAssistantClass : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Assistants", "CareCompanyId");
            AddForeignKey("dbo.Assistants", "CareCompanyId", "dbo.CareCompanies", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Assistants", "CareCompanyId", "dbo.CareCompanies");
            DropIndex("dbo.Assistants", new[] { "CareCompanyId" });
        }
    }
}
