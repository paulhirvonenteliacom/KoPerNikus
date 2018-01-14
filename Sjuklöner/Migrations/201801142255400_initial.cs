namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
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
                        StatusId = c.Int(nullable: false),
                        CareCompanyId = c.Int(nullable: false),
                        ReferenceNumber = c.String(),
                        StatusDate = c.DateTime(),
                        DeadlineDate = c.DateTime(),
                        CustomerFirstName = c.String(),
                        CustomerLastName = c.String(),
                        OrganisationNumber = c.String(),
                        CustomerSSN = c.String(nullable: false),
                        AssistantSSN = c.String(nullable: false),
                        QualifyingDate = c.DateTime(nullable: false),
                        LastDayOfSicknessDate = c.DateTime(nullable: false),
                        NumberOfSickDays = c.Int(nullable: false),
                        SickPay = c.Double(nullable: false),
                        HolidayPay = c.Double(nullable: false),
                        SocialFees = c.Double(nullable: false),
                        PensionAndInsurance = c.Double(nullable: false),
                        ClaimSum = c.Double(nullable: false),
                        ModelSum = c.Double(nullable: false),
                        HoursQualifyingDay = c.Double(nullable: false),
                        HolidayPayQualDay = c.Double(nullable: false),
                        PayrollTaxQualDay = c.Double(nullable: false),
                        InsuranceQualDay = c.Double(nullable: false),
                        PensionQualDay = c.Double(nullable: false),
                        ClaimQualDay = c.Double(nullable: false),
                        HoursDay2To14 = c.Double(nullable: false),
                        HourlySickPay = c.Double(nullable: false),
                        SickPayDay2To14 = c.Double(nullable: false),
                        HolidayPayDay2To14 = c.Double(nullable: false),
                        UnsocialHoursPayDay2To14 = c.Double(nullable: false),
                        OnCallHoursPayDay2To14 = c.Double(nullable: false),
                        PayrollTaxDay2To14 = c.Double(nullable: false),
                        InsuranceDay2To14 = c.Double(nullable: false),
                        PensionDay2To14 = c.Double(nullable: false),
                        ClaimDay2To14 = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CareCompanies", t => t.CareCompanyId, cascadeDelete: true)
                .Index(t => t.CareCompanyId);
            
            CreateTable(
                "dbo.Documents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MimeTypeId = c.Int(nullable: false),
                        StatusId = c.Int(nullable: false),
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
                .ForeignKey("dbo.MimeTypes", t => t.MimeTypeId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.OwnerId)
                .ForeignKey("dbo.Purposes", t => t.PurposeId, cascadeDelete: true)
                .ForeignKey("dbo.DocStatus", t => t.StatusId, cascadeDelete: true)
                .ForeignKey("dbo.Claims", t => t.Claim_Id)
                .Index(t => t.MimeTypeId)
                .Index(t => t.StatusId)
                .Index(t => t.PurposeId)
                .Index(t => t.OwnerId)
                .Index(t => t.Claim_Id);
            
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
                "dbo.DocStatus",
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
                        AuthorId = c.String(),
                        CommentDate = c.DateTime(nullable: false),
                        Comment = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Claims", t => t.ClaimId, cascadeDelete: true)
                .Index(t => t.ClaimId);
            
            CreateTable(
                "dbo.ClaimStatus",
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
            DropForeignKey("dbo.Documents", "StatusId", "dbo.DocStatus");
            DropForeignKey("dbo.Documents", "PurposeId", "dbo.Purposes");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Documents", "OwnerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Documents", "MimeTypeId", "dbo.MimeTypes");
            DropForeignKey("dbo.Claims", "CareCompanyId", "dbo.CareCompanies");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Messages", new[] { "ClaimId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Documents", new[] { "Claim_Id" });
            DropIndex("dbo.Documents", new[] { "OwnerId" });
            DropIndex("dbo.Documents", new[] { "PurposeId" });
            DropIndex("dbo.Documents", new[] { "StatusId" });
            DropIndex("dbo.Documents", new[] { "MimeTypeId" });
            DropIndex("dbo.Claims", new[] { "CareCompanyId" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.CollectiveAgreements");
            DropTable("dbo.ClaimStatus");
            DropTable("dbo.Messages");
            DropTable("dbo.DocStatus");
            DropTable("dbo.Purposes");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.MimeTypes");
            DropTable("dbo.Documents");
            DropTable("dbo.Claims");
            DropTable("dbo.ClaimReferenceNumbers");
            DropTable("dbo.ClaimDays");
            DropTable("dbo.CareCompanies");
        }
    }
}
