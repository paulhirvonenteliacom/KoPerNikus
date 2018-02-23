﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Sjuklöner.Models;
using Sjuklöner.Viewmodels;
using Microsoft.AspNet.Identity;
using System.Globalization;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Net.Mail;
using System.Configuration;
using System.Xml;

namespace Sjuklöner.Controllers
{
    [Authorize]

    public class ClaimsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public bool demoMode = false; //Change this variable to true in order to run in demo mode.

        // GET: Claims
        public ActionResult Index()
        {
            if (User.IsInRole("Ombud"))
            {
                return RedirectToAction("IndexPageOmbud", "Claims");
            }
            else if (User.IsInRole("AdministrativeOfficial"))
            {
                return RedirectToAction("IndexPageAdmOff", "Claims");
            }
            else
            {
                return View(db.Claims.ToList());
            }
        }

        // GET: Claims
        public ActionResult IndexPageOmbud(string searchString, string searchBy = "Referensnummer")
        {
            IndexPageOmbudVM indexPageOmbudVM = new IndexPageOmbudVM();

            var me = db.Users.Find(User.Identity.GetUserId());
            int companyId = (int)me.CareCompanyId;

            var claims = db.Claims.Where(c => c.CareCompanyId == companyId).OrderByDescending(c => c.StatusDate).ToList();
            if (claims.Count > 0)
            {
                var decidedClaims = claims.Where(c => c.ClaimStatusId == 1);
                var draftClaims = claims.Where(c => c.ClaimStatusId == 2);
                var underReviewClaims = claims.Where(c => c.ClaimStatusId == 3 || c.ClaimStatusId == 5);
                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    decidedClaims = Search(decidedClaims, searchString, searchBy);
                    draftClaims = Search(draftClaims, searchString, searchBy);
                    underReviewClaims = Search(underReviewClaims, searchString, searchBy);
                }
                indexPageOmbudVM.DecidedClaims = decidedClaims.ToList(); //Old "Rejected
                indexPageOmbudVM.DraftClaims = draftClaims.ToList();
                indexPageOmbudVM.UnderReviewClaims = underReviewClaims.ToList();
            }

