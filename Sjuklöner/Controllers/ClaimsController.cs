using System;
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
                //indexPageOmbudVM.ApprovedClaims = claims.Where(c => c.ClaimStatusId == 4).ToList();
            }

            return View("IndexPageOmbud", indexPageOmbudVM);
        }

        // GET: Claims
        public ActionResult IndexPageAdmOff(string referenceNumber = null)
        {
            ////Check if the AdmOff arrived from the decision page and rejected the claim. In that case the ClaimStatus of the claim needs to be updated. 
            //if (referenceNumber != null)
            //{
            //    //Rejected claim
            //    var claim = db.Claims.Where(c => c.ReferenceNumber == referenceNumber).FirstOrDefault();
            //    claim.ClaimStatusId = 1;
            //    //claim.DecidedSum = 0;
            //    claim.StatusDate = DateTime.Now;
            //    db.Entry(claim).State = EntityState.Modified;
            //    db.SaveChanges();

            //    if (!string.IsNullOrWhiteSpace(claim.Email))
            //    {
            //        MailMessage message = new MailMessage();
            //        message.From = new MailAddress("ourrobotdemo@gmail.com");

            //        message.To.Add(new MailAddress(claim.Email));
            //        //message.To.Add(new MailAddress("e.niklashagman@gmail.com"));
            //        message.Subject = "Avlsagen ansökan: " + referenceNumber;
            //        message.Body = "Hej, ansökan med referensnummer " + referenceNumber + " har blivit avslagen. Vänligen dubbelkolla informationen";

            //        SendEmail(message);
            //    }

            //    string appdataPath = Environment.ExpandEnvironmentVariables("%appdata%\\Bitoreq AB\\KoPerNikus");

            //    Directory.CreateDirectory(appdataPath);
            //    using (var writer = XmlWriter.Create(appdataPath + "\\decided.xml"))
            //    {
            //        writer.WriteStartDocument();
            //        writer.WriteStartElement("claiminformation");
            //        writer.WriteElementString("SSN", claim.CustomerSSN);
            //        writer.WriteElementString("OrgNumber", claim.OrganisationNumber);
            //        writer.WriteElementString("ReferenceNumber", claim.ReferenceNumber);
            //        writer.WriteEndElement();
            //        writer.WriteEndDocument();
            //    }
            //}

            IndexPageAdmOffVM indexPageAdmOffVM = new IndexPageAdmOffVM();

            var me = db.Users.Find(User.Identity.GetUserId());

            var claims = db.Claims.Include(c => c.CareCompany).ToList();
            if (claims.Count > 0)
            {
                indexPageAdmOffVM.DecidedClaims = claims.Where(c => c.ClaimStatusId == 1).ToList();
                indexPageAdmOffVM.InInboxClaims = claims.Where(c => c.ClaimStatusId == 5).ToList();
                indexPageAdmOffVM.UnderReviewClaims = claims.Where(c => c.ClaimStatusId == 3).ToList();
                //indexPageAdmOffVM.ApprovedClaims = claims.Where(c => c.ClaimStatusId == 4).ToList();
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

        // GET: Claims/Create
        public ActionResult Create(string refNumber, int? id)
        {

            if (refNumber != null && id == 2) //True if returning from the view "ClaimAmount" and wanting to see the view "Hours"
            {
                var claimDays = db.ClaimDays.Where(c => c.ReferenceNumber == refNumber).OrderBy(c => c.SickDayNumber).ToList();

                ScheduleVM scheduleVM = new ScheduleVM();
                List<ScheduleRow> scheduleRowList = new List<ScheduleRow>();

                for (int i = 0; i < claimDays.Count; i++)
                {
                    //Instantiate a new scheduleRow in the viewmodel
                    ScheduleRow scheduleRow = new ScheduleRow();

                    scheduleRow.ScheduleRowDateString = claimDays[i].DateString;
                    scheduleRow.DayDate = DateTime.Now.AddDays(i);

                    scheduleRow.StartTimeHour = claimDays[i].StartHour;
                    scheduleRow.StartTimeMinute = claimDays[i].StartMinute;
                    scheduleRow.StopTimeHour = claimDays[i].StopHour;
                    scheduleRow.StopTimeMinute = claimDays[i].StopMinute;
                    scheduleRow.NumberOfHours = claimDays[i].NumberOfHours;
                    scheduleRow.NumberOfUnsocialHours = claimDays[i].NumberOfUnsocialHours;
                    scheduleRow.NumberOfUnsocialHoursEvening = claimDays[i].NumberOfUnsocialHoursEvening;
                    scheduleRow.NumberOfUnsocialHoursNight = claimDays[i].NumberOfUnsocialHoursNight;

                    scheduleRow.StartTimeHourOnCall = claimDays[i].StartHourOnCall;
                    scheduleRow.StartTimeMinuteOnCall = claimDays[i].StartMinuteOnCall;
                    scheduleRow.StopTimeHourOnCall = claimDays[i].StartHourOnCall;
                    scheduleRow.StopTimeMinuteOnCall = claimDays[i].StopMinuteOnCall;
                    scheduleRow.NumberOfOnCallHours = claimDays[i].NumberOfOnCallHours;
                    scheduleRow.NumberOfOnCallHoursEvening = claimDays[i].NumberOfOnCallHoursEvening;
                    scheduleRow.NumberOfOnCallHoursNight = claimDays[i].NumberOfOnCallHoursNight;

                    scheduleRow.StartTimeHourSI = claimDays[i].StartHourSI;
                    scheduleRow.StartTimeMinuteSI = claimDays[i].StartMinuteSI;
                    scheduleRow.StopTimeHourSI = claimDays[i].StopHourSI;
                    scheduleRow.StopTimeMinuteSI = claimDays[i].StopMinuteSI;
                    scheduleRow.NumberOfHoursSI = claimDays[i].NumberOfHoursSI;
                    scheduleRow.NumberOfUnsocialHoursSI = claimDays[i].NumberOfUnsocialHoursSI;
                    scheduleRow.NumberOfUnsocialHoursEveningSI = claimDays[i].NumberOfUnsocialHoursEveningSI;
                    scheduleRow.NumberOfUnsocialHoursNightSI = claimDays[i].NumberOfUnsocialHoursNightSI;

                    scheduleRow.StartTimeHourOnCallSI = claimDays[i].StartHourOnCallSI;
                    scheduleRow.StartTimeMinuteOnCallSI = claimDays[i].StartMinuteOnCallSI;
                    scheduleRow.StopTimeHourOnCallSI = claimDays[i].StartHourOnCallSI;
                    scheduleRow.StopTimeMinuteOnCallSI = claimDays[i].StopMinuteOnCallSI;
                    scheduleRow.NumberOfOnCallHoursSI = claimDays[i].NumberOfOnCallHoursSI;
                    scheduleRow.NumberOfOnCallHoursEveningSI = claimDays[i].NumberOfOnCallHoursEveningSI;
                    scheduleRow.NumberOfOnCallHoursNightSI = claimDays[i].NumberOfOnCallHoursNightSI;

                    scheduleRowList.Add(scheduleRow);
                }

                scheduleVM.ReferenceNumber = refNumber;
                scheduleVM.ScheduleRowList = scheduleRowList;

                return View("Hours", scheduleVM);
            }
            else if (refNumber != null && id == 1)  //True if returning from the view "ClaimAmount" and wanting to see the view "Create". Also true if updating an existing draft claim.
            {
                var claim = db.Claims.Where(c => c.ReferenceNumber == refNumber).FirstOrDefault();
                if (claim != null)
                {
                    AssistantClaimVM existingClaim = new AssistantClaimVM();
                    existingClaim.AssistantSSN = claim.AssistantSSN;
                    existingClaim.CustomerSSN = claim.CustomerSSN;
                    existingClaim.FirstDayOfSicknessDate = claim.QualifyingDate;
                    existingClaim.LastDayOfSicknessDate = claim.LastDayOfSicknessDate;
                    existingClaim.OrganisationNumber = claim.OrganisationNumber;

                    //THIS IS A DANGEROUS LINE. IT MAKES THE REFERENCENUMBER NULL!
                    existingClaim.ClaimReference = refNumber;
                    return View("Create", existingClaim);
                }
            }

            AssistantClaimVM claimVM = new AssistantClaimVM();

            claimVM.AssistantSSN = "930701-4168";
            claimVM.CustomerSSN = "391025-7246";
            claimVM.FirstDayOfSicknessDate = DateTime.Now.AddDays(-4);
            claimVM.LastDayOfSicknessDate = DateTime.Now.AddDays(-1);

            claimVM.Rejected = false;

            return View(claimVM);
        }

        // POST: Claims/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpGet]
        //public ActionResult AddHours(AssistantClaimVM claimVM)
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Create(AssistantClaimVM claimVM, string refNumber)
        public ActionResult Create(AssistantClaimVM claimVM, string answer)
        //public ActionResult Create([Bind(Include = "Id,CustomerSSN,QualifyingDate,LastDayOfSicknessDate")] Claim claim)
        //public ActionResult Create([Bind(Include = "Id,OwnerId,ClaimStatusId,ReferenceNumber,StatusDate,DeadlineDate,CustomerFirstName,CustomerLastName,CustomerSSN,HourlySalary,HolidayPayRate,PayrollTaxRate,InsuranceRate,PensionRate,QualifyingDate,LastDayOfSicknessDate,HoursQualifyingDay,HolidayPayQualDay,PayrollTaxQualDay,InsuranceQualDay,PensionQualDay,ClaimQualDay,HoursDay2To14,HourlySickPay,SickPayDay2To14,HolidayPayDay2To14,UnsocialHoursBonusDay2To14,OnCallDutyDay2To14,PayrollTaxDay2To14,InsuranceDay2To14,PensionDay2To14,ClaimDay2To14,ClaimSum")] Claim claim)
        {
            if (answer == "Avbryt")
            {
                return RedirectToAction("Index");
            }
            else
            {
                string newReferenceNumber = "";
                string existingReferenceNumber = claimVM.ClaimReference;

                if (ModelState.IsValid)
                {
                    List<ScheduleRow> rowList = new List<ScheduleRow>();
                    ScheduleVM scheduleVM = new ScheduleVM();

                    //Check that the last day of sickness is equal to or greater than the first day of sickness.
                    if (claimVM.LastDayOfSicknessDate >= claimVM.FirstDayOfSicknessDate)
                    {
                        ////Check if a claim with overlapping dates already has been saved THIS CHECK COMMENTED OUT FOR DEMO 
                        //string currentUserId = User.Identity.GetUserId();
                        //if (overlappingClaim(claimVM.LastDayOfSicknessDate, claimVM.FirstDayOfSicknessDate, currentUserId, claimVM.CustomerSSN))
                        //{
                        //    claimVM.Rejected = true;
                        //    claimVM.RejectReason = "Du har redan ansökt om ersättning för minst en av dagarna för samma kund." + "\n" + "Vänligen uppdatera ansökan.";
                        //    return View(claimVM);
                        //}

                        //Check that the assistant's profile is complete. If not complete, then display a message to the assistant and request the missing information

                        if (existingReferenceNumber != null)
                        {
                            //This is an update of an existing claim record. Find the record and update it.
                            var existingClaim = db.Claims.Where(c => c.ReferenceNumber == existingReferenceNumber).FirstOrDefault();
                            if (existingClaim != null)
                            {
                                existingClaim.AssistantSSN = claimVM.AssistantSSN;
                                existingClaim.CustomerSSN = claimVM.CustomerSSN;
                                existingClaim.QualifyingDate = claimVM.FirstDayOfSicknessDate;
                                existingClaim.LastDayOfSicknessDate = claimVM.LastDayOfSicknessDate;
                                existingClaim.NumberOfSickDays = 1 + (claimVM.LastDayOfSicknessDate.Date - claimVM.FirstDayOfSicknessDate.Date).Days;
                                existingClaim.OrganisationNumber = claimVM.OrganisationNumber;
                                db.Entry(existingClaim).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            //New claim
                            //Save the new claim if the assistant's profile is complete.
                            Claim claim = new Claim();
                            claim.OrganisationNumber = claimVM.OrganisationNumber;
                            //Hardcoded SSNs for demo
                            claim.CustomerSSN = claimVM.CustomerSSN;
                            claim.AssistantSSN = "930701-4168";
                            claim.Email = claimVM.Email;
                            //claim.CustomerSSN = claimVM.CustomerSSN;
                            //claim.AssistantSSN = claimVM.AssistantSSN;
                            //claim.StandInSSN = claimVM.StandInSSN;
                            claim.StatusDate = DateTime.Now;
                            claim.QualifyingDate = claimVM.FirstDayOfSicknessDate;
                            claim.LastDayOfSicknessDate = claimVM.LastDayOfSicknessDate;
                            claim.NumberOfSickDays = 1 + (claimVM.LastDayOfSicknessDate.Date - claimVM.FirstDayOfSicknessDate.Date).Days;
                            var currentUserId = User.Identity.GetUserId();
                            claim.OwnerId = currentUserId;
                            claim.ClaimStatusId = 2;  //ClaimStatus.Name = "Utkast"
                            claim.CareCompanyId = (int)db.Users.Where(u => u.Id == currentUserId).First().CareCompanyId;

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
                            claim.ReferenceNumber = newReferenceNumber;
                            db.Claims.Add(claim);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        claimVM.Rejected = true;
                        claimVM.RejectReason = "Sista sjukdag kan inte vara före första sjukdag." + "\n" + "Vänligen uppdatera ansökan.";
                        return View(claimVM);
                    }

                    if (answer == "Till steg 2")
                    {
                        bool claimDaysExist = false;
                        List<ClaimDay> claimDays = new List<ClaimDay>();

                        if (existingReferenceNumber != null)
                        {

                            //This is an existing claim
                            claimDays = db.ClaimDays.Where(c => c.ReferenceNumber == existingReferenceNumber).OrderBy(c => c.SickDayNumber).ToList();

                            //Does this need a null check as well??
                            if (claimDays != null && claimDays.Count() > 0)
                            {
                                claimDaysExist = true;
                            }
                        }

                        if (existingReferenceNumber != null && claimDaysExist)
                        {
                            //This is an existing claim
                            //This means that ClaimDays have been saved for this claim
                            for (int i = 0; i < claimDays.Count; i++)
                            {
                                //Instantiate a new scheduleRow in the viewmodel
                                ScheduleRow scheduleRow = new ScheduleRow();

                                scheduleRow.ScheduleRowDateString = claimDays[i].DateString;
                                scheduleRow.DayDate = DateTime.Now.AddDays(i);

                                scheduleRow.StartTimeHour = claimDays[i].StartHour;
                                scheduleRow.StartTimeMinute = claimDays[i].StartMinute;
                                scheduleRow.StopTimeHour = claimDays[i].StopHour;
                                scheduleRow.StopTimeMinute = claimDays[i].StopMinute;
                                scheduleRow.NumberOfHours = claimDays[i].NumberOfHours;
                                scheduleRow.NumberOfUnsocialHours = claimDays[i].NumberOfUnsocialHours;
                                scheduleRow.NumberOfUnsocialHoursEvening = claimDays[i].NumberOfUnsocialHoursEvening;
                                scheduleRow.NumberOfUnsocialHoursNight = claimDays[i].NumberOfUnsocialHoursNight;

                                scheduleRow.StartTimeHourOnCall = claimDays[i].StartHourOnCall;
                                scheduleRow.StartTimeMinuteOnCall = claimDays[i].StartMinuteOnCall;
                                scheduleRow.StopTimeHourOnCall = claimDays[i].StartHourOnCall;
                                scheduleRow.StopTimeMinuteOnCall = claimDays[i].StopMinuteOnCall;
                                scheduleRow.NumberOfOnCallHours = claimDays[i].NumberOfOnCallHours;
                                scheduleRow.NumberOfOnCallHoursEvening = claimDays[i].NumberOfOnCallHoursEvening;
                                scheduleRow.NumberOfOnCallHoursNight = claimDays[i].NumberOfOnCallHoursNight;

                                scheduleRow.StartTimeHourSI = claimDays[i].StartHourSI;
                                scheduleRow.StartTimeMinuteSI = claimDays[i].StartMinuteSI;
                                scheduleRow.StopTimeHourSI = claimDays[i].StopHourSI;
                                scheduleRow.StopTimeMinuteSI = claimDays[i].StopMinuteSI;
                                scheduleRow.NumberOfHoursSI = claimDays[i].NumberOfHoursSI;
                                scheduleRow.NumberOfUnsocialHoursSI = claimDays[i].NumberOfUnsocialHoursSI;
                                scheduleRow.NumberOfUnsocialHoursEveningSI = claimDays[i].NumberOfUnsocialHoursEveningSI;
                                scheduleRow.NumberOfUnsocialHoursNightSI = claimDays[i].NumberOfUnsocialHoursNightSI;

                                scheduleRow.StartTimeHourOnCallSI = claimDays[i].StartHourOnCallSI;
                                scheduleRow.StartTimeMinuteOnCallSI = claimDays[i].StartMinuteOnCallSI;
                                scheduleRow.StopTimeHourOnCallSI = claimDays[i].StartHourOnCallSI;
                                scheduleRow.StopTimeMinuteOnCallSI = claimDays[i].StopMinuteOnCallSI;
                                scheduleRow.NumberOfOnCallHoursSI = claimDays[i].NumberOfOnCallHoursSI;
                                scheduleRow.NumberOfOnCallHoursEveningSI = claimDays[i].NumberOfOnCallHoursEveningSI;
                                scheduleRow.NumberOfOnCallHoursNightSI = claimDays[i].NumberOfOnCallHoursNightSI;
                                rowList.Add(scheduleRow);
                            }
                            scheduleVM.ReferenceNumber = existingReferenceNumber;

                        }

                        if (existingReferenceNumber == null || (existingReferenceNumber != null && claimDaysExist == false))
                        {
                            //This is a new claim
                            //Calculate number of schedule rows based on sickness start date and end date
                            TimeSpan sicknessSpan;
                            sicknessSpan = claimVM.LastDayOfSicknessDate - claimVM.FirstDayOfSicknessDate;
                            int numberOfDays = int.Parse((sicknessSpan.Days + 1).ToString());

                            scheduleVM.ReferenceNumber = newReferenceNumber;

                            DateTime dateInSchedule;

                            //Seed for demo only
                            var claimDaySeeds = db.ClaimDaySeeds.ToList();

                            //Populate viewmodel properties by iterating over each row in the schedule
                            for (int i = 0; i < numberOfDays; i++)
                            {
                                //Instantiate a new scheduleRow in the viewmodel
                                ScheduleRow scheduleRow = new ScheduleRow();

                                //Assign values to the ScheduleRowDate and ScheduleRowWeekDay properties in the viewmodel
                                dateInSchedule = claimVM.FirstDayOfSicknessDate.AddDays(i);

                                CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;
                                Thread.CurrentThread.CurrentCulture = new CultureInfo("sv-SV");

                                //Test
                                //var dateInScheduleTest = claimVM.FirstDayOfSicknessDate.AddDays(i);
                                //var dateInScheduleTest2 = dateInScheduleTest.ToString(format: "ddd d MMM");
                                //dateInScheduleTest2.ToTitleCase(TitleCase.All);
                                scheduleRow.ScheduleRowDateString = dateInSchedule.ToString(format: "ddd d MMM");
                                scheduleRow.DayDate = dateInSchedule;

                                //scheduleRow.ScheduleRowDate = dateInSchedule.ToShortDateString();
                                scheduleRow.ScheduleRowWeekDay = DateTimeFormatInfo.CurrentInfo.GetDayName(dateInSchedule.DayOfWeek).ToString().Substring(0, 2);
                                //scheduleRow.ScheduleRowWeekDay = dateInSchedule.DayOfWeek.ToString();

                                //For seeding demo only
                                scheduleRow.StartTimeHour = claimDaySeeds[i].StartHour;
                                scheduleRow.StartTimeMinute = claimDaySeeds[i].StartMinute;
                                scheduleRow.StopTimeHour = claimDaySeeds[i].StopHour;
                                scheduleRow.StopTimeMinute = claimDaySeeds[i].StopMinute;

                                scheduleRow.StartTimeHourOnCall = claimDaySeeds[i].StartHourOnCall;
                                scheduleRow.StartTimeMinuteOnCall = claimDaySeeds[i].StartMinuteOnCall;
                                scheduleRow.StopTimeHourOnCall = claimDaySeeds[i].StopHourOnCall;
                                scheduleRow.StopTimeMinuteOnCall = claimDaySeeds[i].StopMinuteOnCall;  //End demo seed

                                rowList.Add(scheduleRow);
                            }
                        }
                        scheduleVM.ScheduleRowList = rowList;
                        return View("Hours", scheduleVM);
                    }
                    else
                    {
                        return View(claimVM);
                    }
                }
                return View(claimVM);
            }
        }

        //POST: Claims/Hours
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddHours(ScheduleVM scheduleVM, string claimNumber)
        {
            //int numberOfHours = 0;
            //int numberOfMinutes = 0;
            //int numberOfMorningHours = 0;
            //int numberOfMorningMinutes = 0;

            //Calculate number of ordinary working hours for each date
            for (int i = 0; i < scheduleVM.ScheduleRowList.Count(); i++)
            {
                if (scheduleVM.ScheduleRowList[i].StartTimeMinute == null)
                {
                    scheduleVM.ScheduleRowList[i].StartTimeMinute = "00";
                }
                if (scheduleVM.ScheduleRowList[i].StopTimeMinute == null)
                {
                    scheduleVM.ScheduleRowList[i].StopTimeMinute = "00";
                }

                if (scheduleVM.ScheduleRowList[i].StartTimeMinuteOnCall == null)
                {
                    scheduleVM.ScheduleRowList[i].StartTimeMinuteOnCall = "00";
                }
                if (scheduleVM.ScheduleRowList[i].StopTimeMinuteOnCall == null)
                {
                    scheduleVM.ScheduleRowList[i].StopTimeMinuteOnCall = "00";
                }

                //Calculate number of working hours for the day
                scheduleVM.ScheduleRowList[i] = CalculateWorkingHours(scheduleVM.ScheduleRowList[i]);

                //Calculate number of unsocial hours for the day. Distinguish between the total number of unsocial hours, unsocial hours in the morning (00.00 - 07.00) and
                //unsocial hours in the evening (18.00 - 24.00). 
                scheduleVM.ScheduleRowList[i] = CalculateUnsocialHours(scheduleVM.ScheduleRowList[i]);

                //Calculate number of oncall hours for the day. Distinguish between the total number of oncall hours, oncall hours in the morning (00.00 - 07.00) and
                //oncall hours in the evening (18.00 - 24.00). 
                scheduleVM.ScheduleRowList[i] = CalculateOnCallHours(scheduleVM.ScheduleRowList[i]);


            }

            //If there are existing ClaimDay records for this claim: remove them. The new records will be added to the db instead.
            db.ClaimDays.RemoveRange(db.ClaimDays.Where(c => c.ReferenceNumber == scheduleVM.ReferenceNumber));
            db.SaveChanges();

            var claim = db.Claims.Where(c => c.ReferenceNumber == scheduleVM.ReferenceNumber).FirstOrDefault();
            DateTime claimDate = claim.QualifyingDate;

            int dayIdx = 1;
            foreach (var day in scheduleVM.ScheduleRowList)
            {
                var claimDay = new ClaimDay
                {
                    ReferenceNumber = scheduleVM.ReferenceNumber,
                    DateString = day.ScheduleRowDateString,
                    ClaimDayDate = claimDate.AddDays(dayIdx - 1),
                    SickDayNumber = dayIdx,

                    StartHour = day.StartTimeHour,
                    StartMinute = day.StartTimeMinute,
                    StopHour = day.StopTimeHour,
                    StopMinute = day.StopTimeMinute,

                    NumberOfHours = day.NumberOfHours,
                    NumberOfUnsocialHours = day.NumberOfUnsocialHours,
                    NumberOfUnsocialHoursEvening = day.NumberOfUnsocialHoursEvening,
                    NumberOfUnsocialHoursNight = day.NumberOfUnsocialHoursNight,

                    StartHourOnCall = day.StartTimeHourOnCall,
                    StartMinuteOnCall = day.StartTimeMinuteOnCall,
                    StopHourOnCall = day.StopTimeHourOnCall,
                    StopMinuteOnCall = day.StartTimeMinuteOnCall,

                    NumberOfOnCallHours = day.NumberOfOnCallHours,
                    NumberOfOnCallHoursEvening = day.NumberOfOnCallHoursEvening,
                    NumberOfOnCallHoursNight = day.NumberOfOnCallHoursNight,

                    StartHourSI = day.StartTimeHourSI,
                    StartMinuteSI = day.StartTimeMinuteSI,
                    StopHourSI = day.StopTimeHourSI,
                    StopMinuteSI = day.StopTimeMinuteSI,

                    NumberOfHoursSI = day.NumberOfHoursSI,
                    NumberOfUnsocialHoursSI = day.NumberOfUnsocialHoursSI,
                    NumberOfUnsocialHoursEveningSI = day.NumberOfUnsocialHoursEveningSI,
                    NumberOfUnsocialHoursNightSI = day.NumberOfUnsocialHoursNightSI,

                    StartHourOnCallSI = day.StartTimeHourOnCall,
                    StartMinuteOnCallSI = day.StartTimeMinuteOnCall,
                    StopHourOnCallSI = day.StopTimeHourOnCall,
                    StopMinuteOnCallSI = day.StartTimeMinuteOnCall,

                    NumberOfOnCallHoursSI = day.NumberOfOnCallHoursSI,
                    NumberOfOnCallHoursEveningSI = day.NumberOfOnCallHoursEveningSI,
                    NumberOfOnCallHoursNightSI = day.NumberOfOnCallHoursNightSI,

                    //These lines are applicable if only number of hours are provided as input
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

            //if (scheduleVM.ScheduleRowList.Count > 0)
            //{
            db.SaveChanges();
            //}

            //TEST FOR INSERTING ANOTHER PAGE FOR ENTERING CLAIM AMOUNT BEFORE THE REVIEW PAGE
            ClaimAmountVM claimAmountVM = new ClaimAmountVM();

            claimAmountVM.ClaimNumber = scheduleVM.ReferenceNumber;

            //Seed for demo only
            var numberOfSickDays = scheduleVM.ScheduleRowList.Count();
            claimAmountVM.SickPay = (decimal)867.23 * (numberOfSickDays - 1);
            claimAmountVM.PensionAndInsurance = (decimal)0.06 * (decimal)867.23 * numberOfSickDays;
            claimAmountVM.SocialFees = (decimal)0.3142 * (decimal)867.23 * numberOfSickDays;
            claimAmountVM.HolidayPay = (decimal)0.12 * (claimAmountVM.SickPay + claimAmountVM.PensionAndInsurance + claimAmountVM.SocialFees);

            claimAmountVM.ClaimSum = claimAmountVM.HolidayPay + claimAmountVM.SickPay + claimAmountVM.PensionAndInsurance + claimAmountVM.SocialFees;


            return View("ClaimAmount", claimAmountVM);


            //REVIEW PAGE THIS CODE IS NOT EXECUTED AS LONG AS THE TEST ABOVE IS DONE

            ClaimFormVM claimFormVM = new ClaimFormVM();

            claimFormVM.Workplace = "Smart Assistans";
            claimFormVM.CollectiveAgreement = "KFO-LO";
            claimFormVM.Salary = 120.00;  //This property is used either as an hourly salary or as a monthly salary in ClaimFormVM.cs.
            claimFormVM.HourlySalary = 120.00;    //This property is used as the hourly salary in calculations.
            claimFormVM.HolidayPayRate = 12.00;
            claimFormVM.SocialFeeRate = 31.42;
            claimFormVM.PensionAndInsuranceRate = 6.00;
            claimFormVM.SickPayRate = 80.00;
            //claimFormVM.QualifyingDayDate = scheduleVM.ScheduleRowList.First().ScheduleRowDate;
            claimFormVM.QualifyingDayDate = "2018-01-01";

            //Kommun
            claimFormVM.Council = "Helsingborgs kommun";
            claimFormVM.Administration = "Vård- och omsorgsförvaltningen";

            //Assistansberättigad
            claimFormVM.CustomerName = "Kund Kundsson";
            claimFormVM.CustomerSSN = "4808025077";
            claimFormVM.CustomerAddress = "Tolvangatan 12, 123 45 Tolvsta";
            claimFormVM.CustomerPhoneNumber = "019-124 6578";

            //Ombud/uppgiftslämnare
            claimFormVM.OmbudName = "Ombud Ombudsson";
            claimFormVM.OmbudPhoneNumber = "010-986 3124";

            //Assistansanordnare
            claimFormVM.CompanyName = "Tolvan Omsorg AB";
            claimFormVM.OrganisationNumber = "";
            claimFormVM.GiroNumber = "4321-9876";
            claimFormVM.CompanyAddress = "Omsorgsgatan 117, 987 00 Omsorgköping";
            claimFormVM.CompanyPhoneNumber = "010-986 0000";
            claimFormVM.CollectiveAgreement = "Vårdföretagarna-Kommunal, Personlig assistans (Branch G)";

            //Insjuknad ordinarie assistent
            //Källa till belopp: https://assistanskoll.se/Guider-Att-arbeta-som-personlig-assistent.html (Vårdföretagarna)
            claimFormVM.PerHourUnsocialEvening = 21.08;
            claimFormVM.PerHourUnsocialNight = 42.54;
            claimFormVM.PerHourUnsocialWeekend = 52.47;
            claimFormVM.PerHourUnsocialHoliday = 105.03;
            claimFormVM.PerHourOnCallWeekday = 28.13;
            claimFormVM.PerHourOnCallWeekend = 56.32;

            //Calculate salary for qualifying day
            double salaryQualifyingDay = 0;
            if (scheduleVM.ScheduleRowList[0].NumberOfHours > 8.00)
            {
                //This calculation needs to be enhanced to take unsocial hours and oncall hours into account. For now only ordinary working hours are considered in the calculation.
                salaryQualifyingDay = claimFormVM.SickPayRate * claimFormVM.HourlySalary * (scheduleVM.ScheduleRowList[0].NumberOfHours - 8) / 100;
            }
            claimFormVM.SalaryQualifyingDay = salaryQualifyingDay;

            //Calculate salary for day 2 to day 14
            double salaryDay2To14 = 0;
            for (int i = 1; i < scheduleVM.ScheduleRowList.Count(); i++)
            {
                salaryDay2To14 = salaryDay2To14 + (claimFormVM.HourlySalary * scheduleVM.ScheduleRowList[i].NumberOfHours);
            }
            salaryDay2To14 = claimFormVM.SickPayRate * salaryDay2To14 / 100;
            claimFormVM.SalaryDay2To14 = salaryDay2To14;

            //The Sickpay property contains the salary for all days during the sickleave period.
            claimFormVM.Sickpay = salaryQualifyingDay + salaryDay2To14;

            claimFormVM.UnsocialHoursPayQualifyingDay = 0;
            //It is a question mark whether unsocial hours beyond the first 8 hours of the qualifying day should be paid. If they should be paid then the code below is needed.
            //Calculate pay for unsocial hours for qualifying day
            //Calculate the point in time which is 8 hours after the assistant was supposed to start working, i.e. 8 hours after startTimeHour
            //Then check if there are any unsocial hours after startTimeHour + 8 hours. Those should be payed for.
            double payUnsocialQualifyingDay = 0;
            int hoursBeyond18 = 0;
            int minutesBeyond18 = 0;

            if (scheduleVM.ScheduleRowList[0].NumberOfHours > 8)
            {
                if (Int32.Parse(scheduleVM.ScheduleRowList[0].StartTimeHour) + 8 < 24)
                {
                    if (Int32.Parse(scheduleVM.ScheduleRowList[0].StartTimeHour) + 8 < 18 || (Int32.Parse(scheduleVM.ScheduleRowList[0].StartTimeHour) + 8 == 18 && Int32.Parse(scheduleVM.ScheduleRowList[0].StartTimeMinute) == 0))
                    {
                        //In this case startTimeHour + 8 goes maximum up to 18.00
                        //All unsocial hours in the evening should be payed in this case
                        if (scheduleVM.ScheduleRowList[0].ScheduleRowDateString.Substring(0, 3) == "lör" || scheduleVM.ScheduleRowList[0].ScheduleRowDateString.Substring(0, 3) == "sön")
                        {
                            payUnsocialQualifyingDay = claimFormVM.PerHourUnsocialWeekend * scheduleVM.ScheduleRowList[0].NumberOfUnsocialHoursEvening;
                        }
                        else
                        {
                            payUnsocialQualifyingDay = claimFormVM.PerHourUnsocialEvening * scheduleVM.ScheduleRowList[0].NumberOfUnsocialHoursEvening;
                        }
                    }
                    //Calculate how much time of startTimeHour + 8 goes beyond 18.00. This amount of time must be subtracted from the evening unsocial hours. The difference should be payed. 
                    else if (Int32.Parse(scheduleVM.ScheduleRowList[0].StartTimeHour) + 8 > 18)
                    {
                        hoursBeyond18 = Int32.Parse(scheduleVM.ScheduleRowList[0].StartTimeHour) + 8 - 18;
                        minutesBeyond18 = Int32.Parse(scheduleVM.ScheduleRowList[0].StartTimeMinute);
                        payUnsocialQualifyingDay = claimFormVM.PerHourUnsocialEvening * (scheduleVM.ScheduleRowList[0].NumberOfUnsocialHoursEvening - (((float)hoursBeyond18 * 60) + minutesBeyond18) / 60);
                    }
                    else if (Int32.Parse(scheduleVM.ScheduleRowList[0].StartTimeHour) + 8 == 18)
                    {
                        minutesBeyond18 = Int32.Parse(scheduleVM.ScheduleRowList[0].StartTimeMinute);
                        payUnsocialQualifyingDay = claimFormVM.PerHourUnsocialEvening * (scheduleVM.ScheduleRowList[0].NumberOfUnsocialHoursEvening - (((float)hoursBeyond18 * 60) + minutesBeyond18) / 60);
                    }
                }
            }
            claimFormVM.UnsocialHoursPayQualifyingDay = claimFormVM.SickPayRate * payUnsocialQualifyingDay / 100;

            //Calculate pay for unsocial hours for day 2 to day 14
            double payUnsocialHoursDay2To14 = 0;
            for (int i = 1; i < scheduleVM.ScheduleRowList.Count(); i++)
            {
                if (scheduleVM.ScheduleRowList[i].ScheduleRowDateString.Substring(0, 3) == "lör")
                {
                    payUnsocialHoursDay2To14 = payUnsocialHoursDay2To14 + (claimFormVM.PerHourUnsocialNight * scheduleVM.ScheduleRowList[i].NumberOfUnsocialHoursNight) +
                        (claimFormVM.PerHourUnsocialWeekend * (scheduleVM.ScheduleRowList[i].NumberOfUnsocialHours - scheduleVM.ScheduleRowList[i].NumberOfUnsocialHoursNight));
                }
                else if (scheduleVM.ScheduleRowList[i].ScheduleRowDateString.Substring(0, 3) == "sön")
                {
                    payUnsocialHoursDay2To14 = payUnsocialHoursDay2To14 + (claimFormVM.PerHourUnsocialWeekend * scheduleVM.ScheduleRowList[i].NumberOfUnsocialHours);
                }
                else
                {
                    payUnsocialHoursDay2To14 = payUnsocialHoursDay2To14 + (claimFormVM.PerHourUnsocialNight * scheduleVM.ScheduleRowList[i].NumberOfUnsocialHoursNight) +
                        (claimFormVM.PerHourUnsocialEvening * scheduleVM.ScheduleRowList[i].NumberOfUnsocialHoursEvening);
                }
            }

            claimFormVM.UnsocialHoursPayDay2To14 = claimFormVM.SickPayRate * payUnsocialHoursDay2To14 / 100;
            claimFormVM.UnsocialHoursPay = claimFormVM.UnsocialHoursPayQualifyingDay + claimFormVM.UnsocialHoursPayDay2To14;

            //Calculate pay for oncall hours
            //Calculate pay for oncall hours for qualifying day.
            //Enligt KFO görs avdrag med max 8 timmar för karensdag.
            double payOnCallHoursQualifyingDay = 0;
            int gapHours = 0;
            int gapMinutes = 0;
            float gapHoursOnCall = 0;

            if (scheduleVM.ScheduleRowList[0].NumberOfHours + scheduleVM.ScheduleRowList[0].NumberOfOnCallHours > 8)
            {
                //If on call hours come before ordinary working hours
                if (Int32.Parse(scheduleVM.ScheduleRowList[0].StartTimeHourOnCall) < Int32.Parse(scheduleVM.ScheduleRowList[0].StartTimeHour))
                {
                    //Calculate the gap between on call hours and ordinary working hours
                    gapHours = Int32.Parse(scheduleVM.ScheduleRowList[0].StartTimeHour) - Int32.Parse(scheduleVM.ScheduleRowList[0].StartTimeHourOnCall);
                    gapMinutes = 60 - Int32.Parse(scheduleVM.ScheduleRowList[0].StopTimeMinuteOnCall) + Int32.Parse(scheduleVM.ScheduleRowList[0].StartTimeMinute);
                    gapHoursOnCall = (((float)gapHours * 60) + gapMinutes) / 60;
                }
                else
                {
                    //On call hours come after ordinary working hours. Calculate the gap between ordinary working hours and on call hours.

                }
            }

            //if (Int32.Parse(scheduleVM.ScheduleRowList[0].StartTimeHour) + 8 < 18 || (Int32.Parse(scheduleVM.ScheduleRowList[0].StartTimeHour) + 8 == 18 && Int32.Parse(scheduleVM.ScheduleRowList[0].StartTimeMinute) == 0))
            //{
            //    //In this case startTimeHour + 8 goes maximum up to 18.00
            //    //All unsocial hours in the evening should be payed in this case
            //    if (scheduleVM.ScheduleRowList[0].ScheduleRowDateString.Substring(0, 3) == "lör" || scheduleVM.ScheduleRowList[0].ScheduleRowDateString.Substring(0, 3) == "sön")
            //    {
            //        payUnsocialQualifyingDay = claimFormVM.PerHourUnsocialWeekend * scheduleVM.ScheduleRowList[0].NumberOfUnsocialHoursEvening;
            //    }
            //    else
            //    {
            //        payUnsocialQualifyingDay = claimFormVM.PerHourUnsocialEvening * scheduleVM.ScheduleRowList[0].NumberOfUnsocialHoursEvening;
            //    }
            //}

            //Calculate pay for oncall hours for day 2 to day 14
            double payOnCallHoursDay2To14 = 0;
            for (int i = 1; i < scheduleVM.ScheduleRowList.Count(); i++)
            {
                if (scheduleVM.ScheduleRowList[i].ScheduleRowDateString.Substring(0, 3) == "lör" || scheduleVM.ScheduleRowList[i].ScheduleRowDateString.Substring(0, 3) == "sön")
                {
                    payOnCallHoursDay2To14 = payOnCallHoursDay2To14 + (claimFormVM.PerHourOnCallWeekend * scheduleVM.ScheduleRowList[i].NumberOfOnCallHours);
                }
                else
                {
                    payOnCallHoursDay2To14 = payOnCallHoursDay2To14 + (claimFormVM.PerHourOnCallWeekend * scheduleVM.ScheduleRowList[i].NumberOfOnCallHoursNight) +
                        (claimFormVM.PerHourOnCallWeekday * (scheduleVM.ScheduleRowList[i].NumberOfOnCallHours - scheduleVM.ScheduleRowList[i].NumberOfOnCallHoursNight));
                }
            }

            claimFormVM.OnCallHoursPayDay2To14 = claimFormVM.SickPayRate * payUnsocialHoursDay2To14 / 100;
            claimFormVM.OnCallHoursPay = claimFormVM.OnCallHoursPayQualifyingDay + claimFormVM.OnCallHoursPayDay2To14;

            //Calculate holiday pay


            //This sum needs to be enhanced with unsocial and oncall hours, holiday pay, social fees, pensions
            claimFormVM.ClaimSum = claimFormVM.SalaryQualifyingDay + claimFormVM.SalaryDay2To14;

            return View("ClaimForm", claimFormVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult SaveAmounts(double sickPay, double parHolidayPay, double parSocialFees, double parPensionAndInsurance, double parClaimSum, string parClaimReference)
        public ActionResult SaveAmounts(ClaimAmountVM claimAmountVM)
        {
            //double sickPay = Convert.ToDouble(parSickPay);
            //double holidayPay = System.Web.Helpers.Json.Decode<double>(parholidayPay);
            //double socialFees = System.Web.Helpers.Json.Decode<double>(parsocialFees);
            //double pensionAndInsurance = System.Web.Helpers.Json.Decode<double>(parpensionAndInsurance);
            //double claimSum = System.Web.Helpers.Json.Decode<double>(parclaimSum);

            var existingClaim = db.Claims.Where(c => c.ReferenceNumber == claimAmountVM.ClaimNumber).FirstOrDefault();
            if (existingClaim != null)
            {
                existingClaim.SickPay = claimAmountVM.SickPay;
                existingClaim.HolidayPay = claimAmountVM.HolidayPay;
                existingClaim.SocialFees = claimAmountVM.SocialFees;
                existingClaim.PensionAndInsurance = claimAmountVM.PensionAndInsurance;
                existingClaim.ClaimSum = claimAmountVM.SickPay + claimAmountVM.HolidayPay + claimAmountVM.SocialFees + claimAmountVM.PensionAndInsurance;
                claimAmountVM.ClaimSum = existingClaim.ClaimSum;

                //Calculate the model sum
                //Find ClaimDay records for the claim
                var claimDays = db.ClaimDays.Where(c => c.ReferenceNumber == claimAmountVM.ClaimNumber).OrderBy(c => c.ReferenceNumber).ToList();

                //ADD CALCULATIONS FOR QUALIFYING DAY LATER, FOR NOW ONLY DAY 2 TO DAY 14 ARE TAKEN INTO ACCOUNT

                //Calculate lost pay for unsocial hours for day 2 to 14, for now with hardcoded amounts according to Vårdföretagarna, for now excluding "storhelger"
                existingClaim.UnsocialHoursPayDay2To14 = CalculateUnsocialHoursPayDay2To14(existingClaim, claimDays);

                //Calculate lost pay for oncall hours for day 2 to 14
                existingClaim.OnCallHoursPayDay2To14 = CalculateOnCallHoursPayDay2To14(existingClaim, claimDays);

                //Calculate 80% of lost salary, including lost pay for unsocial hours and on call hours, for day 2 to day 14 and
                existingClaim.SickPayDay2To14 = 80 * (CalculateSalaryDay2To14(existingClaim, claimDays) + existingClaim.UnsocialHoursPayDay2To14 + existingClaim.OnCallHoursPayDay2To14) / 100;

                //Calculate holiday pay for day 2 to day 14
                existingClaim.HolidayPayDay2To14 = CalculateHolidayPayDay2To14(existingClaim, claimDays);

                //Calculate social fees for day 2 to day 14, hardcoded to 31.42% for now
                existingClaim.PayrollTaxDay2To14 = 31.42m * (existingClaim.SickPayDay2To14 + existingClaim.HolidayPayDay2To14) / 100;

                //Calculate pensions and insurances for day 2 to day 14, hardcoded to 6.00% for now
                existingClaim.PensionAndInsurance = 6.00m * (existingClaim.SickPayDay2To14 + existingClaim.HolidayPayDay2To14) / 100;

                //SO FAR ONLY DAY 2 TO 14 HAVE BEEN TAKEN INTO ACCOUNT. THE QUALYFYING WILL NEED TO BE ADDED.
                existingClaim.ModelSum = existingClaim.SickPayDay2To14 + existingClaim.HolidayPayDay2To14 + existingClaim.PayrollTaxDay2To14 + existingClaim.PensionAndInsurance;

                existingClaim.ClaimStatusId = 5;
                existingClaim.StatusDate = DateTime.Now;
                db.Entry(existingClaim).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("ShowReceipt", claimAmountVM);
        }

        public ActionResult ShowReceipt(ClaimAmountVM claimAmountVM)
        {
            var claim = db.Claims.Where(rn => rn.ReferenceNumber == claimAmountVM.ClaimNumber).FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(claim.Email))
            {
                MailMessage message = new MailMessage();
                message.From = new MailAddress("ourrobotdemo@gmail.com");

                message.To.Add(new MailAddress(claim.Email));
                //message.To.Add(new MailAddress("e.niklashagman@gmail.com"));
                message.Subject = "Ny ansökan med referensnummer: " + claimAmountVM.ClaimNumber;
                message.Body = "Vi har mottagit din ansökan med referensnummer " + claimAmountVM.ClaimNumber + ". Normalt får du ett beslut inom 1 - 3 dagar." + "\n" + "\n" +
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

            return View("Receipt", claimAmountVM);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult AddHours(ScheduleVM scheduleVM, int stage)
        //{
        //    return View();
        //}

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
                recommendationVM.ModelSum = claim.ModelSum;
                recommendationVM.ClaimSum = claim.ClaimSum;
                recommendationVM.ApprovedSum = recommendationVM.ModelSum.ToString();
                recommendationVM.RejectedSum = (recommendationVM.ClaimSum - recommendationVM.ModelSum).ToString();

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
            claimDetailsVM.QualifyingDayDate = claimDays[0].ClaimDayDate.ToString();
            claimDetailsVM.LastDayOfSicknessDate = claimDays.Last().ClaimDayDate.ToString();

            claimDetailsVM.Salary = (decimal)120.00;  //This property is used either as an hourly salary or as a monthly salary in claimDetailsVM.cs.
            claimDetailsVM.HourlySalary = (decimal)120.00;    //This property is used as the hourly salary in calculations.
            claimDetailsVM.Sickpay = claim.SickPay;
            claimDetailsVM.HolidayPay = claim.HolidayPay;
            claimDetailsVM.SocialFees = claim.SocialFees;
            claimDetailsVM.PensionAndInsurance = claim.PensionAndInsurance;

            claimDetailsVM.NumberOfAbsenceHours = claim.NumberOfAbsenceHours;
            claimDetailsVM.NumberOfOrdinaryHours = claim.NumberOfOrdinaryHours;
            claimDetailsVM.NumberOfUnsocialHours = claim.NumberOfUnsocialHours;
            claimDetailsVM.NumberOfOnCallHours = claim.NumberOfOnCallHours;

            claimDetailsVM.NumberOfHoursWithSI = claim.NumberOfHoursWithSI;
            claimDetailsVM.NumberOfOrdinaryHoursSI = claim.NumberOfOrdinaryHoursSI;
            claimDetailsVM.NumberOfUnsocialHoursSI = claim.NumberOfUnsocialHoursSI;
            claimDetailsVM.NumberOfOnCallHoursSI = claim.NumberOfOnCallHoursSI;

            claimDetailsVM.ClaimSum = claim.ClaimSum;
            claimDetailsVM.ApprovedSum = claim.ApprovedSum;
            claimDetailsVM.RejectedSum = claim.RejectedSum;

            //Underlag lönekostnader
            claimDetailsVM.PerHourUnsocialEvening = (decimal)21.08;
            claimDetailsVM.PerHourUnsocialNight = (decimal)42.54;
            claimDetailsVM.PerHourUnsocialWeekend = (decimal)52.47;
            claimDetailsVM.PerHourUnsocialHoliday = (decimal)105.03;
            claimDetailsVM.PerHourOnCallWeekday = (decimal)28.13;
            claimDetailsVM.PerHourOnCallWeekend = (decimal)56.32;

            claimDetailsVM.Workplace = "Björkängen, Birgittagården";
            claimDetailsVM.CollectiveAgreement = "Vårdföretagarna";

            claimDetailsVM.HolidayPayRate = (decimal)12.00;
            claimDetailsVM.SocialFeeRate = (decimal)31.42;
            claimDetailsVM.PensionAndInsuranceRate = (decimal)6.00;
            claimDetailsVM.SickPayRate = (decimal)80.00;
            //claimDetailsVM.QualifyingDayDate = scheduleVM.ScheduleRowList.First().ScheduleRowDate;



            //Recommended amount, using hours only as input (x lines forward)
            claimDetailsVM.SickPayRateAsString = "80,00";
            claimDetailsVM.HolidayPayRateAsString = "12,00";
            claimDetailsVM.SocialFeeRateAsString = "31,42";
            claimDetailsVM.PensionAndInsuranceRateAsString = "6,00";
            claimDetailsVM.HourlySalaryAsString = "120,00";

            claimDetailsVM.PerHourUnsocialEveningAsString = "21,08";
            claimDetailsVM.PerHourUnsocialNightAsString = "42,54";
            claimDetailsVM.PerHourUnsocialWeekendAsString = "52,47";
            claimDetailsVM.PerHourUnsocialHolidayAsString = "105,03";
            claimDetailsVM.PerHourOnCallDayAsString = "28,13";
            claimDetailsVM.PerHourOnCallNightAsString = "56,32";

            //QUALIFYING DAY

            //Hours for qualifying day
            claimDetailsVM.HoursQD = claimDays[0].Hours;

            //Holiday pay for qualifying day
            claimDetailsVM.HolidayPayQD = String.Format("{0:0.00}", (Convert.ToDecimal(claimDetailsVM.HolidayPayRateAsString) * Convert.ToDecimal(claimDetailsVM.HoursQD) * Convert.ToDecimal(claimDetailsVM.HourlySalaryAsString) / 100));
            claimDetailsVM.HolidayPayCalcQD = claimDetailsVM.HolidayPayRateAsString + " % x " + claimDetailsVM.HoursQD + " timmar x " + claimDetailsVM.HourlySalaryAsString + " Kr";

            //Social fees for qualifying day
            claimDetailsVM.SocialFeesQD = String.Format("{0:0.00}", (Convert.ToDecimal(claimDetailsVM.SocialFeeRateAsString) * Convert.ToDecimal(claimDetailsVM.HolidayPayQD) / 100));
            claimDetailsVM.SocialFeesCalcQD = claimDetailsVM.SocialFeeRateAsString + " % x " + claimDetailsVM.HolidayPayQD + " Kr";

            //Pension and insurance for qualifying day
            claimDetailsVM.PensionAndInsuranceQD = String.Format("{0:0.00}", (Convert.ToDecimal(claimDetailsVM.PensionAndInsuranceRateAsString) * Convert.ToDecimal(claimDetailsVM.HolidayPayQD) / 100));
            claimDetailsVM.PensionAndInsuranceCalcQD = claimDetailsVM.PensionAndInsuranceRateAsString + " % x " + claimDetailsVM.HolidayPayQD + " Kr";

            //Sum for qualifying day (sum of the three previous items)
            claimDetailsVM.CostQD = String.Format("{0:0.00}", (Convert.ToDecimal(claimDetailsVM.HolidayPayQD) + Convert.ToDecimal(claimDetailsVM.SocialFeesQD) + Convert.ToDecimal(claimDetailsVM.PensionAndInsuranceQD)));
            claimDetailsVM.CostCalcQD = claimDetailsVM.HolidayPayQD + " Kr + " + claimDetailsVM.SocialFeesQD + " Kr + " + claimDetailsVM.PensionAndInsuranceQD + " Kr";

            //DAY 2 TO DAY 14
            claimDetailsVM.HoursD2T14 = "0,00";
            claimDetailsVM.UnsocialEveningD2T14 = "0,00";
            claimDetailsVM.UnsocialNightD2T14 = "0,00";
            claimDetailsVM.UnsocialWeekendD2T14 = "0,00";
            claimDetailsVM.UnsocialGrandWeekendD2T14 = "0,00";
            claimDetailsVM.UnsocialSumD2T14 = "0,00";
            claimDetailsVM.OnCallDayD2T14 = "0,00";
            claimDetailsVM.OnCallNightD2T14 = "0,00";
            claimDetailsVM.OnCallSumD2T14 = "0,00";

            //Sum up hours by category for day 2 to 14
            for (int i = 1; i < claimDays.Count(); i++)
            {
                claimDetailsVM.HoursD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimDetailsVM.HoursD2T14) + Convert.ToDecimal(claimDays[i].Hours)));

                claimDetailsVM.UnsocialEveningD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimDetailsVM.UnsocialEveningD2T14) + Convert.ToDecimal(claimDays[i].UnsocialEvening)));
                claimDetailsVM.UnsocialNightD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimDetailsVM.UnsocialNightD2T14) + Convert.ToDecimal(claimDays[i].UnsocialNight)));
                claimDetailsVM.UnsocialWeekendD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimDetailsVM.UnsocialWeekendD2T14) + Convert.ToDecimal(claimDays[i].UnsocialWeekend)));
                claimDetailsVM.UnsocialGrandWeekendD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimDetailsVM.UnsocialGrandWeekendD2T14) + Convert.ToDecimal(claimDays[i].UnsocialGrandWeekend)));

                claimDetailsVM.OnCallDayD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimDetailsVM.OnCallDayD2T14) + Convert.ToDecimal(claimDays[i].OnCallDay)));
                claimDetailsVM.OnCallNightD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimDetailsVM.OnCallNightD2T14) + Convert.ToDecimal(claimDays[i].OnCallNight)));
            }
            claimDetailsVM.UnsocialSumD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimDetailsVM.UnsocialEveningD2T14) + Convert.ToDecimal(claimDetailsVM.UnsocialNightD2T14) + Convert.ToDecimal(claimDetailsVM.UnsocialWeekendD2T14) + Convert.ToDecimal(claimDetailsVM.UnsocialGrandWeekendD2T14)));
            claimDetailsVM.OnCallSumD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimDetailsVM.OnCallDayD2T14) + Convert.ToDecimal(claimDetailsVM.OnCallNightD2T14)));

            //Calculate the money by category for day 2 to day 14
            //Sickpay for day 2 to day 14
            claimDetailsVM.SalaryD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimDetailsVM.SickPayRateAsString) * Convert.ToDecimal(claimDetailsVM.HoursD2T14) * Convert.ToDecimal(claimDetailsVM.HourlySalaryAsString) / 100));
            claimDetailsVM.SalaryCalcD2T14 = claimDetailsVM.SickPayRateAsString + " % x " + claimDetailsVM.HoursD2T14 + " timmar x " + claimDetailsVM.HourlySalaryAsString + " Kr";

            //Holiday pay for day 2 to day 14
            claimDetailsVM.HolidayPayD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimDetailsVM.HolidayPayRateAsString) * Convert.ToDecimal(claimDetailsVM.SalaryD2T14) / 100));
            claimDetailsVM.HolidayPayCalcD2T14 = claimDetailsVM.HolidayPayRateAsString + " % x " + claimDetailsVM.SalaryD2T14 + " Kr";

            //Unsocial evening pay for day 2 to day 14
            claimDetailsVM.UnsocialEveningPayD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimDetailsVM.SickPayRateAsString) * Convert.ToDecimal(claimDetailsVM.UnsocialEveningD2T14) * Convert.ToDecimal(claimDetailsVM.PerHourUnsocialEveningAsString) / 100));
            claimDetailsVM.UnsocialEveningPayCalcD2T14 = claimDetailsVM.SickPayRateAsString + " % x " + claimDetailsVM.UnsocialEveningD2T14 + " timmar x " + claimDetailsVM.PerHourUnsocialEveningAsString + " Kr";

            //Unsocial night pay for day 2 to day 14
            claimDetailsVM.UnsocialNightPayD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimDetailsVM.SickPayRateAsString) * Convert.ToDecimal(claimDetailsVM.UnsocialNightD2T14) * Convert.ToDecimal(claimDetailsVM.PerHourUnsocialNightAsString) / 100));
            claimDetailsVM.UnsocialNightPayCalcD2T14 = claimDetailsVM.SickPayRateAsString + " % x " + claimDetailsVM.UnsocialNightD2T14 + " timmar x " + claimDetailsVM.PerHourUnsocialNightAsString + " Kr";

            //Unsocial weekend pay for day 2 to day 14
            claimDetailsVM.UnsocialWeekendPayD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimDetailsVM.SickPayRateAsString) * Convert.ToDecimal(claimDetailsVM.UnsocialWeekendD2T14) * Convert.ToDecimal(claimDetailsVM.PerHourUnsocialWeekendAsString) / 100));
            claimDetailsVM.UnsocialWeekendPayCalcD2T14 = claimDetailsVM.SickPayRateAsString + " % x " + claimDetailsVM.UnsocialWeekendD2T14 + " timmar x " + claimDetailsVM.PerHourUnsocialWeekendAsString + " Kr";

            //Unsocial grand weekend pay for day 2 to day 14
            claimDetailsVM.UnsocialGrandWeekendPayD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimDetailsVM.SickPayRateAsString) * Convert.ToDecimal(claimDetailsVM.UnsocialGrandWeekendD2T14) * Convert.ToDecimal(claimDetailsVM.PerHourUnsocialHolidayAsString) / 100));
            claimDetailsVM.UnsocialGrandWeekendPayCalcD2T14 = claimDetailsVM.SickPayRateAsString + " % x " + claimDetailsVM.UnsocialGrandWeekendD2T14 + " timmar x " + claimDetailsVM.PerHourUnsocialHolidayAsString + " Kr";

            //Unsocial sum pay for day 2 to day 14
            claimDetailsVM.UnsocialSumPayD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimDetailsVM.UnsocialEveningPayD2T14) + Convert.ToDecimal(claimDetailsVM.UnsocialNightPayD2T14) + Convert.ToDecimal(claimDetailsVM.UnsocialWeekendPayD2T14) + Convert.ToDecimal(claimDetailsVM.UnsocialGrandWeekendPayD2T14)));
            claimDetailsVM.UnsocialSumPayCalcD2T14 = claimDetailsVM.UnsocialEveningPayD2T14 + " Kr + " + claimDetailsVM.UnsocialNightPayD2T14 + " Kr + " + claimDetailsVM.UnsocialWeekendPayD2T14 + " Kr + " + claimDetailsVM.UnsocialGrandWeekendPayD2T14;

            //On call day pay for day 2 to day 14
            claimDetailsVM.OnCallDayPayD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimDetailsVM.SickPayRateAsString) * Convert.ToDecimal(claimDetailsVM.OnCallDayD2T14) * Convert.ToDecimal(claimDetailsVM.PerHourOnCallDayAsString) / 100));
            claimDetailsVM.OnCallDayPayCalcD2T14 = claimDetailsVM.SickPayRateAsString + " % x " + claimDetailsVM.OnCallDayD2T14 + " timmar x " + claimDetailsVM.PerHourOnCallDayAsString + " Kr";

            //On call night pay for day 2 to day 14
            claimDetailsVM.OnCallNightPayD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimDetailsVM.SickPayRateAsString) * Convert.ToDecimal(claimDetailsVM.OnCallNightD2T14) * Convert.ToDecimal(claimDetailsVM.PerHourOnCallNightAsString) / 100));
            claimDetailsVM.OnCallNightPayCalcD2T14 = claimDetailsVM.SickPayRateAsString + " % x " + claimDetailsVM.OnCallNightD2T14 + " timmar x " + claimDetailsVM.PerHourOnCallNightAsString + " Kr";

            //On call sum pay for day 2 to day 14
            claimDetailsVM.OnCallSumPayD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimDetailsVM.OnCallDayPayD2T14) + Convert.ToDecimal(claimDetailsVM.OnCallNightPayD2T14)));
            claimDetailsVM.OnCallSumPayCalcD2T14 = claimDetailsVM.OnCallDayPayD2T14 + " Kr + " + claimDetailsVM.OnCallNightPayD2T14;

            //Sick pay for day 2 to day 14
            claimDetailsVM.SickPayD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimDetailsVM.SalaryD2T14) + Convert.ToDecimal(claimDetailsVM.UnsocialSumPayD2T14) + Convert.ToDecimal(claimDetailsVM.OnCallSumPayD2T14)));
            claimDetailsVM.SickPayCalcD2T14 = claimDetailsVM.SalaryD2T14 + " Kr + " + claimDetailsVM.UnsocialSumPayD2T14 + " Kr + " + claimDetailsVM.OnCallSumPayD2T14 + " Kr";

            //Social fees for day 2 to day 14
            claimDetailsVM.SocialFeesD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimDetailsVM.SocialFeeRateAsString) * (Convert.ToDecimal(claimDetailsVM.SalaryD2T14) + Convert.ToDecimal(claimDetailsVM.HolidayPayD2T14) + Convert.ToDecimal(claimDetailsVM.UnsocialSumPayD2T14) + Convert.ToDecimal(claimDetailsVM.OnCallSumPayD2T14)) / 100));
            claimDetailsVM.SocialFeesCalcD2T14 = claimDetailsVM.SocialFeeRateAsString + " % x (" + claimDetailsVM.SalaryD2T14 + " Kr + " + claimDetailsVM.HolidayPayD2T14 + " Kr + " + claimDetailsVM.UnsocialSumPayD2T14 + " Kr + " + claimDetailsVM.OnCallSumPayD2T14 + " Kr)";

            //Pensions and insurances for day 2 to day 14
            claimDetailsVM.PensionAndInsuranceD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimDetailsVM.PensionAndInsuranceRateAsString) * (Convert.ToDecimal(claimDetailsVM.SalaryD2T14) + Convert.ToDecimal(claimDetailsVM.HolidayPayD2T14) + Convert.ToDecimal(claimDetailsVM.UnsocialSumPayD2T14) + Convert.ToDecimal(claimDetailsVM.OnCallSumPayD2T14)) / 100));
            claimDetailsVM.PensionAndInsuranceCalcD2T14 = claimDetailsVM.PensionAndInsuranceRateAsString + " % x (" + claimDetailsVM.SalaryD2T14 + " Kr + " + claimDetailsVM.HolidayPayD2T14 + " Kr + " + claimDetailsVM.UnsocialSumPayD2T14 + " Kr + " + claimDetailsVM.OnCallSumPayD2T14 + " Kr)";

            //Sum for day 2 to day 14
            claimDetailsVM.CostD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimDetailsVM.SalaryD2T14) + Convert.ToDecimal(claimDetailsVM.HolidayPayD2T14) + Convert.ToDecimal(claimDetailsVM.UnsocialSumPayD2T14) + Convert.ToDecimal(claimDetailsVM.OnCallSumPayD2T14) + Convert.ToDecimal(claimDetailsVM.SocialFeesD2T14) + Convert.ToDecimal(claimDetailsVM.PensionAndInsuranceD2T14)));
            claimDetailsVM.CostCalcD2T14 = claimDetailsVM.SalaryD2T14 + " Kr + " + claimDetailsVM.HolidayPayD2T14 + " Kr + " + claimDetailsVM.UnsocialSumPayD2T14 + " Kr + " + claimDetailsVM.OnCallSumPayD2T14 + " Kr + " + claimDetailsVM.SocialFeesD2T14 + " Kr + " + claimDetailsVM.PensionAndInsuranceD2T14 + " Kr";

            //Total sum for day 1 to day 14
            claimDetailsVM.TotalCostD1T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimDetailsVM.CostQD) + Convert.ToDecimal(claimDetailsVM.CostD2T14)));
            claimDetailsVM.TotalCostCalcD1T14 = claimDetailsVM.CostQD + " Kr + " + claimDetailsVM.CostD2T14;

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
            //if (ModelState.IsValid)
            //{
            //Decided claim
            var claim = db.Claims.Where(c => c.ReferenceNumber == recommendationVM.ClaimNumber).FirstOrDefault();
            //claim.ClaimStatusId = 1;
            claim.ApprovedSum = Convert.ToDecimal(recommendationVM.ApprovedSum);
            claim.RejectedSum = Convert.ToDecimal(recommendationVM.RejectedSum);
            claim.StatusDate = DateTime.Now;
            db.Entry(claim).State = EntityState.Modified;
            db.SaveChanges();

            //if (!string.IsNullOrWhiteSpace(claim.Email))
            //{
            //    MailMessage message = new MailMessage();
            //    message.From = new MailAddress("ourrobotdemo@gmail.com");

            //    message.To.Add(new MailAddress(claim.Email));
            //    //message.To.Add(new MailAddress("e.niklashagman@gmail.com"));
            //    message.Subject = "Beslut om ansökan med referensnummer: " + claim.ReferenceNumber;
            //    message.Body = "Beslut om ansökan med referensnummer " + claim.ReferenceNumber + " har fattats." + "\n" + "\n" +
            //                    //"Yrkat belopp: " + claim.ClaimSum + "\n" +
            //                    //"Godkänt belopp: " + claim.ApprovedSum + "\n" +
            //                    //"Avslaget belopp: " + claim.RejectedSum + "\n" + "\n" +
            //                    "Med vänliga hälsningar, Vård- och omsorgsförvaltningen";

            //    SendEmail(message);
            //}

            //string appdataPath = Environment.ExpandEnvironmentVariables("%appdata%\\Bitoreq AB\\KoPerNikus");

            //Directory.CreateDirectory(appdataPath);
            //using (var writer = XmlWriter.Create(appdataPath + "\\decided.xml"))
            //{
            //    writer.WriteStartDocument();
            //    writer.WriteStartElement("claiminformation");
            //    writer.WriteElementString("SSN", claim.CustomerSSN);
            //    writer.WriteElementString("OrgNumber", claim.OrganisationNumber);
            //    writer.WriteElementString("ReferenceNumber", claim.ReferenceNumber);
            //    writer.WriteEndElement();
            //    writer.WriteEndDocument();
            //}


            //Approved claim
            //var claim = db.Claims.Where(c => c.ReferenceNumber == recommendationVM.ClaimNumber).FirstOrDefault();
            //claim.ClaimStatusId = 1;
            //claim.DecidedSum = recommendationVM.DecidedSum;
            //claim.StatusDate = DateTime.Now;
            //db.Entry(claim).State = EntityState.Modified;
            //db.SaveChanges();

            //if (!string.IsNullOrWhiteSpace(claim.Email))
            //{
            //    MailMessage message = new MailMessage();
            //    message.From = new MailAddress("ourrobotdemo@gmail.com");

            //    message.To.Add(new MailAddress(claim.Email));
            //    //message.To.Add(new MailAddress("e.niklashagman@gmail.com"));
            //    message.Subject = "Godkänd ansökan: " + claim.ReferenceNumber;
            //    message.Body = "Hej, ansökan med referensnummer " + claim.ReferenceNumber + " har blivit godkänd. Ha en bra dag.";

            //    SendEmail(message);
            //}

            //string appdataPath = Environment.ExpandEnvironmentVariables("%appdata%\\Bitoreq AB\\KoPerNikus");

            //Directory.CreateDirectory(appdataPath);
            //using (var writer = XmlWriter.Create(appdataPath + "\\decided.xml"))
            //{
            //    writer.WriteStartDocument();
            //    writer.WriteStartElement("claiminformation");
            //    writer.WriteElementString("SSN", claim.CustomerSSN);
            //    writer.WriteElementString("OrgNumber", claim.OrganisationNumber);
            //    writer.WriteElementString("ReferenceNumber", claim.ReferenceNumber);
            //    writer.WriteEndElement();
            //    writer.WriteEndDocument();
            //}
            //}
            return RedirectToAction("ShowRecommendationReceipt", recommendationVM);
        }

        // GET: Claims/ShowRecommendationReceipt
        public ActionResult ShowRecommendationReceipt(RecommendationVM recommendationVM)
        {

            var claim = db.Claims.Where(rn => rn.ReferenceNumber == recommendationVM.ClaimNumber).FirstOrDefault();

            //if (!string.IsNullOrWhiteSpace(claim.Email))
            //{
            //    MailMessage message = new MailMessage();
            //    message.From = new MailAddress("ourrobotdemo@gmail.com");

            //    message.To.Add(new MailAddress(claim.Email));
            //    //message.To.Add(new MailAddress("e.niklashagman@gmail.com"));
            //    message.Subject = "Ny ansökan med referensnummer: " + claimAmountVM.ClaimNumber;
            //    message.Body = "Vi har mottagit din ansökan med referensnummer " + claimAmountVM.ClaimNumber + ". Normalt får du ett beslut inom 1 - 3 dagar." + "\n" + "\n" +
            //                                        "Med vänliga hälsningar, Vård- och omsorgsförvaltningen";

            //    SendEmail(message);
            //}

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
            decisionVM.ClaimSum = claim.ClaimSum;
            decisionVM.ApprovedSum = claim.ApprovedSum;
            decisionVM.RejectedSum = claim.RejectedSum;

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

            claim.ApprovedSum = decisionVM.ApprovedSum;
            claim.RejectedSum = decisionVM.RejectedSum;
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
                                //"Yrkat belopp: " + claim.ClaimSum + "\n" +
                                //"Godkänt belopp: " + claim.ApprovedSum + "\n" +
                                //"Avslaget belopp: " + claim.RejectedSum + "\n" + "\n" +
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


            //Approved claim
            //var claim = db.Claims.Where(c => c.ReferenceNumber == decisionVM.ClaimNumber).FirstOrDefault();
            //claim.ClaimStatusId = 1;
            //claim.DecidedSum = decisionVM.DecidedSum;
            //claim.StatusDate = DateTime.Now;
            //db.Entry(claim).State = EntityState.Modified;
            //db.SaveChanges();

            //if (!string.IsNullOrWhiteSpace(claim.Email))
            //{
            //    MailMessage message = new MailMessage();
            //    message.From = new MailAddress("ourrobotdemo@gmail.com");

            //    message.To.Add(new MailAddress(claim.Email));
            //    //message.To.Add(new MailAddress("e.niklashagman@gmail.com"));
            //    message.Subject = "Godkänd ansökan: " + claim.ReferenceNumber;
            //    message.Body = "Hej, ansökan med referensnummer " + claim.ReferenceNumber + " har blivit godkänd. Ha en bra dag.";

            //    SendEmail(message);
            //}

            //string appdataPath = Environment.ExpandEnvironmentVariables("%appdata%\\Bitoreq AB\\KoPerNikus");

            //Directory.CreateDirectory(appdataPath);
            //using (var writer = XmlWriter.Create(appdataPath + "\\decided.xml"))
            //{
            //    writer.WriteStartDocument();
            //    writer.WriteStartElement("claiminformation");
            //    writer.WriteElementString("SSN", claim.CustomerSSN);
            //    writer.WriteElementString("OrgNumber", claim.OrganisationNumber);
            //    writer.WriteElementString("ReferenceNumber", claim.ReferenceNumber);
            //    writer.WriteEndElement();
            //    writer.WriteEndDocument();
            //}
            //}
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

        //CalculateWorkingHours calculates the number of working hours for one day. Unsocial and oncall hours are not included.
        //The method returns the input row with the calculated number of working hours added to it.
        private ScheduleRow CalculateWorkingHours(ScheduleRow scheduleRow)
        {
            int numberOfHours = 0;
            int numberOfMinutes = 0;

            if (scheduleRow.StartTimeHour != null && scheduleRow.StopTimeHour != null && (Int32.Parse(scheduleRow.StopTimeHour) >= Int32.Parse(scheduleRow.StartTimeHour)))
            {
                numberOfMinutes = Int32.Parse(scheduleRow.StopTimeMinute) - Int32.Parse(scheduleRow.StartTimeMinute);
                if (numberOfMinutes >= 0)
                {
                    numberOfHours = Int32.Parse(scheduleRow.StopTimeHour) - Int32.Parse(scheduleRow.StartTimeHour);
                }
                else
                {
                    numberOfMinutes = numberOfMinutes + 60;
                    numberOfHours = Int32.Parse(scheduleRow.StopTimeHour) - Int32.Parse(scheduleRow.StartTimeHour) - 1;
                }
                scheduleRow.NumberOfHours = (((float)numberOfHours * 60) + numberOfMinutes) / 60;
            }
            return scheduleRow;
        }

        //CalculateUnsocialHours calculates the number of unsocial hours for the day. The method distinguishes between the total number of unsocial hours, unsocial hours in the morning (00.00 - 07.00) and
        //unsocial hours in the evening (18.00 - 24.00).
        //The method returns the input row with the calculated number of unsocial hours added to it.
        private ScheduleRow CalculateUnsocialHours(ScheduleRow scheduleRow)
        {
            int numberOfHours = 0;
            int numberOfMinutes = 0;
            int numberOfMorningHours = 0;
            int numberOfMorningMinutes = 0;

            //Check if there is working time before 07:00
            if (scheduleRow.StartTimeHour != null && scheduleRow.StopTimeHour != null && Int32.Parse(scheduleRow.StartTimeHour) < 7)
            {
                //Calculate number of unsocial hours in the morning
                if (Int32.Parse(scheduleRow.StopTimeHour) < 7 || (Int32.Parse(scheduleRow.StopTimeHour) == 7 && Int32.Parse(scheduleRow.StopTimeMinute) == 0))
                {
                    numberOfMinutes = Int32.Parse(scheduleRow.StopTimeMinute) - Int32.Parse(scheduleRow.StartTimeMinute);
                    if (numberOfMinutes >= 0)
                    {
                        numberOfHours = Int32.Parse(scheduleRow.StopTimeHour) - Int32.Parse(scheduleRow.StartTimeHour);
                    }
                    else
                    {
                        numberOfMinutes = numberOfMinutes + 60;
                        numberOfHours = Int32.Parse(scheduleRow.StopTimeHour) - Int32.Parse(scheduleRow.StartTimeHour) - 1;
                    }
                    scheduleRow.NumberOfUnsocialHoursNight = ((((float)numberOfHours * 60) + numberOfMinutes) / 60);
                }
                else
                {
                    numberOfMinutes = 60 - Int32.Parse(scheduleRow.StartTimeMinute);
                    numberOfHours = 7 - Int32.Parse(scheduleRow.StartTimeHour) - 1;
                    scheduleRow.NumberOfUnsocialHoursNight = ((((float)numberOfHours * 60) + numberOfMinutes) / 60);
                }
                numberOfMorningHours = numberOfHours;
                numberOfMorningMinutes = numberOfMinutes;
            }

            //Check if there is working time after 18:00
            if (scheduleRow.StartTimeHour != null && scheduleRow.StopTimeHour != null && Int32.Parse(scheduleRow.StopTimeHour) >= 18)
            {
                //Calculate number of unsocial hours in the evening
                if (Int32.Parse(scheduleRow.StartTimeHour) >= 18)
                {
                    numberOfMinutes = Int32.Parse(scheduleRow.StopTimeMinute) - Int32.Parse(scheduleRow.StartTimeMinute);
                    if (numberOfMinutes >= 0)
                    {
                        numberOfHours = Int32.Parse(scheduleRow.StopTimeHour) - Int32.Parse(scheduleRow.StartTimeHour);
                    }
                    else
                    {
                        numberOfMinutes = numberOfMinutes + 60;
                        numberOfHours = Int32.Parse(scheduleRow.StopTimeHour) - Int32.Parse(scheduleRow.StartTimeHour) - 1;
                    }
                    scheduleRow.NumberOfUnsocialHoursEvening = ((((float)numberOfHours * 60) + numberOfMinutes) / 60);
                }
                else
                {
                    numberOfMinutes = Int32.Parse(scheduleRow.StopTimeMinute);
                    numberOfHours = Int32.Parse(scheduleRow.StopTimeHour) - 18;
                    scheduleRow.NumberOfUnsocialHoursEvening = ((((float)numberOfHours * 60) + numberOfMinutes) / 60);
                }
                numberOfMinutes = numberOfMinutes + numberOfMorningMinutes;
                numberOfHours = numberOfHours + numberOfMorningHours;
            }
            //Here the total number of unsocial hours are stored (morning + evening hours), all hours if weekend
            //Check if weekend
            if (scheduleRow.ScheduleRowDateString.Substring(0, 3) == "lör" || scheduleRow.ScheduleRowDateString.Substring(0, 3) == "sön")
            {
                scheduleRow.NumberOfUnsocialHours = scheduleRow.NumberOfHours;
            }
            else
            {
                scheduleRow.NumberOfUnsocialHours = (((float)numberOfHours * 60) + numberOfMinutes) / 60;
            }
            return scheduleRow;
        }

        //CalculateOnCalllHours calculates the number of oncall hours for the day. The method distinguishes between the total number of oncall hours, oncall hours in the morning (00.00 - 07.00) and
        //oncall hours in the evening (18.00 - 24.00).
        //The method returns the input row with the calculated number of oncall hours added to it.
        private ScheduleRow CalculateOnCallHours(ScheduleRow scheduleRow)
        {
            int numberOfHours = 0;
            int numberOfMinutes = 0;

            int numberOfMorningHours = 0;
            int numberOfMorningMinutes = 0;

            //Check if there is working time before 07:00
            if (scheduleRow.StartTimeHourOnCall != null && scheduleRow.StopTimeHourOnCall != null && Int32.Parse(scheduleRow.StartTimeHourOnCall) < 7)
            {
                //Calculate number of oncall hours in the morning
                if (Int32.Parse(scheduleRow.StopTimeHourOnCall) < 7 || (Int32.Parse(scheduleRow.StopTimeHourOnCall) == 7 && Int32.Parse(scheduleRow.StopTimeMinuteOnCall) == 0))
                {
                    numberOfMinutes = Int32.Parse(scheduleRow.StopTimeMinuteOnCall) - Int32.Parse(scheduleRow.StartTimeMinuteOnCall);
                    if (numberOfMinutes >= 0)
                    {
                        numberOfHours = Int32.Parse(scheduleRow.StopTimeHourOnCall) - Int32.Parse(scheduleRow.StartTimeHourOnCall);
                    }
                    else
                    {
                        numberOfMinutes = numberOfMinutes + 60;
                        numberOfHours = Int32.Parse(scheduleRow.StopTimeHourOnCall) - Int32.Parse(scheduleRow.StartTimeHourOnCall) - 1;
                    }
                    scheduleRow.NumberOfOnCallHoursNight = ((((float)numberOfHours * 60) + numberOfMinutes) / 60);
                }
                else
                {
                    numberOfMinutes = 60 - Int32.Parse(scheduleRow.StartTimeMinuteOnCall);
                    numberOfHours = 7 - Int32.Parse(scheduleRow.StartTimeHourOnCall) - 1;
                    scheduleRow.NumberOfOnCallHoursNight = ((((float)numberOfHours * 60) + numberOfMinutes) / 60);
                }
                numberOfMorningHours = numberOfHours;
                numberOfMorningMinutes = numberOfMinutes;
            }

            //Check if there is working time after 18:00
            if (scheduleRow.StartTimeHourOnCall != null && scheduleRow.StopTimeHourOnCall != null && Int32.Parse(scheduleRow.StopTimeHourOnCall) >= 18)
            {
                //Calculate number of oncall hours in the evening
                if (Int32.Parse(scheduleRow.StartTimeHourOnCall) >= 18)
                {
                    numberOfMinutes = Int32.Parse(scheduleRow.StopTimeMinuteOnCall) - Int32.Parse(scheduleRow.StartTimeMinuteOnCall);
                    if (numberOfMinutes >= 0)
                    {
                        numberOfHours = Int32.Parse(scheduleRow.StopTimeHourOnCall) - Int32.Parse(scheduleRow.StartTimeHourOnCall);
                    }
                    else
                    {
                        numberOfMinutes = numberOfMinutes + 60;
                        numberOfHours = Int32.Parse(scheduleRow.StopTimeHourOnCall) - Int32.Parse(scheduleRow.StartTimeHourOnCall) - 1;
                    }
                    scheduleRow.NumberOfOnCallHoursEvening = ((((float)numberOfHours * 60) + numberOfMinutes) / 60);
                }
                else
                {
                    numberOfMinutes = Int32.Parse(scheduleRow.StopTimeMinuteOnCall);
                    numberOfHours = Int32.Parse(scheduleRow.StopTimeHourOnCall) - 18;
                    scheduleRow.NumberOfOnCallHoursEvening = ((((float)numberOfHours * 60) + numberOfMinutes) / 60);
                }
                numberOfMinutes = numberOfMinutes + numberOfMorningMinutes;
                numberOfHours = numberOfHours + numberOfMorningHours;
            }
            //Here the total number of oncall hours are stored (morning + evening hours)
            scheduleRow.NumberOfOnCallHours = (((float)numberOfHours * 60) + numberOfMinutes) / 60;



            if (scheduleRow.StartTimeHourOnCall != null && scheduleRow.StopTimeHourOnCall != null && (Int32.Parse(scheduleRow.StopTimeHourOnCall) >= Int32.Parse(scheduleRow.StartTimeHourOnCall)))
            {
                numberOfMinutes = Int32.Parse(scheduleRow.StopTimeMinuteOnCall) - Int32.Parse(scheduleRow.StartTimeMinuteOnCall);
                if (numberOfMinutes >= 0)
                {
                    numberOfHours = Int32.Parse(scheduleRow.StopTimeHourOnCall) - Int32.Parse(scheduleRow.StartTimeHourOnCall);
                }
                else
                {
                    numberOfMinutes = numberOfMinutes + 60;
                    numberOfHours = Int32.Parse(scheduleRow.StopTimeHourOnCall) - Int32.Parse(scheduleRow.StartTimeHourOnCall) - 1;
                }
                scheduleRow.NumberOfOnCallHours = (((float)numberOfHours * 60) + numberOfMinutes) / 60;
            }
            return scheduleRow;
        }

        private decimal CalculateUnsocialHoursPayDay2To14(Claim existingClaim, List<ClaimDay> claimDayList)
        {
            decimal unsocialHoursPayDay2To14 = 0;

            //Do not count the qualifying day
            for (int i = 1; i < claimDayList.Count(); i++)
            {
                //Hourly rates hardcoded according to Vårdföretagarna, to be improved later
                if (claimDayList[i].DateString.Substring(0, 3) == "lör")
                {
                    unsocialHoursPayDay2To14 = unsocialHoursPayDay2To14 + (42.54m * (decimal)claimDayList[i].NumberOfUnsocialHoursNight) +
                        (52.47m * ((decimal)claimDayList[i].NumberOfUnsocialHours - (decimal)claimDayList[i].NumberOfUnsocialHoursNight));
                }
                else if (claimDayList[i].DateString.Substring(0, 3) == "sön")
                {
                    unsocialHoursPayDay2To14 = unsocialHoursPayDay2To14 + (52.47m * (decimal)claimDayList[i].NumberOfUnsocialHours);
                }
                else
                {
                    unsocialHoursPayDay2To14 = unsocialHoursPayDay2To14 + (21.08m * (decimal)claimDayList[i].NumberOfUnsocialHoursEvening) +
                    (42.54m * (decimal)claimDayList[i].NumberOfUnsocialHoursNight);
                }
            }
            return unsocialHoursPayDay2To14;
        }

        public decimal CalculateOnCallHoursPayDay2To14(Claim existingClaim, List<ClaimDay> claimDayList)
        {
            decimal onCallHoursPayDay2To14 = 0;

            //Do not count the qualifying day
            for (int i = 1; i < claimDayList.Count(); i++)
            {
                //Hourly rates hardcoded according to Vårdföretagarna, to be improved later
                if (claimDayList[i].DateString.Substring(0, 3) == "lör" || claimDayList[i].DateString.Substring(0, 3) == "sön")
                {
                    onCallHoursPayDay2To14 = onCallHoursPayDay2To14 + (56.32m * (decimal)claimDayList[i].NumberOfOnCallHours);
                }
                else
                {
                    onCallHoursPayDay2To14 = onCallHoursPayDay2To14 + (56.32m * (decimal)claimDayList[i].NumberOfOnCallHoursNight) +
                        (28.13m * ((decimal)claimDayList[i].NumberOfOnCallHours - (decimal)claimDayList[i].NumberOfOnCallHoursNight));
                }
            }
            return onCallHoursPayDay2To14;
        }

        public decimal CalculateSalaryDay2To14(Claim claim, List<ClaimDay> claimDayList)
        {
            decimal salaryDay2To14 = 0;

            //Do not count the qualifying day
            for (int i = 1; i < claimDayList.Count(); i++)
            {
                //salaryDay2To14 = salaryDay2To14 + (claimFormVM.HourlySalary * scheduleVM.ScheduleRowList[i].NumberOfHours);
                salaryDay2To14 = salaryDay2To14 + (120 * (decimal)claimDayList[i].NumberOfHours); //Hourly salary hardcoded to 120,00 kr/hour
            }

            return salaryDay2To14;
        }

        public decimal CalculateHolidayPayDay2To14(Claim claim, List<ClaimDay> claimDayList)
        {
            decimal holidayPayDay2To14 = 0;
            decimal numberOfHours = 0;

            //Do not count the qualifying day
            for (int i = 1; i < claimDayList.Count(); i++)
            {
                numberOfHours = numberOfHours + (decimal)claimDayList[i].NumberOfHours;
            }
            holidayPayDay2To14 = 12 * 0.8m * 120 * numberOfHours / 100;

            return holidayPayDay2To14;
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
