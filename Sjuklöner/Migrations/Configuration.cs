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
            IdentityRole assistantRole, ombudRole, administrativeOfficialRole, adminRole;
            IdentityResult result;
            string userId;

            if (roleManager.FindByName("Assistant") == null)
            {
                assistantRole = new IdentityRole("Assistant");
                result = roleManager.Create(assistantRole);
            }

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

            var assistants = new List<Assistant>
            {
                new Assistant{FirstName = "Isabella", LastName = "Gulldén",Email = "isabella.gullden@assistans.se",UserName = "isabella.gullden@assistans.se", LastLogon=DateTime.Now.AddDays(-2)},
                new Assistant{FirstName = "Nathalie",LastName = "Hagman",Email = "nathalie.hagman@assistans.se",UserName = "nathalie.hagman@assistans.se", LastLogon=DateTime.Now.AddDays(-2)}
            };

            foreach (var assistant in assistants)
            {
                user = userManager.FindByEmail(assistant.Email);
                if (user == null)
                {
                    user = assistant;
                    result = userManager.Create(user, "sjukLÖN123!");
                }
                if (user.Roles.Count == 0)
                {
                    result = userManager.AddToRole(user.Id, "Assistant");
                }
                userId = assistant.Id;
            }

            var ombuds = new List<Ombud>
            {
                new Ombud{FirstName = "Ombud",LastName = "Ombudsson",Email = "ombud.ombudsson@assistans.se",UserName = "ombud.ombudsson@assistans.se", LastLogon=DateTime.Now.AddDays(-2), CareCompanyId = 1},
                new Ombud{FirstName = "Olle",LastName = "Nilsson",Email = "olle.nilsson@assistans.se",UserName = "olle.nilsson@assistans.se", LastLogon=DateTime.Now.AddDays(-2), CareCompanyId = 2}
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
                new AdministrativeOfficial{FirstName = "Henrik",LastName = "Signell",Email = "henrik.signell@helsingborg.se",UserName = "henrik.signell@helsingborg.se", LastLogon=DateTime.Now.AddDays(-2)}
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
                new CareCompany{CompanyName = "Frösunda Omsorg AB"},
                new CareCompany{CompanyName = "MaxAssistans AB"}
            };
            foreach (var careCompany in careCompanies)
            {
                context.CareCompanies.AddOrUpdate(c => c.Id, careCompany);
            }

            var claimStatuses = new List<ClaimStatus>
            {
                new ClaimStatus { Id = 1, Name = "Avslagen" },
                new ClaimStatus { Id = 2, Name = "Utkast" },
                new ClaimStatus { Id = 3, Name = "Under handläggning" },
                new ClaimStatus { Id = 4, Name = "Godkänd" },
                new ClaimStatus { Id = 5, Name = "Inkorgen" }
            };
            foreach (var claimStatus in claimStatuses)
            {
                context.ClaimStatuses.AddOrUpdate(c => c.Id, claimStatus);
            }

            var claimDaySeeds = new List<ClaimDaySeed>
            {
                new ClaimDaySeed { Id = 1, StartHour = "7", StartMinute = "20", StopHour = "16", StopMinute = "45", StartHourOnCall = "21", StartMinuteOnCall = "45", StopHourOnCall = "23", StopMinuteOnCall = "45" },
                new ClaimDaySeed { Id = 2, StartHour = "13", StartMinute = "00", StopHour = "19", StopMinute = "30", StartHourOnCall = "5", StartMinuteOnCall = "15", StopHourOnCall = "9", StopMinuteOnCall = "00" },
                new ClaimDaySeed { Id = 3, StartHour = "6", StartMinute = "45", StopHour = "12", StopMinute = "30" },
                new ClaimDaySeed { Id = 4, StartHourOnCall = "12", StartMinuteOnCall = "15", StopHourOnCall = "16", StopMinuteOnCall = "20" },
                new ClaimDaySeed { Id = 5 },
                new ClaimDaySeed { Id = 6, StartHour = "15", StartMinute = "00", StopHour = "21", StopMinute = "10", StartHourOnCall = "5", StartMinuteOnCall = "15", StopHourOnCall = "9", StopMinuteOnCall = "00" },
                new ClaimDaySeed { Id = 7, StartHour = "6", StartMinute = "45", StopHour = "10", StopMinute = "30" },
                new ClaimDaySeed { Id = 8, StartHour = "7", StartMinute = "20", StopHour = "16", StopMinute = "45", StartHourOnCall = "21", StartMinuteOnCall = "45", StopHourOnCall = "23", StopMinuteOnCall = "45" },
                new ClaimDaySeed { Id = 9, StartHour = "13", StartMinute = "00", StopHour = "19", StopMinute = "30", StartHourOnCall = "5", StartMinuteOnCall = "15", StopHourOnCall = "9", StopMinuteOnCall = "00" },
                new ClaimDaySeed { Id = 10, StartHour = "6", StartMinute = "45", StopHour = "12", StopMinute = "30" },
                new ClaimDaySeed { Id = 11, StartHourOnCall = "12", StartMinuteOnCall = "15", StopHourOnCall = "16", StopMinuteOnCall = "20" },
                new ClaimDaySeed { Id = 12 },
                new ClaimDaySeed { Id = 13, StartHour = "15", StartMinute = "00", StopHour = "21", StopMinute = "10", StartHourOnCall = "5", StartMinuteOnCall = "15", StopHourOnCall = "9", StopMinuteOnCall = "00" },
                new ClaimDaySeed { Id = 14, StartHour = "6", StartMinute = "45", StopHour = "10", StopMinute = "30" }

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
