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
            //    new Assistant{FirstName = "Isabella", LastName = "Gulldén",Email = "isabella.gullden@assistans.se",PhoneNumber = "034-234 5556",AssistantSSN = "630802-5066" },
            //    new Assistant{FirstName = "Nathalie",LastName = "Hagman",Email = "nathalie.hagman@assistans.se",PhoneNumber = "065-455 5678",AssistantSSN = "890314-4033" }
            //};
            //foreach (var assistant in assistants)
            //{
            //    context.Assistants.AddOrUpdate(a => a.Id, assistant);
            //}

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
                new Ombud{FirstName = "Ombud",LastName = "Ombudsson",Email = "ombud.ombudsson@assistans.se",UserName = "ombud.ombudsson@assistans.se", LastLogon=DateTime.Now.AddDays(-2), CareCompanyId = 1, SSN = "19750326-6251", PhoneNumber = "034-453 7685"},
                new Ombud{FirstName = "Olle",LastName = "Nilsson",Email = "olle.nilsson@assistans.se",UserName = "olle.nilsson@assistans.se", LastLogon=DateTime.Now.AddDays(-2), CareCompanyId = 2, SSN = "19850326-3351", PhoneNumber = "032-987 1290"}
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
                new AdministrativeOfficial{FirstName = "Henrik",LastName = "Signell",Email = "henrik.signell@helsingborg.se",UserName = "henrik.signell@helsingborg.se", LastLogon=DateTime.Now.AddDays(-2), SSN = "19670507-8134"}
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

            //var defaultCollectiveAgreementHeaders = new List<DefaultCollectiveAgreementHeader>
            //{
            //    new DefaultCollectiveAgreementHeader{ Id = 1, Name = "Kommunal - Vårdföretagarna", Counter = 1 }
            //};
            //foreach (var defaultCollectiveAgreementHeader in defaultCollectiveAgreementHeaders)
            //{
            //    context.DefaultCollectiveAgreementHeaders.AddOrUpdate(c => c.Id, defaultCollectiveAgreementHeader);
            //}

            var defaultCollectiveAgreementInfos = new List<DefaultCollectiveAgreementInfo>
            {
                new DefaultCollectiveAgreementInfo{ Id = 1, CollectiveAgreementHeaderId = 1, StartDate = new DateTime(2000, 1, 1, 0, 0, 0), EndDate = new DateTime(2099, 12, 31, 0, 0, 0), PerHourUnsocialEvening = "21,08", PerHourUnsocialNight = "42,54", PerHourUnsocialWeekend = "52,47", PerHourUnsocialHoliday = "105,03", PerHourOnCallWeekday = "10,78", PerHourOnCallWeekend = "21,55" }
            };
            foreach (var defaultCollectiveAgreementInfo in defaultCollectiveAgreementInfos)
            {
                context.DefaultCollectiveAgreementInfos.AddOrUpdate(c => c.Id, defaultCollectiveAgreementInfo);
            }

            var collectiveAgreementHeaders = new List<CollectiveAgreementHeader>
            {
                new CollectiveAgreementHeader{ Name = "Kommunal - Vårdföretagarna", Counter = 3 },
                new CollectiveAgreementHeader{ Name = "Kommunal - Arbetsgivarföreningen KFO", Counter = 4 },
                new CollectiveAgreementHeader{ Name = "Kommunal - KFS", Counter = 4 }
                //Pay for oncall hours for the agreements below is a percentage of the hourly salary. It has been left to later to include them if necessary at a later point in time. 
                //new CollectiveAgreementHeader{ Id = 4, Name = "SKL/PACTA, HÖK 16", Counter = 3 },
                //new CollectiveAgreementHeader{ Id = 5, Name = "SKL/PACTA, PAN 16, personlig assistent", Counter = 3 },
                //new CollectiveAgreementHeader{ Id = 6, Name = "SKL/PACTA, PAN 16, anhörigvårdare", Counter = 3 },
                //new CollectiveAgreementHeader{ Id = 7, Name = "AB-P", Counter = 3 },
                //new CollectiveAgreementHeader{ Id = 8, Name = "PAN-P", Counter = 3 }
            };
            foreach (var collectiveAgreementHeader in collectiveAgreementHeaders)
            {
                context.CollectiveAgreementHeaders.AddOrUpdate(c => c.Id, collectiveAgreementHeader);
            }
            context.SaveChanges();
            var collectiveAgreementInfos = new List<CollectiveAgreementInfo>
            {
                new CollectiveAgreementInfo{ Id = 1, CollectiveAgreementHeaderId = context.CollectiveAgreementHeaders.Where(c => c.Name == "Kommunal - Vårdföretagarna").FirstOrDefault().Id, StartDate = new DateTime(2017, 7, 1, 0, 0, 0), EndDate = new DateTime(2018, 9, 30, 0, 0, 0), PerHourUnsocialEvening = "21,08", PerHourUnsocialNight = "42,54", PerHourUnsocialWeekend = "52,47", PerHourUnsocialHoliday = "105,03", PerHourOnCallWeekday = "10,78", PerHourOnCallWeekend = "21,55" },
                new CollectiveAgreementInfo{ Id = 2, CollectiveAgreementHeaderId = context.CollectiveAgreementHeaders.Where(c => c.Name == "Kommunal - Vårdföretagarna").FirstOrDefault().Id, StartDate = new DateTime(2018, 10,1, 0, 0, 0), EndDate = new DateTime(2019, 9, 30, 0, 0, 0), PerHourUnsocialEvening = "21,50", PerHourUnsocialNight = "43,39", PerHourUnsocialWeekend = "53,52", PerHourUnsocialHoliday = "107,13", PerHourOnCallWeekday = "11,00", PerHourOnCallWeekend = "21,98" },
                new CollectiveAgreementInfo{ Id = 3, CollectiveAgreementHeaderId = context.CollectiveAgreementHeaders.Where(c => c.Name == "Kommunal - Vårdföretagarna").FirstOrDefault().Id, StartDate = new DateTime(2019, 10,1, 0, 0, 0), EndDate = new DateTime(2020, 6, 30, 0, 0, 0), PerHourUnsocialEvening = "21,91", PerHourUnsocialNight = "44,39", PerHourUnsocialWeekend = "54,75", PerHourUnsocialHoliday = "109,59", PerHourOnCallWeekday = "11,25", PerHourOnCallWeekend = "22,49" },
                new CollectiveAgreementInfo{ Id = 4, CollectiveAgreementHeaderId = context.CollectiveAgreementHeaders.Where(c => c.Name == "Kommunal - Arbetsgivarföreningen KFO").FirstOrDefault().Id, StartDate = new DateTime(2017, 9, 1, 0, 0, 0), EndDate = new DateTime(2017,12, 31, 0, 0, 0), PerHourUnsocialEvening = "20,55", PerHourUnsocialNight = "41,43", PerHourUnsocialWeekend = "51,10", PerHourUnsocialHoliday = "102,35", PerHourOnCallWeekday = "15,54", PerHourOnCallWeekend = "30,66" },
                new CollectiveAgreementInfo{ Id = 5, CollectiveAgreementHeaderId = context.CollectiveAgreementHeaders.Where(c => c.Name == "Kommunal - Arbetsgivarföreningen KFO").FirstOrDefault().Id, StartDate = new DateTime(2018, 1, 1, 0, 0, 0), EndDate = new DateTime(2018,12, 31, 0, 0, 0), PerHourUnsocialEvening = "20,96", PerHourUnsocialNight = "42,25", PerHourUnsocialWeekend = "52,12", PerHourUnsocialHoliday = "104,39", PerHourOnCallWeekday = "16,00", PerHourOnCallWeekend = "31,57" },
                new CollectiveAgreementInfo{ Id = 6, CollectiveAgreementHeaderId = context.CollectiveAgreementHeaders.Where(c => c.Name == "Kommunal - Arbetsgivarföreningen KFO").FirstOrDefault().Id, StartDate = new DateTime(2019, 1, 1, 0, 0, 0), EndDate = new DateTime(2019,12, 31, 0, 0, 0), PerHourUnsocialEvening = "21,37", PerHourUnsocialNight = "43,09", PerHourUnsocialWeekend = "53,16", PerHourUnsocialHoliday = "106,47", PerHourOnCallWeekday = "16,22", PerHourOnCallWeekend = "32,00" },
                new CollectiveAgreementInfo{ Id = 7, CollectiveAgreementHeaderId = context.CollectiveAgreementHeaders.Where(c => c.Name == "Kommunal - Arbetsgivarföreningen KFO").FirstOrDefault().Id, StartDate = new DateTime(2020, 1, 1, 0, 0, 0), EndDate = new DateTime(2020, 8, 31, 0, 0, 0), PerHourUnsocialEvening = "21,90", PerHourUnsocialNight = "44,16", PerHourUnsocialWeekend = "54,48", PerHourUnsocialHoliday = "109,13", PerHourOnCallWeekday = "16,47", PerHourOnCallWeekend = "32,51" },
                new CollectiveAgreementInfo{ Id = 8, CollectiveAgreementHeaderId = context.CollectiveAgreementHeaders.Where(c => c.Name == "Kommunal - KFS").FirstOrDefault().Id, StartDate = new DateTime(2017, 11,1, 0, 0, 0), EndDate = new DateTime(2017, 12,31, 0, 0, 0), PerHourUnsocialEvening = "20,76", PerHourUnsocialNight = "41,54", PerHourUnsocialWeekend = "50,63", PerHourUnsocialHoliday = "101,21", PerHourOnCallWeekday = "15,59", PerHourOnCallWeekend = "31,17" },
                new CollectiveAgreementInfo{ Id = 9, CollectiveAgreementHeaderId = context.CollectiveAgreementHeaders.Where(c => c.Name == "Kommunal - KFS").FirstOrDefault().Id, StartDate = new DateTime(2018, 1, 1, 0, 0, 0), EndDate = new DateTime(2018, 12,31, 0, 0, 0), PerHourUnsocialEvening = "21,18", PerHourUnsocialNight = "42,37", PerHourUnsocialWeekend = "51,64", PerHourUnsocialHoliday = "103,23", PerHourOnCallWeekday = "15,90", PerHourOnCallWeekend = "31,79" },
                new CollectiveAgreementInfo{ Id =10, CollectiveAgreementHeaderId = context.CollectiveAgreementHeaders.Where(c => c.Name == "Kommunal - KFS").FirstOrDefault().Id, StartDate = new DateTime(2019, 1, 1, 0, 0, 0), EndDate = new DateTime(2019, 12,31, 0, 0, 0), PerHourUnsocialEvening = "21,60", PerHourUnsocialNight = "43,22", PerHourUnsocialWeekend = "52,67", PerHourUnsocialHoliday = "105,29", PerHourOnCallWeekday = "16,22", PerHourOnCallWeekend = "32,43" },
                new CollectiveAgreementInfo{ Id =11, CollectiveAgreementHeaderId = context.CollectiveAgreementHeaders.Where(c => c.Name == "Kommunal - KFS").FirstOrDefault().Id, StartDate = new DateTime(2020, 1, 1, 0, 0, 0), EndDate = new DateTime(2020, 10,31, 0, 0, 0), PerHourUnsocialEvening = "22,14", PerHourUnsocialNight = "44,30", PerHourUnsocialWeekend = "53,99", PerHourUnsocialHoliday = "107,92", PerHourOnCallWeekday = "16,63", PerHourOnCallWeekend = "33,24" }
            };
            foreach (var collectiveAgreementInfo in collectiveAgreementInfos)
            {
                context.CollectiveAgreementInfos.AddOrUpdate(c => c.Id, collectiveAgreementInfo);
            }

            var careCompanies = new List<CareCompany>
            {
                new CareCompany{ Id = 1, CompanyName = "Smart Assistans", OrganisationNumber = "556881-2118", StreetAddress = "Assistansvägen 55", Postcode = "155 55", City = "Assistansköping", AccountNumber = "1234-1234", CompanyPhoneNumber = "024-323 2356", SelectedCollectiveAgreementId = 1, CollectiveAgreementName = "Vårdföretagarna", CollectiveAgreementSpecName = "Branch G" },
                new CareCompany{ Id = 2, CompanyName = "Assistans Direkt AB", OrganisationNumber = "556833-2198", StreetAddress = "Assistansgatan 33", Postcode = "133 33", City = "Assistansborg", AccountNumber = "6543-9876", CompanyPhoneNumber = "024-987 2356", SelectedCollectiveAgreementId = 1, CollectiveAgreementName = "Vårdföretagarna", CollectiveAgreementSpecName = "Branch G" }
            };
            foreach (var careCompany in careCompanies)
            {
                context.CareCompanies.AddOrUpdate(c => c.Id, careCompany);
            }

            var assistants = new List<Assistant>
            {
                new Assistant{ Id = 1, CareCompanyId = 1, FirstName = "Astrid", LastName = "Assistentsson", AssistantSSN = "19730423-5076", Email = "astrid.assistentsson@smartassistans.se", PhoneNumber = "034-232 5678", HourlySalary = "120,00", HolidayPayRate = "12,00", PayrollTaxRate = "31,42", PensionAndInsuranceRate = "6,00" },
                new Assistant{ Id = 2, CareCompanyId = 1, FirstName = "Björn", LastName = "Björnsson", AssistantSSN = "19830423-5076", Email = "bjorn.bjornsson@smartassistans.se", PhoneNumber = "034-131 4578", HourlySalary = "120,00", HolidayPayRate = "12,00", PayrollTaxRate = "31,42", PensionAndInsuranceRate = "6,00" }
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
