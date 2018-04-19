using Sjuklöner.Models;
using Sjuklöner.Viewmodels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sjuklöner.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize]
        public ActionResult Index()
        {
            if (User.IsInRole("Ombud"))
            {
                return RedirectToAction("Index", "Claims");
            }
            else if (User.IsInRole("AdministrativeOfficial"))
            {
                return RedirectToAction("Index", "Claims");
            }
            else if (User.IsInRole("Admin"))
            {
                AdminIndexVM adminIndexVM = new AdminIndexVM();

                var role = db.Roles.SingleOrDefault(m => m.Name == "AdministrativeOfficial");
                if (role != null)
                {
                    adminIndexVM.NumberOfAdmOffs = db.Users.Where(m => m.Roles.Any(r => r.RoleId == role.Id)).Count();
                }
                else
                {
                    adminIndexVM.NumberOfAdmOffs = 0;
                }

                role = db.Roles.SingleOrDefault(m => m.Name == "Ombud");
                if (role != null)
                {
                    adminIndexVM.NumberOfOmbuds = db.Users.Where(m => m.Roles.Any(r => r.RoleId == role.Id)).Count();
                }
                else
                {
                    adminIndexVM.NumberOfOmbuds = 0;
                }

                adminIndexVM.NumberOfAssistants = db.Assistants.Count();
                adminIndexVM.NumberOfCareCompanies = db.CareCompanies.Count();
                adminIndexVM.NumberOfClaims = db.Claims.Where(c => c.ClaimStatusId >= 5).Count(); //Claims that have been submitted and where Robin has done its checks.
                adminIndexVM.NumberOfCollectiveAgreements = db.CollectiveAgreementHeaders.Count();
                adminIndexVM.AutomaticTransferToProcapita = db.AppAdmins.FirstOrDefault().AutomaticTransferToProcapita;

                return View("Index", adminIndexVM);
            }
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult SaveAdminSetting(AdminIndexVM adminIndexVM)
        {
            var appAdmin = db.AppAdmins.FirstOrDefault();
            appAdmin.AutomaticTransferToProcapita = adminIndexVM.AutomaticTransferToProcapita;
            db.Entry(appAdmin).State = EntityState.Modified;
            db.SaveChanges();

            var role = db.Roles.SingleOrDefault(m => m.Name == "AdministrativeOfficial");
            if (role != null)
            {
                adminIndexVM.NumberOfAdmOffs = db.Users.Where(m => m.Roles.Any(r => r.RoleId == role.Id)).Count();
            }
            else
            {
                adminIndexVM.NumberOfAdmOffs = 0;
            }

            role = db.Roles.SingleOrDefault(m => m.Name == "Ombud");
            if (role != null)
            {
                adminIndexVM.NumberOfOmbuds = db.Users.Where(m => m.Roles.Any(r => r.RoleId == role.Id)).Count();
            }
            else
            {
                adminIndexVM.NumberOfOmbuds = 0;
            }
            adminIndexVM.NumberOfAssistants = db.Assistants.Count();
            adminIndexVM.NumberOfCareCompanies = db.CareCompanies.Count();
            adminIndexVM.NumberOfClaims = db.Claims.Where(c => c.ClaimStatusId >= 5).Count(); //Claims that have been submitted and where Robin has done its checks.
            adminIndexVM.NumberOfCollectiveAgreements = db.CollectiveAgreementHeaders.Count();
            return View("Index", adminIndexVM);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}