namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removedRequiredOnAssistantClassProperties : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Assistants", "FirstName", c => c.String());
            AlterColumn("dbo.Assistants", "LastName", c => c.String());
            AlterColumn("dbo.Assistants", "AssistantSSN", c => c.String());
            AlterColumn("dbo.Assistants", "Email", c => c.String());
            AlterColumn("dbo.Assistants", "PhoneNumber", c => c.String());
            AlterColumn("dbo.Assistants", "HolidayPayRate", c => c.String());
            AlterColumn("dbo.Assistants", "PayrollTaxRate", c => c.String());
            AlterColumn("dbo.Assistants", "PensionAndInsuranceRate", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Assistants", "PensionAndInsuranceRate", c => c.String(nullable: false));
            AlterColumn("dbo.Assistants", "PayrollTaxRate", c => c.String(nullable: false));
            AlterColumn("dbo.Assistants", "HolidayPayRate", c => c.String(nullable: false));
            AlterColumn("dbo.Assistants", "PhoneNumber", c => c.String(nullable: false));
            AlterColumn("dbo.Assistants", "Email", c => c.String(nullable: false));
            AlterColumn("dbo.Assistants", "AssistantSSN", c => c.String(nullable: false));
            AlterColumn("dbo.Assistants", "LastName", c => c.String(nullable: false));
            AlterColumn("dbo.Assistants", "FirstName", c => c.String(nullable: false));
        }
    }
}
