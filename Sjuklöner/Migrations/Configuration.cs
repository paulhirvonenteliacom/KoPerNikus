namespace Sjuklöner.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Sjuklöner.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Sjuklöner.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Sjuklöner.Models.ApplicationDbContext context)
        {
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);

            ApplicationUser user;
            IdentityRole ombudRole, administrativeOfficialRole, adminRole;
            IdentityResult result;
            string userId;

            if (roleManager.FindByName("Ombud") == null)
            {
                ombudRole = new IdentityRole("Ombud");
                result = roleManager.Create(ombudRole);
            }

            if (roleManager.FindByName("AdministrativeOfficial") == null)
            {
                administrativeOfficialRole = new IdentityRole("AdministrativeOfficial");
                result = roleManager.Create(administrativeOfficialRole);
            }

            if (roleManager.FindByName("Admin") == null)
            {
                adminRole = new IdentityRole("Admin");
                result = roleManager.Create(adminRole);
            }

            //var assistants = new List<Assistant>
            //{
            //    new Assistant{FirstName = "Isabella", LastName = "Gulldén",Email = "isabella.gullden@assistans.se",UserName = "isabella.gullden@assistans.se", LastLogon=DateTime.Now.AddDays(-2)},
            //    new Assistant{FirstName = "Nathalie",LastName = "Hagman",Email = "nathalie.hagman@assistans.se",UserName = "nathalie.hagman@assistans.se", LastLogon=DateTime.Now.AddDays(-2)}
            //};

            //foreach (var assistant in assistants)
            //{
            //    user = userManager.FindByEmail(assistant.Email);
            //    if (user == null)
            //    {
            //        user = assistant;
            //        result = userManager.Create(user, "sjukLÖN123!");
            //    }
            //    if (user.Roles.Count == 0)
            //    {
            //        result = userManager.AddToRole(user.Id, "Assistant");
            //    }
            //    userId = assistant.Id;
            //}

            var ombuds = new List<Ombud>
            {
                new Ombud{FirstName = "Ombud",LastName = "Ombudsson",Email = "ombud.ombudsson@assistans.se",UserName = "ombud.ombudsson@assistans.se", LastLogon=DateTime.Now.AddDays(-2), CareCompanyId = 1, SSN = "197503266251", PhoneNumber = "034-453 7685"},
                new Ombud{FirstName = "Olle",LastName = "Nilsson",Email = "olle.nilsson@assistans.se",UserName = "olle.nilsson@assistans.se", LastLogon=DateTime.Now.AddDays(-2), CareCompanyId = 2, PhoneNumber = "032-987 1290"}
            };

            foreach (var ombud in ombuds)
            {
                user = userManager.FindByEmail(ombud.Email);
                if (user == null)
                {
                    user = ombud;
                    result = userManager.Create(user, "sjukLÖN123!");
                }
                if (user.Roles.Count == 0)
                {
                    result = userManager.AddToRole(user.Id, "Ombud");
                }
                userId = ombud.Id;
            }

            var administrativeOfficials = new List<AdministrativeOfficial>
            {
                new AdministrativeOfficial{FirstName = "Henrik",LastName = "Signell",Email = "henrik.signell@helsingborg.se",UserName = "henrik.signell@helsingborg.se", LastLogon=DateTime.Now.AddDays(-2), SSN = "196705078134"}
            };

            foreach (var administrativeOfficial in administrativeOfficials)
            {
                user = userManager.FindByEmail(administrativeOfficial.Email);
                if (user == null)
                {
                    user = administrativeOfficial;
                    result = userManager.Create(user, "sjukLÖN123!");
                }
                if (user.Roles.Count == 0)
                {
                    result = userManager.AddToRole(user.Id, "AdministrativeOfficial");
                }
                userId = administrativeOfficial.Id;
            }

            var admins = new List<Admin>
            {
                new Admin{FirstName = "Admin",LastName = "Adminsson",Email = "admin.adminsson@helsingborg.se",UserName = "admin.adminsson@helsingborg.se", LastLogon=DateTime.Now.AddDays(-2)}
            };

            foreach (var admin in admins)
            {
                user = userManager.FindByEmail(admin.Email);
                if (user == null)
                {
                    user = admin;
                    result = userManager.Create(user, "sjukLÖN123!");
                }
                if (user.Roles.Count == 0)
                {
                    result = userManager.AddToRole(user.Id, "Admin");
                }
                userId = admin.Id;
            }

            var careCompanies = new List<CareCompany>
            {
                new CareCompany{ Id = 1, CompanyName = "Smart Assistans", OrganisationNumber = "556881-2118", StreetAddress = "Assistansvägen 55", Postcode = "155 55", City = "Assistansköping", AccountNumber = "1234-1234", CompanyPhoneNumber = "024-323 2356", SelectedCollectiveAgreementId = 1, CollectiveAgreementSpecName = "Branch G" },
                new CareCompany{ Id = 2, CompanyName = "Assistans Direkt AB", OrganisationNumber = "556833-2198", StreetAddress = "Assistansgatan 33", Postcode = "133 33", City = "Assistansborg", AccountNumber = "6543-9876", CompanyPhoneNumber = "024-987 2356", SelectedCollectiveAgreementId = 1, CollectiveAgreementSpecName = "Branch G" }
            };
            foreach (var careCompany in careCompanies)
            {
                context.CareCompanies.AddOrUpdate(c => c.Id, careCompany);
            }

            var assistants = new List<Assistant>
            {
                new Assistant{ Id = 1, CareCompanyId = 1, FirstName = "Astrid", LastName = "Assistentsson", AssistantSSN = "19730423-5076", Email = "astrid.assistentsson@smartassistans.se", PhoneNumber = "034-232 5678", HourlySalary = "120,00", HolidayPayRate = "12,00", PayrollTaxRate = "31,42", InsuranceRate = "5,00", PensionRate = "1,00"},
                new Assistant{ Id = 2, CareCompanyId = 1, FirstName = "Björn", LastName = "Björnsson", AssistantSSN = "19830423-5076", Email = "bjorn.bjornsson@smartassistans.se", PhoneNumber = "034-131 4578", HourlySalary = "120,00", HolidayPayRate = "12,00", PayrollTaxRate = "31,42", InsuranceRate = "5,00", PensionRate = "1,00"},
            };
            foreach (var assistant in assistants)
            {
                context.Assistants.AddOrUpdate(c => c.Id, assistant);
            }

            var claimStatuses = new List<ClaimStatus>
            {
                new ClaimStatus { Id = 1, Name = "Beslutad" }, //Old "Avslagen
                new ClaimStatus { Id = 2, Name = "Utkast" },
                new ClaimStatus { Id = 3, Name = "Under handläggning" },
                new ClaimStatus { Id = 4, Name = "Godkänd" }, //Not used
                new ClaimStatus { Id = 5, Name = "Inkorgen" }
            };
            foreach (var claimStatus in claimStatuses)
            {
                context.ClaimStatuses.AddOrUpdate(c => c.Id, claimStatus);
            }

            var claimDaySeeds = new List<ClaimDaySeed>
            {
                new ClaimDaySeed { Id = 1, Hours = "8,00", OnCallNight = "2,00"},
                new ClaimDaySeed { Id = 2, Hours = "6,50", UnsocialWeekend = "6,50", OnCallNight = "3,00" },
                new ClaimDaySeed { Id = 3, Hours = "5,75", UnsocialWeekend = "5,75"},
                new ClaimDaySeed { Id = 4, Hours = "4,00", UnsocialEvening = "4,00" },
                new ClaimDaySeed { Id = 5 },
                new ClaimDaySeed { Id = 6, Hours = "6", OnCallDay = "4"},
                new ClaimDaySeed { Id = 7, Hours = "3,75" },
                new ClaimDaySeed { Id = 8, Hours = "9,50", OnCallNight = "2"},
                new ClaimDaySeed { Id = 9, Hours = "6,50", OnCallNight = "4" },
                new ClaimDaySeed { Id = 10, Hours = "5,75"},
                new ClaimDaySeed { Id = 11, OnCallDay = "8"},
                new ClaimDaySeed { Id = 12 },
                new ClaimDaySeed { Id = 13, Hours = "6", UnsocialEvening = "2"},
                new ClaimDaySeed { Id = 14, Hours = "3,75" }

            };
            foreach (var claimDaySeed in claimDaySeeds)
            {
                context.ClaimDaySeeds.AddOrUpdate(c => c.Id, claimDaySeed);
            }

            context.ClaimReferenceNumbers.AddOrUpdate(c => c.Id, new ClaimReferenceNumber
            {
                Id = 1,
                LatestYear = 2018,
                LatestReferenceNumber = 00000
            });

            context.SaveChanges();
        }
    }
}
