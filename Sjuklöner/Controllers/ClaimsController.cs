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
        public ActionResult IndexPageOmbud()
        {
            IndexPageOmbudVM indexPageOmbudVM = new IndexPageOmbudVM();

            var me = db.Users.Find(User.Identity.GetUserId());

            var claims = db.Claims.Where(c => c.OwnerId == me.Id).ToList();
            if (claims.Count > 0)
            {
                indexPageOmbudVM.DecidedClaims = claims.Where(c => c.ClaimStatusId == 1).ToList(); //Old "Rejected
                indexPageOmbudVM.DraftClaims = claims.Where(c => c.ClaimStatusId == 2).ToList();
                indexPageOmbudVM.UnderReviewClaims = claims.Where(c => c.ClaimStatusId == 3).ToList().Concat(claims.Where(c => c.ClaimStatusId == 5)).ToList();
            }

            return View("IndexPageOmbud", indexPageOmbudVM);
        }

        // GET: Claims
        public ActionResult IndexPageAdmOff(string referenceNumber = null)
        {
            IndexPageAdmOffVM indexPageAdmOffVM = new IndexPageAdmOffVM();

            var me = db.Users.Find(User.Identity.GetUserId());

            var claims = db.Claims.Include(c => c.CareCompany).ToList();
            if (claims.Count > 0)
            {
                indexPageAdmOffVM.DecidedClaims = claims.Where(c => c.ClaimStatusId == 1).ToList();
                indexPageAdmOffVM.InInboxClaims = claims.Where(c => c.ClaimStatusId == 5).ToList();
                indexPageAdmOffVM.UnderReviewClaims = claims.Where(c => c.ClaimStatusId == 3).ToList();
            }

            return View("IndexPageAdmOff", indexPageAdmOffVM);
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
                create1VM.FirstDayOfSicknessDate = DateTime.Now.AddDays(-1);
                create1VM.LastDayOfSicknessDate = DateTime.Now.AddDays(-1);
                return View("Create1", create1VM);
            }
            else if (refNumber != null) //This is an existing claim (either demo or no demo)
            {
                create1VM = LoadExistingValuesCreate1(refNumber);
                return View("Create1", create1VM);
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
            create1VM.AssistantSSN = claim.AssistantSSN;
            create1VM.CustomerSSN = claim.CustomerSSN;
            create1VM.FirstDayOfSicknessDate = claim.QualifyingDate;
            create1VM.LastDayOfSicknessDate = claim.LastDayOfSicknessDate;
            create1VM.OrganisationNumber = claim.OrganisationNumber;
            create1VM.ReferenceNumber = claim.ReferenceNumber;

            return create1VM;
        }

        private Create1VM SetDefaultValuesCreate1()
        {
            Create1VM defaultValuesCreate1VM = new Create1VM();

            defaultValuesCreate1VM.OrganisationNumber = "556881-2118";
            defaultValuesCreate1VM.AssistantSSN = "930701-4168";
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
            if (submitButton == "Till steg 2" || submitButton == "Spara")
            {
                if (refNumber == null) //new claim
                {
                    refNumber = SaveNewClaim(create1VM);
                }
                else if (refNumber != null) //existing claim
                {
                    UpdateExistingClaim(create1VM);
                    var claim = db.Claims.Where(c => c.ReferenceNumber == create1VM.ReferenceNumber).FirstOrDefault();
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
            else
            {
                return View(create1VM);
            }
        }

        private void AdjustClaimDays(Create1VM create1VM)
        {
            //Find existing claim days
            var existingClaimDays = db.ClaimDays.Where(c => c.ReferenceNumber == create1VM.ReferenceNumber).OrderBy(c => c.SickDayNumber).ToList();

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
                    claimDay.ReferenceNumber = create1VM.ReferenceNumber;
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
                        claimDay.ReferenceNumber = create1VM.ReferenceNumber;
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
                        claimDay.ReferenceNumber = create1VM.ReferenceNumber;
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
                        claimDay.ReferenceNumber = create1VM.ReferenceNumber;
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
                    claimDay.ReferenceNumber = create1VM.ReferenceNumber;
                    claimDay.SickDayNumber = i + 1;
                    newClaimDays.Add(claimDay);
                }
            }
            //Remove not neeeded claim days (they are the remaining claim days in the existing claim day list) from the db
            foreach (var claimDay in existingClaimDays)
            {
                db.ClaimDays.Remove(db.ClaimDays.Where(c => c.ReferenceNumber == create1VM.ReferenceNumber).Where(c => c.Id == claimDay.Id).First());
            }
            db.ClaimDays.AddRange(newClaimDays);
            db.SaveChanges();
        }

        private string SaveNewClaim(Create1VM create1VM)
        {
            Claim claim = new Claim();
            claim.ReferenceNumber = GenerateReferenceNumber();
            claim.CompletionStage = 1;
            claim.OrganisationNumber = create1VM.OrganisationNumber;
            claim.CustomerSSN = create1VM.CustomerSSN;
            claim.AssistantSSN = create1VM.AssistantSSN;
            claim.StandInSSN = create1VM.StandInSSN;
            claim.Email = create1VM.Email;
            claim.StatusDate = DateTime.Now;
            claim.QualifyingDate = create1VM.FirstDayOfSicknessDate;
            claim.LastDayOfSicknessDate = create1VM.LastDayOfSicknessDate;
            claim.NumberOfSickDays = 1 + (create1VM.LastDayOfSicknessDate.Date - create1VM.FirstDayOfSicknessDate.Date).Days;
            var currentUserId = User.Identity.GetUserId();
            claim.OwnerId = currentUserId;
            claim.ClaimStatusId = 2;  //ClaimStatus.Name = "Utkast"
            claim.CareCompanyId = (int)db.Users.Where(u => u.Id == currentUserId).First().CareCompanyId;
            db.Claims.Add(claim);
            db.SaveChanges();
            return claim.ReferenceNumber;
        }

        private void UpdateExistingClaim(Create1VM create1VM)
        {
            var claim = db.Claims.Where(c => c.ReferenceNumber == create1VM.ReferenceNumber).FirstOrDefault();
            claim.OrganisationNumber = create1VM.OrganisationNumber;
            claim.CustomerSSN = create1VM.CustomerSSN;
            claim.AssistantSSN = create1VM.AssistantSSN;
            claim.StandInSSN = create1VM.StandInSSN;
            claim.Email = create1VM.Email;
            claim.StatusDate = DateTime.Now;
            claim.QualifyingDate = create1VM.FirstDayOfSicknessDate;
            claim.LastDayOfSicknessDate = create1VM.LastDayOfSicknessDate;
            claim.NumberOfSickDays = 1 + (create1VM.LastDayOfSicknessDate.Date - create1VM.FirstDayOfSicknessDate.Date).Days;
            var currentUserId = User.Identity.GetUserId();
            claim.OwnerId = currentUserId;
            claim.ClaimStatusId = 2;  //ClaimStatus.Name = "Utkast"
            claim.CareCompanyId = (int)db.Users.Where(u => u.Id == currentUserId).First().CareCompanyId;
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
                Thread.CurrentThread.CurrentCulture = new CultureInfo("sv-SV");

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
                Thread.CurrentThread.CurrentCulture = new CultureInfo("sv-SV");

                scheduleRow.ScheduleRowDateString = dateInSchedule.ToString(format: "ddd d MMM");
                scheduleRow.DayDate = dateInSchedule;
                scheduleRow.ScheduleRowWeekDay = DateTimeFormatInfo.CurrentInfo.GetDayName(dateInSchedule.DayOfWeek).ToString().Substring(0, 3);

                if (claim.CompletionStage >= 2)
                {
                    scheduleRow.Hours = claimDays[i].Hours;
                    scheduleRow.UnsocialEvening = claimDays[i].UnsocialEvening;
                    scheduleRow.UnsocialWeekend = claimDays[i].UnsocialWeekend;
                    scheduleRow.UnsocialGrandWeekend = claimDays[i].UnsocialGrandWeekend;

                    scheduleRow.OnCallDay = claimDays[i].OnCallDay;
                    scheduleRow.OnCallNight = claimDays[i].OnCallNight;
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
            if (claim.CompletionStage < 2)
            {
                claim.CompletionStage = 2;
                db.Entry(claim).State = EntityState.Modified;
            }

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
                dayIdx++;
            }
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
            else //Demo
            {
                create3VM = LoadDemoClaimCreate3VM(refNumber);
            }
            return View("Create3", create3VM);
        }

        private Create3VM LoadClaimCreate3VM(Claim claim)
        {
            Create3VM create3VM = new Create3VM();
            create3VM.ClaimNumber = claim.ReferenceNumber;
            if (claim.CompletionStage >= 3) //CompletionStage >= 3 means that stage 3 has been filled in earlier. This is an update of stage 3
            {
                create3VM.SickPay = claim.ClaimedSickPay;
                create3VM.HolidayPay = claim.ClaimedHolidayPay;
                create3VM.SocialFees = claim.ClaimedSocialFees;
                create3VM.PensionAndInsurance = claim.ClaimedPensionAndInsurance;
                create3VM.ClaimSum = claim.ClaimedSum;
            }
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
            create3VM.SickPay = (decimal)0.8 * ((120 * hours) + ((decimal)65.5 * unsocialSum) + ((decimal)43.2 * oncallSum)) - (decimal)0.8 * ((120 * Convert.ToDecimal(claimDays[0].Hours)) + ((decimal)65.5 * Convert.ToDecimal(claimDays[0].UnsocialEvening)) + ((decimal)65.5 * Convert.ToDecimal(claimDays[0].UnsocialNight)) + ((decimal)65.5 * Convert.ToDecimal(claimDays[0].UnsocialWeekend)) + ((decimal)65.5 * Convert.ToDecimal(claimDays[0].UnsocialGrandWeekend)) + ((decimal)65.5 * Convert.ToDecimal(claimDays[0].OnCallDay)) + ((decimal)65.5 * Convert.ToDecimal(claimDays[0].OnCallNight)));
            create3VM.HolidayPay = (decimal)0.12 * (decimal)0.8 * ((120 * hours) + ((decimal)65.5 * unsocialSum) + ((decimal)43.2 * oncallSum));
            create3VM.SocialFees = (decimal)0.3142 * (create3VM.SickPay + create3VM.HolidayPay);
            create3VM.PensionAndInsurance = (decimal)0.06 * (create3VM.SickPay + create3VM.HolidayPay);
            create3VM.ClaimSum = create3VM.HolidayPay + create3VM.SickPay + create3VM.PensionAndInsurance + create3VM.SocialFees;
            return create3VM;
        }

        // POST: Claims/Create3
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create3(Create3VM create3VM, string refNumber, string submitButton)
        {
            if (submitButton == "Skicka in")
            {
                create3VM.ClaimSum = create3VM.SickPay + create3VM.HolidayPay + create3VM.SocialFees + create3VM.PensionAndInsurance;
                SaveClaim3(create3VM);
                var claim = db.Claims.Where(c => c.ReferenceNumber == refNumber).FirstOrDefault();
                claim.ClaimStatusId = 5;
                claim.StatusDate = DateTime.Now;
                db.Entry(claim).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ShowReceipt", create3VM);
            }
            else if (submitButton == "Avbryt")
            {
                return RedirectToAction("IndexPageOmbud");
            }
            else
            {
                SaveClaim3(create3VM);
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
                claim.ClaimedSickPay = create3VM.SickPay;
                claim.ClaimedHolidayPay = create3VM.HolidayPay;
                claim.ClaimedSocialFees = create3VM.SocialFees;
                claim.ClaimedPensionAndInsurance = create3VM.PensionAndInsurance;
                claim.ClaimedSum = create3VM.ClaimSum;
                db.Entry(claim).State = EntityState.Modified;

                //Calculate the model sum
                //Find ClaimDay records for the claim
                var claimDays = db.ClaimDays.Where(c => c.ReferenceNumber == create3VM.ClaimNumber).OrderBy(c => c.ReferenceNumber).ToList();
                //The id of the applicable collective agreement needs to be included in the method call. FOR NOW IT IS HARDCODED TO 1! This should be updated when the collective agreement is integrated with the assistant/carecompany class(es) and the claim process. 

                CalculateModelSum(claim, claimDays, 1);
            }
        }

        public ActionResult ShowReceipt(Create3VM create3VM)
        {
            var claim = db.Claims.Where(c => c.ReferenceNumber == create3VM.ClaimNumber).FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(claim.Email))
            {
                MailMessage message = new MailMessage();
                message.From = new MailAddress("ourrobotdemo@gmail.com");

                message.To.Add(new MailAddress(claim.Email));
                //message.To.Add(new MailAddress("e.niklashagman@gmail.com"));
                message.Subject = "Ny ansökan med referensnummer: " + create3VM.ClaimNumber;
                message.Body = "Vi har mottagit din ansökan med referensnummer " + create3VM.ClaimNumber + ". Normalt får du ett beslut inom 1 - 3 dagar." + "\n" + "\n" +
                                                    "Med vänliga hälsningar, Vård- och omsorgsförvaltningen";

                SendEmail(message);
            }

            string appdataPath = Environment.ExpandEnvironmentVariables("%appdata%\\Bitoreq AB\\KoPerNikus");

            Directory.CreateDirectory(appdataPath);
            using (var writer = XmlWriter.Create(appdataPath + "\\info.xml"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("claiminformation");
                writer.WriteElementString("SSN", claim.CustomerSSN);
                writer.WriteElementString("OrgNumber", claim.OrganisationNumber);
                writer.WriteElementString("ReferenceNumber", claim.ReferenceNumber);
                writer.WriteElementString("ClaimId", claim.Id.ToString());
                writer.WriteElementString("UserId", claim.OwnerId);
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
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
                recommendationVM.ProxyCheck = true;
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
            ApplicationUser user = db.Users.Where(u => u.Id == message.ApplicationUser_Id).FirstOrDefault();
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
            message.ApplicationUser_Id = User.Identity.GetUserId();
            db.Messages.Add(message);
            db.SaveChanges();
            return PartialView("_SendMessage");
        }


        public ActionResult ShowClaimDetails(string referenceNumber)
        {
            var claim = db.Claims.Include(c => c.ClaimStatus).Where(c => c.ReferenceNumber == referenceNumber).FirstOrDefault();

            List<ClaimDay> claimDays = new List<ClaimDay>();
            claimDays = db.ClaimDays.Where(c => c.ReferenceNumber == referenceNumber).OrderBy(c => c.SickDayNumber).ToList();

            var ombudId = claim.OwnerId;
            var ombud = db.Users.Where(u => u.Id == ombudId).FirstOrDefault();


            ClaimDetailsVM claimDetailsVM = new ClaimDetailsVM();

            claimDetailsVM.ReferenceNumber = referenceNumber;
            claimDetailsVM.StatusName = claim.ClaimStatus.Name;

            //Kommun
            claimDetailsVM.Council = "Helsingborgs kommun";
            claimDetailsVM.Administration = "Vård- och omsorgsförvaltningen";

            //Assistansberättigad
            claimDetailsVM.CustomerName = "Kund Kundsson";
            claimDetailsVM.CustomerSSN = claim.CustomerSSN;
            claimDetailsVM.CustomerAddress = "Tolvangatan 12, 123 45 Tolvsta";
            claimDetailsVM.CustomerPhoneNumber = "019-124 6578";

            //Ombud/uppgiftslämnare
            claimDetailsVM.OmbudName = ombud.FirstName + " " + ombud.LastName;
            claimDetailsVM.OmbudPhoneNumber = ombud.PhoneNumber;

            //Assistansanordnare
            claimDetailsVM.CompanyName = "Smart Assistans";
            claimDetailsVM.OrganisationNumber = claim.OrganisationNumber;
            claimDetailsVM.GiroNumber = "4321-9876";
            claimDetailsVM.CompanyAddress = "Omsorgsgatan 117, 987 00 Omsorgköping";
            claimDetailsVM.CompanyPhoneNumber = "010-986 0000";
            claimDetailsVM.CollectiveAgreement = "Vårdföretagarna-Kommunal, Personlig assistans (Branch G)";

            //Insjuknad ordinarie assistent
            //Källa till belopp: https://assistanskoll.se/Guider-Att-arbeta-som-personlig-assistent.html (Vårdföretagarna)
            claimDetailsVM.AssistantName = "Sixten Assistentsson";
            claimDetailsVM.AssistantSSN = claim.AssistantSSN;
            if (claimDays.Count() > 0)
            {
                claimDetailsVM.QualifyingDayDate = claimDays[0].Date.ToShortDateString();
                claimDetailsVM.LastDayOfSicknessDate = claimDays.Last().Date.ToShortDateString();
            }

            claimDetailsVM.Salary = (decimal)120.00;  //This property is used either as an hourly salary or as a monthly salary in claimDetailsVM.cs.
            claimDetailsVM.HourlySalary = (decimal)120.00;    //This property is used as the hourly salary in calculations.
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

            claimDetailsVM.QualifyingDayDate = claim.QualifyingDate.ToShortDateString();
            claimDetailsVM.NumberOfSickDays = claim.NumberOfSickDays;

            //Underlag lönekostnader
            claimDetailsVM.PerHourUnsocialEvening = claim.PerHourUnsocialEvening;
            claimDetailsVM.PerHourUnsocialNight = claim.PerHourUnsocialNight;
            claimDetailsVM.PerHourUnsocialWeekend = claim.PerHourUnsocialWeekend;
            claimDetailsVM.PerHourUnsocialHoliday = claim.PerHourUnsocialHoliday;
            claimDetailsVM.PerHourOnCallWeekday = claim.PerHourOnCallWeekday;
            claimDetailsVM.PerHourOnCallWeekend = claim.PerHourOnCallWeekend;

            claimDetailsVM.Workplace = "Björkängen, Birgittagården";
            claimDetailsVM.CollectiveAgreement = "Vårdföretagarna";

            claimDetailsVM.HolidayPayRate = claim.HolidayPayRate;
            claimDetailsVM.SocialFeeRate = claim.SocialFeeRate;
            claimDetailsVM.PensionAndInsuranceRate = claim.PensionAndInsuranceRate;
            claimDetailsVM.SickPayRate = claim.SickPayRate;
            //claimDetailsVM.QualifyingDayDate = scheduleVM.ScheduleRowList.First().ScheduleRowDate;



            //Recommended amount, using hours only as input (x lines forward)
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
            decisionVM.AssistantSSN = claim.AssistantSSN;
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
                comment.ApplicationUser_Id = "17899c7e-8dd1-4950-9cd4-beeab81f5cf3";
                comment.CommentDate = DateTime.Now;
                comment.Comment = decisionVM.Comment;
                db.Messages.Add(comment);
                db.SaveChanges();
            }

            if (!string.IsNullOrWhiteSpace(claim.Email))
            {
                MailMessage message = new MailMessage();
                message.From = new MailAddress("ourrobotdemo@gmail.com");

                message.To.Add(new MailAddress(claim.Email));
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
        public ActionResult DeleteConfirmed(int id)
        {
            Claim claim = db.Claims.Find(id);
            db.ClaimDays.RemoveRange(db.ClaimDays.Where(c => c.ReferenceNumber == claim.ReferenceNumber));
            db.Claims.Remove(claim);
            db.SaveChanges();
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

        private void CalculateModelSum(Claim claim, List<ClaimDay> claimDays, int collectiveAgreementId)
        {
            claim.HolidayPayRate = (decimal)12.00;
            claim.SocialFeeRate = (decimal)31.42;
            claim.PensionAndInsuranceRate = (decimal)6.00;
            claim.SickPayRate = (decimal)80.00;
            //claim.QualifyingDayDate = scheduleVM.ScheduleRowList.First().ScheduleRowDate;

            //Recommended amount, using hours only as input (x lines forward)
            claim.SickPayRateAsString = "80,00";
            claim.HolidayPayRateAsString = "12,00";
            claim.SocialFeeRateAsString = "31,42";
            claim.PensionAndInsuranceRateAsString = "6,00";
            claim.HourlySalaryAsString = "120,00";

            //Assign a CollAgreementInfo id to each claim day. This is required in order for the correct hourly pay to be used in the calculations.
            var collectiveAgreementInfos = db.CollectiveAgreementInfos.Where(c => c.CollectiveAgreementHeaderId == collectiveAgreementId).OrderBy(c => c.StartDate).ToList();
            DateTime? claimDayDate;
            bool infoFound;
            bool infoUsed;
            int infoIdx;
            int infoUsedIdx;
            List<int> usedCollectiveAgreementInfoIds = new List<int>(); //This list is used for figuring out which collective agreement infos have been used.
            claimDays.OrderBy(c => c.SickDayNumber);
            foreach (var claimDay in claimDays)
            {
                claimDayDate = claim.QualifyingDate.AddDays(claimDay.SickDayNumber - 1);
                infoFound = false;
                infoIdx = 0;
                do
                {
                    if (claimDayDate >= collectiveAgreementInfos[infoIdx].StartDate && claimDayDate <= collectiveAgreementInfos[infoIdx].EndDate)
                    {
                        //claimDay.PerHourUnsocialEveningAsString = collectiveAgreementInfos[infoIdx].PerHourUnsocialEvening;
                        //claimDay.PerHourUnsocialNightAsString = collectiveAgreementInfos[infoIdx].PerHourUnsocialNight;
                        //claimDay.PerHourUnsocialWeekendAsString = collectiveAgreementInfos[infoIdx].PerHourUnsocialWeekend;
                        //claimDay.PerHourUnsocialHolidayAsString = collectiveAgreementInfos[infoIdx].PerHourUnsocialHoliday;
                        //claimDay.PerHourOnCallDayAsString = collectiveAgreementInfos[infoIdx].PerHourOnCallWeekday;
                        //claimDay.PerHourOnCallNightAsString = collectiveAgreementInfos[infoIdx].PerHourOnCallWeekend;

                        //claimDay.PerHourUnsocialEvening = Convert.ToDecimal(collectiveAgreementInfos[infoIdx].PerHourUnsocialEvening);
                        //claimDay.PerHourUnsocialNight = Convert.ToDecimal(collectiveAgreementInfos[infoIdx].PerHourUnsocialNight);
                        //claimDay.PerHourUnsocialWeekend = Convert.ToDecimal(collectiveAgreementInfos[infoIdx].PerHourUnsocialWeekend);
                        //claimDay.PerHourUnsocialHoliday = Convert.ToDecimal(collectiveAgreementInfos[infoIdx].PerHourUnsocialHoliday);
                        //claimDay.PerHourOnCallWeekday = Convert.ToDecimal(collectiveAgreementInfos[infoIdx].PerHourOnCallWeekday);
                        //claimDay.PerHourOnCallWeekend = Convert.ToDecimal(collectiveAgreementInfos[infoIdx].PerHourOnCallWeekend);

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
                    //claimDay.PerHourUnsocialEveningAsString = "21,08";
                    //claimDay.PerHourUnsocialNightAsString = "42,54";
                    //claimDay.PerHourUnsocialWeekendAsString = "52,47";
                    //claimDay.PerHourUnsocialHolidayAsString = "105,03";
                    //claimDay.PerHourOnCallDayAsString = "28,13";
                    //claimDay.PerHourOnCallNightAsString = "56,32";

                    //claimDay.PerHourUnsocialEvening = (decimal)21.08;
                    //claimDay.PerHourUnsocialNight = (decimal)42.54;
                    //claimDay.PerHourUnsocialWeekend = (decimal)52.47;
                    //claimDay.PerHourUnsocialHoliday = (decimal)105.03;
                    //claimDay.PerHourOnCallWeekday = (decimal)28.13;
                    //claimDay.PerHourOnCallWeekend = (decimal)56.32;
                    //Id = 0 indicates that default values are used for this claim day
                    infoUsed = false;
                    infoUsedIdx = 0;
                    if (usedCollectiveAgreementInfoIds.Count() == 0)
                    {
                        usedCollectiveAgreementInfoIds.Add(0);
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
                            usedCollectiveAgreementInfoIds.Add(0);
                        }
                    }
                }
            }

            //Repeat the calculation for each CollectiveAgreementInfo that applies to the sickleave period
            int applicableSickDays;
            int prevSickDayIdx = 0;
            int startIdx;
            int stopIdx;
            decimal totalCostD1D14 = 0;
            string totalCostCalcD1D14 = "";

            for (int idx = 0; idx < usedCollectiveAgreementInfoIds.Count(); idx++)
            {
                ClaimCalculation claimCalculation = new ClaimCalculation();
                claimCalculation.ReferenceNumber = claim.ReferenceNumber;
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
                claimCalculation.EndDate = claim.QualifyingDate.AddDays(prevSickDayIdx + applicableSickDays);

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
                if (idx == 0)
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
