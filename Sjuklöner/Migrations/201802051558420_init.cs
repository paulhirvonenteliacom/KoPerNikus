namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CareCompanies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyName = c.String(),
                        CompanyOrganisationNumber = c.String(),
                        StreetAddress = c.String(),
                        Postcode = c.String(),
                        City = c.String(),
                        AccountNumber = c.String(),
                        PhoneNumber = c.String(),
                        CollectiveAgreement = c.String(),
                        ReferenceCode = c.String(),
                        StorageApproval = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ClaimDays",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReferenceNumber = c.String(),
                        ClaimDayDate = c.DateTime(),
                        DateString = c.String(),
                        SickDayNumber = c.Int(nullable: false),
                        StartHour = c.String(),
                        StartMinute = c.String(),
                        StopHour = c.String(),
                        StopMinute = c.String(),
                        NumberOfHours = c.Single(nullable: false),
                        NumberOfUnsocialHours = c.Single(nullable: false),
                        NumberOfUnsocialHoursNight = c.Single(nullable: false),
                        NumberOfUnsocialHoursEvening = c.Single(nullable: false),
                        StartHourOnCall = c.String(),
                        StartMinuteOnCall = c.String(),
                        StopHourOnCall = c.String(),
                        StopMinuteOnCall = c.String(),
                        NumberOfOnCallHours = c.Single(nullable: false),
                        NumberOfOnCallHoursNight = c.Single(nullable: false),
                        NumberOfOnCallHoursEvening = c.Single(nullable: false),
                        StartHourSI = c.String(),
                        StartMinuteSI = c.String(),
                        StopHourSI = c.String(),
                        StopMinuteSI = c.String(),
                        NumberOfHoursSI = c.Single(nullable: false),
                        NumberOfUnsocialHoursSI = c.Single(nullable: false),
                        NumberOfUnsocialHoursNightSI = c.Single(nullable: false),
                        NumberOfUnsocialHoursEveningSI = c.Single(nullable: false),
                        StartHourOnCallSI = c.String(),
                        StartMinuteOnCallSI = c.String(),
                        StopHourOnCallSI = c.String(),
                        StopMinuteOnCallSI = c.String(),
                        NumberOfOnCallHoursSI = c.Single(nullable: false),
                        NumberOfOnCallHoursNightSI = c.Single(nullable: false),
                        NumberOfOnCallHoursEveningSI = c.Single(nullable: false),
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
                        OwnerId = c.String(),
                        ClaimStatusId = c.Int(nullable: false),
                        CareCompanyId = c.Int(nullable: false),
                        IVOCheck = c.Boolean(nullable: false),
                        ProCapitaCheck = c.Boolean(nullable: false),
                        ReferenceNumber = c.String(),
                        StatusDate = c.DateTime(),
                        DeadlineDate = c.DateTime(),
                        CustomerFirstName = c.String(),
                        CustomerLastName = c.String(),
                        OrganisationNumber = c.String(),
                        CustomerSSN = c.String(nullable: false),
                        AssistantSSN = c.String(),
                        QualifyingDate = c.DateTime(nullable: false),
                        LastDayOfSicknessDate = c.DateTime(nullable: false),
                        Email = c.String(),
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
                        HoursQualifyingDay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HolidayPayQualDay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PayrollTaxQualDay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InsuranceQualDay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PensionQualDay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ClaimQualDay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HoursDay2To14 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HourlySickPay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SickPayDay2To14 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HolidayPayDay2To14 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnsocialHoursPayDay2To14 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OnCallHoursPayDay2To14 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PayrollTaxDay2To14 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InsuranceDay2To14 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PensionDay2To14 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ClaimDay2To14 = c.Decimal(nullable: false, precision: 18, scale: 2),
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
                        SickPayRateAsString = c.String(),
                        HolidayPayRateAsString = c.String(),
                        SocialFeeRateAsString = c.String(),
                        PensionAndInsuranceRateAsString = c.String(),
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
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CareCompanies", t => t.CareCompanyId, cascadeDelete: true)
                .ForeignKey("dbo.ClaimStatus", t => t.ClaimStatusId, cascadeDelete: true)
                .Index(t => t.ClaimStatusId)
                .Index(t => t.CareCompanyId);
            
            CreateTable(
                "dbo.ClaimStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Documents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MimeTypeId = c.Int(nullable: false),
                        DocStatusId = c.Int(nullable: false),
                        PurposeId = c.Int(nullable: false),
                        OwnerId = c.String(maxLength: 128),
                        Filename = c.String(nullable: false),
                        FileSize = c.Int(nullable: false),
                        Title = c.String(),
                        FileType = c.String(),
                        DateUploaded = c.DateTime(nullable: false),
                        Claim_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DocStatus", t => t.DocStatusId, cascadeDelete: true)
                .ForeignKey("dbo.MimeTypes", t => t.MimeTypeId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.OwnerId)
                .ForeignKey("dbo.Purposes", t => t.PurposeId, cascadeDelete: true)
                .ForeignKey("dbo.Claims", t => t.Claim_Id)
                .Index(t => t.MimeTypeId)
                .Index(t => t.DocStatusId)
                .Index(t => t.PurposeId)
                .Index(t => t.OwnerId)
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
                        UserId = c.String(),
                        Approved = c.Boolean(),
                        CompanyName = c.String(),
                        StreetAddress = c.String(),
                        Postcode = c.String(),
                        City = c.String(),
                        ClearingNumber = c.String(),
                        AccountNumber = c.String(),
                        HourlySalary = c.Decimal(precision: 18, scale: 2),
                        HolidayPayRate = c.Decimal(precision: 18, scale: 2),
                        PayrollTaxRate = c.Decimal(precision: 18, scale: 2),
                        InsuranceRate = c.Decimal(precision: 18, scale: 2),
                        PensionRate = c.Decimal(precision: 18, scale: 2),
                        StorageApproval = c.Boolean(),
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
                "dbo.Purposes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CollectiveAgreements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        PerHourUnsocialEvening = c.Double(nullable: false),
                        PerHourUnsocialNight = c.Double(nullable: false),
                        PerHourUnsocialWeekend = c.Double(nullable: false),
                        PerHourUnsocialHoliday = c.Double(nullable: false),
                        PerHourOnCallWeekday = c.Double(nullable: false),
                        PerHourOnCallWeekend = c.Double(nullable: false),
                        PerHourPreparedWeekday = c.Double(nullable: false),
                        PerHourPreparedWeekend = c.Double(nullable: false),
                        OvertimeFactor = c.Double(nullable: false),
                        ExtraHOurFactor = c.Double(nullable: false),
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
            DropForeignKey("dbo.Documents", "Claim_Id", "dbo.Claims");
            DropForeignKey("dbo.Documents", "PurposeId", "dbo.Purposes");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Messages", "applicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Documents", "OwnerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Documents", "MimeTypeId", "dbo.MimeTypes");
            DropForeignKey("dbo.Documents", "DocStatusId", "dbo.DocStatus");
            DropForeignKey("dbo.Claims", "ClaimStatusId", "dbo.ClaimStatus");
            DropForeignKey("dbo.Claims", "CareCompanyId", "dbo.CareCompanies");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.Messages", new[] { "applicationUser_Id" });
            DropIndex("dbo.Messages", new[] { "ClaimId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Documents", new[] { "Claim_Id" });
            DropIndex("dbo.Documents", new[] { "OwnerId" });
            DropIndex("dbo.Documents", new[] { "PurposeId" });
            DropIndex("dbo.Documents", new[] { "DocStatusId" });
            DropIndex("dbo.Documents", new[] { "MimeTypeId" });
            DropIndex("dbo.Claims", new[] { "CareCompanyId" });
            DropIndex("dbo.Claims", new[] { "ClaimStatusId" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.CollectiveAgreements");
            DropTable("dbo.Purposes");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.Messages");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.MimeTypes");
            DropTable("dbo.DocStatus");
            DropTable("dbo.Documents");
            DropTable("dbo.ClaimStatus");
            DropTable("dbo.Claims");
            DropTable("dbo.ClaimReferenceNumbers");
            DropTable("dbo.ClaimDaySeeds");
            DropTable("dbo.ClaimDays");
            DropTable("dbo.CareCompanies");
        }
    }
}
