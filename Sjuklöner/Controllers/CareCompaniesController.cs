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
    [Authorize]
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
            careCompanyEditVM.SelectedCollectiveAgreementId = careCompanyEditVM.CareCompany.SelectedCollectiveAgreementId;
            List<SelectListItem> collectiveAgreements = new List<SelectListItem>();
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

                    //collectiveAgreements = db.CollectiveAgreementHeaders.ToList().ConvertAll(c => new SelectListItem
                    //{
                    //    Value = $"{c.Id}",
                    //    Text = c.Name
                    //});
                    //careCompanyEditVM.CollectiveAgreement = new SelectList(collectiveAgreements, "Value", "Text");
                    //return View(careCompanyEditVM);
                }
                return RedirectToAction("Index", "Claims");
            }

            collectiveAgreements = db.CollectiveAgreementHeaders.ToList().ConvertAll(c => new SelectListItem
            {
                Value = $"{c.Id}",
                Text = c.Name
            });
            careCompanyEditVM.CollectiveAgreement = new SelectList(collectiveAgreements, "Value", "Text");
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
            OmbudEditVM ombudVM = new OmbudEditVM();
            ombudVM.FirstName = ombud.FirstName;
            ombudVM.LastName = ombud.LastName;
            ombudVM.SSN = ombud.SSN;
            ombudVM.Email = ombud.Email;
            ombudVM.PhoneNumber = ombud.PhoneNumber;
            return View("DetailsOmbud", ombudVM);
        }

        /*// GET: Ombud/Create
        public ActionResult CreateOmbud()
        {
            OmbudCreateVM ombudCreateVM = new OmbudCreateVM();
            var currentId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.Where(u => u.Id == currentId).FirstOrDefault();
            ombudCreateVM.CareCompanyId = (int)currentUser.CareCompanyId;
            ombudCreateVM.CareCompanyName = db.CareCompanies.Where(c => c.Id == ombudCreateVM.CareCompanyId).FirstOrDefault().CompanyName;
            return View("CreateOmbud", ombudCreateVM);
        }*/

        /*/ POST: Ombud/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOmbud([Bind(Include = "Id,FirstName,LastName,CareCompanyId,CareCompanyName,SSN,Email,PhoneNumber")] OmbudCreateVM ombudCreateVM, string submitButton)
        {
            if (submitButton == "Spara")
            {
                ModelState.Remove(nameof(OmbudCreateVM.Id));
                if (ModelState.IsValid)
                {
                    ApplicationUser newOmbud = new ApplicationUser();
                    newOmbud.FirstName = ombudCreateVM.FirstName;
                    newOmbud.LastName = ombudCreateVM.LastName;
                    newOmbud.CareCompanyId = ombudCreateVM.CareCompanyId;
                    newOmbud.UserName = $"{ombudCreateVM.FirstName} {ombudCreateVM.LastName}";
                    newOmbud.Email = ombudCreateVM.Email;
                    newOmbud.PhoneNumber = ombudCreateVM.PhoneNumber;
                    newOmbud.SSN = ombudCreateVM.SSN;
                    newOmbud.LastLogon = DateTime.Now;
                    db.Users.Add(newOmbud);
                    db.SaveChanges();
                    return RedirectToAction("IndexOmbud");
                }
            }
            return View();
        }
        */
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
            OmbudEditVM ombudEditVM = new OmbudEditVM();
            ombudEditVM.Id = id;
            ombudEditVM.CareCompanyId = (int)currentUser.CareCompanyId;
            ombudEditVM.FirstName = currentUser.FirstName;
            ombudEditVM.LastName = currentUser.LastName;
            ombudEditVM.PhoneNumber = currentUser.PhoneNumber;
            ombudEditVM.Email = currentUser.Email;
            ombudEditVM.SSN = currentUser.SSN;
            ombudEditVM.CareCompanyName = db.CareCompanies.Where(c => c.Id == ombudEditVM.CareCompanyId).FirstOrDefault().CompanyName;
            return View("EditOmbud", ombudEditVM);
        }

        // POST: Ombud/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult EditOmbud([Bind(Include = "Id,FirstName,LastName,LastLogon,CareCompanyId,SSN,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser applicationUser)
        public ActionResult EditOmbud([Bind(Include = "Id,FirstName,LastName,CareCompanyId,CareCompanyName,SSN,Email,PhoneNumber")] OmbudEditVM ombudEditVM, string submitButton)
        {
            //Check that the ombud SSN is 12 or 13 characters. If it is 13 then the 9th shall be a "-". t will always be saved as 13 characters where the 9th is a "-".
            bool errorFound = false;
            if (ombudEditVM.SSN.Length == 12 || ombudEditVM.SSN.Length == 13)
            {
                if (ombudEditVM.SSN.Length == 12 && ombudEditVM.SSN.Contains("-"))
                {
                    errorFound = true;
                }
                if (ombudEditVM.SSN.Length == 12 && !errorFound)
                {
                    ombudEditVM.SSN = ombudEditVM.SSN.Insert(8, "-");
                }
                if (ombudEditVM.SSN.Length == 13 && ombudEditVM.SSN.Substring(8, 1) != "-")
                {
                    errorFound = true;
                }
            }
            else
            {
                errorFound = true;
            }
            if (errorFound)
            {
                ModelState.AddModelError("SSN", "Ej giltigt personnummer. Formaten YYYYMMDD-NNNN och YYYYMMDDNNNN är giltiga.");
            }

            if (submitButton == "Spara")
            {
                var possibleTwin = db.Users.Where(u => u.Email == ombudEditVM.Email).FirstOrDefault();
                if (possibleTwin != null && possibleTwin.Id != ombudEditVM.Id)
                    ModelState.AddModelError("Email", "Det finns redan en användare med den emailadressen");
                if (ModelState.IsValid)
                {
                    var editedOmbud = db.Users.Where(u => u.Id == ombudEditVM.Id).FirstOrDefault();
                    editedOmbud.FirstName = ombudEditVM.FirstName;
                    editedOmbud.LastName = ombudEditVM.LastName;
                    editedOmbud.CareCompanyId = ombudEditVM.CareCompanyId;
                    editedOmbud.UserName = ombudEditVM.Email;
                    editedOmbud.Email = ombudEditVM.Email;
                    editedOmbud.PhoneNumber = ombudEditVM.PhoneNumber;
                    editedOmbud.SSN = ombudEditVM.SSN;
                    db.Entry(editedOmbud).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("IndexOmbud");
                }
            }
            return View();
        }

        // GET: Ombud/Delete/5
        public ActionResult DeleteOmbud(string id)
        {
            if (id == null || id == User.Identity.GetUserId())
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            OmbudDeleteVM ombudVM = new OmbudDeleteVM();
            ombudVM.Id = id;
            ombudVM.FirstName = applicationUser.FirstName;
            ombudVM.LastName = applicationUser.LastName;
            ombudVM.SSN = applicationUser.SSN;
            ombudVM.PhoneNumber = applicationUser.PhoneNumber;
            ombudVM.Email = applicationUser.Email;
            return View("DeleteOmbud", ombudVM);
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
