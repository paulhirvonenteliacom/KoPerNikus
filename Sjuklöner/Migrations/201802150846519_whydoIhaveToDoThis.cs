namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class whydoIhaveToDoThis : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Documents", "OwnerId", "dbo.AspNetUsers");
            DropIndex("dbo.Documents", new[] { "OwnerId" });
            CreateTable(
                "dbo.DefaultCollectiveAgreementInfoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CollectiveAgreementHeaderId = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        PerHourUnsocialEvening = c.String(nullable: false),
                        PerHourUnsocialNight = c.String(nullable: false),
                        PerHourUnsocialWeekend = c.String(nullable: false),
                        PerHourUnsocialHoliday = c.String(nullable: false),
                        PerHourOnCallWeekday = c.String(nullable: false),
                        PerHourOnCallWeekend = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Assistants", "PensionAndInsuranceRate", c => c.String());
            AddColumn("dbo.Claims", "DefaultCollectiveAgreement", c => c.Boolean(nullable: false));
            AddColumn("dbo.Claims", "CompanyName", c => c.String());
            AddColumn("dbo.Claims", "StreetAddress", c => c.String());
            AddColumn("dbo.Claims", "Postcode", c => c.String());
            AddColumn("dbo.Claims", "City", c => c.String());
            AddColumn("dbo.Claims", "AccountNumber", c => c.String());
            AddColumn("dbo.Claims", "CompanyPhoneNumber", c => c.String());
            AddColumn("dbo.Claims", "CollectiveAgreementName", c => c.String());
            AddColumn("dbo.Claims", "CollectiveAgreementSpecName", c => c.String());
            AddColumn("dbo.Claims", "OmbudFirstName", c => c.String());
            AddColumn("dbo.Claims", "OmbudLastName", c => c.String());
            AddColumn("dbo.Claims", "OmbudPhoneNumber", c => c.String());
            AddColumn("dbo.Claims", "OmbudEmail", c => c.String());
            AddColumn("dbo.Claims", "CustomerName", c => c.String(nullable: false));
            AddColumn("dbo.Claims", "CustomerAddress", c => c.String(nullable: false));
            AddColumn("dbo.Claims", "CustomerPhoneNumber", c => c.String(nullable: false));
            AddColumn("dbo.Claims", "RegAssistantSSN", c => c.String());
            AddColumn("dbo.Claims", "RegFirstName", c => c.String());
            AddColumn("dbo.Claims", "RegLastName", c => c.String());
            AddColumn("dbo.Claims", "RegEmail", c => c.String());
            AddColumn("dbo.Claims", "RegPhoneNumber", c => c.String());
            AddColumn("dbo.Claims", "SubAssistantSSN", c => c.String());
            AddColumn("dbo.Claims", "SubFirstName", c => c.String());
            AddColumn("dbo.Claims", "SubLastName", c => c.String());
            AddColumn("dbo.Claims", "SubEmail", c => c.String());
            AddColumn("dbo.Claims", "SubPhoneNumber", c => c.String());
            DropColumn("dbo.Assistants", "InsuranceRate");
            DropColumn("dbo.Assistants", "PensionRate");
            DropColumn("dbo.Claims", "CustomerFirstName");
            DropColumn("dbo.Claims", "CustomerLastName");
            DropColumn("dbo.Claims", "AssistantSSN");
            DropColumn("dbo.Claims", "Email");
            DropColumn("dbo.Documents", "OwnerId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Documents", "OwnerId", c => c.String(maxLength: 128));
            AddColumn("dbo.Claims", "Email", c => c.String());
            AddColumn("dbo.Claims", "AssistantSSN", c => c.String());
            AddColumn("dbo.Claims", "CustomerLastName", c => c.String());
            AddColumn("dbo.Claims", "CustomerFirstName", c => c.String());
            AddColumn("dbo.Assistants", "PensionRate", c => c.String());
            AddColumn("dbo.Assistants", "InsuranceRate", c => c.String());
            DropColumn("dbo.Claims", "SubPhoneNumber");
            DropColumn("dbo.Claims", "SubEmail");
            DropColumn("dbo.Claims", "SubLastName");
            DropColumn("dbo.Claims", "SubFirstName");
            DropColumn("dbo.Claims", "SubAssistantSSN");
            DropColumn("dbo.Claims", "RegPhoneNumber");
            DropColumn("dbo.Claims", "RegEmail");
            DropColumn("dbo.Claims", "RegLastName");
            DropColumn("dbo.Claims", "RegFirstName");
            DropColumn("dbo.Claims", "RegAssistantSSN");
            DropColumn("dbo.Claims", "CustomerPhoneNumber");
            DropColumn("dbo.Claims", "CustomerAddress");
            DropColumn("dbo.Claims", "CustomerName");
            DropColumn("dbo.Claims", "OmbudEmail");
            DropColumn("dbo.Claims", "OmbudPhoneNumber");
            DropColumn("dbo.Claims", "OmbudLastName");
            DropColumn("dbo.Claims", "OmbudFirstName");
            DropColumn("dbo.Claims", "CollectiveAgreementSpecName");
            DropColumn("dbo.Claims", "CollectiveAgreementName");
            DropColumn("dbo.Claims", "CompanyPhoneNumber");
            DropColumn("dbo.Claims", "AccountNumber");
            DropColumn("dbo.Claims", "City");
            DropColumn("dbo.Claims", "Postcode");
            DropColumn("dbo.Claims", "StreetAddress");
            DropColumn("dbo.Claims", "CompanyName");
            DropColumn("dbo.Claims", "DefaultCollectiveAgreement");
            DropColumn("dbo.Assistants", "PensionAndInsuranceRate");
            DropTable("dbo.DefaultCollectiveAgreementInfoes");
            CreateIndex("dbo.Documents", "OwnerId");
            AddForeignKey("dbo.Documents", "OwnerId", "dbo.AspNetUsers", "Id");
        }
    }
}
