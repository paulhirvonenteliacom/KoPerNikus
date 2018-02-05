namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modifyAssistant3 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Assistants",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CareCompanyId = c.Int(nullable: false),
                        FirstName = c.String(),
                        LastName = c.String(),
                        AssistantSSN = c.String(),
                        Email = c.String(),
                        PhoneNumber = c.String(),
                        HourlySalary = c.String(),
                        HolidayPayRate = c.String(),
                        PayrollTaxRate = c.String(),
                        InsuranceRate = c.String(),
                        PensionRate = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropColumn("dbo.AspNetUsers", "UserId");
            DropColumn("dbo.AspNetUsers", "Approved");
            DropColumn("dbo.AspNetUsers", "CompanyName");
            DropColumn("dbo.AspNetUsers", "StreetAddress");
            DropColumn("dbo.AspNetUsers", "Postcode");
            DropColumn("dbo.AspNetUsers", "City");
            DropColumn("dbo.AspNetUsers", "ClearingNumber");
            DropColumn("dbo.AspNetUsers", "AccountNumber");
            DropColumn("dbo.AspNetUsers", "HourlySalary");
            DropColumn("dbo.AspNetUsers", "HolidayPayRate");
            DropColumn("dbo.AspNetUsers", "PayrollTaxRate");
            DropColumn("dbo.AspNetUsers", "InsuranceRate");
            DropColumn("dbo.AspNetUsers", "PensionRate");
            DropColumn("dbo.AspNetUsers", "StorageApproval");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "StorageApproval", c => c.Boolean());
            AddColumn("dbo.AspNetUsers", "PensionRate", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.AspNetUsers", "InsuranceRate", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.AspNetUsers", "PayrollTaxRate", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.AspNetUsers", "HolidayPayRate", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.AspNetUsers", "HourlySalary", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.AspNetUsers", "AccountNumber", c => c.String());
            AddColumn("dbo.AspNetUsers", "ClearingNumber", c => c.String());
            AddColumn("dbo.AspNetUsers", "City", c => c.String());
            AddColumn("dbo.AspNetUsers", "Postcode", c => c.String());
            AddColumn("dbo.AspNetUsers", "StreetAddress", c => c.String());
            AddColumn("dbo.AspNetUsers", "CompanyName", c => c.String());
            AddColumn("dbo.AspNetUsers", "Approved", c => c.Boolean());
            AddColumn("dbo.AspNetUsers", "UserId", c => c.String());
            DropTable("dbo.Assistants");
        }
    }
}
