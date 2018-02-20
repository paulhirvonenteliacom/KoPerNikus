namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
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
                        PensionAndInsuranceRate = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CareCompanies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyName = c.String(nullable: false),
                        OrganisationNumber = c.String(nullable: false),
                        StreetAddress = c.String(nullable: false),
                        Postcode = c.String(nullable: false),
                        City = c.String(nullable: false),
                        AccountNumber = c.String(nullable: false),
                        CompanyPhoneNumber = c.String(nullable: false),
                        SelectedCollectiveAgreementId = c.Int(nullable: false),
                        CollectiveAgreementName = c.String(),
                        CollectiveAgreementSpecName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ClaimCalculations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReferenceNumber = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        NumberOfSickDays = c.Int(nullable: false),
                        ModelSum = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SickPayRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HolidayPayRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SocialFeeRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PensionAndInsuranceRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HourlySalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PerHourUnsocialEvening = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PerHourUnsocialNight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PerHourUnsocialWeekend = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PerHourUnsocialHoliday = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PerHourOnCallWeekday = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PerHourOnCallWeekend = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HourlySalaryAsString = c.String(),
                        PerHourUnsocialEveningAsString = c.String(),
                        PerHourUnsocialNightAsString = c.String(),
                        PerHourUnsocialWeekendAsString = c.String(),
                        PerHourUnsocialHolidayAsString = c.String(),
                        PerHourOnCallDayAsString = c.String(),
                        PerHourOnCallNightAsString = c.String(),
                        HoursQD = c.String(),
                        HolidayPayQD = c.String(),
                        HolidayPayCalcQD = c.String(),
                        SocialFeesQD = c.String(),
                        SocialFeesCalcQD = c.String(),
                        PensionAndInsuranceQD = c.String(),
                        PensionAndInsuranceCalcQD = c.String(),
                        CostQD = c.String(),
                        CostCalcQD = c.String(),
                        HoursD2T14 = c.String(),
                        SalaryD2T14 = c.String(),
                        SalaryCalcD2T14 = c.String(),
                        SickPayD2T14 = c.String(),
                        SickPayCalcD2T14 = c.String(),
                        HolidayPayD2T14 = c.String(),
                        HolidayPayCalcD2T14 = c.String(),
                        UnsocialEveningD2T14 = c.String(),
                        UnsocialEveningPayD2T14 = c.String(),
                        UnsocialEveningPayCalcD2T14 = c.String(),
                        UnsocialNightD2T14 = c.String(),
                        UnsocialNightPayD2T14 = c.String(),
                        UnsocialNightPayCalcD2T14 = c.String(),
                        UnsocialWeekendD2T14 = c.String(),
                        UnsocialWeekendPayD2T14 = c.String(),
                        UnsocialWeekendPayCalcD2T14 = c.String(),
                        UnsocialGrandWeekendD2T14 = c.String(),
                        UnsocialGrandWeekendPayD2T14 = c.String(),
                        UnsocialGrandWeekendPayCalcD2T14 = c.String(),
                        UnsocialSumD2T14 = c.String(),
                        UnsocialSumPayD2T14 = c.String(),
                        UnsocialSumPayCalcD2T14 = c.String(),
                        OnCallDayD2T14 = c.String(),
                        OnCallDayPayD2T14 = c.String(),
                        OnCallDayPayCalcD2T14 = c.String(),
                        OnCallNightD2T14 = c.String(),
                        OnCallNightPayD2T14 = c.String(),
                        OnCallNightPayCalcD2T14 = c.String(),
                        OnCallSumD2T14 = c.String(),
                        OnCallSumPayD2T14 = c.String(),
                        OnCallSumPayCalcD2T14 = c.String(),
                        SocialFeesD2T14 = c.String(),
                        SocialFeesCalcD2T14 = c.String(),
                        PensionAndInsuranceD2T14 = c.String(),
                        PensionAndInsuranceCalcD2T14 = c.String(),
                        CostD2T14 = c.String(),
                        CostCalcD2T14 = c.String(),
                        TotalCostD1T14 = c.String(),
                        TotalCostCalcD1T14 = c.String(),
                        ClaimStatus_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClaimStatus", t => t.ClaimStatus_Id)
                .Index(t => t.ClaimStatus_Id);
            
            CreateTable(
                "dbo.ClaimStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ClaimDays",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReferenceNumber = c.String(),
                        Date = c.DateTime(nullable: false),
                        ClaimDayDate = c.DateTime(),
                        DateString = c.String(),
                        CollectiveAgreementInfoId = c.Int(nullable: false),
                        SickDayNumber = c.Int(nullable: false),
                        Hours = c.String(),
                        UnsocialEvening = c.String(),
                        UnsocialNight = c.String(),
                        UnsocialWeekend = c.String(),
                        UnsocialGrandWeekend = c.String(),
                        OnCallDay = c.String(),
                        OnCallNight = c.String(),
                        HoursSI = c.String(),
                        UnsocialEveningSI = c.String(),
                        UnsocialNightSI = c.String(),
                        UnsocialWeekendSI = c.String(),
                        UnsocialGrandWeekendSI = c.String(),
                        OnCallDaySI = c.String(),
                        OnCallNightSI = c.String(),
                        PerHourUnsocialEvening = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PerHourUnsocialNight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PerHourUnsocialWeekend = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PerHourUnsocialHoliday = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PerHourOnCallWeekday = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PerHourOnCallWeekend = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PerHourUnsocialEveningAsString = c.String(),
                        PerHourUnsocialNightAsString = c.String(),
                        PerHourUnsocialWeekendAsString = c.String(),
                        PerHourUnsocialHolidayAsString = c.String(),
                        PerHourOnCallDayAsString = c.String(),
                        PerHourOnCallNightAsString = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ClaimDaySeeds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReferenceNumber = c.String(),
                        ClaimDayDate = c.DateTime(),
                        DateString = c.String(),
                        SickDayNumber = c.Int(nullable: false),
                        Hours = c.String(),
                        UnsocialEvening = c.String(),
                        UnsocialNight = c.String(),
                        UnsocialWeekend = c.String(),
                        UnsocialGrandWeekend = c.String(),
                        OnCallDay = c.String(),
                        OnCallNight = c.String(),
                        HoursSI = c.String(),
                        UnsocialEveningSI = c.String(),
                        UnsocialNightSI = c.String(),
                        UnsocialWeekendSI = c.String(),
                        UnsocialGrandWeekendSI = c.String(),
                        OnCallDaySI = c.String(),
                        OnCallNightSI = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ClaimReferenceNumbers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LatestYear = c.Int(nullable: false),
                        LatestReferenceNumber = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Claims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClaimStatusId = c.Int(nullable: false),
                        ReferenceNumber = c.String(),
                        CompletionStage = c.Int(),
                        StatusDate = c.DateTime(),
                        DeadlineDate = c.DateTime(),
                        DefaultCollectiveAgreement = c.Boolean(nullable: false),
                        CareCompanyId = c.Int(nullable: false),
                        CompanyName = c.String(),
                        OrganisationNumber = c.String(),
                        StreetAddress = c.String(),
                        Postcode = c.String(),
                        City = c.String(),
                        AccountNumber = c.String(),
                        CompanyPhoneNumber = c.String(),
                        CollectiveAgreementName = c.String(),
                        CollectiveAgreementSpecName = c.String(),
                        OwnerId = c.String(),
                        OmbudFirstName = c.String(),
                        OmbudLastName = c.String(),
                        OmbudPhoneNumber = c.String(),
                        OmbudEmail = c.String(),
                        CustomerName = c.String(nullable: false),
                        CustomerSSN = c.String(nullable: false),
                        CustomerAddress = c.String(nullable: false),
                        CustomerPhoneNumber = c.String(nullable: false),
                        SelectedRegAssistantId = c.Int(),
                        RegAssistantSSN = c.String(),
                        RegFirstName = c.String(),
                        RegLastName = c.String(),
                        RegEmail = c.String(),
                        RegPhoneNumber = c.String(),
                        HourlySalaryAsString = c.String(),
                        SickPayRateAsString = c.String(),
                        HolidayPayRateAsString = c.String(),
                        SocialFeeRateAsString = c.String(),
                        PensionAndInsuranceRateAsString = c.String(),
                        SelectedSubAssistantId = c.Int(),
                        SubAssistantSSN = c.String(),
                        SubFirstName = c.String(),
                        SubLastName = c.String(),
                        SubEmail = c.String(),
                        SubPhoneNumber = c.String(),
                        IVOCheck = c.Boolean(nullable: false),
                        ProCapitaCheck = c.Boolean(nullable: false),
                        ProxyCheck = c.Boolean(nullable: false),
                        StandInSSN = c.String(),
                        QualifyingDate = c.DateTime(nullable: false),
                        LastDayOfSicknessDate = c.DateTime(nullable: false),
                        NumberOfSickDays = c.Int(nullable: false),
                        NumberOfAbsenceHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NumberOfOrdinaryHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NumberOfUnsocialHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NumberOfOnCallHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NumberOfHoursWithSI = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NumberOfOrdinaryHoursSI = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NumberOfUnsocialHoursSI = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NumberOfOnCallHoursSI = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ClaimedSickPay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ClaimedHolidayPay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ClaimedSocialFees = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ClaimedPensionAndInsurance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ClaimedSum = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ModelSum = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DecidedSum = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ApprovedSum = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RejectedSum = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SickPayRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HolidayPayRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SocialFeeRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PensionAndInsuranceRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HourlySalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PerHourUnsocialEvening = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PerHourUnsocialNight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PerHourUnsocialWeekend = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PerHourUnsocialHoliday = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PerHourOnCallWeekday = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PerHourOnCallWeekend = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCostD1T14 = c.String(),
                        TotalCostCalcD1T14 = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CareCompanies", t => t.CareCompanyId, cascadeDelete: true)
                .ForeignKey("dbo.ClaimStatus", t => t.ClaimStatusId, cascadeDelete: true)
                .Index(t => t.ClaimStatusId)
                .Index(t => t.CareCompanyId);
            
            CreateTable(
                "dbo.Documents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MimeTypeId = c.Int(),
                        DocStatusId = c.Int(),
                        PurposeId = c.Int(),
                        Filename = c.String(nullable: false),
                        FileSize = c.Int(nullable: false),
                        Title = c.String(),
                        FileType = c.String(),
                        DateUploaded = c.DateTime(nullable: false),
                        Claim_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DocStatus", t => t.DocStatusId)
                .ForeignKey("dbo.MimeTypes", t => t.MimeTypeId)
                .ForeignKey("dbo.Purposes", t => t.PurposeId)
                .ForeignKey("dbo.Claims", t => t.Claim_Id)
                .Index(t => t.MimeTypeId)
                .Index(t => t.DocStatusId)
                .Index(t => t.PurposeId)
                .Index(t => t.Claim_Id);
            
            CreateTable(
                "dbo.DocStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MimeTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        DefaultExtension = c.String(),
                        IconURL = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Purposes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClaimId = c.Int(nullable: false),
                        CommentDate = c.DateTime(nullable: false),
                        Comment = c.String(nullable: false),
                        applicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.applicationUser_Id)
                .ForeignKey("dbo.Claims", t => t.ClaimId, cascadeDelete: true)
                .Index(t => t.ClaimId)
                .Index(t => t.applicationUser_Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(),
                        LastName = c.String(),
                        LastLogon = c.DateTime(nullable: false),
                        CareCompanyId = c.Int(),
                        SSN = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.CollectiveAgreementHeaders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Counter = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CollectiveAgreementInfoes",
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
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Messages", "ClaimId", "dbo.Claims");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Messages", "applicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Documents", "Claim_Id", "dbo.Claims");
            DropForeignKey("dbo.Documents", "PurposeId", "dbo.Purposes");
            DropForeignKey("dbo.Documents", "MimeTypeId", "dbo.MimeTypes");
            DropForeignKey("dbo.Documents", "DocStatusId", "dbo.DocStatus");
            DropForeignKey("dbo.Claims", "ClaimStatusId", "dbo.ClaimStatus");
            DropForeignKey("dbo.Claims", "CareCompanyId", "dbo.CareCompanies");
            DropForeignKey("dbo.ClaimCalculations", "ClaimStatus_Id", "dbo.ClaimStatus");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Messages", new[] { "applicationUser_Id" });
            DropIndex("dbo.Messages", new[] { "ClaimId" });
            DropIndex("dbo.Documents", new[] { "Claim_Id" });
            DropIndex("dbo.Documents", new[] { "PurposeId" });
            DropIndex("dbo.Documents", new[] { "DocStatusId" });
            DropIndex("dbo.Documents", new[] { "MimeTypeId" });
            DropIndex("dbo.Claims", new[] { "CareCompanyId" });
            DropIndex("dbo.Claims", new[] { "ClaimStatusId" });
            DropIndex("dbo.ClaimCalculations", new[] { "ClaimStatus_Id" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.DefaultCollectiveAgreementInfoes");
            DropTable("dbo.CollectiveAgreementInfoes");
            DropTable("dbo.CollectiveAgreementHeaders");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Messages");
            DropTable("dbo.Purposes");
            DropTable("dbo.MimeTypes");
            DropTable("dbo.DocStatus");
            DropTable("dbo.Documents");
            DropTable("dbo.Claims");
            DropTable("dbo.ClaimReferenceNumbers");
            DropTable("dbo.ClaimDaySeeds");
            DropTable("dbo.ClaimDays");
            DropTable("dbo.ClaimStatus");
            DropTable("dbo.ClaimCalculations");
            DropTable("dbo.CareCompanies");
            DropTable("dbo.Assistants");
        }
    }
}