            return View("IndexPageOmbud", indexPageOmbudVM);
        }


        // GET: Claims
        [Authorize(Roles = "AdministrativeOfficial")]
        public ActionResult IndexPageAdmOff(string searchString, string searchBy = "Referensnummer")
        {
            IndexPageAdmOffVM indexPageAdmOffVM = new IndexPageAdmOffVM();

            var me = db.Users.Find(User.Identity.GetUserId());

            var claims = db.Claims.Include(c => c.CareCompany).OrderByDescending(c => c.StatusDate).ToList();
            if (claims.Count > 0)
            {
                var decidedClaims = claims.Where(c => c.ClaimStatusId == 1);
                var inInobxClaims = claims.Where(c => c.ClaimStatusId == 5);
                var underReviewClaims = claims.Where(c => c.ClaimStatusId == 3);
                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    decidedClaims = Search(decidedClaims, searchString, searchBy);
                    inInobxClaims = Search(inInobxClaims, searchString, searchBy);
                    underReviewClaims = Search(underReviewClaims, searchString, searchBy);
                }
                indexPageAdmOffVM.DecidedClaims = decidedClaims.ToList();
                indexPageAdmOffVM.InInboxClaims = inInobxClaims.ToList();
                indexPageAdmOffVM.UnderReviewClaims = underReviewClaims.ToList();
            }

            return View("IndexPageAdmOff", indexPageAdmOffVM);
        }

        private IEnumerable<Claim> Search(IEnumerable<Claim> Claims, string searchString, string searchBy)
        {
            if (searchBy == "Referensnummer")
                Claims = Claims.Where(c => c.ReferenceNumber.Contains(searchString));
            else if (searchBy == "CSSN")
                Claims = Claims.Where(c => c.CustomerSSN.Contains(searchString));
            else if (searchBy == "ASSN")
                Claims = Claims.Where(c => c.RegAssistantSSN.Contains(searchString));

            return Claims;
        }

        // GET: Claims/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Claim claim = db.Claims.Find(id);
            if (claim == null)
            {
                return HttpNotFound();
            }
            return View(claim);
        }

        // GET: Claims/Create1
        [HttpGet]
        public ActionResult Create1(string refNumber)
        {
            Create1VM create1VM = new Create1VM();

            if (!demoMode && refNumber == null)  //new claim
            {
                var currentId = User.Identity.GetUserId();
                ApplicationUser currentUser = db.Users.Where(u => u.Id == currentId).FirstOrDefault();
                var companyId = currentUser.CareCompanyId;
                create1VM.OrganisationNumber = db.CareCompanies.Where(c => c.Id == companyId).FirstOrDefault().OrganisationNumber;

                var assistants = db.Assistants.Where(a => a.CareCompanyId == companyId).OrderBy(a => a.LastName).ToList();
                List<int> assistantIds = new List<int>(); //This list is required in order to be able to map the selected ddl ids to Assistant records in the db.
                var assistantDdlString = new List<SelectListItem>();
                for (int i = 0; i < assistants.Count(); i++)
                {
                    assistantDdlString.Add(new SelectListItem() { Text = assistants[i].AssistantSSN + ", " + assistants[i].FirstName + " " + assistants[i].LastName, Value = assistants[i].Id.ToString() });
                    assistantIds.Add(assistants[i].Id);
                }
                create1VM.RegularAssistants = assistantDdlString;
                create1VM.SubstituteAssistants = assistantDdlString;
                create1VM.AssistantIds = assistantIds;

                create1VM.FirstDayOfSicknessDate = DateTime.Now.AddDays(-1);
                create1VM.LastDayOfSicknessDate = DateTime.Now.AddDays(-1);
                return View("Create1", create1VM);
            }
            else if (refNumber != null) //This is an existing claim (either demo or no demo)
            {
                return View("Create1", LoadExistingValuesCreate1(refNumber));
            }
            else if (demoMode && refNumber == null) //Demo and new claim
            {
                create1VM = SetDefaultValuesCreate1();
                return View("Create1", create1VM);
            }
            return View("Create1", create1VM);
        }

        private Create1VM LoadExistingValuesCreate1(String refNumber)
        {
            var claim = db.Claims.Where(c => c.ReferenceNumber == refNumber).FirstOrDefault();

            Create1VM create1VM = new Create1VM();
            var currentId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.Where(u => u.Id == currentId).FirstOrDefault();
            var companyId = currentUser.CareCompanyId;

            var assistants = db.Assistants.Where(a => a.CareCompanyId == companyId).OrderBy(a => a.LastName).ToList();
            List<int> assistantIds = new List<int>(); //This list is required in order to be able to map the selected ddl ids to Assistant records in the db.
            var regAssistantDdlString = new List<SelectListItem>();
            var subAssistantDdlString = new List<SelectListItem>();
            create1VM.SelectedRegAssistantId = 1;
            create1VM.SelectedSubAssistantId = 1;

            for (int i = 0; i < assistants.Count(); i++)
            {
                regAssistantDdlString.Add(new SelectListItem() { Text = assistants[i].AssistantSSN + ", " + assistants[i].FirstName + " " + assistants[i].LastName, Value = assistants[i].Id.ToString() });
                if (assistants[i].Id == claim.SelectedRegAssistantId)
                {
                    create1VM.SelectedRegAssistantId = claim.SelectedRegAssistantId;
                }
                subAssistantDdlString.Add(new SelectListItem() { Text = assistants[i].AssistantSSN + ", " + assistants[i].FirstName + " " + assistants[i].LastName, Value = assistants[i].Id.ToString() });
                if (assistants[i].Id == claim.SelectedSubAssistantId)
                {
                    create1VM.SelectedSubAssistantId = claim.SelectedSubAssistantId;
                }
                assistantIds.Add(assistants[i].Id);
            }
            create1VM.RegularAssistants = regAssistantDdlString;
            create1VM.SubstituteAssistants = subAssistantDdlString;
            create1VM.AssistantIds = assistantIds;

            create1VM.CustomerName = claim.CustomerName;
            create1VM.CustomerSSN = claim.CustomerSSN;
            create1VM.CustomerAddress = claim.CustomerAddress;
            create1VM.CustomerPhoneNumber = claim.CustomerPhoneNumber;
            create1VM.FirstDayOfSicknessDate = claim.QualifyingDate;
            create1VM.LastDayOfSicknessDate = claim.LastDayOfSicknessDate;
            create1VM.OrganisationNumber = claim.OrganisationNumber;
            create1VM.ClaimNumber = claim.ReferenceNumber;
            create1VM.CompletionStage = (int)claim.CompletionStage;

            return create1VM;
        }

        private Create1VM SetDefaultValuesCreate1()
        {
            Create1VM defaultValuesCreate1VM = new Create1VM();

            defaultValuesCreate1VM.OrganisationNumber = "556881-2118";
            var currentId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.Where(u => u.Id == currentId).FirstOrDefault();
            var companyId = currentUser.CareCompanyId;
            defaultValuesCreate1VM.OrganisationNumber = db.CareCompanies.Where(c => c.Id == companyId).FirstOrDefault().OrganisationNumber;

            var assistants = db.Assistants.Where(a => a.CareCompanyId == companyId).OrderBy(a => a.LastName).ToList();
            var assistantDdlString = new List<SelectListItem>();
            for (int i = 0; i < assistants.Count(); i++)
            {
                assistantDdlString.Add(new SelectListItem() { Text = assistants[i].AssistantSSN + ", " + assistants[i].FirstName + " " + assistants[i].LastName, Value = assistants[i].Id.ToString() });
            }
            defaultValuesCreate1VM.RegularAssistants = assistantDdlString;
            defaultValuesCreate1VM.SubstituteAssistants = assistantDdlString;

            defaultValuesCreate1VM.CustomerSSN = "391025-7246";
            defaultValuesCreate1VM.FirstDayOfSicknessDate = DateTime.Now.AddDays(-4);
            defaultValuesCreate1VM.LastDayOfSicknessDate = DateTime.Now.AddDays(-1);

            return defaultValuesCreate1VM;
        }

        // POST: Claims/Create1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create1(Create1VM create1VM, string refNumber, string submitButton)
        {
            if (ModelState.IsValid)
            {

                if (submitButton == "Till steg 2" || submitButton == "Spara")
                {
                    if (refNumber == null) //new claim
                    {
                        refNumber = SaveNewClaim(create1VM);
                    }
                    else if (refNumber != null) //existing claim
                    {
                        create1VM.ClaimNumber = refNumber;
                        UpdateExistingClaim(create1VM);
                        var claim = db.Claims.Where(c => c.ReferenceNumber == create1VM.ClaimNumber).FirstOrDefault();
                        if (claim.CompletionStage > 1)
                        {
                            AdjustClaimDays(create1VM);
                        }
                    }
                }
                if (submitButton == "Till steg 2")
                {
                    return RedirectToAction("Create2", "Claims", new { refNumber = refNumber });
                }
                else if (submitButton == "Avbryt")
                {
                    return RedirectToAction("IndexPageOmbud");
                }
                else if (submitButton == "Spara")
                {
                    var claim = db.Claims.Where(c => c.ReferenceNumber == refNumber).FirstOrDefault();
                    var currentId = User.Identity.GetUserId();
                    ApplicationUser currentUser = db.Users.Where(u => u.Id == currentId).FirstOrDefault();
                    var companyId = currentUser.CareCompanyId;
                    var assistants = db.Assistants.Where(a => a.CareCompanyId == companyId).OrderBy(a => a.LastName).ToList();
                    var regAssistantDdlString = new List<SelectListItem>();
                    var subAssistantDdlString = new List<SelectListItem>();

                    for (int i = 0; i < assistants.Count(); i++)
                    {
                        regAssistantDdlString.Add(new SelectListItem() { Text = assistants[i].AssistantSSN + ", " + assistants[i].FirstName + " " + assistants[i].LastName, Value = assistants[i].Id.ToString() });
                        subAssistantDdlString.Add(new SelectListItem() { Text = assistants[i].AssistantSSN + ", " + assistants[i].FirstName + " " + assistants[i].LastName, Value = assistants[i].Id.ToString() });
                    }
                    create1VM.RegularAssistants = regAssistantDdlString;
                    create1VM.SubstituteAssistants = subAssistantDdlString;
                    create1VM.CompletionStage = (int)claim.CompletionStage;
                    create1VM.ClaimNumber = claim.ReferenceNumber;

                    return View(create1VM);
                }
                else
                {
                    return RedirectToAction("IndexPageOmbud");
                }
            }
            else
            {
                return View(create1VM);
            }
        }

        private void AdjustClaimDays(Create1VM create1VM)
        {
            //Find existing claim days
            var existingClaimDays = db.ClaimDays.Where(c => c.ReferenceNumber == create1VM.ClaimNumber).OrderBy(c => c.SickDayNumber).ToList();

            //Calculate offsets at the beginning and end of the date range for the existing claim days
            var offsetStart = (create1VM.FirstDayOfSicknessDate.Date - existingClaimDays[0].Date.Date).Days;
            var offsetEnd = (create1VM.LastDayOfSicknessDate.Date - existingClaimDays.Last().Date.Date).Days;
            int claimDayPos = 1;
            int maxIdx;

            List<ClaimDay> newClaimDays = new List<ClaimDay>();

            if (offsetStart < 0) //Add new claim days to the beginning of the sickleave period 
            {
                if (create1VM.LastDayOfSicknessDate.Date < existingClaimDays[0].Date.Date)
                {
                    maxIdx = 1 + (create1VM.LastDayOfSicknessDate.Date - create1VM.FirstDayOfSicknessDate.Date).Days;
                }
                else
                {
                    maxIdx = (existingClaimDays[0].Date.Date - create1VM.FirstDayOfSicknessDate.Date).Days;
                }
                for (int i = 0; i < maxIdx; i++)
                {
                    ClaimDay claimDay = new ClaimDay();
                    claimDay.Date = create1VM.FirstDayOfSicknessDate.AddDays(i);
                    claimDay.DateString = create1VM.FirstDayOfSicknessDate.AddDays(i).ToString(format: "ddd d MMM");
                    claimDay.ReferenceNumber = create1VM.ClaimNumber;
                    claimDay.SickDayNumber = i + 1;
                    newClaimDays.Add(claimDay);
                    claimDayPos++;
                }
                //Copy relevant existing claim days to the new claim day list
                if (create1VM.LastDayOfSicknessDate.Date >= existingClaimDays[0].Date.Date)
                {
                    if (create1VM.LastDayOfSicknessDate.Date < existingClaimDays.Last().Date)
                    {
                        maxIdx = 1 + (create1VM.LastDayOfSicknessDate.Date - existingClaimDays[0].Date.Date).Days;
                    }
                    else
                    {
                        maxIdx = 1 + (existingClaimDays.Last().Date - existingClaimDays[0].Date.Date).Days;
                    }
                    for (int i = 0; i < maxIdx; i++)
                    {
                        existingClaimDays[0].SickDayNumber = claimDayPos;
                        db.Entry(existingClaimDays[0]).State = EntityState.Modified;
                        existingClaimDays.Remove(existingClaimDays[0]);
                        claimDayPos++;
                    }
                }
                //Add new claim days at the end of the list
                if (offsetEnd > 0)
                {
                    for (int i = 0; i < offsetEnd; i++)
                    {
                        ClaimDay claimDay = new ClaimDay();
                        claimDay.ReferenceNumber = create1VM.ClaimNumber;
                        claimDay.SickDayNumber = claimDayPos;
                        claimDay.Date = create1VM.FirstDayOfSicknessDate.AddDays(claimDayPos - 1);
                        claimDay.DateString = create1VM.FirstDayOfSicknessDate.AddDays(claimDayPos - 1).ToString(format: "ddd d MMM");
                        newClaimDays.Add(claimDay);
                        claimDayPos++;
                    }
                }
            }
            else if (offsetStart == 0) //Copy claim days to the new claim day list starting from the beginning of the sickleave period
            {
                if (offsetEnd <= 0)
                {
                    maxIdx = 1 + (create1VM.LastDayOfSicknessDate.Date - create1VM.FirstDayOfSicknessDate.Date).Days;
                }
                else
                {
                    maxIdx = existingClaimDays.Last().SickDayNumber;
                }
                for (int i = 0; i < maxIdx; i++)
                {
                    existingClaimDays.Remove(existingClaimDays[0]);
                    claimDayPos++;
                }
                if (offsetEnd > 0)
                {
                    for (int i = 0; i < offsetEnd; i++)
                    {
                        ClaimDay claimDay = new ClaimDay();
                        claimDay.ReferenceNumber = create1VM.ClaimNumber;
                        claimDay.SickDayNumber = claimDayPos;
                        claimDay.Date = create1VM.FirstDayOfSicknessDate.AddDays(claimDayPos - 1);
                        claimDay.DateString = create1VM.FirstDayOfSicknessDate.AddDays(claimDayPos - 1).ToString(format: "ddd d MMM");
                        newClaimDays.Add(claimDay);
                        claimDayPos++;
                    }
                }
            }
            else if (offsetStart > 0 && offsetStart <= ((existingClaimDays.Last().Date - existingClaimDays[0].Date.Date).Days)) //The updated sickleave period starts somewhere in the existing sick leave period. Copy only the relevant claim days from the existing claim days list.
            {
                if (offsetEnd <= 0)
                {
                    maxIdx = 1 + (create1VM.LastDayOfSicknessDate.Date - create1VM.FirstDayOfSicknessDate.Date).Days;
                }
                else
                {
                    maxIdx = 1 + (existingClaimDays.Last().Date.Date - create1VM.FirstDayOfSicknessDate.Date).Days;
                }
                for (int i = 0; i < maxIdx; i++)
                {
                    existingClaimDays[offsetStart].SickDayNumber = claimDayPos;
                    db.Entry(existingClaimDays[offsetStart]).State = EntityState.Modified;
                    existingClaimDays.Remove(existingClaimDays[offsetStart]);
                    claimDayPos++;
                }
                //Add new claim days at the end of the list
                if (offsetEnd > 0)
                {
                    for (int i = 0; i < offsetEnd; i++)
                    {
                        ClaimDay claimDay = new ClaimDay();
                        claimDay.ReferenceNumber = create1VM.ClaimNumber;
                        claimDay.SickDayNumber = claimDayPos;
                        claimDay.Date = create1VM.FirstDayOfSicknessDate.AddDays(claimDayPos - 1);
                        claimDay.DateString = create1VM.FirstDayOfSicknessDate.AddDays(claimDayPos - 1).ToString(format: "ddd d MMM");
                        newClaimDays.Add(claimDay);
                        claimDayPos++;
                    }
                }
            }
            else //The updated sickleave period is later than the existing and the old and new do not overlap.
            {
                maxIdx = 1 + (create1VM.LastDayOfSicknessDate.Date - create1VM.FirstDayOfSicknessDate.Date).Days;
                for (int i = 0; i < maxIdx; i++)
                {
                    ClaimDay claimDay = new ClaimDay();
                    claimDay.Date = create1VM.FirstDayOfSicknessDate.AddDays(i);
                    claimDay.DateString = create1VM.FirstDayOfSicknessDate.AddDays(i).ToString(format: "ddd d MMM");
                    claimDay.ReferenceNumber = create1VM.ClaimNumber;
                    claimDay.SickDayNumber = i + 1;
                    newClaimDays.Add(claimDay);
                }
            }
            //Remove not neeeded claim days (they are the remaining claim days in the existing claim day list) from the db
            foreach (var claimDay in existingClaimDays)
            {
                db.ClaimDays.Remove(db.ClaimDays.Where(c => c.ReferenceNumber == create1VM.ClaimNumber).Where(c => c.Id == claimDay.Id).First());
            }
            db.ClaimDays.AddRange(newClaimDays);
            db.SaveChanges();
        }

        private string SaveNewClaim(Create1VM create1VM)
        {
            Claim claim = new Claim();
            claim.ReferenceNumber = GenerateReferenceNumber();
            claim.CompletionStage = 1; //CompletionsStage is used for keeping track on what stage in the claim process has been completed. Used when jumping back in the process and also when updating a draft claim.
            claim.ClaimStatusId = 2;  //ClaimStatus.Name = "Utkast"

            var currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.Where(u => u.Id == currentUserId).FirstOrDefault();

            //Save company information
            SaveCompanyInformation(claim, currentUser);

            //Save ombud information
            SaveOmbudInformation(claim, currentUserId, currentUser);

            //Save customer information
            SaveCustomerInformation(create1VM, claim);

            //Save regular assistant information
            SaveRegAssistantInformation(create1VM, claim);

            //Save substitute assistant information
            SaveSubAssistantInformation(create1VM, claim);

            claim.StatusDate = DateTime.Now;
            claim.QualifyingDate = create1VM.FirstDayOfSicknessDate;
            claim.LastDayOfSicknessDate = create1VM.LastDayOfSicknessDate;
            claim.NumberOfSickDays = 1 + (create1VM.LastDayOfSicknessDate.Date - create1VM.FirstDayOfSicknessDate.Date).Days;
            db.Claims.Add(claim);
            db.SaveChanges();
            return claim.ReferenceNumber;
        }

        private static void SaveCustomerInformation(Create1VM create1VM, Claim claim)
        {
            claim.CustomerSSN = create1VM.CustomerSSN;
            claim.CustomerName = create1VM.CustomerName;
            claim.CustomerAddress = create1VM.CustomerAddress;
            claim.CustomerPhoneNumber = create1VM.CustomerPhoneNumber;
        }

        private void SaveSubAssistantInformation(Create1VM create1VM, Claim claim)
        {
            if (create1VM.SelectedSubAssistantId != null)
            {
                var subAssistant = db.Assistants.Where(a => a.Id == create1VM.SelectedSubAssistantId).FirstOrDefault();
                claim.SubAssistantSSN = subAssistant.AssistantSSN;
                claim.SubFirstName = subAssistant.FirstName;
                claim.SubLastName = subAssistant.LastName;
                claim.SubPhoneNumber = subAssistant.PhoneNumber;
                claim.SubEmail = subAssistant.Email;
            }
            else
            {
                claim.StandInSSN = null;
            }
            claim.SelectedSubAssistantId = create1VM.SelectedSubAssistantId;
        }

        private void SaveRegAssistantInformation(Create1VM create1VM, Claim claim)
        {
            if (create1VM.SelectedRegAssistantId != null)
            {
                var regAssistant = db.Assistants.Where(a => a.Id == create1VM.SelectedRegAssistantId).FirstOrDefault();
                claim.RegAssistantSSN = regAssistant.AssistantSSN;
                claim.RegFirstName = regAssistant.FirstName;
                claim.RegLastName = regAssistant.LastName;
                claim.RegPhoneNumber = regAssistant.PhoneNumber;
                claim.RegEmail = regAssistant.Email;
                claim.HourlySalaryAsString = regAssistant.HourlySalary;
                claim.HourlySalary = Convert.ToDecimal(claim.HourlySalaryAsString);
                claim.HolidayPayRateAsString = regAssistant.HolidayPayRate;
                claim.HolidayPayRate = Convert.ToDecimal(regAssistant.HolidayPayRate);
                claim.SickPayRateAsString = "80,00";
                claim.SickPayRate = Convert.ToDecimal("80,00");
                claim.SocialFeeRateAsString = regAssistant.PayrollTaxRate;
                claim.SocialFeeRate = Convert.ToDecimal(regAssistant.PayrollTaxRate);
                claim.PensionAndInsuranceRateAsString = regAssistant.PensionAndInsuranceRate;
                claim.PensionAndInsuranceRate = Convert.ToDecimal(regAssistant.PensionAndInsuranceRate);
            }
            else
            {
                claim.RegAssistantSSN = null;
            }
            claim.SelectedRegAssistantId = create1VM.SelectedRegAssistantId;
        }

        private static void SaveOmbudInformation(Claim claim, string currentUserId, ApplicationUser currentUser)
        {
            claim.OwnerId = currentUserId;
            claim.OmbudFirstName = currentUser.FirstName;
            claim.OmbudLastName = currentUser.LastName;
            claim.OmbudPhoneNumber = currentUser.PhoneNumber;
            claim.OmbudEmail = currentUser.Email;
        }

        private void SaveCompanyInformation(Claim claim, ApplicationUser currentUser)
        {
            var companyId = currentUser.CareCompanyId;
            var careCompany = db.CareCompanies.Where(c => c.Id == companyId).FirstOrDefault();
            claim.CareCompanyId = (int)companyId;
            claim.CompanyName = careCompany.CompanyName;
            claim.OrganisationNumber = careCompany.OrganisationNumber;
            claim.StreetAddress = careCompany.StreetAddress;
            claim.Postcode = careCompany.Postcode;
            claim.City = careCompany.City;
            claim.AccountNumber = careCompany.AccountNumber;
            claim.CompanyPhoneNumber = careCompany.CompanyPhoneNumber;
            claim.CollectiveAgreementName = careCompany.CollectiveAgreementName;
            claim.CollectiveAgreementSpecName = careCompany.CollectiveAgreementSpecName;
        }

        private void UpdateExistingClaim(Create1VM create1VM)
        {
            var claim = db.Claims.Where(c => c.ReferenceNumber == create1VM.ClaimNumber).FirstOrDefault();
            claim.ClaimStatusId = 2;  //ClaimStatus.Name = "Utkast"

            var currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.Where(u => u.Id == currentUserId).FirstOrDefault();

            //Save company information
            SaveCompanyInformation(claim, currentUser);

            //Save ombud information
            SaveOmbudInformation(claim, currentUserId, currentUser);

            //Save customer information
            SaveCustomerInformation(create1VM, claim);

            //Save regular assistant information
            SaveRegAssistantInformation(create1VM, claim);

            //Save substitute assistant information
            SaveSubAssistantInformation(create1VM, claim);

            //int? assistantId = null;
            ////Assign the SSN to the AssistantSSN property
            //if (create1VM.SelectedRegAssistantId != null)
            //{
            //    claim.AssistantSSN = db.Assistants.Where(a => a.Id == create1VM.SelectedRegAssistantId).FirstOrDefault().AssistantSSN;
            //    claim.SelectedRegAssistantId = create1VM.SelectedRegAssistantId;
            //}
            //else
            //{
            //    claim.AssistantSSN = null;
            //    claim.SelectedRegAssistantId = assistantId;
            //}

            ////Assign the SSN to the StandInSSN property
            //if (create1VM.SelectedSubAssistantId != null)
            //{
            //    claim.StandInSSN = db.Assistants.Where(a => a.Id == create1VM.SelectedSubAssistantId).FirstOrDefault().AssistantSSN;
            //    claim.SelectedSubAssistantId = create1VM.SelectedSubAssistantId;
            //}
            //else
            //{
            //    claim.StandInSSN = null;
            //    claim.SelectedSubAssistantId = assistantId;
            //}

            claim.StatusDate = DateTime.Now;
            claim.QualifyingDate = create1VM.FirstDayOfSicknessDate;
            claim.LastDayOfSicknessDate = create1VM.LastDayOfSicknessDate;
            claim.NumberOfSickDays = 1 + (create1VM.LastDayOfSicknessDate.Date - create1VM.FirstDayOfSicknessDate.Date).Days;
            db.Entry(claim).State = EntityState.Modified;
            db.SaveChanges();
        }

        private string GenerateReferenceNumber()
        {
            string newReferenceNumber = "";

            //Generate a Reference Number for the claim and update the latest Reference Number in the db
            //There is always only one row in the ClaimReferenceNo class in the database
            var latestReference = db.ClaimReferenceNumbers.FirstOrDefault();
            //Check if first claim in a new year. Need to update the LatestYear property and reset the LatestReferenceNumber property.
            if (latestReference.LatestYear != DateTime.Now.Year)
            {
                db.ClaimReferenceNumbers.FirstOrDefault().LatestYear = DateTime.Now.Year;
                db.ClaimReferenceNumbers.FirstOrDefault().LatestReferenceNumber = 0;
                db.SaveChanges();
                latestReference = db.ClaimReferenceNumbers.FirstOrDefault();
                newReferenceNumber = DateTime.Now.Year.ToString() + (latestReference.LatestReferenceNumber + 1).ToString("D5");
            }
            else
            {
                db.ClaimReferenceNumbers.FirstOrDefault().LatestReferenceNumber = latestReference.LatestReferenceNumber + 1;
                newReferenceNumber = DateTime.Now.Year.ToString() + (latestReference.LatestReferenceNumber).ToString("D5");
            }
            return newReferenceNumber;
        }

        // GET: Claims/Create2
        [HttpGet]
        public ActionResult Create2(string refNumber)
        {
            Create2VM create2VM = new Create2VM();
            var claim = db.Claims.Where(c => c.ReferenceNumber == refNumber).FirstOrDefault();

            if (!demoMode || (demoMode && claim.CompletionStage >= 2)) //CompletionStage >= 2 means that stage 2 has been filled in earlier. This is an update
            {
                create2VM = LoadClaimCreate2VM(claim);
            }
            else if (demoMode && claim.CompletionStage == 1) //Demo, new claim
            {
                create2VM = LoadDemoClaimCreate2VM(claim);
            }
            return View("Create2", create2VM);
        }

        private Create2VM LoadDemoClaimCreate2VM(Claim claim)
        {
            Create2VM create2VM = new Create2VM();
            var numberOfDays = 1 + (claim.LastDayOfSicknessDate.Date - claim.QualifyingDate.Date).Days;

            create2VM.ReferenceNumber = claim.ReferenceNumber;
            List<ScheduleRow> rowList = new List<ScheduleRow>();

            DateTime dateInSchedule;

            //Seed for demo only
            var claimDaySeeds = db.ClaimDaySeeds.ToList();

            //Populate viewmodel properties by iterating over each row in the schedule
            for (int i = 0; i < numberOfDays; i++)
            {
                //Instantiate a new scheduleRow in the viewmodel
                ScheduleRow scheduleRow = new ScheduleRow();

                //Assign values to the ScheduleRowDate and ScheduleRowWeekDay properties in the viewmodel
                dateInSchedule = claim.QualifyingDate.AddDays(i);

                CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture = new CultureInfo("sv-SE");

                scheduleRow.ScheduleRowDateString = dateInSchedule.ToString(format: "ddd d MMM");
                scheduleRow.DayDate = dateInSchedule;
                scheduleRow.ScheduleRowWeekDay = DateTimeFormatInfo.CurrentInfo.GetDayName(dateInSchedule.DayOfWeek).ToString().Substring(0, 3);

                //For seeding demo
                scheduleRow.Hours = claimDaySeeds[i].Hours;
                scheduleRow.UnsocialEvening = claimDaySeeds[i].UnsocialEvening;
                scheduleRow.UnsocialWeekend = claimDaySeeds[i].UnsocialWeekend;
                scheduleRow.UnsocialGrandWeekend = claimDaySeeds[i].UnsocialGrandWeekend;

                scheduleRow.OnCallDay = claimDaySeeds[i].OnCallDay;
                scheduleRow.OnCallNight = claimDaySeeds[i].OnCallNight;

                rowList.Add(scheduleRow);
            }
            create2VM.ScheduleRowList = rowList;

            return create2VM;
        }

        private Create2VM LoadClaimCreate2VM(Claim claim)
        {
            Create2VM create2VM = new Create2VM();
            var numberOfDays = (claim.LastDayOfSicknessDate.Date - claim.QualifyingDate.Date).Days + 1;

            create2VM.ReferenceNumber = claim.ReferenceNumber;
            List<ScheduleRow> rowList = new List<ScheduleRow>();

            DateTime dateInSchedule;

            List<ClaimDay> claimDays = new List<ClaimDay>();

            if (claim.CompletionStage >= 2)
            {
                claimDays = db.ClaimDays.Where(c => c.ReferenceNumber == claim.ReferenceNumber).OrderBy(c => c.SickDayNumber).ToList();
            }

            //Populate viewmodel properties by iterating over each row in the schedule
            for (int i = 0; i < numberOfDays; i++)
            {
                //Instantiate a new scheduleRow in the viewmodel
                ScheduleRow scheduleRow = new ScheduleRow();

                //Assign values to the ScheduleRowDate and ScheduleRowWeekDay properties in the viewmodel
                dateInSchedule = claim.QualifyingDate.AddDays(i);

                CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture = new CultureInfo("sv-SE");

                scheduleRow.ScheduleRowDateString = dateInSchedule.ToString(format: "ddd d MMM");
                scheduleRow.DayDate = dateInSchedule;
                scheduleRow.ScheduleRowWeekDay = DateTimeFormatInfo.CurrentInfo.GetDayName(dateInSchedule.DayOfWeek).ToString().Substring(0, 3);

                if (claim.CompletionStage >= 2)
                {
                    scheduleRow.Hours = claimDays[i].Hours;
                    scheduleRow.UnsocialEvening = claimDays[i].UnsocialEvening;
                    scheduleRow.UnsocialNight = claimDays[i].UnsocialNight;
                    scheduleRow.UnsocialWeekend = claimDays[i].UnsocialWeekend;
                    scheduleRow.UnsocialGrandWeekend = claimDays[i].UnsocialGrandWeekend;

                    scheduleRow.OnCallDay = claimDays[i].OnCallDay;
                    scheduleRow.OnCallNight = claimDays[i].OnCallNight;

                    scheduleRow.HoursSI = claimDays[i].HoursSI;
                    scheduleRow.UnsocialEveningSI = claimDays[i].UnsocialEveningSI;
                    scheduleRow.UnsocialNightSI = claimDays[i].UnsocialNightSI;
                    scheduleRow.UnsocialWeekendSI = claimDays[i].UnsocialWeekendSI;
                    scheduleRow.UnsocialGrandWeekendSI = claimDays[i].UnsocialGrandWeekendSI;

                    scheduleRow.OnCallDaySI = claimDays[i].OnCallDaySI;
                    scheduleRow.OnCallNightSI = claimDays[i].OnCallNightSI;
                }
                rowList.Add(scheduleRow);
            }
            create2VM.ScheduleRowList = rowList;

            return create2VM;
        }

        // POST: Claims/Create2
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create2(Create2VM create2VM, string refNumber, string submitButton)
        {
            if (submitButton == "Till steg 3")
            {
                SaveClaim2(create2VM);
                return RedirectToAction("Create3", "Claims", new { refNumber = refNumber });
            }
            else if (submitButton == "Avbryt")
            {
                return RedirectToAction("IndexPageOmbud");
            }
            else
            {
                SaveClaim2(create2VM);
                return View(create2VM);
            }
        }

        private void SaveClaim2(Create2VM create2VM)
        {
            //If there are existing ClaimDay records for this claim: remove them. The new records will be added to the db instead.
            db.ClaimDays.RemoveRange(db.ClaimDays.Where(c => c.ReferenceNumber == create2VM.ReferenceNumber));
            db.SaveChanges();

            var claim = db.Claims.Where(c => c.ReferenceNumber == create2VM.ReferenceNumber).FirstOrDefault();
            decimal numberOfAbsenceHours = 0;
            decimal numberOfUnsocialHours = 0;
            decimal numberOfOnCallHours = 0;
            decimal numberOfAbsenceHoursWithSI = 0;
            decimal numberOfUnsocialHoursSI = 0;
            decimal numberOfOnCallHoursSI = 0;

            DateTime claimDate = claim.QualifyingDate;

            int dayIdx = 1;
            foreach (var day in create2VM.ScheduleRowList)
            {
                var claimDay = new ClaimDay
                {
                    ReferenceNumber = create2VM.ReferenceNumber,
                    DateString = day.ScheduleRowDateString,
                    Date = claimDate.AddDays(dayIdx - 1),
                    SickDayNumber = dayIdx,

                    Hours = day.Hours,
                    UnsocialEvening = day.UnsocialEvening,
                    UnsocialNight = day.UnsocialNight,
                    UnsocialWeekend = day.UnsocialWeekend,
                    UnsocialGrandWeekend = day.UnsocialGrandWeekend,
                    OnCallDay = day.OnCallDay,
                    OnCallNight = day.OnCallNight,
                    HoursSI = day.HoursSI,
                    UnsocialEveningSI = day.UnsocialEveningSI,
                    UnsocialNightSI = day.UnsocialNightSI,
                    UnsocialWeekendSI = day.UnsocialWeekendSI,
                    UnsocialGrandWeekendSI = day.UnsocialGrandWeekendSI,
                    OnCallDaySI = day.OnCallDaySI,
                    OnCallNightSI = day.OnCallNightSI
                };
                db.ClaimDays.Add(claimDay);

                numberOfAbsenceHours = numberOfAbsenceHours + Convert.ToDecimal(day.Hours);
                numberOfUnsocialHours = numberOfUnsocialHours + Convert.ToDecimal(day.UnsocialEvening) + Convert.ToDecimal(day.UnsocialNight) + Convert.ToDecimal(day.UnsocialWeekend) + Convert.ToDecimal(day.UnsocialGrandWeekend);
                numberOfOnCallHours = numberOfOnCallHours + Convert.ToDecimal(day.OnCallDay) + Convert.ToDecimal(day.OnCallNight);
                numberOfAbsenceHoursWithSI = numberOfAbsenceHoursWithSI + Convert.ToDecimal(day.HoursSI);
                numberOfUnsocialHoursSI = numberOfUnsocialHoursSI + Convert.ToDecimal(day.UnsocialEveningSI) + Convert.ToDecimal(day.UnsocialNightSI) + Convert.ToDecimal(day.UnsocialWeekendSI) + Convert.ToDecimal(day.UnsocialGrandWeekendSI);
                numberOfOnCallHoursSI = numberOfOnCallHoursSI + Convert.ToDecimal(day.OnCallDaySI) + Convert.ToDecimal(day.OnCallNightSI);

                dayIdx++;
            }

            claim.NumberOfAbsenceHours = numberOfAbsenceHours;
            claim.NumberOfUnsocialHours = numberOfUnsocialHours;
            claim.NumberOfOnCallHours = numberOfOnCallHours;
            claim.NumberOfHoursWithSI = numberOfAbsenceHoursWithSI;
            claim.NumberOfUnsocialHoursSI = numberOfUnsocialHoursSI;
            claim.NumberOfOnCallHoursSI = numberOfOnCallHoursSI;

            if (claim.CompletionStage < 2)
            {
                claim.CompletionStage = 2;
            }
            db.Entry(claim).State = EntityState.Modified;
            db.SaveChanges();
        }

        // GET: Claims/Create3
        [HttpGet]
        public ActionResult Create3(string refNumber)
        {
            Create3VM create3VM = new Create3VM();
            var claim = db.Claims.Where(c => c.ReferenceNumber == refNumber).FirstOrDefault();

            if (!demoMode || (demoMode && claim.CompletionStage >= 3)) //CompletionStage >= 3 means that stage 3 has been filled in earlier. This is an update
            {
                create3VM = LoadClaimCreate3VM(claim);
            }
            else if (!demoMode)
            {
                create3VM = LoadNewClaimCreate3VM(claim);
            }
            else //Demo
            {
                create3VM = LoadDemoClaimCreate3VM(refNumber);
            }
            return View("Create3", create3VM);
        }

        private Create3VM LoadNewClaimCreate3VM(Claim claim)
        {
            Create3VM create3VM = new Create3VM();
            create3VM.ClaimNumber = claim.ReferenceNumber;
            create3VM.SickPay = "00,00";
            create3VM.HolidayPay = "00,00";
            create3VM.SocialFees = "00,00";
            create3VM.PensionAndInsurance = "00,00";
            create3VM.ClaimSum = "00,00";
            return create3VM;
        }

        private Create3VM LoadClaimCreate3VM(Claim claim)
        {
            Create3VM create3VM = new Create3VM();
            create3VM.ClaimNumber = claim.ReferenceNumber;
            //if (claim.CompletionStage >= 3) //CompletionStage >= 3 means that stage 3 has been filled in earlier. This is an update of stage 3
            //{
            create3VM.SickPay = String.Format("{0:0.00}", claim.ClaimedSickPay);
            create3VM.HolidayPay = String.Format("{0:0.00}", claim.ClaimedHolidayPay);
            create3VM.SocialFees = String.Format("{0:0.00}", claim.ClaimedSocialFees);
            create3VM.PensionAndInsurance = String.Format("{0:0.00}", claim.ClaimedPensionAndInsurance);
            create3VM.ClaimSum = String.Format("{0:0.00}", claim.ClaimedSum);
            //}
            return create3VM;
        }

        private Create3VM LoadDemoClaimCreate3VM(string refNumber)
        {
            Create3VM create3VM = new Create3VM();
            create3VM.ClaimNumber = refNumber;

            var claimDays = db.ClaimDays.Where(c => c.ReferenceNumber == refNumber).OrderBy(c => c.SickDayNumber).ToList();

            decimal hours = 0;
            decimal unsocialEvening = 0;
            decimal unsocialNight = 0;
            decimal unsocialWeekend = 0;
            decimal unsocialGrandWeekend = 0;
            decimal unsocialSum = 0;
            decimal oncallDay = 0;
            decimal oncallNight = 0;
            decimal oncallSum = 0;

            for (int i = 0; i < claimDays.Count(); i++)
            {
                hours = hours + Convert.ToDecimal(claimDays[i].Hours);
                unsocialEvening = unsocialEvening + Convert.ToDecimal(claimDays[i].UnsocialEvening);
                unsocialNight = unsocialNight + Convert.ToDecimal(claimDays[i].UnsocialNight);
                unsocialWeekend = unsocialWeekend + Convert.ToDecimal(claimDays[i].UnsocialWeekend);
                unsocialGrandWeekend = unsocialGrandWeekend + Convert.ToDecimal(claimDays[i].UnsocialGrandWeekend);
                oncallDay = oncallDay + Convert.ToDecimal(claimDays[i].OnCallDay);
                oncallNight = oncallNight + Convert.ToDecimal(claimDays[i].OnCallNight);
            }
            unsocialSum = unsocialEvening + unsocialNight + unsocialWeekend + unsocialGrandWeekend;
            oncallSum = oncallDay + oncallNight;
            create3VM.SickPay = String.Format("{0:0.00}", (decimal)0.8 * ((120 * hours) + ((decimal)65.5 * unsocialSum) + ((decimal)43.2 * oncallSum)) - (decimal)0.8 * ((120 * Convert.ToDecimal(claimDays[0].Hours)) + ((decimal)65.5 * Convert.ToDecimal(claimDays[0].UnsocialEvening)) + ((decimal)65.5 * Convert.ToDecimal(claimDays[0].UnsocialNight)) + ((decimal)65.5 * Convert.ToDecimal(claimDays[0].UnsocialWeekend)) + ((decimal)65.5 * Convert.ToDecimal(claimDays[0].UnsocialGrandWeekend)) + ((decimal)65.5 * Convert.ToDecimal(claimDays[0].OnCallDay)) + ((decimal)65.5 * Convert.ToDecimal(claimDays[0].OnCallNight))));
            create3VM.HolidayPay = String.Format("{0:0.00}", (decimal)0.12 * (decimal)0.8 * ((120 * hours) + ((decimal)65.5 * unsocialSum) + ((decimal)43.2 * oncallSum)));
            create3VM.SocialFees = String.Format("{0:0.00}", (decimal)0.3142 * (Convert.ToDecimal(create3VM.SickPay) + Convert.ToDecimal(create3VM.HolidayPay)));
            create3VM.PensionAndInsurance = String.Format("{0:0.00}", (decimal)0.06 * (Convert.ToDecimal(create3VM.SickPay) + Convert.ToDecimal(create3VM.HolidayPay)));
            create3VM.ClaimSum = String.Format("{0:0.00}", Convert.ToDecimal(create3VM.HolidayPay) + Convert.ToDecimal(create3VM.SickPay) + Convert.ToDecimal(create3VM.PensionAndInsurance) + Convert.ToDecimal(create3VM.SocialFees));
            return create3VM;
        }

        // POST: Claims/Create3
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create3(Create3VM create3VM, string refNumber, string submitButton)
        {
            if (submitButton == "Till steg 4")
            {
                create3VM.ClaimSum = String.Format("{0:0.00}", Convert.ToDecimal(create3VM.SickPay) + Convert.ToDecimal(create3VM.HolidayPay) + Convert.ToDecimal(create3VM.SocialFees) + Convert.ToDecimal(create3VM.PensionAndInsurance));
                SaveClaim3(create3VM);
                var claim = db.Claims.Where(c => c.ReferenceNumber == refNumber).FirstOrDefault();
                db.Entry(claim).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Create4", new { ClaimNumber = refNumber });
            }
            else if (submitButton == "Avbryt")
            {
                return RedirectToAction("IndexPageOmbud");
            }
            else
            {
                SaveClaim3(create3VM);
                create3VM.ClaimSum = String.Format("{0:0.00}", Convert.ToDecimal(create3VM.SickPay) + Convert.ToDecimal(create3VM.HolidayPay) + Convert.ToDecimal(create3VM.SocialFees) + Convert.ToDecimal(create3VM.PensionAndInsurance));
                return View(create3VM);
            }
        }

        private void SaveClaim3(Create3VM create3VM)
        {
            var claim = db.Claims.Where(c => c.ReferenceNumber == create3VM.ClaimNumber).FirstOrDefault();
            if (claim != null)
            {
                if (claim.CompletionStage < 3)
                {
                    claim.CompletionStage = 3;
                }
                claim.ClaimedSickPay = Convert.ToDecimal(create3VM.SickPay);
                claim.ClaimedHolidayPay = Convert.ToDecimal(create3VM.HolidayPay);
                claim.ClaimedSocialFees = Convert.ToDecimal(create3VM.SocialFees);
                claim.ClaimedPensionAndInsurance = Convert.ToDecimal(create3VM.PensionAndInsurance);
                claim.ClaimedSum = Convert.ToDecimal(create3VM.SickPay) + Convert.ToDecimal(create3VM.HolidayPay) + Convert.ToDecimal(create3VM.SocialFees) + Convert.ToDecimal(create3VM.PensionAndInsurance);
                //claim.ClaimedSum = Convert.ToDecimal(create3VM.ClaimSum);
                db.Entry(claim).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        //
        // GET: Claims/Create4
        public ActionResult Create4(string ClaimNumber)
        {
            //This get action needs to be updated to handle the case where attachments have been added to the claim but the claim was only saved with attachments, not submitted.
            var VM = new Create4VM();
            VM.ClaimNumber = ClaimNumber;

            return View("Create4", VM);
        }

        //
        // POST: Claims/Create4
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create4([Bind(Include = "ClaimNumber, SalaryAttachment, SalaryAttachmentStandIn, SickLeaveNotification, DoctorsCertificate, TimeReport, TimeReportStandIn")]Create4VM model, string submitButton)
        {
            if (submitButton == "Skicka in" || submitButton == "Spara")
            {
                if (!Directory.Exists(Server.MapPath("~/Uploads")))
                    Directory.CreateDirectory(Server.MapPath("~/Uploads"));
                if (!Directory.Exists(Server.MapPath($"~/Uploads/{model.ClaimNumber}")))
                    Directory.CreateDirectory(Server.MapPath($"~/Uploads/{model.ClaimNumber}"));

                var claim = db.Claims.Where(c => c.ReferenceNumber == model.ClaimNumber).FirstOrDefault();

                if (!CheckExistingDocument(claim, "SalaryAttachment", model.SalaryAttachment))
                    ModelState.AddModelError("1", "Lönespecifikation för ordinarie assistent behövs");

                if (!CheckExistingDocument(claim, "SalaryAttachmentStandIn", model.SalaryAttachmentStandIn))
                    ModelState.AddModelError("1", "Lönespecifikation för vikarierande assistent behövs");

                if (CheckExistingDocument(claim, "SickLeaveNotification", model.SickLeaveNotification))
                    ModelState.AddModelError("1", "Sjukfrånvaroanmälan behövs");

                if (!CheckExistingDocument(claim, "DoctorsCertificate", model.DoctorsCertificate) && claim.NumberOfSickDays > 7)
                    ModelState.AddModelError("1", "Läkarintyg behövs");

                if (!CheckExistingDocument(claim, "TimeReport", model.TimeReport))
                    ModelState.AddModelError("1", "Tidsrapportering för ordinarie assistent behövs");

                if (!CheckExistingDocument(claim, "TimeReportStandIn", model.TimeReportStandIn))
                    ModelState.AddModelError("1", "Tidsrapportering för vikarierande assistent behövs");

                if (ModelState.IsValid)
                {
                    try
                    {
                        string path = Server.MapPath($"~/Uploads/{model.ClaimNumber}");

                        if (model.SalaryAttachment != null)
                            NewDocument(model.SalaryAttachment, path, "SalaryAttachment", claim);

                        if (model.SalaryAttachmentStandIn != null)
                            NewDocument(model.SalaryAttachmentStandIn, path, "SalaryAttachmentStandIn", claim);

                        if (model.SickLeaveNotification != null)
                            NewDocument(model.SickLeaveNotification, path, "SickLeaveNotification", claim);

                        if (model.DoctorsCertificate != null)
                            NewDocument(model.DoctorsCertificate, path, "DoctorsCertificate", claim);

                        if (model.TimeReport != null)
                            NewDocument(model.TimeReport, path, "TimeReport", claim);

                        if (model.TimeReportStandIn != null)
                            NewDocument(model.TimeReportStandIn, path, "TimeReportStandIn", claim);

                        if (claim.CompletionStage < 4)
                        {
                            claim.CompletionStage = 4;
                        }
                        if (submitButton == "Skicka in")
                        {
                            claim.ClaimStatusId = 5;
                            claim.StatusDate = DateTime.Now;
                            db.Entry(claim).State = EntityState.Modified;
                            db.SaveChanges();

                            return RedirectToAction("ShowReceipt", new { model.ClaimNumber });
                        }
                        else
                        {
                            return View("Create4", model);
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = "ERROR: " + ex.Message.ToString();
                    }
                }
                else
                {
                    return View("Create4", model);
                }
            }
            return RedirectToAction("IndexPageOmbud");
        }

        private bool CheckExistingDocument(Claim claim, string queryValue, HttpPostedFileBase file)
        {
            var linqQuery = claim.Documents.Where(d => d.Title == queryValue);
            if (!linqQuery.Any() && file == null)
                return false;
            if (linqQuery.Any() && file != null)
            {
                System.IO.File.Delete(linqQuery.FirstOrDefault().Filename);
                db.Documents.Remove(linqQuery.FirstOrDefault());
                db.SaveChanges();
            }
            return true;

        }

        private void NewDocument(HttpPostedFileBase file, string path, string title, Claim claim)
        {
            var document = new Document();
            document.DateUploaded = DateTime.Now;
            document.Filename = Path.Combine(path, $"{title}_{Path.GetFileName(file.FileName)}");
            document.FileSize = file.ContentLength;
            document.FileType = file.ContentType;
            //document.OwnerId = User.Identity.GetUserId();
            document.Title = title;
            db.Documents.Add(document);
            claim.Documents.Add(document);
            db.SaveChanges();
            file.SaveAs(Path.Combine(path, $"{title}_{Path.GetFileName(file.FileName)}"));
            return;
        }

        public ActionResult ShowReceipt(string ClaimNumber)
        {
            var claim = db.Claims.Where(c => c.ReferenceNumber == ClaimNumber).FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(claim.OmbudEmail))
            {
                MailMessage message = new MailMessage();
                message.From = new MailAddress("ourrobotdemo@gmail.com");

                message.To.Add(new MailAddress(claim.OmbudEmail));
                //message.To.Add(new MailAddress("e.niklashagman@gmail.com"));
                message.Subject = "Ny ansökan med referensnummer: " + ClaimNumber;
                message.Body = "Vi har mottagit din ansökan med referensnummer " + ClaimNumber + ". Normalt får du ett beslut inom 1 - 3 dagar." + "\n" + "\n" +
                                                    "Med vänliga hälsningar, Vård- och omsorgsförvaltningen";

                SendEmail(message);
            }

            string appdataPath = Environment.ExpandEnvironmentVariables("%appdata%\\Bitoreq AB\\KoPerNikus");

            var UserId = db.Users.Where(u => u.FirstName == "Henrik").FirstOrDefault().Id;

            Directory.CreateDirectory(appdataPath);
            using (var writer = XmlWriter.Create(appdataPath + "\\info.xml"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("claiminformation");
                writer.WriteElementString("SSN", claim.CustomerSSN);
                writer.WriteElementString("OrgNumber", claim.OrganisationNumber);
                writer.WriteElementString("ReferenceNumber", claim.ReferenceNumber);
                writer.WriteElementString("ClaimId", claim.Id.ToString());
                writer.WriteElementString("UserId", UserId);
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            var create3VM = new Create3VM();
            create3VM.ClaimNumber = claim.ReferenceNumber;
            create3VM.ClaimSum = String.Format("{0:0.00}", claim.ClaimedSum);
            return View("Receipt", create3VM);
        }

        // GET: Claims/Decide/5
        public ActionResult Recommend(int? id)
        {
            RecommendationVM recommendationVM = new RecommendationVM();
            var claim = db.Claims.Where(c => c.Id == id).FirstOrDefault();
            if (claim != null)
            {
                //Find ClaimDay records for the claim
                var claimDays = db.ClaimDays.Where(c => c.ReferenceNumber == claim.ReferenceNumber).OrderBy(c => c.ReferenceNumber).ToList();

                //These check results are hardcoded for the demo. Need to be changed for the real solution.
                recommendationVM.CompleteCheck = true;
                recommendationVM.ProxyCheck = claim.ProxyCheck;
                if (!recommendationVM.CompleteCheck)
                {
                    recommendationVM.CompleteCheckMsg = "Bilaga saknas";
                }
                else
                {
                    recommendationVM.CompleteCheckMsg = "Alla bilagor är med";
                }
                if (!recommendationVM.ProxyCheck)
                {
                    recommendationVM.ProxyCheckMsg = "Ombudet saknar giltig fullmakt";
                }
                else
                {
                    recommendationVM.ProxyCheckMsg = "Ombudet har giltig fullmakt";
                }

                recommendationVM.AssistanceCheck = false;
                recommendationVM.IvoCheck = false;

                recommendationVM.AssistanceCheck = claim.ProCapitaCheck;
                recommendationVM.IvoCheck = claim.IVOCheck;
                if (!recommendationVM.AssistanceCheck)
                {
                    recommendationVM.AssistanceCheckMsg = "Beslut om assistans saknas";
                }
                else
                {
                    recommendationVM.AssistanceCheckMsg = "Giltigt beslut om assistans finns";
                }

                if (!recommendationVM.IvoCheck)
                {
                    recommendationVM.IvoCheckMsg = "Verksamheten saknas i IVO";
                }
                else
                {
                    recommendationVM.IvoCheckMsg = "Verksamheten finns i IVO";
                }

                recommendationVM.ClaimNumber = claim.ReferenceNumber;
                recommendationVM.ModelSum = Convert.ToDecimal(claim.TotalCostD1T14);
                recommendationVM.ClaimSum = claim.ClaimedSum;
                recommendationVM.ApprovedSum = recommendationVM.ModelSum.ToString();
                if (recommendationVM.ModelSum > recommendationVM.ClaimSum)
                {
                    recommendationVM.RejectedSum = "0,00";
                }
                else
                {
                    recommendationVM.RejectedSum = (recommendationVM.ClaimSum - recommendationVM.ModelSum).ToString();
                }

                return View("Recommend", recommendationVM);
            }
            else
            {
                return View();
            }
        }

        public ActionResult _Message(Message message)
        {
            ApplicationUser user = db.Users.Where(u => u.Id == message.applicationUser.Id).FirstOrDefault();
            string userName = $"{user.FirstName} {user.LastName}";
            MessageVM messageVM = new MessageVM(message.CommentDate, message.Comment, userName);

            return PartialView("_Message", messageVM);
        }

        // GET: Claims/_SendMessage
        public ActionResult _SendMessage(string claimRefNr)
        {
            SendMessageVM sendMessageVM = new SendMessageVM();
            sendMessageVM.ClaimId = db.Claims.Where(r => r.ReferenceNumber == claimRefNr).FirstOrDefault().Id;
            var user = db.Users.Where(u => u.Id == db.Claims.Where(c => c.ReferenceNumber == claimRefNr).FirstOrDefault().OwnerId).FirstOrDefault();
            sendMessageVM.Name = $"{user.FirstName} {user.LastName}";
            return PartialView("_SendMessage", sendMessageVM);
        }
        // POST: Claims/_SendMessage
        [HttpPost]
        public ActionResult _SendMessage(SendMessageVM sendMessage)
        {
            Message message = new Message();
            message.ClaimId = sendMessage.ClaimId;
            message.Comment = sendMessage.Comment;
            message.CommentDate = DateTime.Now;
            string uid = User.Identity.GetUserId();
            message.applicationUser = db.Users.Where(u => u.Id == uid).FirstOrDefault();
            db.Messages.Add(message);
            db.SaveChanges();
            return PartialView("_SendMessage");
        }

        [HttpGet]
        public ActionResult _ShowClaim(string refNumber)
        {
            var claim = db.Claims.Include(c => c.ClaimStatus).Where(c => c.ReferenceNumber == refNumber).FirstOrDefault();
            var currentId = User.Identity.GetUserId();
            ApplicationUser ombud = db.Users.Where(u => u.Id == currentId).FirstOrDefault();
            ClaimDetailsOmbudVM claimDetailsOmbudVM = new ClaimDetailsOmbudVM();
            claimDetailsOmbudVM.CompletionStage = claim.CompletionStage;
            if (claim.CompletionStage >= 1)
            {
                claimDetailsOmbudVM.ReferenceNumber = refNumber;
                claimDetailsOmbudVM.StatusName = claim.ClaimStatus.Name;
                claimDetailsOmbudVM.DefaultCollectiveAgreement = claim.DefaultCollectiveAgreement;

                //Kommun
                claimDetailsOmbudVM.Council = "Helsingborgs kommun";
                claimDetailsOmbudVM.Administration = "Vård- och omsorgsförvaltningen";

                //Assistansberättigad
                claimDetailsOmbudVM.CustomerName = claim.CustomerName;
                claimDetailsOmbudVM.CustomerSSN = claim.CustomerSSN;
                claimDetailsOmbudVM.CustomerAddress = claim.CustomerAddress;
                claimDetailsOmbudVM.CustomerPhoneNumber = claim.CustomerPhoneNumber;

                //Ombud/uppgiftslämnare
                claimDetailsOmbudVM.OmbudName = ombud.FirstName + " " + ombud.LastName;
                claimDetailsOmbudVM.OmbudPhoneNumber = ombud.PhoneNumber;

                //Assistansanordnare
                claimDetailsOmbudVM.CompanyName = claim.CompanyName; ;
                claimDetailsOmbudVM.OrganisationNumber = claim.OrganisationNumber;
                claimDetailsOmbudVM.GiroNumber = claim.AccountNumber;
                claimDetailsOmbudVM.CompanyAddress = claim.StreetAddress;
                claimDetailsOmbudVM.CompanyPhoneNumber = claim.CompanyPhoneNumber;
                claimDetailsOmbudVM.CollectiveAgreement = claim.CollectiveAgreementName + ", " + claim.CollectiveAgreementSpecName;
                claimDetailsOmbudVM.Workplace = "Björkängen, Birgittagården"; //This can probably be removed

                //Insjuknad ordinarie assistent
                claimDetailsOmbudVM.RegAssistantName = claim.RegFirstName + " " + claim.RegLastName;
                claimDetailsOmbudVM.RegAssistantSSN = claim.RegAssistantSSN;
                claimDetailsOmbudVM.RegPhoneNumber = claim.RegPhoneNumber;
                claimDetailsOmbudVM.RegEmail = claim.RegEmail;
                claimDetailsOmbudVM.QualifyingDayDate = claim.QualifyingDate.ToShortDateString();
                claimDetailsOmbudVM.LastDayOfSicknessDate = claim.LastDayOfSicknessDate.ToShortDateString();

                //Vikarierande assistent
                claimDetailsOmbudVM.SubAssistantName = claim.SubFirstName + " " + claim.SubLastName;
                claimDetailsOmbudVM.SubAssistantSSN = claim.SubAssistantSSN;
                claimDetailsOmbudVM.SubPhoneNumber = claim.SubPhoneNumber;
                claimDetailsOmbudVM.SubEmail = claim.SubEmail;

                claimDetailsOmbudVM.NumberOfSickDays = claim.NumberOfSickDays;

                //claimDetailsOmbudVM.Salary = claim.HourlySalary;  //This property is used either as an hourly salary or as a monthly salary in claimDetailsOmbudVM.cs.
                //claimDetailsOmbudVM.HourlySalary = claim.HourlySalary;    //This property is used as the hourly salary in calculations.
                claimDetailsOmbudVM.HourlySalaryAsString = claim.HourlySalaryAsString;
                claimDetailsOmbudVM.SickPayRateAsString = claim.SickPayRateAsString;
                claimDetailsOmbudVM.HolidayPayRateAsString = claim.HolidayPayRateAsString;
                claimDetailsOmbudVM.SocialFeeRateAsString = claim.SocialFeeRateAsString;
                claimDetailsOmbudVM.PensionAndInsuranceRateAsString = claim.PensionAndInsuranceRateAsString;
            }

            if (claim.CompletionStage >= 2)
            {
                //Hours for regular assistant
                claimDetailsOmbudVM.NumberOfAbsenceHours = claim.NumberOfAbsenceHours;
                claimDetailsOmbudVM.NumberOfUnsocialHours = claim.NumberOfUnsocialHours;
                claimDetailsOmbudVM.NumberOfOnCallHours = claim.NumberOfOnCallHours;
                //claimDetailsVM.NumberOfOrdinaryHours = claim.NumberOfOrdinaryHours;

                //Hours for substitute assistant
                claimDetailsOmbudVM.NumberOfHoursWithSI = claim.NumberOfHoursWithSI;
                claimDetailsOmbudVM.NumberOfUnsocialHoursSI = claim.NumberOfUnsocialHoursSI;
                claimDetailsOmbudVM.NumberOfOnCallHoursSI = claim.NumberOfOnCallHoursSI;

                var claimDays = db.ClaimDays.Where(c => c.ReferenceNumber == claim.ReferenceNumber).OrderBy(c => c.SickDayNumber).ToList();
                claimDetailsOmbudVM.ClaimDays = claimDays;
            }

            if (claim.CompletionStage >= 3)
            {
                claimDetailsOmbudVM.Sickpay = claim.ClaimedSickPay;
                claimDetailsOmbudVM.HolidayPay = claim.ClaimedHolidayPay;
                claimDetailsOmbudVM.SocialFees = claim.ClaimedSocialFees;
                claimDetailsOmbudVM.PensionAndInsurance = claim.ClaimedPensionAndInsurance;
                claimDetailsOmbudVM.ClaimSum = claim.ClaimedSum;
            }

            if (claim.CompletionStage >= 4)
            {
                claimDetailsOmbudVM.Documents = claim.Documents;
                claimDetailsOmbudVM.messages = db.Messages.Where(c => c.ClaimId == claim.Id).ToList();
                claimDetailsOmbudVM.DecisionMade = false;
                if (claim.ClaimStatus.Name == "Beslutad")
                {
                    claimDetailsOmbudVM.ApprovedSum = claim.ApprovedSum;
                    claimDetailsOmbudVM.RejectedSum = claim.RejectedSum;
                    claimDetailsOmbudVM.DecisionMade = true;
                }
            }

            if (claim.CompletionStage >= 4 && (User.IsInRole("AdministrativeOfficial") || User.IsInRole("Admin")))
            {
                //
                var claimCalculations = db.ClaimCalculations.Where(c => c.ReferenceNumber == claim.ReferenceNumber).OrderBy(c => c.StartDate).ToList();
                List<ClaimCalculation> claimCalcs = new List<ClaimCalculation>();
                for (int i = 0; i < claimCalculations.Count(); i++)
                {
                    ClaimCalculation claimCalc = new ClaimCalculation();
                    claimCalc.StartDate = claimCalculations[i].StartDate.Date;
                    claimCalc.EndDate = claimCalculations[i].EndDate.Date;

                    claimCalc.PerHourUnsocialEvening = claimCalculations[i].PerHourUnsocialEvening;
                    claimCalc.PerHourUnsocialNight = claimCalculations[i].PerHourUnsocialNight;
                    claimCalc.PerHourUnsocialWeekend = claimCalculations[i].PerHourUnsocialWeekend;
                    claimCalc.PerHourUnsocialHoliday = claimCalculations[i].PerHourUnsocialHoliday;
                    claimCalc.PerHourOnCallWeekday = claimCalculations[i].PerHourOnCallWeekday;
                    claimCalc.PerHourOnCallWeekend = claimCalculations[i].PerHourOnCallWeekend;

                    if (i == 0)
                    {
                        //QUALIFYING DAY

                        //Hours for qualifying day
                        claimCalc.HoursQD = claimCalculations[i].HoursQD;

                        //Holiday pay for qualifying day
                        claimCalc.HolidayPayQD = claimCalculations[i].HolidayPayQD;
                        claimCalc.HolidayPayCalcQD = claimCalculations[i].HolidayPayCalcQD;

                        //Social fees for qualifying day
                        claimCalc.SocialFeesQD = claimCalculations[i].SocialFeesQD;
                        claimCalc.SocialFeesCalcQD = claimCalculations[i].SocialFeesCalcQD;

                        //Pension and insurance for qualifying day
                        claimCalc.PensionAndInsuranceQD = claimCalculations[i].PensionAndInsuranceQD;
                        claimCalc.PensionAndInsuranceCalcQD = claimCalculations[i].PensionAndInsuranceCalcQD;

                        //Sum for qualifying day (sum of the three previous items)
                        claimCalc.CostQD = claimCalculations[i].CostQD;
                        claimCalc.CostCalcQD = claimCalculations[i].CostCalcQD;
                    }

                    //DAY 2 TO DAY 14
                    claimCalc.HoursD2T14 = "0,00";
                    claimCalc.UnsocialEveningD2T14 = "0,00";
                    claimCalc.UnsocialNightD2T14 = "0,00";
                    claimCalc.UnsocialWeekendD2T14 = "0,00";
                    claimCalc.UnsocialGrandWeekendD2T14 = "0,00";
                    claimCalc.UnsocialSumD2T14 = "0,00";
                    claimCalc.OnCallDayD2T14 = "0,00";
                    claimCalc.OnCallNightD2T14 = "0,00";
                    claimCalc.OnCallSumD2T14 = "0,00";

                    claimCalc.HoursD2T14 = claimCalculations[i].HoursD2T14;

                    claimCalc.UnsocialEveningD2T14 = claimCalculations[i].UnsocialEveningD2T14;
                    claimCalc.UnsocialNightD2T14 = claimCalculations[i].UnsocialNightD2T14;
                    claimCalc.UnsocialWeekendD2T14 = claimCalculations[i].UnsocialWeekendD2T14;
                    claimCalc.UnsocialGrandWeekendD2T14 = claimCalculations[i].UnsocialGrandWeekendD2T14;

                    claimCalc.OnCallDayD2T14 = claimCalculations[i].OnCallDayD2T14;
                    claimCalc.OnCallNightD2T14 = claimCalculations[i].OnCallNightD2T14;

                    claimCalc.UnsocialSumD2T14 = claimCalculations[i].UnsocialSumD2T14;
                    claimCalc.OnCallSumD2T14 = claimCalculations[i].OnCallSumD2T14;
                    
                    //Load the money by category for day 2 to day 14
                    //Sickpay for day 2 to day 14
                    claimCalc.SalaryD2T14 = claimCalculations[i].SalaryD2T14;
                    claimCalc.SalaryCalcD2T14 = claimCalculations[i].SalaryCalcD2T14;

                    //Holiday pay for day 2 to day 14
                    claimCalc.HolidayPayD2T14 = claimCalculations[i].HolidayPayD2T14;
                    claimCalc.HolidayPayCalcD2T14 = claimCalculations[i].HolidayPayCalcD2T14;

                    //Unsocial evening pay for day 2 to day 14
                    claimCalc.UnsocialEveningPayD2T14 = claimCalculations[i].UnsocialEveningPayD2T14;
                    claimCalc.UnsocialEveningPayCalcD2T14 = claimCalculations[i].UnsocialEveningPayCalcD2T14;

                    //Unsocial night pay for day 2 to day 14
                    claimCalc.UnsocialNightPayD2T14 = claimCalculations[i].UnsocialNightPayD2T14;
                    claimCalc.UnsocialNightPayCalcD2T14 = claimCalculations[i].UnsocialNightPayCalcD2T14;

                    //Unsocial weekend pay for day 2 to day 14
                    claimCalc.UnsocialWeekendPayD2T14 = claimCalculations[i].UnsocialWeekendPayD2T14;
                    claimCalc.UnsocialWeekendPayCalcD2T14 = claimCalculations[i].UnsocialWeekendPayCalcD2T14;

                    //Unsocial grand weekend pay for day 2 to day 14
                    claimCalc.UnsocialGrandWeekendPayD2T14 = claimCalculations[i].UnsocialGrandWeekendPayD2T14;
                    claimCalc.UnsocialGrandWeekendPayCalcD2T14 = claimCalculations[i].UnsocialGrandWeekendPayCalcD2T14;

                    //Unsocial sum pay for day 2 to day 14
                    claimCalc.UnsocialSumPayD2T14 = claimCalculations[i].UnsocialSumPayD2T14;
                    claimCalc.UnsocialSumPayCalcD2T14 = claimCalculations[i].UnsocialSumPayCalcD2T14;

                    //On call day pay for day 2 to day 14
                    claimCalc.OnCallDayPayD2T14 = claimCalculations[i].OnCallDayPayD2T14;
                    claimCalc.OnCallDayPayCalcD2T14 = claimCalculations[i].OnCallDayPayCalcD2T14;

                    //On call night pay for day 2 to day 14
                    claimCalc.OnCallNightPayD2T14 = claimCalculations[i].OnCallNightPayD2T14;
                    claimCalc.OnCallNightPayCalcD2T14 = claimCalculations[i].OnCallNightPayCalcD2T14;

                    //On call sum pay for day 2 to day 14
                    claimCalc.OnCallSumPayD2T14 = claimCalculations[i].OnCallSumPayD2T14;
                    claimCalc.OnCallSumPayCalcD2T14 = claimCalculations[i].OnCallSumPayCalcD2T14;

                    //Sick pay for day 2 to day 14
                    claimCalc.SickPayD2T14 = claimCalculations[i].SickPayD2T14;
                    claimCalc.SickPayCalcD2T14 = claimCalculations[i].SickPayCalcD2T14;

                    //Social fees for day 2 to day 14
                    claimCalc.SocialFeesD2T14 = claimCalculations[i].SocialFeesD2T14;
                    claimCalc.SocialFeesCalcD2T14 = claimCalculations[i].SocialFeesCalcD2T14;

                    //Pensions and insurances for day 2 to day 14
                    claimCalc.PensionAndInsuranceD2T14 = claimCalculations[i].PensionAndInsuranceD2T14;
                    claimCalc.PensionAndInsuranceCalcD2T14 = claimCalculations[i].PensionAndInsuranceCalcD2T14;

                    //Sum for day 2 to day 14
                    claimCalc.CostD2T14 = claimCalculations[i].CostD2T14;
                    claimCalc.CostCalcD2T14 = claimCalculations[i].CostCalcD2T14;

                    claimCalcs.Add(claimCalc);
                }
                claimDetailsOmbudVM.ClaimCalculations = claimCalcs;

                //Total sum for day 1 to day 14
                claimDetailsOmbudVM.TotalCostD1T14 = claim.TotalCostD1T14;
                claimDetailsOmbudVM.TotalCostCalcD1T14 = claim.TotalCostCalcD1T14;
                //
            }

            return PartialView("_ClaimForOmbud", claimDetailsOmbudVM);
        }

        public ActionResult ShowClaimDetails(string referenceNumber)
        {
            var claim = db.Claims.Include(c => c.ClaimStatus).Where(c => c.ReferenceNumber == referenceNumber).FirstOrDefault();

            List<ClaimDay> claimDays = new List<ClaimDay>();
            claimDays = db.ClaimDays.Where(c => c.ReferenceNumber == referenceNumber).OrderBy(c => c.SickDayNumber).ToList();

            //Calculate the model sum
            if (claimDays.Count() > 0)
            {
                CalculateModelSum(claim, claimDays);
            }

            ClaimDetailsVM claimDetailsVM = new ClaimDetailsVM();

            claimDetailsVM.ReferenceNumber = referenceNumber;
            //claimDetailsVM.StatusName = claim.ClaimStatus.Name;
            claimDetailsVM.StatusName = claim.ClaimStatus.Name;
            claimDetailsVM.DefaultCollectiveAgreement = claim.DefaultCollectiveAgreement;

            //Kommun
            claimDetailsVM.Council = "Helsingborgs kommun";
            claimDetailsVM.Administration = "Vård- och omsorgsförvaltningen";

            //Assistansberättigad
            claimDetailsVM.CustomerName = claim.CustomerName;
            claimDetailsVM.CustomerSSN = claim.CustomerSSN;
            claimDetailsVM.CustomerAddress = claim.CustomerAddress;
            claimDetailsVM.CustomerPhoneNumber = claim.CustomerPhoneNumber;

            //Ombud/uppgiftslämnare
            claimDetailsVM.OmbudName = claim.OmbudFirstName + " " + claim.OmbudLastName;
            claimDetailsVM.OmbudPhoneNumber = claim.OmbudPhoneNumber;

            //Assistansanordnare
            claimDetailsVM.CompanyName = claim.CompanyName; ;
            claimDetailsVM.OrganisationNumber = claim.OrganisationNumber;
            claimDetailsVM.GiroNumber = claim.AccountNumber;
            claimDetailsVM.CompanyAddress = claim.StreetAddress;
            claimDetailsVM.CompanyPhoneNumber = claim.CompanyPhoneNumber;
            claimDetailsVM.CollectiveAgreement = claim.CollectiveAgreementName + ", " + claim.CollectiveAgreementSpecName;
            claimDetailsVM.Workplace = "Björkängen, Birgittagården"; //This can probably be removed

            //Insjuknad ordinarie assistent
            //Källa till belopp: https://assistanskoll.se/Guider-Att-arbeta-som-personlig-assistent.html (Vårdföretagarna)
            claimDetailsVM.AssistantName = claim.RegFirstName + " " + claim.RegLastName;
            claimDetailsVM.AssistantSSN = claim.RegAssistantSSN;
            claimDetailsVM.QualifyingDayDate = claim.QualifyingDate.ToShortDateString();
            claimDetailsVM.LastDayOfSicknessDate = claim.LastDayOfSicknessDate.ToShortDateString();

            claimDetailsVM.NumberOfSickDays = claim.NumberOfSickDays;

            claimDetailsVM.Salary = claim.HourlySalary;  //This property is used either as an hourly salary or as a monthly salary in claimDetailsVM.cs.
            claimDetailsVM.HourlySalary = claim.HourlySalary;    //This property is used as the hourly salary in calculations.
            claimDetailsVM.Sickpay = claim.ClaimedSickPay;
            claimDetailsVM.HolidayPay = claim.ClaimedHolidayPay;
            claimDetailsVM.SocialFees = claim.ClaimedSocialFees;
            claimDetailsVM.PensionAndInsurance = claim.ClaimedPensionAndInsurance;

            claimDetailsVM.NumberOfAbsenceHours = claim.NumberOfAbsenceHours;
            claimDetailsVM.NumberOfOrdinaryHours = claim.NumberOfOrdinaryHours;
            claimDetailsVM.NumberOfUnsocialHours = claim.NumberOfUnsocialHours;
            claimDetailsVM.NumberOfOnCallHours = claim.NumberOfOnCallHours;

            claimDetailsVM.NumberOfHoursWithSI = claim.NumberOfHoursWithSI;
            claimDetailsVM.NumberOfOrdinaryHoursSI = claim.NumberOfOrdinaryHoursSI;
            claimDetailsVM.NumberOfUnsocialHoursSI = claim.NumberOfUnsocialHoursSI;
            claimDetailsVM.NumberOfOnCallHoursSI = claim.NumberOfOnCallHoursSI;

            claimDetailsVM.ClaimSum = claim.ClaimedSum;

            claimDetailsVM.DecisionMade = false;
            if (claim.ClaimStatus.Name == "Beslutad")
            {
                claimDetailsVM.ApprovedSum = claim.ApprovedSum;
                claimDetailsVM.RejectedSum = claim.RejectedSum;
                claimDetailsVM.DecisionMade = true;
            }

            //Underlag lönekostnader
            claimDetailsVM.PerHourUnsocialEvening = claim.PerHourUnsocialEvening;
            claimDetailsVM.PerHourUnsocialNight = claim.PerHourUnsocialNight;
            claimDetailsVM.PerHourUnsocialWeekend = claim.PerHourUnsocialWeekend;
            claimDetailsVM.PerHourUnsocialHoliday = claim.PerHourUnsocialHoliday;
            claimDetailsVM.PerHourOnCallWeekday = claim.PerHourOnCallWeekday;
            claimDetailsVM.PerHourOnCallWeekend = claim.PerHourOnCallWeekend;


            claimDetailsVM.HolidayPayRate = claim.HolidayPayRate;
            claimDetailsVM.SocialFeeRate = claim.SocialFeeRate;
            claimDetailsVM.PensionAndInsuranceRate = claim.PensionAndInsuranceRate;
            claimDetailsVM.SickPayRate = claim.SickPayRate;

            claimDetailsVM.SickPayRateAsString = claim.SickPayRateAsString;
            claimDetailsVM.HolidayPayRateAsString = claim.HolidayPayRateAsString;
            claimDetailsVM.SocialFeeRateAsString = claim.SocialFeeRateAsString;
            claimDetailsVM.PensionAndInsuranceRateAsString = claim.PensionAndInsuranceRateAsString;
            claimDetailsVM.HourlySalaryAsString = claim.HourlySalaryAsString;

            var claimCalculations = db.ClaimCalculations.Where(c => c.ReferenceNumber == referenceNumber).OrderBy(c => c.StartDate).ToList();
            List<ClaimCalculation> claimCalcs = new List<ClaimCalculation>();
            for (int i = 0; i < claimCalculations.Count(); i++)
            {
                ClaimCalculation claimCalc = new ClaimCalculation();
                claimCalc.StartDate = claimCalculations[i].StartDate.Date;
                claimCalc.EndDate = claimCalculations[i].EndDate.Date;

                claimCalc.PerHourUnsocialEvening = claimCalculations[i].PerHourUnsocialEvening;
                claimCalc.PerHourUnsocialNight = claimCalculations[i].PerHourUnsocialNight;
                claimCalc.PerHourUnsocialWeekend = claimCalculations[i].PerHourUnsocialWeekend;
                claimCalc.PerHourUnsocialHoliday = claimCalculations[i].PerHourUnsocialHoliday;
                claimCalc.PerHourOnCallWeekday = claimCalculations[i].PerHourOnCallWeekday;
                claimCalc.PerHourOnCallWeekend = claimCalculations[i].PerHourOnCallWeekend;

                if (i == 0)
                {
                    //QUALIFYING DAY

                    //Hours for qualifying day
                    claimCalc.HoursQD = claimCalculations[i].HoursQD;

                    //Holiday pay for qualifying day
                    claimCalc.HolidayPayQD = claimCalculations[i].HolidayPayQD;
                    claimCalc.HolidayPayCalcQD = claimCalculations[i].HolidayPayCalcQD;

                    //Social fees for qualifying day
                    claimCalc.SocialFeesQD = claimCalculations[i].SocialFeesQD;
                    claimCalc.SocialFeesCalcQD = claimCalculations[i].SocialFeesCalcQD;

                    //Pension and insurance for qualifying day
                    claimCalc.PensionAndInsuranceQD = claimCalculations[i].PensionAndInsuranceQD;
                    claimCalc.PensionAndInsuranceCalcQD = claimCalculations[i].PensionAndInsuranceCalcQD;

                    //Sum for qualifying day (sum of the three previous items)
                    claimCalc.CostQD = claimCalculations[i].CostQD;
                    claimCalc.CostCalcQD = claimCalculations[i].CostCalcQD;
                }

                //DAY 2 TO DAY 14
                claimCalc.HoursD2T14 = "0,00";
                claimCalc.UnsocialEveningD2T14 = "0,00";
                claimCalc.UnsocialNightD2T14 = "0,00";
                claimCalc.UnsocialWeekendD2T14 = "0,00";
                claimCalc.UnsocialGrandWeekendD2T14 = "0,00";
                claimCalc.UnsocialSumD2T14 = "0,00";
                claimCalc.OnCallDayD2T14 = "0,00";
                claimCalc.OnCallNightD2T14 = "0,00";
                claimCalc.OnCallSumD2T14 = "0,00";

                claimCalc.HoursD2T14 = claimCalculations[i].HoursD2T14;

                claimCalc.UnsocialEveningD2T14 = claimCalculations[i].UnsocialEveningD2T14;
                claimCalc.UnsocialNightD2T14 = claimCalculations[i].UnsocialNightD2T14;
                claimCalc.UnsocialWeekendD2T14 = claimCalculations[i].UnsocialWeekendD2T14;
                claimCalc.UnsocialGrandWeekendD2T14 = claimCalculations[i].UnsocialGrandWeekendD2T14;

                claimCalc.OnCallDayD2T14 = claimCalculations[i].OnCallDayD2T14;
                claimCalc.OnCallNightD2T14 = claimCalculations[i].OnCallNightD2T14;

                claimCalc.UnsocialSumD2T14 = claimCalculations[i].UnsocialSumD2T14;
                claimCalc.OnCallSumD2T14 = claimCalculations[i].OnCallSumD2T14;

                //These numbers go to the assistant's part of the view
                claimDetailsVM.NumberOfAbsenceHours = claim.NumberOfAbsenceHours;
                claimDetailsVM.NumberOfOrdinaryHours = claim.NumberOfOrdinaryHours;
                claimDetailsVM.NumberOfUnsocialHours = claim.NumberOfUnsocialHours;
                claimDetailsVM.NumberOfOnCallHours = claim.NumberOfOnCallHours;

                //Load the money by category for day 2 to day 14
                //Sickpay for day 2 to day 14
                claimCalc.SalaryD2T14 = claimCalculations[i].SalaryD2T14;
                claimCalc.SalaryCalcD2T14 = claimCalculations[i].SalaryCalcD2T14;

                //Holiday pay for day 2 to day 14
                claimCalc.HolidayPayD2T14 = claimCalculations[i].HolidayPayD2T14;
                claimCalc.HolidayPayCalcD2T14 = claimCalculations[i].HolidayPayCalcD2T14;

                //Unsocial evening pay for day 2 to day 14
                claimCalc.UnsocialEveningPayD2T14 = claimCalculations[i].UnsocialEveningPayD2T14;
                claimCalc.UnsocialEveningPayCalcD2T14 = claimCalculations[i].UnsocialEveningPayCalcD2T14;

                //Unsocial night pay for day 2 to day 14
                claimCalc.UnsocialNightPayD2T14 = claimCalculations[i].UnsocialNightPayD2T14;
                claimCalc.UnsocialNightPayCalcD2T14 = claimCalculations[i].UnsocialNightPayCalcD2T14;

                //Unsocial weekend pay for day 2 to day 14
                claimCalc.UnsocialWeekendPayD2T14 = claimCalculations[i].UnsocialWeekendPayD2T14;
                claimCalc.UnsocialWeekendPayCalcD2T14 = claimCalculations[i].UnsocialWeekendPayCalcD2T14;

                //Unsocial grand weekend pay for day 2 to day 14
                claimCalc.UnsocialGrandWeekendPayD2T14 = claimCalculations[i].UnsocialGrandWeekendPayD2T14;
                claimCalc.UnsocialGrandWeekendPayCalcD2T14 = claimCalculations[i].UnsocialGrandWeekendPayCalcD2T14;

                //Unsocial sum pay for day 2 to day 14
                claimCalc.UnsocialSumPayD2T14 = claimCalculations[i].UnsocialSumPayD2T14;
                claimCalc.UnsocialSumPayCalcD2T14 = claimCalculations[i].UnsocialSumPayCalcD2T14;

                //On call day pay for day 2 to day 14
                claimCalc.OnCallDayPayD2T14 = claimCalculations[i].OnCallDayPayD2T14;
                claimCalc.OnCallDayPayCalcD2T14 = claimCalculations[i].OnCallDayPayCalcD2T14;

                //On call night pay for day 2 to day 14
                claimCalc.OnCallNightPayD2T14 = claimCalculations[i].OnCallNightPayD2T14;
                claimCalc.OnCallNightPayCalcD2T14 = claimCalculations[i].OnCallNightPayCalcD2T14;

                //On call sum pay for day 2 to day 14
                claimCalc.OnCallSumPayD2T14 = claimCalculations[i].OnCallSumPayD2T14;
                claimCalc.OnCallSumPayCalcD2T14 = claimCalculations[i].OnCallSumPayCalcD2T14;

                //Sick pay for day 2 to day 14
                claimCalc.SickPayD2T14 = claimCalculations[i].SickPayD2T14;
                claimCalc.SickPayCalcD2T14 = claimCalculations[i].SickPayCalcD2T14;

                //Social fees for day 2 to day 14
                claimCalc.SocialFeesD2T14 = claimCalculations[i].SocialFeesD2T14;
                claimCalc.SocialFeesCalcD2T14 = claimCalculations[i].SocialFeesCalcD2T14;

                //Pensions and insurances for day 2 to day 14
                claimCalc.PensionAndInsuranceD2T14 = claimCalculations[i].PensionAndInsuranceD2T14;
                claimCalc.PensionAndInsuranceCalcD2T14 = claimCalculations[i].PensionAndInsuranceCalcD2T14;

                //Sum for day 2 to day 14
                claimCalc.CostD2T14 = claimCalculations[i].CostD2T14;
                claimCalc.CostCalcD2T14 = claimCalculations[i].CostCalcD2T14;

                claimCalcs.Add(claimCalc);
            }
            claimDetailsVM.ClaimCalculations = claimCalcs;

            //Total sum for day 1 to day 14
            claimDetailsVM.TotalCostD1T14 = claim.TotalCostD1T14;
            claimDetailsVM.TotalCostCalcD1T14 = claim.TotalCostCalcD1T14;

            claimDetailsVM.Documents = claim.Documents;

            claimDetailsVM.messages = db.Messages.Where(c => c.ClaimId == claim.Id).ToList();

            return View("ClaimDetails", claimDetailsVM);
        }

        // POST: Claims/Decide/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Recommend(RecommendationVM recommendationVM)
        {
            var claim = db.Claims.Where(c => c.ReferenceNumber == recommendationVM.ClaimNumber).FirstOrDefault();
            claim.ApprovedSum = Convert.ToDecimal(recommendationVM.ApprovedSum);
            claim.RejectedSum = Convert.ToDecimal(recommendationVM.RejectedSum);
            claim.StatusDate = DateTime.Now;
            db.Entry(claim).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ShowRecommendationReceipt", recommendationVM);
        }

        // GET: Claims/ShowRecommendationReceipt
        public ActionResult ShowRecommendationReceipt(RecommendationVM recommendationVM)
        {

            var claim = db.Claims.Where(rn => rn.ReferenceNumber == recommendationVM.ClaimNumber).FirstOrDefault();

            string appdataPath = Environment.ExpandEnvironmentVariables("%appdata%\\Bitoreq AB\\KoPerNikus");

            Directory.CreateDirectory(appdataPath);
            using (var writer = XmlWriter.Create(appdataPath + "\\stodsystem.xml"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("claiminformation");
                writer.WriteElementString("ReferenceNumber", claim.ReferenceNumber);
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            return View("RecommendationReceipt", recommendationVM);
        }

        // GET: Claims/StodSystemLogin
        [OverrideAuthorization]
        [HttpGet]
        public ActionResult StodSystemLogin()
        {

            StodSystemLoginVM StodSystemLoginVM = new StodSystemLoginVM();
            return View("StodSystemLogin", StodSystemLoginVM);
        }

        // POST: Claims/StodSystemLogin
        [OverrideAuthorization]
        [HttpPost]
        public ActionResult StodSystemLogin(StodSystemLoginVM stodSystemLoginVM)
        {
            return RedirectToAction("Decide", stodSystemLoginVM);
        }

        // GET: Claims/Decide
        [OverrideAuthorization]
        [HttpGet]
        public ActionResult Decide(StodSystemLoginVM stodSystemLoginVM)
        {
            var claim = db.Claims.Where(c => c.ReferenceNumber == stodSystemLoginVM.ReferenceNumber).FirstOrDefault();

            DecisionVM decisionVM = new DecisionVM();

            decisionVM.ClaimNumber = claim.ReferenceNumber;
            decisionVM.CareCompany = "Smart Assistans";
            decisionVM.AssistantSSN = claim.RegAssistantSSN;
            decisionVM.QualifyingDate = claim.QualifyingDate;
            decisionVM.LastDayOfSickness = claim.LastDayOfSicknessDate;
            decisionVM.ClaimSum = claim.ClaimedSum;
            decisionVM.ApprovedSum = claim.ApprovedSum.ToString();
            decisionVM.RejectedSum = claim.RejectedSum.ToString();

            return View("Decide", decisionVM);
        }

        // POST: Claims/Decide/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [OverrideAuthorization]
        [ValidateAntiForgeryToken]
        public ActionResult Decide(DecisionVM decisionVM)
        {
            //Decided claim
            var claim = db.Claims.Where(c => c.ReferenceNumber == decisionVM.ClaimNumber).FirstOrDefault();
            claim.ClaimStatusId = 1;

            claim.ApprovedSum = Convert.ToDecimal(decisionVM.ApprovedSum);
            claim.RejectedSum = Convert.ToDecimal(decisionVM.RejectedSum);
            claim.StatusDate = DateTime.Now;
            db.Entry(claim).State = EntityState.Modified;
            db.SaveChanges();

            if (!string.IsNullOrWhiteSpace(decisionVM.Comment))
            {
                Message comment = new Message();
                comment.ClaimId = claim.Id;
                comment.applicationUser = db.Users.Where(u => u.FirstName == "Henrik").FirstOrDefault();
                comment.CommentDate = DateTime.Now;
                comment.Comment = decisionVM.Comment;
                db.Messages.Add(comment);
                db.SaveChanges();
            }

            if (!string.IsNullOrWhiteSpace(claim.OmbudEmail))
            {
                MailMessage message = new MailMessage();
                message.From = new MailAddress("ourrobotdemo@gmail.com");

                message.To.Add(new MailAddress(claim.OmbudEmail));
                //message.To.Add(new MailAddress("e.niklashagman@gmail.com"));
                message.Subject = "Beslut om ansökan med referensnummer: " + claim.ReferenceNumber;
                message.Body = "Beslut om ansökan med referensnummer " + claim.ReferenceNumber + " har fattats." + "\n" + "\n" +
                                "Med vänliga hälsningar, Vård- och omsorgsförvaltningen";

                SendEmail(message);
            }

            string appdataPath = Environment.ExpandEnvironmentVariables("%appdata%\\Bitoreq AB\\KoPerNikus");

            Directory.CreateDirectory(appdataPath);
            using (var writer = XmlWriter.Create(appdataPath + "\\decided.xml"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("claiminformation");
                writer.WriteElementString("SSN", claim.CustomerSSN);
                writer.WriteElementString("OrgNumber", claim.OrganisationNumber);
                writer.WriteElementString("ReferenceNumber", claim.ReferenceNumber);
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            return RedirectToAction("StodsystemLogout");
        }

        // GET: Claims/StodsystemLogout
        [OverrideAuthorization]
        public ActionResult StodsystemLogout()
        {
            return View();
        }

        // GET: Claims/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Claim claim = db.Claims.Find(id);
            if (claim == null)
            {
                return HttpNotFound();
            }
            return View(claim);
        }

        // POST: Claims/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,OwnerId,ClaimStatusId,ReferenceNumber,StatusDate,DeadlineDate,CustomerFirstName,CustomerLastName,CustomerSSN,HourlySalary,HolidayPayRate,PayrollTaxRate,InsuranceRate,PensionRate,QualifyingDate,LastDayOfSicknessDate,HoursQualifyingDay,HolidayPayQualDay,PayrollTaxQualDay,InsuranceQualDay,PensionQualDay,ClaimQualDay,HoursDay2To14,HourlySickPay,SickPayDay2To14,HolidayPayDay2To14,UnsocialHoursBonusDay2To14,OnCallDutyDay2To14,PayrollTaxDay2To14,InsuranceDay2To14,PensionDay2To14,ClaimDay2To14,ClaimSum")] Claim claim)
        {
            if (ModelState.IsValid)
            {
                db.Entry(claim).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(claim);
        }

        // GET: Claims/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Claim claim = db.Claims.Find(id);
            if (claim == null)
            {
                return HttpNotFound();
            }
            return View(claim);
        }

        // POST: Claims/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string refNumber, string submitButton)
        {
            if (submitButton == "Bekräfta")
            {
                Claim claim = db.Claims.Where(c => c.ReferenceNumber == refNumber).FirstOrDefault();
                if (claim.CompletionStage > 1)
                {
                    //Come back to this and consider what should be done with other entities (ClaimDays,ClaimCalculations, CollectiveAgreementHeader and Info). Probably depending on the CompletionStage of the claim.
                    db.ClaimDays.RemoveRange(db.ClaimDays.Where(c => c.ReferenceNumber == claim.ReferenceNumber));
                }
                db.Claims.Remove(claim);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        private bool overlappingClaim(DateTime? lastDayOfSicknessDate, DateTime? firstDayOfSicknessDate, string userId, string SSN)
        {
            //Check if a claim for overlapping dates already exists.
            var claims = db.Claims.Where(c => c.OwnerId == userId).Where(c => c.CustomerSSN == SSN).ToList();
            if (claims != null)
            {
                foreach (var claim in claims)
                {
                    if (firstDayOfSicknessDate <= claim.LastDayOfSicknessDate && firstDayOfSicknessDate >= claim.QualifyingDate)
                    {
                        return true;
                    }
                    if (lastDayOfSicknessDate <= claim.LastDayOfSicknessDate && lastDayOfSicknessDate >= claim.QualifyingDate)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private void CalculateModelSum(Claim claim, List<ClaimDay> claimDays)
        {
            //Check if there are any previous ClaimCalculation records for this claim. Existing records need to be removed.
            var prevClaimCalculations = db.ClaimCalculations.Where(c => c.ReferenceNumber == claim.ReferenceNumber).ToList();
            if (prevClaimCalculations.Count() > 0)
            {
                db.ClaimCalculations.RemoveRange(prevClaimCalculations);
            }

            //Reset number of hours in claim record. If not reset, the same hours will be included several times.
            claim.NumberOfAbsenceHours = (decimal)0.00;
            claim.NumberOfOrdinaryHours = (decimal)0.00;
            claim.NumberOfUnsocialHours = (decimal)0.00;
            claim.NumberOfOnCallHours = (decimal)0.00;
            db.Entry(claim).State = EntityState.Modified;
            db.SaveChanges();

            //Assign a CollAgreementInfo id to each claim day. This is required in order for the correct hourly pay to be used in the calculations.
            var collectiveAgreementInfos = db.CollectiveAgreementInfos.Where(c => c.CollectiveAgreementHeaderId == claim.CareCompany.SelectedCollectiveAgreementId).OrderBy(c => c.StartDate).ToList();
            List<int> usedCollectiveAgreementInfoIds = new List<int>(); //This list is used for figuring out which collective agreement infos have been used.
            bool useDefaultCollectiveAgreement = false;
            claim.DefaultCollectiveAgreement = false;

            if (collectiveAgreementInfos.Count() > 0) //Figure out which collectiveAgreementInfo records apply to the claim if at least one collectiveAgreementInfo has been assigned to the claim. 
            {
                DateTime? claimDayDate;
                bool infoFound;
                bool infoUsed;
                int infoIdx;
                int infoUsedIdx;
                int claimDayIdx = 0;
                claimDays.OrderBy(c => c.SickDayNumber);
                do
                {
                    claimDayDate = claim.QualifyingDate.AddDays(claimDays[claimDayIdx].SickDayNumber - 1);
                    infoFound = false;
                    infoIdx = 0;
                    do
                    {
                        if (claimDayDate >= collectiveAgreementInfos[infoIdx].StartDate && claimDayDate <= collectiveAgreementInfos[infoIdx].EndDate)
                        {
                            infoFound = true;

                            //The lines below are used to keep track on how many different CollectiveAgreementInfo records are used in the claim. Calculations for each different CollectiveAgreementInfo needs to be shown separately in ShowClaimDetails. 
                            infoUsed = false;
                            infoUsedIdx = 0;
                            if (usedCollectiveAgreementInfoIds.Count() == 0)
                            {
                                usedCollectiveAgreementInfoIds.Add(collectiveAgreementInfos[infoIdx].Id);
                            }
                            else
                            {
                                do
                                {
                                    if (usedCollectiveAgreementInfoIds[infoUsedIdx] == collectiveAgreementInfos[infoIdx].Id)
                                    {
                                        infoUsed = true;
                                    }
                                    infoUsedIdx++;
                                } while (!infoUsed && infoUsedIdx < usedCollectiveAgreementInfoIds.Count());
                                if (!infoUsed)
                                {
                                    usedCollectiveAgreementInfoIds.Add(collectiveAgreementInfos[infoIdx].Id);
                                }
                            }
                        }
                        infoIdx++;
                    } while (!infoFound && infoIdx < collectiveAgreementInfos.Count());
                    //Assign default values if no info found
                    if (!infoFound)
                    {
                        useDefaultCollectiveAgreement = true;
                    }
                    claimDayIdx++;
                } while (claimDayIdx < claimDays.Count() && !useDefaultCollectiveAgreement);

                //foreach (var claimDay in claimDays)
                //{
                //    claimDayDate = claim.QualifyingDate.AddDays(claimDay.SickDayNumber - 1);
                //    infoFound = false;
                //    infoIdx = 0;
                //    do
                //    {
                //        if (claimDayDate >= collectiveAgreementInfos[infoIdx].StartDate && claimDayDate <= collectiveAgreementInfos[infoIdx].EndDate)
                //        {
                //            infoFound = true;

                //            //The lines below are used to keep track on how many different CollectiveAgreementInfo records are used in the claim. Calculations for each different CollectiveAgreementInfo needs to be shown separately in ShowClaimDetails. 
                //            infoUsed = false;
                //            infoUsedIdx = 0;
                //            if (usedCollectiveAgreementInfoIds.Count() == 0)
                //            {
                //                usedCollectiveAgreementInfoIds.Add(collectiveAgreementInfos[infoIdx].Id);
                //            }
                //            else
                //            {
                //                do
                //                {
                //                    if (usedCollectiveAgreementInfoIds[infoUsedIdx] == collectiveAgreementInfos[infoIdx].Id)
                //                    {
                //                        infoUsed = true;
                //                    }
                //                    infoUsedIdx++;
                //                } while (!infoUsed && infoUsedIdx < usedCollectiveAgreementInfoIds.Count());
                //                if (!infoUsed)
                //                {
                //                    usedCollectiveAgreementInfoIds.Add(collectiveAgreementInfos[infoIdx].Id);
                //                }
                //            }
                //        }
                //        infoIdx++;
                //    } while (!infoFound && infoIdx < collectiveAgreementInfos.Count());
                //    //Assign default values if no info found
                //    if (!infoFound)
                //    {
                //        useDefaultCollectiveAgreement = true;
                //        //infoUsed = false;
                //        //infoUsedIdx = 0;
                //        //if (usedCollectiveAgreementInfoIds.Count() == 0)
                //        //{
                //        //    usedCollectiveAgreementInfoIds.Add(0);
                //        //}
                //        //else
                //        //{
                //        //    do
                //        //    {
                //        //        if (usedCollectiveAgreementInfoIds[infoUsedIdx] == collectiveAgreementInfos[infoIdx].Id)
                //        //        {
                //        //            infoUsed = true;
                //        //        }
                //        //        infoUsedIdx++;
                //        //    } while (!infoUsed && infoUsedIdx < usedCollectiveAgreementInfoIds.Count());
                //        //    if (!infoUsed)
                //        //    {
                //        //        usedCollectiveAgreementInfoIds.Add(0);
                //        //    }
                //        //}
                //    }
                //}
            }
            else
            {
                useDefaultCollectiveAgreement = true;
            }

            int applicableSickDays = 0;
            int prevSickDayIdx = 0;
            //Check that each day in the sickleave period is covered by an applicable CollectiveAgreementInfo, i.e. that each sickleave day is within the date range for a CollectiveAgreementInfo record. 
            if (!useDefaultCollectiveAgreement)
            {
                for (int idx = 0; idx < usedCollectiveAgreementInfoIds.Count(); idx++)
                {
                    if (collectiveAgreementInfos[idx].EndDate.Date >= claim.LastDayOfSicknessDate.Date)
                    {
                        applicableSickDays = 1 + (claim.LastDayOfSicknessDate.Date - claim.QualifyingDate).Days - prevSickDayIdx;
                    }
                    else
                    {
                        applicableSickDays = 1 + (collectiveAgreementInfos[idx].EndDate - claim.QualifyingDate).Days - prevSickDayIdx;
                    }
                    prevSickDayIdx = prevSickDayIdx + applicableSickDays;
                }
                if (applicableSickDays < claim.NumberOfSickDays)
                {
                    useDefaultCollectiveAgreement = true;
                }
            }
            applicableSickDays = 0;
            prevSickDayIdx = 0;

            //Repeat the calculation for each CollectiveAgreementInfo that applies to the sickleave period
            int startIdx;
            int stopIdx;
            decimal totalCostD1D14 = 0;
            string totalCostCalcD1D14 = "";

            if (useDefaultCollectiveAgreement)
            {
                usedCollectiveAgreementInfoIds.Clear();
                usedCollectiveAgreementInfoIds.Add(999999); //999999 used for default. Default will apply to the whole sickleave period even if some days in the period are covered by a CollectiveAgreementInfo record.
                claim.DefaultCollectiveAgreement = true;
            }
            for (int idx = 0; idx < usedCollectiveAgreementInfoIds.Count(); idx++)
            {
                ClaimCalculation claimCalculation = new ClaimCalculation();
                claimCalculation.ReferenceNumber = claim.ReferenceNumber;

                if (!useDefaultCollectiveAgreement)
                {
                    claimCalculation.PerHourUnsocialEveningAsString = collectiveAgreementInfos[idx].PerHourUnsocialEvening;
                    claimCalculation.PerHourUnsocialNightAsString = collectiveAgreementInfos[idx].PerHourUnsocialNight;
                    claimCalculation.PerHourUnsocialWeekendAsString = collectiveAgreementInfos[idx].PerHourUnsocialWeekend;
                    claimCalculation.PerHourUnsocialHolidayAsString = collectiveAgreementInfos[idx].PerHourUnsocialHoliday;
                    claimCalculation.PerHourOnCallDayAsString = collectiveAgreementInfos[idx].PerHourOnCallWeekday;
                    claimCalculation.PerHourOnCallNightAsString = collectiveAgreementInfos[idx].PerHourOnCallWeekend;

                    claimCalculation.PerHourUnsocialEvening = Convert.ToDecimal(collectiveAgreementInfos[idx].PerHourUnsocialEvening);
                    claimCalculation.PerHourUnsocialNight = Convert.ToDecimal(collectiveAgreementInfos[idx].PerHourUnsocialNight);
                    claimCalculation.PerHourUnsocialWeekend = Convert.ToDecimal(collectiveAgreementInfos[idx].PerHourUnsocialWeekend);
                    claimCalculation.PerHourUnsocialHoliday = Convert.ToDecimal(collectiveAgreementInfos[idx].PerHourUnsocialHoliday);
                    claimCalculation.PerHourOnCallWeekday = Convert.ToDecimal(collectiveAgreementInfos[idx].PerHourOnCallWeekday);
                    claimCalculation.PerHourOnCallWeekend = Convert.ToDecimal(collectiveAgreementInfos[idx].PerHourOnCallWeekend);

                    //Calculate how many days the CollectiveAgreementInfo applies to
                    if (collectiveAgreementInfos[idx].EndDate.Date >= claim.LastDayOfSicknessDate.Date)
                    {
                        applicableSickDays = 1 + (claim.LastDayOfSicknessDate.Date - claim.QualifyingDate).Days - prevSickDayIdx;
                    }
                    else
                    {
                        applicableSickDays = 1 + (collectiveAgreementInfos[idx].EndDate - claim.QualifyingDate).Days - prevSickDayIdx;
                    }
                    claimCalculation.StartDate = claim.QualifyingDate.AddDays(prevSickDayIdx);
                    claimCalculation.EndDate = claim.QualifyingDate.AddDays(prevSickDayIdx + applicableSickDays - 1);
                }
                else
                {
                    var defaultCollectiveAgreementInfos = db.DefaultCollectiveAgreementInfos.ToList(); //There is only one DefaultCollectiveAgreementInfo record.
                    claimCalculation.PerHourUnsocialEveningAsString = defaultCollectiveAgreementInfos[0].PerHourUnsocialEvening;
                    claimCalculation.PerHourUnsocialNightAsString = defaultCollectiveAgreementInfos[0].PerHourUnsocialNight;
                    claimCalculation.PerHourUnsocialWeekendAsString = defaultCollectiveAgreementInfos[0].PerHourUnsocialWeekend;
                    claimCalculation.PerHourUnsocialHolidayAsString = defaultCollectiveAgreementInfos[0].PerHourUnsocialHoliday;
                    claimCalculation.PerHourOnCallDayAsString = defaultCollectiveAgreementInfos[0].PerHourOnCallWeekday;
                    claimCalculation.PerHourOnCallNightAsString = defaultCollectiveAgreementInfos[0].PerHourOnCallWeekend;

                    claimCalculation.PerHourUnsocialEvening = Convert.ToDecimal(defaultCollectiveAgreementInfos[0].PerHourUnsocialEvening);
                    claimCalculation.PerHourUnsocialNight = Convert.ToDecimal(defaultCollectiveAgreementInfos[0].PerHourUnsocialNight);
                    claimCalculation.PerHourUnsocialWeekend = Convert.ToDecimal(defaultCollectiveAgreementInfos[0].PerHourUnsocialWeekend);
                    claimCalculation.PerHourUnsocialHoliday = Convert.ToDecimal(defaultCollectiveAgreementInfos[0].PerHourUnsocialHoliday);
                    claimCalculation.PerHourOnCallWeekday = Convert.ToDecimal(defaultCollectiveAgreementInfos[0].PerHourOnCallWeekday);
                    claimCalculation.PerHourOnCallWeekend = Convert.ToDecimal(defaultCollectiveAgreementInfos[0].PerHourOnCallWeekend);

                    claimCalculation.StartDate = claim.QualifyingDate;
                    claimCalculation.EndDate = claim.LastDayOfSicknessDate;
                    applicableSickDays = claim.NumberOfSickDays;
                }

                if (idx == 0) //Include qualifying day only in first ClaimCalculation record
                {
                    // QUALIFYING DAY

                    //Hours for qualifying day
                    claimCalculation.HoursQD = claimDays[0].Hours;

                    //Holiday pay for qualifying day
                    claimCalculation.HolidayPayQD = String.Format("{0:0.00}", (Convert.ToDecimal(claim.HolidayPayRateAsString) * Convert.ToDecimal(claimCalculation.HoursQD) * Convert.ToDecimal(claim.HourlySalaryAsString) / 100));
                    claimCalculation.HolidayPayCalcQD = claim.HolidayPayRateAsString + " % x " + claimCalculation.HoursQD + " timmar x " + claim.HourlySalaryAsString + " Kr";

                    //Social fees for qualifying day
                    claimCalculation.SocialFeesQD = String.Format("{0:0.00}", (Convert.ToDecimal(claim.SocialFeeRateAsString) * Convert.ToDecimal(claimCalculation.HolidayPayQD) / 100));
                    claimCalculation.SocialFeesCalcQD = claim.SocialFeeRateAsString + " % x " + claimCalculation.HolidayPayQD + " Kr";

                    //Pension and insurance for qualifying day
                    claimCalculation.PensionAndInsuranceQD = String.Format("{0:0.00}", (Convert.ToDecimal(claim.PensionAndInsuranceRateAsString) * Convert.ToDecimal(claimCalculation.HolidayPayQD) / 100));
                    claimCalculation.PensionAndInsuranceCalcQD = claim.PensionAndInsuranceRateAsString + " % x " + claimCalculation.HolidayPayQD + " Kr";

                    //Sum for qualifying day (sum of the three previous items)
                    claimCalculation.CostQD = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.HolidayPayQD) + Convert.ToDecimal(claimCalculation.SocialFeesQD) + Convert.ToDecimal(claimCalculation.PensionAndInsuranceQD)));
                    claimCalculation.CostCalcQD = claimCalculation.HolidayPayQD + " Kr + " + claimCalculation.SocialFeesQD + " Kr + " + claimCalculation.PensionAndInsuranceQD + " Kr";
                }

                //DAY 2 TO DAY 14
                claimCalculation.HoursD2T14 = "0,00";
                claimCalculation.UnsocialEveningD2T14 = "0,00";
                claimCalculation.UnsocialNightD2T14 = "0,00";
                claimCalculation.UnsocialWeekendD2T14 = "0,00";
                claimCalculation.UnsocialGrandWeekendD2T14 = "0,00";
                claimCalculation.UnsocialSumD2T14 = "0,00";
                claimCalculation.OnCallDayD2T14 = "0,00";
                claimCalculation.OnCallNightD2T14 = "0,00";
                claimCalculation.OnCallSumD2T14 = "0,00";

                //Sum up hours by category for day 2 to 14
                if (idx == 0)
                {
                    startIdx = 1;
                    stopIdx = startIdx + applicableSickDays - 1;
                }
                else
                {
                    startIdx = prevSickDayIdx;
                    stopIdx = startIdx + applicableSickDays;
                }
                for (int i = startIdx; i < stopIdx; i++)
                {
                    claimCalculation.HoursD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.HoursD2T14) + Convert.ToDecimal(claimDays[i].Hours)));

                    claimCalculation.UnsocialEveningD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.UnsocialEveningD2T14) + Convert.ToDecimal(claimDays[i].UnsocialEvening)));
                    claimCalculation.UnsocialNightD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.UnsocialNightD2T14) + Convert.ToDecimal(claimDays[i].UnsocialNight)));
                    claimCalculation.UnsocialWeekendD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.UnsocialWeekendD2T14) + Convert.ToDecimal(claimDays[i].UnsocialWeekend)));
                    claimCalculation.UnsocialGrandWeekendD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.UnsocialGrandWeekendD2T14) + Convert.ToDecimal(claimDays[i].UnsocialGrandWeekend)));

                    claimCalculation.OnCallDayD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.OnCallDayD2T14) + Convert.ToDecimal(claimDays[i].OnCallDay)));
                    claimCalculation.OnCallNightD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.OnCallNightD2T14) + Convert.ToDecimal(claimDays[i].OnCallNight)));
                }
                claimCalculation.UnsocialSumD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.UnsocialEveningD2T14) + Convert.ToDecimal(claimCalculation.UnsocialNightD2T14) + Convert.ToDecimal(claimCalculation.UnsocialWeekendD2T14) + Convert.ToDecimal(claimCalculation.UnsocialGrandWeekendD2T14)));
                claimCalculation.OnCallSumD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.OnCallDayD2T14) + Convert.ToDecimal(claimCalculation.OnCallNightD2T14)));

                //These numbers go to the assistant's part of the view
                claim.NumberOfAbsenceHours = claim.NumberOfAbsenceHours + Convert.ToDecimal(claimCalculation.HoursQD) + Convert.ToDecimal(claimCalculation.HoursD2T14) + Convert.ToDecimal(claimDays[0].OnCallDay) + Convert.ToDecimal(claimCalculation.OnCallDayD2T14) + Convert.ToDecimal(claimDays[0].OnCallNight) + Convert.ToDecimal(claimCalculation.OnCallNightD2T14);
                claim.NumberOfOrdinaryHours = claim.NumberOfOrdinaryHours + Convert.ToDecimal(claimCalculation.HoursQD) + Convert.ToDecimal(claimCalculation.HoursD2T14);
                claim.NumberOfUnsocialHours = claim.NumberOfUnsocialHours + Convert.ToDecimal(claimCalculation.UnsocialEveningD2T14) + Convert.ToDecimal(claimCalculation.UnsocialNightD2T14) + Convert.ToDecimal(claimCalculation.UnsocialWeekendD2T14) + Convert.ToDecimal(claimCalculation.UnsocialGrandWeekendD2T14) + Convert.ToDecimal(claimDays[0].UnsocialEvening) + Convert.ToDecimal(claimDays[0].UnsocialNight) + Convert.ToDecimal(claimDays[0].UnsocialWeekend) + Convert.ToDecimal(claimDays[0].UnsocialGrandWeekend);
                claim.NumberOfOnCallHours = claim.NumberOfOnCallHours + Convert.ToDecimal(claimDays[0].OnCallDay) + Convert.ToDecimal(claimCalculation.OnCallDayD2T14) + Convert.ToDecimal(claimDays[0].OnCallNight) + Convert.ToDecimal(claimCalculation.OnCallNightD2T14);

                //Calculate the money by category for day 2 to day 14
                //Sickpay for day 2 to day 14
                claimCalculation.SalaryD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claim.SickPayRateAsString) * Convert.ToDecimal(claimCalculation.HoursD2T14) * Convert.ToDecimal(claim.HourlySalaryAsString) / 100));
                claimCalculation.SalaryCalcD2T14 = claim.SickPayRateAsString + " % x " + claimCalculation.HoursD2T14 + " timmar x " + claim.HourlySalaryAsString + " Kr";

                //Holiday pay for day 2 to day 14
                claimCalculation.HolidayPayD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claim.HolidayPayRateAsString) * Convert.ToDecimal(claimCalculation.SalaryD2T14) / 100));
                claimCalculation.HolidayPayCalcD2T14 = claim.HolidayPayRateAsString + " % x " + claimCalculation.SalaryD2T14 + " Kr";

                //Unsocial evening pay for day 2 to day 14
                claimCalculation.UnsocialEveningPayD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claim.SickPayRateAsString) * Convert.ToDecimal(claimCalculation.UnsocialEveningD2T14) * Convert.ToDecimal(claimCalculation.PerHourUnsocialEveningAsString) / 100));
                claimCalculation.UnsocialEveningPayCalcD2T14 = claim.SickPayRateAsString + " % x " + claimCalculation.UnsocialEveningD2T14 + " timmar x " + claimCalculation.PerHourUnsocialEveningAsString + " Kr";

                //Unsocial night pay for day 2 to day 14
                claimCalculation.UnsocialNightPayD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claim.SickPayRateAsString) * Convert.ToDecimal(claimCalculation.UnsocialNightD2T14) * Convert.ToDecimal(claimCalculation.PerHourUnsocialNightAsString) / 100));
                claimCalculation.UnsocialNightPayCalcD2T14 = claim.SickPayRateAsString + " % x " + claimCalculation.UnsocialNightD2T14 + " timmar x " + claimCalculation.PerHourUnsocialNightAsString + " Kr";

                //Unsocial weekend pay for day 2 to day 14
                claimCalculation.UnsocialWeekendPayD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claim.SickPayRateAsString) * Convert.ToDecimal(claimCalculation.UnsocialWeekendD2T14) * Convert.ToDecimal(claimCalculation.PerHourUnsocialWeekendAsString) / 100));
                claimCalculation.UnsocialWeekendPayCalcD2T14 = claim.SickPayRateAsString + " % x " + claimCalculation.UnsocialWeekendD2T14 + " timmar x " + claimCalculation.PerHourUnsocialWeekendAsString + " Kr";

                //Unsocial grand weekend pay for day 2 to day 14
                claimCalculation.UnsocialGrandWeekendPayD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claim.SickPayRateAsString) * Convert.ToDecimal(claimCalculation.UnsocialGrandWeekendD2T14) * Convert.ToDecimal(claimCalculation.PerHourUnsocialHolidayAsString) / 100));
                claimCalculation.UnsocialGrandWeekendPayCalcD2T14 = claim.SickPayRateAsString + " % x " + claimCalculation.UnsocialGrandWeekendD2T14 + " timmar x " + claimCalculation.PerHourUnsocialHolidayAsString + " Kr";

                //Unsocial sum pay for day 2 to day 14
                claimCalculation.UnsocialSumPayD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.UnsocialEveningPayD2T14) + Convert.ToDecimal(claimCalculation.UnsocialNightPayD2T14) + Convert.ToDecimal(claimCalculation.UnsocialWeekendPayD2T14) + Convert.ToDecimal(claimCalculation.UnsocialGrandWeekendPayD2T14)));
                claimCalculation.UnsocialSumPayCalcD2T14 = claimCalculation.UnsocialEveningPayD2T14 + " Kr + " + claimCalculation.UnsocialNightPayD2T14 + " Kr + " + claimCalculation.UnsocialWeekendPayD2T14 + " Kr + " + claimCalculation.UnsocialGrandWeekendPayD2T14;

                //On call day pay for day 2 to day 14
                claimCalculation.OnCallDayPayD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claim.SickPayRateAsString) * Convert.ToDecimal(claimCalculation.OnCallDayD2T14) * Convert.ToDecimal(claimCalculation.PerHourOnCallDayAsString) / 100));
                claimCalculation.OnCallDayPayCalcD2T14 = claim.SickPayRateAsString + " % x " + claimCalculation.OnCallDayD2T14 + " timmar x " + claimCalculation.PerHourOnCallDayAsString + " Kr";

                //On call night pay for day 2 to day 14
                claimCalculation.OnCallNightPayD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claim.SickPayRateAsString) * Convert.ToDecimal(claimCalculation.OnCallNightD2T14) * Convert.ToDecimal(claimCalculation.PerHourOnCallNightAsString) / 100));
                claimCalculation.OnCallNightPayCalcD2T14 = claim.SickPayRateAsString + " % x " + claimCalculation.OnCallNightD2T14 + " timmar x " + claimCalculation.PerHourOnCallNightAsString + " Kr";

                //On call sum pay for day 2 to day 14
                claimCalculation.OnCallSumPayD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.OnCallDayPayD2T14) + Convert.ToDecimal(claimCalculation.OnCallNightPayD2T14)));
                claimCalculation.OnCallSumPayCalcD2T14 = claimCalculation.OnCallDayPayD2T14 + " Kr + " + claimCalculation.OnCallNightPayD2T14;

                //Sick pay for day 2 to day 14
                claimCalculation.SickPayD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.SalaryD2T14) + Convert.ToDecimal(claimCalculation.UnsocialSumPayD2T14) + Convert.ToDecimal(claimCalculation.OnCallSumPayD2T14)));
                claimCalculation.SickPayCalcD2T14 = claimCalculation.SalaryD2T14 + " Kr + " + claimCalculation.UnsocialSumPayD2T14 + " Kr + " + claimCalculation.OnCallSumPayD2T14 + " Kr";

                //Social fees for day 2 to day 14
                claimCalculation.SocialFeesD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claim.SocialFeeRateAsString) * (Convert.ToDecimal(claimCalculation.SalaryD2T14) + Convert.ToDecimal(claimCalculation.HolidayPayD2T14) + Convert.ToDecimal(claimCalculation.UnsocialSumPayD2T14) + Convert.ToDecimal(claimCalculation.OnCallSumPayD2T14)) / 100));
                claimCalculation.SocialFeesCalcD2T14 = claim.SocialFeeRateAsString + " % x (" + claimCalculation.SalaryD2T14 + " Kr + " + claimCalculation.HolidayPayD2T14 + " Kr + " + claimCalculation.UnsocialSumPayD2T14 + " Kr + " + claimCalculation.OnCallSumPayD2T14 + " Kr)";

                //Pensions and insurances for day 2 to day 14
                claimCalculation.PensionAndInsuranceD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claim.PensionAndInsuranceRateAsString) * (Convert.ToDecimal(claimCalculation.SalaryD2T14) + Convert.ToDecimal(claimCalculation.HolidayPayD2T14) + Convert.ToDecimal(claimCalculation.UnsocialSumPayD2T14) + Convert.ToDecimal(claimCalculation.OnCallSumPayD2T14)) / 100));
                claimCalculation.PensionAndInsuranceCalcD2T14 = claim.PensionAndInsuranceRateAsString + " % x (" + claimCalculation.SalaryD2T14 + " Kr + " + claimCalculation.HolidayPayD2T14 + " Kr + " + claimCalculation.UnsocialSumPayD2T14 + " Kr + " + claimCalculation.OnCallSumPayD2T14 + " Kr)";

                //Sum for day 2 to day 14
                claimCalculation.CostD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.SalaryD2T14) + Convert.ToDecimal(claimCalculation.HolidayPayD2T14) + Convert.ToDecimal(claimCalculation.UnsocialSumPayD2T14) + Convert.ToDecimal(claimCalculation.OnCallSumPayD2T14) + Convert.ToDecimal(claimCalculation.SocialFeesD2T14) + Convert.ToDecimal(claimCalculation.PensionAndInsuranceD2T14)));
                claimCalculation.CostCalcD2T14 = claimCalculation.SalaryD2T14 + " Kr + " + claimCalculation.HolidayPayD2T14 + " Kr + " + claimCalculation.UnsocialSumPayD2T14 + " Kr + " + claimCalculation.OnCallSumPayD2T14 + " Kr + " + claimCalculation.SocialFeesD2T14 + " Kr + " + claimCalculation.PensionAndInsuranceD2T14 + " Kr";

                //Total sum for day 1 to day 14
                claimCalculation.TotalCostD1T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.CostQD) + Convert.ToDecimal(claimCalculation.CostD2T14)));
                claimCalculation.TotalCostCalcD1T14 = claimCalculation.CostQD + " Kr + " + claimCalculation.CostD2T14;

                //claim.ClaimStatusId = 5;
                //claim.StatusDate = DateTime.Now;
                prevSickDayIdx = prevSickDayIdx + applicableSickDays;
                db.ClaimCalculations.Add(claimCalculation);
                totalCostD1D14 = totalCostD1D14 + Convert.ToDecimal(claimCalculation.TotalCostD1T14);
                if (claim.NumberOfSickDays == 1)
                {
                    totalCostCalcD1D14 = claimCalculation.CostQD;
                }
                else if (idx == 0)
                {
                    totalCostCalcD1D14 = claimCalculation.CostQD + " Kr + " + claimCalculation.CostD2T14;
                }
                else
                {
                    totalCostCalcD1D14 = totalCostCalcD1D14 + " Kr " + claimCalculation.CostD2T14;
                }
            }
            claim.TotalCostD1T14 = String.Format("{0:0.00}", totalCostD1D14);
            claim.TotalCostCalcD1T14 = totalCostCalcD1D14;
            db.Entry(claim).State = EntityState.Modified;
            db.SaveChanges();
        }

        private static void SendEmail(MailMessage message)
        {
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", Convert.ToInt32(587));
            NetworkCredential credentials = new NetworkCredential(ConfigurationManager.AppSettings["mailAccount"], ConfigurationManager.AppSettings["mailPassword"]);
            smtpClient.Credentials = credentials;
            smtpClient.EnableSsl = true;
            smtpClient.Send(message);
            return;
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
