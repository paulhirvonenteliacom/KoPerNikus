﻿using System;
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
using System.Text.RegularExpressions;

namespace Sjuklöner.Controllers
{
    [Authorize]
    public class CareCompaniesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CareCompanies
        [Authorize(Roles = "Admin, Ombud")]
        public ActionResult Index()
        {
            if (User.IsInRole("Ombud"))
            {
                var currentId = User.Identity.GetUserId();
                ApplicationUser currentUser = db.Users.Where(u => u.Id == currentId).FirstOrDefault();
                var companyId = currentUser.CareCompanyId;
                return RedirectToAction("Edit", new { id = companyId });
            }

            CareCompanyIndexVM companyIndexVM = new CareCompanyIndexVM();

            var companies = db.CareCompanies.ToList();

            companyIndexVM.CareCompanyList = companies;

            var users = db.Users.ToList();
            companyIndexVM.UserList = users;

            return View(companyIndexVM);          
        }

        // GET: CareCompanies/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return RedirectToAction("Index", "Home");               
            }
            CareCompany careCompany = db.CareCompanies.Find(id);
            if (careCompany == null)
            {
                return HttpNotFound();
            }      

            CareCompanyDetailsVM detailsVM = new CareCompanyDetailsVM();
            detailsVM.IsActive = careCompany.IsActive;
            detailsVM.CompanyName = careCompany.CompanyName;
            detailsVM.OrganisationNumber = careCompany.OrganisationNumber;
            detailsVM.StreetAddress = careCompany.StreetAddress;
            detailsVM.Postcode = careCompany.Postcode;
            detailsVM.City = careCompany.City;
            detailsVM.AccountNumber = careCompany.AccountNumber;
            detailsVM.CompanyPhoneNumber = careCompany.CompanyPhoneNumber;

            CollectiveAgreementHeader collectiveAgreement = db.CollectiveAgreementHeaders.Where(c => c.Id == careCompany.SelectedCollectiveAgreementId).SingleOrDefault();
            if (collectiveAgreement != null)
            {
                detailsVM.CollectiveAgreementName = collectiveAgreement.Name;
            }

            detailsVM.CollectiveAgreementSpecName = careCompany.CollectiveAgreementSpecName;

            return View(detailsVM);
        }

        // GET: CareCompanies/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            var vm = new CareCompanyCreateVM();
            List<SelectListItem> collectiveAgreements = new List<SelectListItem>();
            collectiveAgreements = new ApplicationDbContext().CollectiveAgreementHeaders.ToList().ConvertAll(c => new SelectListItem
            {
                Value = $"{c.Id}",
                Text = c.Name
            });
            vm.CollectiveAgreements = new SelectList(collectiveAgreements, "Value", "Text");
            return View(vm);

        }

        // POST: CareCompanies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(CareCompanyCreateVM model)
        {
            if (db.CareCompanies.Where(c => c.OrganisationNumber == model.OrganisationNumber).Any())
            {
                ModelState.AddModelError("OrganisationNumber", "Det finns redan ett bolag med det organisationsnumret.");
            }

            if (ModelState.IsValid)
            {
                CareCompany company = new CareCompany()
                {
                    IsActive = true,
                    CompanyPhoneNumber = model.CompanyPhoneNumber,
                    Postcode = model.Postcode,
                    City = model.City,
                    OrganisationNumber = model.OrganisationNumber,
                    StreetAddress = model.StreetAddress,
                    SelectedCollectiveAgreementId = model.SelectedCollectiveAgreementId,
                    CollectiveAgreementSpecName = model.CollectiveAgreementSpecName,
                    AccountNumber = model.AccountNumber,
                    CompanyName = model.CompanyName
                };

                db.CareCompanies.Add(company);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            List<SelectListItem> collectiveAgreements = new List<SelectListItem>();
            collectiveAgreements = new ApplicationDbContext().CollectiveAgreementHeaders.ToList().ConvertAll(c => new SelectListItem
            {
                Value = $"{c.Id}",
                Text = c.Name
            });
            model.CollectiveAgreements = new SelectList(collectiveAgreements, "Value", "Text");
            // If we got this far, something failed, redisplay form
            return View(model);

            /*
            ModelState.Remove(nameof(CareCompany.CollectiveAgreementSpecName));
            if (ModelState.IsValid)
            {
                db.CareCompanies.Add(careCompany);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(careCompany);
            */
        }

        // GET: CareCompanies/Edit/5
        [Authorize(Roles = "Admin, Ombud")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return RedirectToAction("Index", "Home");              
            }
            CareCompany careCompany = db.CareCompanies.Find(id);
            if (careCompany == null)
            {
                return HttpNotFound();
            }

            // It should not be possible to update passive Companies
            if (User.IsInRole("Admin") && careCompany.IsActive == false)
            {
                return RedirectToAction("Index");
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
        [Authorize(Roles = "Admin, Ombud")]
        public ActionResult Edit([Bind(Include = "CareCompanyId,CareCompany,SelectedCollectiveAgreementId,CollectiveAgreement")] CareCompanyEditVM careCompanyEditVM, string submitButton)
        {
            //ModelState.Remove(nameof(CareCompany.CollectiveAgreementSpecName));
            if (db.CareCompanies.Where(c => (c.OrganisationNumber == careCompanyEditVM.CareCompany.OrganisationNumber && c.Id != careCompanyEditVM.CareCompany.Id)).Any())
            {
                ModelState.AddModelError("", "Det finns redan ett bolag med det organisationsnumret.");
            }

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
                if (User.IsInRole("Ombud"))
                {
                    return RedirectToAction("Index", "Claims");
                }

                // User is in role "Admin"
                return RedirectToAction("Index");
            }

            collectiveAgreements = db.CollectiveAgreementHeaders.ToList().ConvertAll(c => new SelectListItem
            {
                Value = $"{c.Id}",
                Text = c.Name
            });
            careCompanyEditVM.CollectiveAgreement = new SelectList(collectiveAgreements, "Value", "Text");
            return View(careCompanyEditVM);
        }

        // GET: CareCompanies/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return RedirectToAction("Index", "Home");              
            }
            CareCompany careCompany = db.CareCompanies.Find(id);
            if (careCompany == null)
            {
                return HttpNotFound();
            }

            // It should not be possible to delete passive Companies
            if (careCompany.IsActive == false)
            {
                return RedirectToAction("Index");
            }

            CareCompanyDeleteVM deleteVM = new CareCompanyDeleteVM();
            deleteVM.CareCompanyId = careCompany.Id;
            deleteVM.CompanyName = careCompany.CompanyName;
            deleteVM.OrganisationNumber = careCompany.OrganisationNumber;
            deleteVM.StreetAddress = careCompany.StreetAddress;
            deleteVM.Postcode = careCompany.Postcode;
            deleteVM.City = careCompany.City;
            deleteVM.AccountNumber = careCompany.AccountNumber;
            deleteVM.CompanyPhoneNumber = careCompany.CompanyPhoneNumber;

            CollectiveAgreementHeader collectiveAgreement = db.CollectiveAgreementHeaders.Where(c => c.Id == careCompany.SelectedCollectiveAgreementId).SingleOrDefault();
            if (collectiveAgreement != null)
            {
                deleteVM.CollectiveAgreementName = collectiveAgreement.Name;
            }

            deleteVM.CollectiveAgreementSpecName = careCompany.CollectiveAgreementSpecName;

            return View(deleteVM);
        }

        // POST: CareCompanies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id, string submitButton)
        {
            if (submitButton == "Bekräfta")
            {
                // All Assistants connected to this CareCompany will be automatically deleted
                // because of Cascade Delete in table dbo.Assistants
                
                // All Users connected to this CareCompany should be deleted
                var usersToDelete = db.Users.Where(u => u.CareCompanyId == id).ToList();
                foreach(ApplicationUser user in usersToDelete)
                {
                    db.Users.Remove(user);                   
                }              

                // All Unsent Claims(Claims with StatusId == 2) and connected to this company should be deleted
                var unsentClaims = db.Claims.Where(c => (c.CareCompanyId == id && c.ClaimStatusId == 2)).ToList();
                foreach(var claim in unsentClaims)
                {
                    if (claim.CompletionStage > 1)
                    {
                        db.ClaimDays.RemoveRange(db.ClaimDays.Where(c => c.ReferenceNumber == claim.ReferenceNumber));
                    }
                    if (claim.CompletionStage >= 2)
                    {
                        db.ClaimCalculations.RemoveRange(db.ClaimCalculations.Where(c => c.ReferenceNumber == claim.ReferenceNumber));
                    }
                    if (claim.CompletionStage >= 4)
                    {
                        if (claim.Documents.Count() > 0)
                        {
                            db.Documents.RemoveRange(db.Documents.Where(d => d.ReferenceNumber == claim.ReferenceNumber));
                        }
                    }
                }
                db.SaveChanges();

                // We don't want sent Claims to be deleted when the Company is deleted
                // Because of that we create a new CareCompany with status Passive, and copy all the other properties
                // to this passive Company from the Company that will be deleted  
                CareCompany companyToDelete = db.CareCompanies.Find(id);

                CareCompany passiveCompany = new CareCompany()
                {
                    IsActive = false,
                    CompanyPhoneNumber = companyToDelete.CompanyPhoneNumber,
                    Postcode = companyToDelete.Postcode,
                    City = companyToDelete.City,
                    OrganisationNumber = companyToDelete.OrganisationNumber,
                    StreetAddress = companyToDelete.StreetAddress,
                    SelectedCollectiveAgreementId = companyToDelete.SelectedCollectiveAgreementId,
                    CollectiveAgreementName = companyToDelete.CollectiveAgreementName,
                    CollectiveAgreementSpecName = companyToDelete.CollectiveAgreementSpecName,
                    AccountNumber = companyToDelete.AccountNumber,
                    CompanyName = companyToDelete.CompanyName
                };
                db.CareCompanies.Add(passiveCompany);
                db.SaveChanges();               
                
                // All the Claims that has been sent should point to the passive Company when the active Company has been deleted 
                var sentClaims = db.Claims.Where(c => (c.CareCompanyId == id && c.ClaimStatusId != 2)).ToList();

                foreach (var claim in sentClaims)
                {
                    claim.CareCompanyId = passiveCompany.Id;
                    db.Entry(claim).State = EntityState.Modified;
                    db.SaveChanges();                
                }                                

                db.CareCompanies.Remove(companyToDelete);
                db.SaveChanges();                
            }

            return RedirectToAction("Index");
        }

        // GET: Ombud
        [Authorize(Roles = "Ombud")]
        public ActionResult IndexOmbud()
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

        // GET: Ombud/Details/5
        [Authorize(Roles = "Admin, Ombud")]
        public ActionResult DetailsOmbud(string id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return RedirectToAction("Index", "Home");               
            }
            ApplicationUser ombud = db.Users.Where(u => u.Id == id).FirstOrDefault();
            if (ombud == null)
            {
                return HttpNotFound();
            }
            //var currentId = User.Identity.GetUserId();
            //ApplicationUser currentUser = db.Users.Where(u => u.Id == currentId).FirstOrDefault();
            var companyId = ombud.CareCompanyId;
            var companyName = db.CareCompanies.Where(c => c.Id == companyId).FirstOrDefault().CompanyName;

            OmbudEditVM ombudVM = new OmbudEditVM();
            ombudVM.CareCompanyName = companyName;
            ombudVM.FirstName = ombud.FirstName;
            ombudVM.LastName = ombud.LastName;
            ombudVM.SSN = ombud.SSN;
            ombudVM.Email = ombud.Email;
            ombudVM.PhoneNumber = ombud.PhoneNumber;
            return View("DetailsOmbud", ombudVM);
        }

        /*// GET: Ombud/Create
        [Authorize(Roles = "Admin, Ombud")]
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
        [Authorize(Roles = "Admin, Ombud")]
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
        [Authorize(Roles = "Admin, Ombud")]
        public ActionResult EditOmbud(string id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return RedirectToAction("Index", "Home");                
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
        [Authorize(Roles = "Admin, Ombud")]
        //public ActionResult EditOmbud([Bind(Include = "Id,FirstName,LastName,LastLogon,CareCompanyId,SSN,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser applicationUser)
        public ActionResult EditOmbud([Bind(Include = "Id,FirstName,LastName,CareCompanyId,CareCompanyName,SSN,Email,PhoneNumber")] OmbudEditVM ombudEditVM, string submitButton)
        {
            var currentId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.Where(u => u.Id == currentId).FirstOrDefault();

            bool errorFound = false;
            //Check that the SSN has the correct format
            if (!string.IsNullOrWhiteSpace(ombudEditVM.SSN))
            {
                ombudEditVM.SSN = ombudEditVM.SSN.Trim();
                Regex regex = new Regex(@"^([1-9][0-9]{3})(((0[13578]|1[02])(0[1-9]|[12][0-9]|3[01]))|((0[469]|11)(0[1-9]|[12][0-9]|30))|(02(0[1-9]|[12][0-9])))[-]?\d{4}$");
                Match match = regex.Match(ombudEditVM.SSN);
                if (!match.Success)
                {
                    ModelState.AddModelError("SSN", "Ej giltigt personnummer. Formaten YYYYMMDD-NNNN och YYYYMMDDNNNN är giltiga.");
                    errorFound = true;
                }
            }
            else
            {
                errorFound = true;
            }

            //Check that the ombud is born in the 20th or 21st century
            if (!errorFound)
            {
                if (int.Parse(ombudEditVM.SSN.Substring(0, 2)) != 19 && int.Parse(ombudEditVM.SSN.Substring(0, 2)) != 20)
                {
                    ModelState.AddModelError("SSN", "Ombudet måste vara fött på 1900- eller 2000-talet.");
                    errorFound = true;
                }
            }

            //Check that the ombud is at least 18 years old and was not born in the future:-)
            if (!errorFound)
            {
                DateTime ombudBirthday = new DateTime(int.Parse(ombudEditVM.SSN.Substring(0, 4)), int.Parse(ombudEditVM.SSN.Substring(4, 2)), int.Parse(ombudEditVM.SSN.Substring(6, 2)));
                if (ombudBirthday.Date > DateTime.Now.Date)
                {
                    ModelState.AddModelError("SSN", "Födelsedatumet får inte vara senare än idag.");
                    errorFound = true;
                }
                else if (ombudBirthday > DateTime.Now.AddYears(-18))
                {
                    ModelState.AddModelError("SSN", "Ombudet måste vara minst 18 år.");
                    errorFound = true;
                }
            }

            //Check if there is an ombud with the same SSN already in the company. The same ombud is allowed in another company.
            if (!errorFound)
            {
                var twinOmbud = db.Users.Where(u => u.SSN == ombudEditVM.SSN).Where(u => u.Id != ombudEditVM.Id).FirstOrDefault();
                if (twinOmbud != null && twinOmbud.CareCompanyId == currentUser.CareCompanyId)
                {
                    ModelState.AddModelError("SSN", "Det finns redan ett ombud med detta personnummer");
                    errorFound = true;
                }
            }

            if (!errorFound)
            {
                if (ombudEditVM.SSN.Length == 12)
                {
                    ombudEditVM.SSN = ombudEditVM.SSN.Insert(8, "-");
                }
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
                    if (User.IsInRole("Admin"))
                    {
                        return RedirectToAction("IndexAllOmbuds", "Account");
                    }

                    // User is in role "Ombud"                   
                    return RedirectToAction("IndexOmbud");
                }
            }

            return View(ombudEditVM);
        }

        // GET: Ombud/Delete/5
        [Authorize(Roles = "Ombud")]
        public ActionResult DeleteOmbud(string id)
        {
            if (id == null || id == User.Identity.GetUserId())
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return RedirectToAction("Index", "Home");             
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
        [Authorize(Roles = "Ombud")]
        public ActionResult DeleteConfirmedOmbud(string id, string submitButton)
        {
            if (submitButton == "Bekräfta")
            {
                var myId = User.Identity.GetUserId();
                var me = db.Users.Where(u => u.Id == myId).FirstOrDefault();
                ApplicationUser applicationUser = db.Users.Find(id);
                if (applicationUser != me && applicationUser.CareCompanyId == me.CareCompanyId)
                {
                    var claimsForOmbud = db.Claims.Where(c => c.OwnerId == applicationUser.Id).ToList();

                    foreach (var claim in claimsForOmbud)
                    {
                        // All Unsent Claims (Claims with StatusId == 2) for the deleted Ombud should be moved to a "Dummy ombud"
                        if (claim.ClaimStatusId == 2)
                        {
                            claim.OwnerId = "";
                            claim.OmbudFirstName = "-";
                            claim.OmbudLastName = "-";
                            claim.OmbudPhoneNumber = "-";
                            claim.OmbudEmail = "-";
                            db.Entry(claim).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    db.Users.Remove(applicationUser);
                    db.SaveChanges();
                }                            
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
