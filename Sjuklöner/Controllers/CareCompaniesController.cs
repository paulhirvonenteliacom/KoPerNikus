using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Sjuklöner.Models;
using Microsoft.AspNet.Identity;
using Sjuklöner.Viewmodels;
using static Sjuklöner.Viewmodels.OmbudIndexVM;

namespace Sjuklöner.Controllers
{
    public class CareCompaniesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CareCompanies
        public ActionResult Index()
        {
            if (User.IsInRole("Ombud"))
            {
                var currentId = User.Identity.GetUserId();
                ApplicationUser currentUser = db.Users.Where(u => u.Id == currentId).FirstOrDefault();
                var companyId = currentUser.CareCompanyId;
                return RedirectToAction("Edit", new { id = companyId });
            }
            return View(db.CareCompanies.ToList());
        }

        // GET: CareCompanies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CareCompany careCompany = db.CareCompanies.Find(id);
            if (careCompany == null)
            {
                return HttpNotFound();
            }
            return View(careCompany);
        }

        // GET: CareCompanies/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CareCompanies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CompanyName,OrganisationNumber,StreetAddress,Postcode,City,AccountNumber,CompanyPhoneNumber,SelectedCollectiveAgreementId,CollectiveAgreementSpecName")] CareCompany careCompany)
        {
            ModelState.Remove(nameof(CareCompany.CollectiveAgreementSpecName));
            if (ModelState.IsValid)
            {
                db.CareCompanies.Add(careCompany);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(careCompany);
        }

        // GET: CareCompanies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CareCompany careCompany = db.CareCompanies.Find(id);
            if (careCompany == null)
            {
                return HttpNotFound();
            }
            CareCompanyEditVM careCompanyEditVM = new CareCompanyEditVM();
            careCompanyEditVM.CareCompany = careCompany;
            careCompanyEditVM.CareCompanyId = (int)id;

            List<SelectListItem> collectiveAgreements = new List<SelectListItem>();
            collectiveAgreements = db.CollectiveAgreementHeaders.ToList().ConvertAll(c => new SelectListItem
            {
                Value = $"{c.Id}",
                Text = c.Name
            });
            careCompanyEditVM.CollectiveAgreement = new SelectList(collectiveAgreements, "Value", "Text");
            careCompanyEditVM.SelectedCollectiveAgreementId = careCompany.SelectedCollectiveAgreementId;
            return View(careCompanyEditVM);
        }

        // POST: CareCompanies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CareCompanyId,CareCompany,SelectedCollectiveAgreementId,CollectiveAgreement")] CareCompanyEditVM careCompanyEditVM, string submitButton)
        {
            //ModelState.Remove(nameof(CareCompany.CollectiveAgreementSpecName));
            if (ModelState.IsValid)
            {
                if (submitButton == "Spara")
                {
                    var careCompany = db.CareCompanies.Where(c => c.Id == careCompanyEditVM.CareCompanyId).FirstOrDefault();
                    careCompany.CompanyName = careCompanyEditVM.CareCompany.CompanyName;
                    careCompany.OrganisationNumber = careCompanyEditVM.CareCompany.OrganisationNumber;
                    careCompany.StreetAddress = careCompanyEditVM.CareCompany.StreetAddress;
                    careCompany.Postcode = careCompanyEditVM.CareCompany.Postcode;
                    careCompany.City = careCompanyEditVM.CareCompany.City;
                    careCompany.CompanyPhoneNumber = careCompanyEditVM.CareCompany.CompanyPhoneNumber;
                    careCompany.AccountNumber = careCompanyEditVM.CareCompany.AccountNumber;
                    careCompany.CollectiveAgreementName = careCompanyEditVM.CareCompany.CollectiveAgreementName;
                    careCompany.CollectiveAgreementSpecName = careCompanyEditVM.CareCompany.CollectiveAgreementSpecName;
                    careCompany.SelectedCollectiveAgreementId = careCompanyEditVM.CareCompany.SelectedCollectiveAgreementId;
                    db.Entry(careCompany).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index", "Claims");
            }
            return View();
        }

        // GET: CareCompanies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CareCompany careCompany = db.CareCompanies.Find(id);
            if (careCompany == null)
            {
                return HttpNotFound();
            }
            return View(careCompany);
        }

        // POST: CareCompanies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CareCompany careCompany = db.CareCompanies.Find(id);
            db.CareCompanies.Remove(careCompany);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Ombud
        public ActionResult IndexOmbud()
        {
            if (User.IsInRole("Ombud"))
            {
                var currentId = User.Identity.GetUserId();
                ApplicationUser currentUser = db.Users.Where(u => u.Id == currentId).FirstOrDefault();
                var companyId = currentUser.CareCompanyId;
                var companyName = db.CareCompanies.Where(c => c.Id == companyId).FirstOrDefault().CompanyName;
                var ombuds = db.Users.Where(u => u.CareCompanyId == companyId).OrderBy(u => u.LastName).ToList();

                OmbudIndexVM ombudIndexVM = new OmbudIndexVM();
                List<OmbudForVM> ombudForVMList = new List<OmbudForVM>();
                foreach (var ombud in ombuds)
                {
                    OmbudForVM ombudForVM = new OmbudForVM();
                    ombudForVM.Id = ombud.Id;
                    ombudForVM.FirstName = ombud.FirstName;
                    ombudForVM.LastName = ombud.LastName;
                    ombudForVM.SSN = ombud.SSN;
                    ombudForVM.Email = ombud.Email;
                    ombudForVM.PhoneNumber = ombud.PhoneNumber;
                    ombudForVMList.Add(ombudForVM);
                }
                ombudIndexVM.OmbudForVMList = ombudForVMList;
                ombudIndexVM.CareCompanyId = (int)companyId;
                ombudIndexVM.CareCompanyName = companyName;
                ombudIndexVM.CurrentUserId = currentId;
                return View("IndexOmbud", ombudIndexVM);
            }
            return View();
        }

        // GET: Ombud/Details/5
        public ActionResult DetailsOmbud(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser ombud = db.Users.Where(u => u.Id == id).FirstOrDefault();
            if (ombud == null)
            {
                return HttpNotFound();
            }
            return View(ombud);
        }

        // GET: Ombud/Edit/5
        public ActionResult EditOmbud(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser ombud = db.Users.Where(u => u.Id == id).FirstOrDefault();
            if (ombud == null)
            {
                return HttpNotFound();
            }
            ApplicationUser currentUser = db.Users.Where(u => u.Id == id).FirstOrDefault();
            var companyId = currentUser.CareCompanyId;
            UserEditVM userEditVM = new UserEditVM();
            userEditVM.UserId = id;
            userEditVM.FirstName = currentUser.FirstName;
            userEditVM.LastName = currentUser.LastName;
            userEditVM.PhoneNumber = currentUser.PhoneNumber;
            userEditVM.Email = currentUser.Email;
            return View(userEditVM);
        }

        // POST: Ombud/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOmbud([Bind(Include = "Id,FirstName,LastName,LastLogon,CareCompanyId,SSN,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(applicationUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(applicationUser);
        }

        // GET: Ombud/Delete/5
        public ActionResult DeleteOmbud(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            var ombud = new DeleteOmbudVM
            {
                Id = applicationUser.Id,
                FirstName = applicationUser.FirstName,
                LastName = applicationUser.LastName,
                SSN = applicationUser.SSN,
                Email = applicationUser.Email,
                PhoneNumber = applicationUser.PhoneNumber
            };
            return View(ombud);
        }

        // POST: Ombud/Delete/5
        [HttpPost, ActionName("DeleteOmbud")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedOmbud(string id, string submitButton)
        {
            if (submitButton == "Bekräfta")
            {
                ApplicationUser applicationUser = db.Users.Find(id);
                db.Users.Remove(applicationUser);
                db.SaveChanges();
            }
            return RedirectToAction("IndexOmbud");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
