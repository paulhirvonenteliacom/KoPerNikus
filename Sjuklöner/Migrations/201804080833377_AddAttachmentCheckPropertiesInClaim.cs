namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAttachmentCheckPropertiesInClaim : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Claims", "SalarySpecRegAssistantCheck", c => c.Boolean(nullable: false));
            AddColumn("dbo.Claims", "SalarySpecRegAssistantCheckMsg", c => c.String());
            AddColumn("dbo.Claims", "SalarySpecSubAssistantCheck", c => c.Boolean(nullable: false));
            AddColumn("dbo.Claims", "SalarySpecSubAssistantCheckMsg", c => c.String());
            AddColumn("dbo.Claims", "SickleaveNotificationCheck", c => c.Boolean(nullable: false));
            AddColumn("dbo.Claims", "SickleaveNotificationCheckMsg", c => c.String());
            AddColumn("dbo.Claims", "MedicalCertificateCheck", c => c.Boolean(nullable: false));
            AddColumn("dbo.Claims", "MedicalCertificateCheckMsg", c => c.String());
            AddColumn("dbo.Claims", "FKRegAssistantCheck", c => c.Boolean(nullable: false));
            AddColumn("dbo.Claims", "FKRegAssistantCheckMsg", c => c.String());
            AddColumn("dbo.Claims", "FKSubAssistantCheck", c => c.Boolean(nullable: false));
            AddColumn("dbo.Claims", "FKSubAssistantCheckMsg", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Claims", "FKSubAssistantCheckMsg");
            DropColumn("dbo.Claims", "FKSubAssistantCheck");
            DropColumn("dbo.Claims", "FKRegAssistantCheckMsg");
            DropColumn("dbo.Claims", "FKRegAssistantCheck");
            DropColumn("dbo.Claims", "MedicalCertificateCheckMsg");
            DropColumn("dbo.Claims", "MedicalCertificateCheck");
            DropColumn("dbo.Claims", "SickleaveNotificationCheckMsg");
            DropColumn("dbo.Claims", "SickleaveNotificationCheck");
            DropColumn("dbo.Claims", "SalarySpecSubAssistantCheckMsg");
            DropColumn("dbo.Claims", "SalarySpecSubAssistantCheck");
            DropColumn("dbo.Claims", "SalarySpecRegAssistantCheckMsg");
            DropColumn("dbo.Claims", "SalarySpecRegAssistantCheck");
        }
    }
}
