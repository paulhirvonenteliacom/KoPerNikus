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
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Sjuklöner.Controllers
{
    //[Authorize]
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
            else if (User.IsInRole("Admin"))
            {
                return RedirectToAction("IndexPageAdmin", "Claims");
            }
            else  // This should never happen     
            {
                return RedirectToAction("Login", "Account");
            }
        }

        // GET: Claims
        [Authorize(Roles = "Ombud")]
        public ActionResult IndexPageOmbud(string searchString, string searchBy = "Referensnummer")
        {
            IndexPageOmbudVM indexPageOmbudVM = new IndexPageOmbudVM();

            var me = db.Users.Find(User.Identity.GetUserId());
            int companyId = (int)me.CareCompanyId;
            indexPageOmbudVM.CompanyName = db.CareCompanies.Where(c => c.Id == companyId).FirstOrDefault().CompanyName;

            var claims = db.Claims.Where(c => c.CareCompanyId == companyId).OrderByDescending(c => c.CreationDate).ToList();
            if (claims.Count > 0)
            {
                var decidedClaims = claims.Where(c => c.ClaimStatusId == 1);
                var draftClaims = claims.Where(c => c.ClaimStatusId == 2);
                var underReviewClaims = claims.Where(c => c.ClaimStatusId == 3 || c.ClaimStatusId == 4 || c.ClaimStatusId == 5 || c.ClaimStatusId == 6 || c.ClaimStatusId == 7);
                if (searchBy == "Mine")
                {
                    decidedClaims = decidedClaims.Where(c => c.OmbudEmail == me.Email);
                    draftClaims = draftClaims.Where(c => c.OmbudEmail == me.Email);
                    underReviewClaims = underReviewClaims.Where(c => c.OmbudEmail == me.Email);
                }
                else if (!string.IsNullOrWhiteSpace(searchString))
                {
                    decidedClaims = Search(decidedClaims, searchString, searchBy);
                    draftClaims = Search(draftClaims, searchString, searchBy);
                    underReviewClaims = Search(underReviewClaims, searchString, searchBy);
                }

                indexPageOmbudVM.DecidedClaims = decidedClaims.ToList(); //Old "Rejected
                indexPageOmbudVM.DraftClaims = draftClaims.ToList();
                indexPageOmbudVM.UnderReviewClaims = underReviewClaims.ToList();
            }

            //Check if at least two assistants have been defined for the company
            var assistants = db.Assistants.Where(a => a.CareCompanyId == companyId).ToList();
            indexPageOmbudVM.AssistantsExist = false;
            if (assistants.Count() >= 2)
            {
                indexPageOmbudVM.AssistantsExist = true;
            }

            return View("IndexPageOmbud", indexPageOmbudVM);
        }

        // GET: Claims
        [Authorize(Roles = "AdministrativeOfficial")]
        public ActionResult IndexPageAdmOff(string searchString, string searchBy = "Referensnummer")
        {
            IndexPageAdmOffVM indexPageAdmOffVM = new IndexPageAdmOffVM();

            var me = db.Users.Find(User.Identity.GetUserId());

            var claims = db.Claims.Include(c => c.CareCompany).OrderByDescending(c => c.SentInDate).ToList();
            if (claims.Count > 0)
            {
                var decidedClaims = claims.Where(c => c.ClaimStatusId == 1);
                var inInboxClaims = claims.Where(c => c.ClaimStatusId == 5);
                var underReviewClaims = claims.Where(c => (c.ClaimStatusId == 6 || c.ClaimStatusId == 7)); //Claims that have been transferred to Procapita
                if (searchBy == "Mine")
                {
                    decidedClaims = decidedClaims.Where(c => c.AdmOffName.Contains(me.FirstName) && c.AdmOffName.Contains(me.LastName));
                    inInboxClaims = inInboxClaims.Where(c => c.AdmOffName.Contains(me.FirstName) && c.AdmOffName.Contains(me.LastName));
                    underReviewClaims = underReviewClaims.Where(c => c.AdmOffName.Contains(me.FirstName) && c.AdmOffName.Contains(me.LastName));
                }
                else if (!string.IsNullOrWhiteSpace(searchString))
                {
                    decidedClaims = Search(decidedClaims, searchString, searchBy);
                    inInboxClaims = Search(inInboxClaims, searchString, searchBy);
                    underReviewClaims = Search(underReviewClaims, searchString, searchBy);
                }
                indexPageAdmOffVM.DecidedClaims = decidedClaims.ToList();
                indexPageAdmOffVM.InInboxClaims = inInboxClaims.ToList();
                indexPageAdmOffVM.UnderReviewClaims = underReviewClaims.ToList();
            }

            return View("IndexPageAdmOff", indexPageAdmOffVM);
        }


        // GET: Claims
        [Authorize(Roles = "Admin")]
        public ActionResult IndexPageAdmin(string searchString, string searchBy = "Referensnummer")
        {
            IndexPageAdmOffVM indexPageAdmin = new IndexPageAdmOffVM();

            var me = db.Users.Find(User.Identity.GetUserId());

            var claims = db.Claims.Include(c => c.CareCompany).OrderByDescending(c => c.SentInDate).ToList();
            if (claims.Count > 0)
            {
                var decidedClaims = claims.Where(c => c.ClaimStatusId == 1);
                var inInboxClaims = claims.Where(c => c.ClaimStatusId == 5);
                var underReviewClaims = claims.Where(c => (c.ClaimStatusId == 6 || c.ClaimStatusId == 7)); //Claims that have been transferred to Procapita               

                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    decidedClaims = Search(decidedClaims, searchString, searchBy);
                    inInboxClaims = Search(inInboxClaims, searchString, searchBy);
                    underReviewClaims = Search(underReviewClaims, searchString, searchBy);
                }
                indexPageAdmin.DecidedClaims = decidedClaims.ToList();
                indexPageAdmin.InInboxClaims = inInboxClaims.ToList();
                indexPageAdmin.UnderReviewClaims = underReviewClaims.ToList();
            }

            return View("IndexPageAdmin", indexPageAdmin);
        }

        private IEnumerable<Claim> Search(IEnumerable<Claim> Claims, string searchString, string searchBy)
        {
            if (searchBy == "Referensnummer")
                Claims = Claims.Where(c => c.ReferenceNumber.Contains(searchString));
            else if (searchBy == "CSSN")    // Sökning på Kundens personnummer
            {
                searchString = searchString.Replace("-", "");
                if (searchString.Length > 10)
                    searchString = searchString.Substring(2);
                Claims = Claims.Where(c => c.CustomerSSN.Replace("-", "").Contains(searchString));
            }
            else if (searchBy == "ASSN")    // Sökning på Assistentens personnummer
            {
                searchString = searchString.Replace("-", "");
                if (searchString.Length > 10)
                    searchString = searchString.Substring(1);
                Claims = Claims.Where(c => c.RegAssistantSSN.Replace("-", "").Contains(searchString));
            }
            else if (searchBy == "Handl")             // Sökning på Handläggarens efternamn
            {
                Claims = Claims.Where(c => c.AdmOffName.Contains(searchString));
            }
            else if (searchBy == "Ombud")             // Sökning på Ombudets efternamn
            {
                Claims = Claims.Where(c => c.OmbudLastName.Contains(searchString));
            }
            else if (searchBy == "Bolag")             // Sökning på Bolagsnamn
            {
                Claims = Claims.Where(c => c.CareCompany.CompanyName.Contains(searchString));
            }

            return Claims;
        }

        /*
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
        */

        // GET: Claims/Create1
        [HttpGet]
        [Authorize(Roles = "Ombud")]
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

                //Number of substitute assistants is always at least one. One is used as default for a new claim.
                create1VM.NumberOfSubAssistants = 1;

                //Initialize array that will hold selected substitute assistants (beyond the first sub that is mandatory)
                int[] subIndeces = new int[20];
                string[] selectedSubAssistants = new string[20];
                for (int i = 0; i < 20; i++)
                {
                    subIndeces[i] = 0;
                    selectedSubAssistants[i] = "";
                }
                create1VM.SelectedSubIndeces = subIndeces;
                create1VM.SelectedSubAssistants = selectedSubAssistants;

                create1VM.FirstClaimDate = DateTime.Now.AddDays(-1);
                create1VM.LastClaimDate = DateTime.Now.AddDays(-1);
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

            //Initialize the array holding the selected substitute assistant indeces
            int[] selectedSubAssistantIndeces = new int[20];
            for (int i = 0; i < 20; i++)
            {
                selectedSubAssistantIndeces[i] = 0;
            }

            //Figure out the index of each saved substitue assistant. They could have changed due to addition and removal of Assistants since the draft claim was saved. 
            string[] selectedSubAssistantSSNs = new string[20];
            int numberOfRemovedSubAssistants = 0;
            bool indexFound;
            int idx;
            int selectedIndex;
            for (int i = 0; i < claim.NumberOfSubAssistants - 1; i++) //- 1 because the first (mandatory substitue assistant is included in NumberOfSubAssistants but not in the concatenated strings.
            {
                idx = 0;
                indexFound = false;
                selectedIndex = 0;
                selectedSubAssistantSSNs[i] = claim.SubAssistantsSSNConcat.Split('£')[i];
                while (!indexFound && idx < assistants.Count())
                {
                    if (selectedSubAssistantSSNs[i] == assistants[idx].AssistantSSN)
                    {
                        selectedIndex = idx + 1;
                        indexFound = true;
                    }
                    idx++;
                }
                if (indexFound)
                {
                    selectedSubAssistantIndeces[i - numberOfRemovedSubAssistants] = selectedIndex;
                }
                else
                {
                    //Need to remove the substitute assistant??
                    numberOfRemovedSubAssistants++;
                }
            }

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
            create1VM.NumberOfSubAssistants = claim.NumberOfSubAssistants - numberOfRemovedSubAssistants;

            //Possibly code could be added here to handle the case where a sub assistant has been removed after creation of the claim but before submission of it

            //Load substitute assistant indeces for assistants beyond the first
            //int[] selectedSubAssistantIndeces = new int[20];

            //for (int i = 0; i < claim.NumberOfSubAssistants - 1; i++)
            //{
            //    selectedSubAssistantIndeces[i] = Convert.ToInt32(claim.SelectedSubAssistantIndeces.Split(',')[i]);
            //}
            //for (int i = claim.NumberOfSubAssistants - 1; i < 20; i++)
            //{
            //    selectedSubAssistantIndeces[i] = 0;
            //}
            create1VM.SelectedSubIndeces = selectedSubAssistantIndeces;

            create1VM.CustomerName = claim.CustomerName;
            create1VM.CustomerSSN = claim.CustomerSSN;
            //create1VM.CustomerAddress = claim.CustomerAddress;
            create1VM.CustomerPhoneNumber = claim.CustomerPhoneNumber;
            create1VM.FirstClaimDate = claim.FirstClaimDate;
            create1VM.LastClaimDate = claim.LastClaimDate;
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

            //Number of substitute assistants is always at least one. One is used as default for a new claim.
            defaultValuesCreate1VM.NumberOfSubAssistants = 1;

            defaultValuesCreate1VM.CustomerSSN = "391025-7246";
            defaultValuesCreate1VM.FirstClaimDate = DateTime.Now.AddDays(-4);
            defaultValuesCreate1VM.LastClaimDate = DateTime.Now.AddDays(-1);

            return defaultValuesCreate1VM;
        }

        // POST: Claims/Create1
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Ombud")]
        public ActionResult Create1(Create1VM create1VM, string refNumber, string submitButton)
        {
            bool errorFound = false;
            //Check that the SSN has the correct format
            if (!string.IsNullOrWhiteSpace(create1VM.CustomerSSN))
            {
                create1VM.CustomerSSN = create1VM.CustomerSSN.Trim();
                Regex regex = new Regex(@"^([1-9][0-9]{3})(((0[13578]|1[02])(0[1-9]|[12][0-9]|3[01]))|((0[469]|11)(0[1-9]|[12][0-9]|30))|(02(0[1-9]|[12][0-9])))[-]?([a-zåäö]|[0-9]){4}$");
                Match match = regex.Match(create1VM.CustomerSSN);
                if (!match.Success)
                {
                    ModelState.AddModelError("CustomerSSN", "Ej giltigt personnummer. Formaten YYYYMMDD-NNNN och YYYYMMDDNNNN är giltiga.");
                    errorFound = true;
                }
            }
            else
            {
                errorFound = true;
            }

            //Check that the customer is born in the 20th or 21st century
            if (!errorFound)
            {
                if (int.Parse(create1VM.CustomerSSN.Substring(0, 2)) != 19 && int.Parse(create1VM.CustomerSSN.Substring(0, 2)) != 20)
                {
                    ModelState.AddModelError("CustomerSSN", "Kunden måste vara född på 1900- eller 2000-talet.");
                    errorFound = true;
                }
            }

            //Check that the customer was not born in the future:-)
            if (!errorFound)
            {
                DateTime assistantBirthday = new DateTime(int.Parse(create1VM.CustomerSSN.Substring(0, 4)), int.Parse(create1VM.CustomerSSN.Substring(4, 2)), int.Parse(create1VM.CustomerSSN.Substring(6, 2)));
                if (assistantBirthday.Date > DateTime.Now.Date)
                {
                    ModelState.AddModelError("CustomerSSN", "Födelsedatumet får inte vara senare än idag.");
                    errorFound = true;
                }
            }

            if (!errorFound)
            {
                if (create1VM.CustomerSSN.Length == 12)
                {
                    create1VM.CustomerSSN = create1VM.CustomerSSN.Insert(8, "-");
                }
            }

            //Check if the sickleave period is in the future.
            if (create1VM.FirstClaimDate.Date >= DateTime.Now.Date)
            {
                ModelState.AddModelError("FirstClaimDate", "Datumet får vara senast gårdagens datum.");
            }
            if (create1VM.LastClaimDate.Date >= DateTime.Now.Date)
            {
                ModelState.AddModelError("LastClaimDate", "Datumet får vara senast gårdagens datum.");
            }
            //Check that the last day of sickness is equal to or greater than the first day of sickness
            if (create1VM.FirstClaimDate.Date > create1VM.LastClaimDate.Date)
            {
                ModelState.AddModelError("LastDayOfSicknessDate", "Sjukperiodens sista dag får inte vara tidigare än sjukperiodens första dag.");
            }
            //Check that the number of calendar days in the claim is maximum 70. This takes the 5-day rule into consideration. 
            if ((create1VM.LastClaimDate.Date - create1VM.FirstClaimDate.Date).Days > 69)
            {
                ModelState.AddModelError("LastDayOfSicknessDate", "Antalet kalenderdagar överstiger gränsen för vad som är möjligt.");
            }
            //Check that the last day in the sickleave period is not older than one year
            //First check if one of the 3 previous years is a leap year
            bool leapYearFound = false;
            int yearIdx = 0;
            if (DateTime.Now.Month < 3)
            {
                yearIdx = 1;
            }
            do
            {
                if (DateTime.IsLeapYear(DateTime.Now.AddYears(-yearIdx).Year))
                {
                    leapYearFound = true;
                }
                yearIdx++;
            } while (!leapYearFound && yearIdx < 4);
            int numberOfDays = 3 * 365;

            if (leapYearFound)
            {
                numberOfDays++;
            }
            if ((DateTime.Now.Date - create1VM.LastClaimDate.Date).Days > numberOfDays)
            {
                ModelState.AddModelError("LastDayOfSicknessDate", "Det går inte att ansöka om ersättning mer än tre år tillbaka i tiden.");
            }
            //Check if the regular assistant has been selected
            if (create1VM.SelectedRegAssistantId == null)
            {
                ModelState.AddModelError("RegularAssistants", "Ordinarie assistent måste väljas.");
            }
            //Check if the substitue assistant has been selected
            if (create1VM.SelectedSubAssistantId == null)
            {
                ModelState.AddModelError("SubstituteAssistants", "Vikarierande assistent måste väljas.");
            }
            //Check if the substitute assistant is the same as the regular assistant
            if (create1VM.SelectedRegAssistantId == create1VM.SelectedSubAssistantId)
            {
                ModelState.AddModelError("SubstituteAssistants", "Vikarierande assistent får inte vara samma som ordinarie assistent.");
            }
            string regAssistantSSN = db.Assistants.Where(a => a.Id == create1VM.SelectedRegAssistantId).FirstOrDefault().AssistantSSN;
            if (OverlappingClaim(refNumber, create1VM.FirstClaimDate, create1VM.LastClaimDate, create1VM.CustomerSSN, regAssistantSSN))
            {
                ModelState.AddModelError("FirstDayOfSicknessDate", "En eller flera sjukdagar överlappar med en existerande ansökan för samma kund och samma ordinarie assistent.");
            }
            //Check if no substitute was selected in the substitutes beyond the first mandatory substitute assistant
            for (int i = 0; i < create1VM.NumberOfSubAssistants - 1; i++)
            {
                if (create1VM.SelectedSubIndeces[i] == 0 || create1VM.SelectedSubIndeces[i] == 9999)  //9999 is used as a non valid value
                {
                    ModelState.AddModelError("SelectedSubAssistants[" + i.ToString() + "]", "Vikarierande assistent måste väljas.");
                }
            }
            //Check if a substitute assistant already has been selected
            int startIdx = 1;
            for (int i = 0; i < create1VM.NumberOfSubAssistants - 1; i++)
            {
                if (create1VM.SelectedSubIndeces[i] != 0 && create1VM.SelectedSubIndeces[i] != 9999)  //9999 is used as a non valid value
                {
                    for (int k = startIdx; k < create1VM.NumberOfSubAssistants - 1; k++)
                    {
                        if (create1VM.SelectedSubIndeces[i] == create1VM.SelectedSubIndeces[k])
                        {
                            ModelState.AddModelError("SelectedSubAssistants[" + k.ToString() + "]", "Assistenten är redan vald.");
                        }
                    }
                    //Check if the selected substitute assistant is equal to the first (mandatory) substitute assistant or if it is equal to the regular assistant
                    var firstSub = db.Assistants.Where(a => a.Id == create1VM.SelectedSubAssistantId).FirstOrDefault();
                    var regAssistant = db.Assistants.Where(a => a.Id == create1VM.SelectedRegAssistantId).FirstOrDefault();
                    if (firstSub != null)
                    {
                        if (create1VM.SelectedSubAssistants[i].Substring(0, 13) == firstSub.AssistantSSN)
                        {
                            ModelState.AddModelError("SelectedSubAssistants[" + i.ToString() + "]", "Assistenten är redan vald.");
                        }
                    }
                    if (regAssistant != null)
                    {
                        if (create1VM.SelectedSubAssistants[i].Substring(0, 13) == regAssistant.AssistantSSN)
                        {
                            ModelState.AddModelError("SelectedSubAssistants[" + i.ToString() + "]", "Assistenten är redan vald.");
                        }
                    }
                }
                startIdx++;
            }

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
                    create1VM.NumberOfSubAssistants = claim.NumberOfSubAssistants;
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
                //Should this branch also handle the case where an attempt to update an existing claim is made and where the update does not have a valid model? 
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
                //create1VM.NumberOfSubAssistants = 1;

                return View(create1VM);
            }
        }

        private void AdjustClaimDays(Create1VM create1VM)
        {
            //Find existing claim days
            var existingClaimDays = db.ClaimDays.Where(c => c.ReferenceNumber == create1VM.ClaimNumber).OrderBy(c => c.CalendarDayNumber).ToList();

            //Calculate offsets at the beginning and end of the date range for the existing claim days
            var offsetStart = (create1VM.FirstClaimDate.Date - existingClaimDays[0].Date.Date).Days;
            var offsetEnd = (create1VM.LastClaimDate.Date - existingClaimDays.Last().Date.Date).Days;
            int claimDayPos = 1;
            int maxIdx;

            List<ClaimDay> newClaimDays = new List<ClaimDay>();

            if (offsetStart < 0) //Add new claim days to the beginning of the sickleave period 
            {
                if (create1VM.LastClaimDate.Date < existingClaimDays[0].Date.Date)
                {
                    maxIdx = 1 + (create1VM.LastClaimDate.Date - create1VM.FirstClaimDate.Date).Days;
                }
                else
                {
                    maxIdx = (existingClaimDays[0].Date.Date - create1VM.FirstClaimDate.Date).Days;
                }
                for (int i = 0; i < maxIdx; i++)
                {
                    ClaimDay claimDay = new ClaimDay();
                    claimDay.Date = create1VM.FirstClaimDate.AddDays(i);
                    claimDay.DateString = create1VM.FirstClaimDate.AddDays(i).ToString(format: "ddd d MMM");
                    claimDay.ReferenceNumber = create1VM.ClaimNumber;
                    claimDay.CalendarDayNumber = i + 1;
                    InitializeHoursInClaimDay(claimDay);
                    newClaimDays.Add(claimDay);
                    claimDayPos++;
                }
                //Copy relevant existing claim days to the new claim day list
                if (create1VM.LastClaimDate.Date >= existingClaimDays[0].Date.Date)
                {
                    if (create1VM.LastClaimDate.Date < existingClaimDays.Last().Date)
                    {
                        maxIdx = 1 + (create1VM.LastClaimDate.Date - existingClaimDays[0].Date.Date).Days;
                    }
                    else
                    {
                        maxIdx = 1 + (existingClaimDays.Last().Date - existingClaimDays[0].Date.Date).Days;
                    }
                    for (int i = 0; i < maxIdx; i++)
                    {
                        existingClaimDays[0].CalendarDayNumber = claimDayPos;
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
                        claimDay.CalendarDayNumber = claimDayPos;
                        claimDay.Date = create1VM.FirstClaimDate.AddDays(claimDayPos - 1);
                        claimDay.DateString = create1VM.FirstClaimDate.AddDays(claimDayPos - 1).ToString(format: "ddd d MMM");
                        InitializeHoursInClaimDay(claimDay);
                        newClaimDays.Add(claimDay);
                        claimDayPos++;
                    }
                }
            }
            else if (offsetStart == 0) //Copy claim days to the new claim day list starting from the beginning of the sickleave period
            {
                if (offsetEnd <= 0)
                {
                    maxIdx = 1 + (create1VM.LastClaimDate.Date - create1VM.FirstClaimDate.Date).Days;
                }
                else
                {
                    maxIdx = existingClaimDays.Last().CalendarDayNumber;
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
                        claimDay.CalendarDayNumber = claimDayPos;
                        claimDay.Date = create1VM.FirstClaimDate.AddDays(claimDayPos - 1);
                        claimDay.DateString = create1VM.FirstClaimDate.AddDays(claimDayPos - 1).ToString(format: "ddd d MMM");
                        InitializeHoursInClaimDay(claimDay);
                        newClaimDays.Add(claimDay);
                        claimDayPos++;
                    }
                }
            }
            else if (offsetStart > 0 && offsetStart <= ((existingClaimDays.Last().Date - existingClaimDays[0].Date.Date).Days)) //The updated sickleave period starts somewhere in the existing sick leave period. Copy only the relevant claim days from the existing claim days list.
            {
                if (offsetEnd <= 0)
                {
                    maxIdx = 1 + (create1VM.LastClaimDate.Date - create1VM.FirstClaimDate.Date).Days;
                }
                else
                {
                    maxIdx = 1 + (existingClaimDays.Last().Date.Date - create1VM.FirstClaimDate.Date).Days;
                }
                for (int i = 0; i < maxIdx; i++)
                {
                    existingClaimDays[offsetStart].CalendarDayNumber = claimDayPos;
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
                        claimDay.CalendarDayNumber = claimDayPos;
                        claimDay.Date = create1VM.FirstClaimDate.AddDays(claimDayPos - 1);
                        claimDay.DateString = create1VM.FirstClaimDate.AddDays(claimDayPos - 1).ToString(format: "ddd d MMM");
                        InitializeHoursInClaimDay(claimDay);
                        newClaimDays.Add(claimDay);
                        claimDayPos++;
                    }
                }
            }
            else //The updated sickleave period is later than the existing and the old and new do not overlap.
            {
                maxIdx = 1 + (create1VM.LastClaimDate.Date - create1VM.FirstClaimDate.Date).Days;
                for (int i = 0; i < maxIdx; i++)
                {
                    ClaimDay claimDay = new ClaimDay();
                    claimDay.Date = create1VM.FirstClaimDate.AddDays(i);
                    claimDay.DateString = create1VM.FirstClaimDate.AddDays(i).ToString(format: "ddd d MMM");
                    claimDay.ReferenceNumber = create1VM.ClaimNumber;
                    claimDay.CalendarDayNumber = i + 1;
                    InitializeHoursInClaimDay(claimDay);
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

            //The code below is probably not needed since this update is done after create2 post.
            //Update SickDayNumber in claimdays. Very likely the previous numbering is not valid after changing first and/or last date of the sickleave period.
            //var claimDays = db.ClaimDays.Where(c => c.ReferenceNumber == create1VM.ClaimNumber).OrderBy(c => c.CalendarDayNumber).ToList();
            //int sickDayCounter = 1;
            //foreach (var day in claimDays)
            //{
            //    if (!day.Well)
            //    {
            //        day.SickDayNumber = sickDayCounter;
            //        sickDayCounter++;
            //        db.Entry(day).State = EntityState.Modified;
            //    }
            //    else
            //    {
            //        day.SickDayNumber = null;
            //    }
            //}
            //db.SaveChanges();
        }

        private void InitializeHoursInClaimDay(ClaimDay claimDay)
        {
            claimDay.Hours = "";
            claimDay.UnsocialEvening = "";
            claimDay.UnsocialNight = "";
            claimDay.UnsocialWeekend = "";
            claimDay.UnsocialGrandWeekend = "";
            claimDay.OnCallDay = "";
            claimDay.OnCallNight = "";
            claimDay.HoursSI = "+++++++++++++++++++";
            claimDay.UnsocialEveningSI = "+++++++++++++++++++";
            claimDay.UnsocialNightSI = "+++++++++++++++++++";
            claimDay.UnsocialWeekendSI = "+++++++++++++++++++";
            claimDay.UnsocialGrandWeekendSI = "+++++++++++++++++++";
            claimDay.OnCallDaySI = "+++++++++++++++++++";
            claimDay.OnCallNightSI = "+++++++++++++++++++";
        }

        private string SaveNewClaim(Create1VM create1VM)
        {
            Claim claim = new Claim();
            claim.ReferenceNumber = GenerateReferenceNumber();
            claim.CompletionStage = 1; //CompletionsStage is used for keeping track on what stage in the claim process has been completed. Used when jumping back in the process and also when updating a draft claim.
            claim.ClaimStatusId = 2;  //ClaimStatus.Name = "Utkast"
            claim.FirstAssistanceDate = null; //This property is later set by Robin if there is a date in Procapita for first date for approved personal assistance. 
            claim.LastAssistanceDate = null; //This property is later set by Robin if there is a date in Procapita for last date for approved personal assistance. 

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

            claim.CreationDate = DateTime.Now;
            claim.StatusDate = DateTime.Now;
            claim.FirstClaimDate = create1VM.FirstClaimDate;
            claim.FirstClaimDayShort = create1VM.FirstClaimDate.ToShortDateString();
            claim.LastClaimDate = create1VM.LastClaimDate;
            claim.LastClaimDayShort = create1VM.LastClaimDate.ToShortDateString();
            claim.NumberOfCalendarDays = 1 + (create1VM.LastClaimDate.Date - create1VM.FirstClaimDate.Date).Days;

            // Need to set start values for theses 3 properties to avoid DbUpdateException
            claim.BasisForDecisionTransferFinishTimeStamp = new DateTime(1900, 1, 1);
            claim.BasisForDecisionTransferStartTimeStamp = new DateTime(1900, 1, 1);
            claim.DecisionTransferTimeStamp = new DateTime(1900, 1, 1);

            // No AdmOfficial assigned to the Claim when it is created
            claim.AdmOffId = null;
            claim.AdmOffName = "-";

            db.Claims.Add(claim);
            db.SaveChanges();
            return claim.ReferenceNumber;
        }

        private static void SaveCustomerInformation(Create1VM create1VM, Claim claim)
        {
            claim.CustomerSSN = create1VM.CustomerSSN;
            claim.CustomerName = create1VM.CustomerName;
            //claim.CustomerAddress = create1VM.CustomerAddress;
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

            claim.NumberOfSubAssistants = create1VM.NumberOfSubAssistants;

            //Save substitute assistant indeces for assistants beyond the first
            string selectedIndeces = "";
            string selectedCustomerNames = "";
            string selectedCustomerSSNs = "";
            string selectedCustomerPhoneNumbers = "";
            string selectedCustomerEmails = "";
            string subAssistantSSN;

            for (int i = 0; i < create1VM.NumberOfSubAssistants - 1; i++)
            {
                selectedIndeces += create1VM.SelectedSubIndeces[i] + ",";
                selectedCustomerSSNs += create1VM.SelectedSubAssistants[i].Substring(0, 13) + "£";
                selectedCustomerNames += create1VM.SelectedSubAssistants[i].Substring(15) + "£";
                subAssistantSSN = create1VM.SelectedSubAssistants[i].Substring(0, 13);

                //find record of sub assistant in order to read the phone number and emailaddress
                var subAssistant = db.Assistants.Where(a => a.AssistantSSN == subAssistantSSN).FirstOrDefault();
                if (subAssistant != null)
                {
                    selectedCustomerPhoneNumbers += subAssistant.PhoneNumber + "£";
                    selectedCustomerEmails += subAssistant.Email + "£";
                }
                else
                {
                    selectedCustomerPhoneNumbers += "NN£";    //This can only happen if the assistant has been removed after the creation of the claim and before submitting it.
                    selectedCustomerEmails += "NN£";    //This can only happen if the assistant has been removed after the creation of the claim and before submitting it.
                }
            }
            claim.SubAssistantsNameConcat = selectedCustomerNames;
            claim.SubAssistantsSSNConcat = selectedCustomerSSNs;
            claim.SubAssistantsPhoneConcat = selectedCustomerPhoneNumbers;
            claim.SubAssistantsEmailConcat = selectedCustomerEmails;

            for (int i = claim.NumberOfSubAssistants - 1; i < 20; i++)
            {
                selectedIndeces += "0,";
            }
            claim.SelectedSubAssistantIndeces = selectedIndeces;
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

            claim.CreationDate = DateTime.Now;
            claim.StatusDate = DateTime.Now;
            claim.FirstClaimDate = create1VM.FirstClaimDate;
            claim.FirstClaimDayShort = create1VM.FirstClaimDate.ToShortDateString();
            claim.LastClaimDate = create1VM.LastClaimDate;
            claim.LastClaimDayShort = create1VM.LastClaimDate.ToShortDateString();
            claim.NumberOfCalendarDays = 1 + (create1VM.LastClaimDate.Date - create1VM.FirstClaimDate.Date).Days;
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
                if (db.ClaimReferenceNumbers.FirstOrDefault().LatestReferenceNumber != 0)
                {
                    newReferenceNumber = DateTime.Now.Year.ToString() + Convert.ToInt32(latestReference.LatestReferenceNumber + 1).ToString("D5");
                    db.ClaimReferenceNumbers.FirstOrDefault().LatestReferenceNumber = latestReference.LatestReferenceNumber + 1;

                }
                //The code below avoids starting with ref number "YYYY00001" after updating the database if there are claims in the database
                else
                {
                    var lastClaim = db.Claims.ToList().LastOrDefault();
                    if (lastClaim != null)
                    {
                        newReferenceNumber = DateTime.Now.Year.ToString() + (Convert.ToInt32(lastClaim.ReferenceNumber.Substring(4)) + 1).ToString("D5");
                        db.ClaimReferenceNumbers.FirstOrDefault().LatestReferenceNumber = Convert.ToInt32(newReferenceNumber.Substring(4));
                    }
                    else
                    {
                        newReferenceNumber = DateTime.Now.Year.ToString() + "00001";
                        db.ClaimReferenceNumbers.FirstOrDefault().LatestReferenceNumber = 1;

                    }
                }
            }
            return newReferenceNumber;
        }

        // GET: Claims/Create2
        [HttpGet]
        [Authorize(Roles = "Ombud")]
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
            var numberOfDays = 1 + (claim.LastClaimDate.Date - claim.FirstClaimDate.Date).Days;

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
                dateInSchedule = claim.FirstClaimDate.AddDays(i);

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
            var numberOfDays = (claim.LastClaimDate.Date - claim.FirstClaimDate.Date).Days + 1;

            create2VM.ReferenceNumber = claim.ReferenceNumber;
            create2VM.NumberOfSubAssistants = claim.NumberOfSubAssistants;

            create2VM.RegAssistantSSNAndName = claim.RegAssistantSSN + ", " + claim.RegFirstName + " " + claim.RegLastName;

            string[] subAssistantSSN = new string[20];
            string[] subAssistantName = new string[20];
            string[] subAssistantSSNAndName = new string[20];

            subAssistantSSN = claim.SubAssistantsSSNConcat.Split('£').ToArray();
            subAssistantName = claim.SubAssistantsNameConcat.Split('£').ToArray();
            subAssistantSSNAndName[0] = claim.SubAssistantSSN + ", " + claim.SubFirstName + " " + claim.SubLastName;

            for (int i = 1; i < claim.NumberOfSubAssistants; i++)
            {
                subAssistantSSNAndName[i] = subAssistantSSN[i - 1] + ", " + subAssistantName[i - 1];
            }

            create2VM.SubAssistantSSNAndName = subAssistantSSNAndName;

            List<ScheduleRow> rowList = new List<ScheduleRow>();

            DateTime dateInSchedule;

            List<ClaimDay> claimDays = new List<ClaimDay>();

            string[] hoursSIArray = new string[20];
            string[] unsocialEveningSIArray = new string[20];
            string[] unsocialNightSIArray = new string[20];
            string[] unsocialWeekendSIArray = new string[20];
            string[] unsocialGrandWeekendSIArray = new string[20];
            string[] onCallDaySIArray = new string[20];
            string[] onCallNightSIArray = new string[20];


            //string[] hoursPerSubAndDay2 = new string[20];

            List<string[]> hoursSIPerSubAndDay = new List<string[]>();
            List<string[]> unsocialEveningSIPerSubAndDay = new List<string[]>();
            List<string[]> unsocialNightSIPerSubAndDay = new List<string[]>();
            List<string[]> unsocialWeekendSIPerSubAndDay = new List<string[]>();
            List<string[]> unsocialGrandWeekendSIPerSubAndDay = new List<string[]>();
            List<string[]> onCallDaySIPerSubAndDay = new List<string[]>();
            List<string[]> onCallNightSIPerSubAndDay = new List<string[]>();



            //string[,] hoursSIPerSubAndDay = new string[20, 70];
            //string[,] unsocialEveningSIPerSubAndDay = new string[20, 70];
            //string[,] unsocialNightSIPerSubAndDay = new string[20, 70];
            //string[,] unsocialWeekendSIPerSubAndDay = new string[20, 70];
            //string[,] unsocialGrandWeekendSIPerSubAndDay = new string[20, 70];
            //string[,] onCallDaySIPerSubAndDay = new string[20, 70];
            //string[,] onCallNightSIPerSubAndDay = new string[20, 70];

            if (claim.CompletionStage >= 2)
            {
                var dayIdx = 0;
                claimDays = db.ClaimDays.Where(c => c.ReferenceNumber == claim.ReferenceNumber).OrderBy(c => c.CalendarDayNumber).ToList();
                foreach (var day in claimDays)
                {
                    hoursSIArray = day.HoursSI.Split('+').ToArray();
                    unsocialEveningSIArray = day.UnsocialEveningSI.Split('+').ToArray();
                    unsocialNightSIArray = day.UnsocialNightSI.Split('+').ToArray();
                    unsocialWeekendSIArray = day.UnsocialWeekendSI.Split('+').ToArray();
                    unsocialGrandWeekendSIArray = day.UnsocialGrandWeekendSI.Split('+').ToArray();
                    onCallDaySIArray = day.OnCallDaySI.Split('+').ToArray();
                    onCallNightSIArray = day.OnCallNightSI.Split('+').ToArray();

                    for (int subIdx = 0; subIdx < claim.NumberOfSubAssistants; subIdx++)
                    {
                        hoursSIPerSubAndDay.Add(hoursSIArray);
                        unsocialEveningSIPerSubAndDay.Add(unsocialEveningSIArray);
                        unsocialNightSIPerSubAndDay.Add(unsocialNightSIArray);
                        unsocialWeekendSIPerSubAndDay.Add(unsocialWeekendSIArray);
                        unsocialGrandWeekendSIPerSubAndDay.Add(unsocialGrandWeekendSIArray);
                        onCallDaySIPerSubAndDay.Add(onCallDaySIArray);
                        onCallNightSIPerSubAndDay.Add(onCallNightSIArray);
                        //hoursSIPerSubAndDay[subIdx, dayIdx] = hoursSIArray[subIdx];
                        //unsocialEveningSIPerSubAndDay[subIdx, dayIdx] = unsocialEveningSIArray[subIdx];
                        //unsocialNightSIPerSubAndDay[subIdx, dayIdx] = unsocialNightSIArray[subIdx];
                        //unsocialWeekendSIPerSubAndDay[subIdx, dayIdx] = unsocialWeekendSIArray[subIdx];
                        //unsocialGrandWeekendSIPerSubAndDay[subIdx, dayIdx] = unsocialGrandWeekendSIArray[subIdx];
                        //onCallDaySIPerSubAndDay[subIdx, dayIdx] = onCallDaySIArray[subIdx];
                        //onCallNightSIPerSubAndDay[subIdx, dayIdx] = onCallNightSIArray[subIdx];
                    }
                    dayIdx++;
                }
            }

            //Populate viewmodel properties by iterating over each row in the schedule
            for (int i = 0; i < numberOfDays; i++)
            {
                //Instantiate a new scheduleRow in the viewmodel
                ScheduleRow scheduleRow = new ScheduleRow();

                string[] hoursArray = new string[20];
                string[] unsocialEveningArray = new string[20];
                string[] unsocialNightArray = new string[20];
                string[] unsocialWeekendArray = new string[20];
                string[] unsocialGrandWeekendArray = new string[20];
                string[] onCallDayArray = new string[20];
                string[] onCallNightArray = new string[20];

                scheduleRow.HoursSI = hoursArray;
                scheduleRow.UnsocialEveningSI = unsocialEveningArray;
                scheduleRow.UnsocialNightSI = unsocialNightArray;
                scheduleRow.UnsocialWeekendSI = unsocialWeekendArray;
                scheduleRow.UnsocialGrandWeekendSI = unsocialGrandWeekendArray;
                scheduleRow.OnCallDaySI = onCallDayArray;
                scheduleRow.OnCallNightSI = onCallNightArray;

                //Assign values to the ScheduleRowDate and ScheduleRowWeekDay properties in the viewmodel
                dateInSchedule = claim.FirstClaimDate.AddDays(i);

                CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture = new CultureInfo("sv-SE");

                scheduleRow.ScheduleRowDateString = dateInSchedule.ToString(format: "ddd d MMM");
                scheduleRow.DayDate = dateInSchedule;
                scheduleRow.ScheduleRowWeekDay = DateTimeFormatInfo.CurrentInfo.GetDayName(dateInSchedule.DayOfWeek).ToString().Substring(0, 3);

                if (claim.CompletionStage >= 2)
                {
                    scheduleRow.Well = claimDays[i].Well;
                    scheduleRow.Hours = claimDays[i].Hours;
                    scheduleRow.UnsocialEvening = claimDays[i].UnsocialEvening;
                    scheduleRow.UnsocialNight = claimDays[i].UnsocialNight;
                    scheduleRow.UnsocialWeekend = claimDays[i].UnsocialWeekend;
                    scheduleRow.UnsocialGrandWeekend = claimDays[i].UnsocialGrandWeekend;

                    scheduleRow.OnCallDay = claimDays[i].OnCallDay;
                    scheduleRow.OnCallNight = claimDays[i].OnCallNight;

                    for (int k = 0; k < claim.NumberOfSubAssistants; k++)
                    {
                        scheduleRow.HoursSI[k] = hoursSIPerSubAndDay[i][k];
                        scheduleRow.UnsocialEveningSI[k] = unsocialEveningSIPerSubAndDay[i][k];
                        scheduleRow.UnsocialNightSI[k] = unsocialNightSIPerSubAndDay[i][k];
                        scheduleRow.UnsocialWeekendSI[k] = unsocialWeekendSIPerSubAndDay[i][k];
                        scheduleRow.UnsocialGrandWeekendSI[k] = unsocialGrandWeekendSIPerSubAndDay[i][k];

                        scheduleRow.OnCallDaySI[k] = onCallDaySIPerSubAndDay[i][k];
                        scheduleRow.OnCallNightSI[k] = onCallNightSIPerSubAndDay[i][k];
                    }
                }

                //Test
                //List<string> test = new List<string>();

                //string test2 = "";
                //for (int idx = 0; idx < claim.NumberOfSubAssistants; idx++)
                //{
                //    test.Add(test2);
                //}
                //scheduleRow.Test = test;

                rowList.Add(scheduleRow);
            }
            create2VM.ScheduleRowList = rowList;

            return create2VM;
        }

        // POST: Claims/Create2
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Ombud")]
        public ActionResult Create2(Create2VM create2VM, string refNumber, string submitButton)
        {
            if (ModelState.IsValid)
            {

                int idx = 0;
                foreach (var row in create2VM.ScheduleRowList)
                {
                    //For some reason these properties appear in the model validation even though they are not used and should therefore not be causing a validation error.
                    //Since these non-array properties are not used in any way in the action/model, they are removed from the validation.
                    //Array properties with the same names are used and also validated.
                    ModelState.Remove("ScheduleRowList[" + idx.ToString() + "].HoursSI");
                    ModelState.Remove("ScheduleRowList[" + idx.ToString() + "].UnsocialEveningSI");
                    ModelState.Remove("ScheduleRowList[" + idx.ToString() + "].UnsocialNightSI");
                    ModelState.Remove("ScheduleRowList[" + idx.ToString() + "].UnsocialWeekendSI");
                    ModelState.Remove("ScheduleRowList[" + idx.ToString() + "].UnsocialGrandWeekendSI");
                    ModelState.Remove("ScheduleRowList[" + idx.ToString() + "].OnCallDaySI");
                    ModelState.Remove("ScheduleRowList[" + idx.ToString() + "].OnCallNightSI");

                    //Set all empty hour properties to zero
                    if (row.Hours == "")
                    {
                        row.Hours = "0";
                    }
                    if (row.UnsocialEvening == "")
                    {
                        row.UnsocialEvening = "0";
                    }
                    if (row.UnsocialNight == "")
                    {
                        row.UnsocialNight = "0";
                    }
                    if (row.UnsocialWeekend == "")
                    {
                        row.UnsocialWeekend = "0";
                    }
                    if (row.UnsocialGrandWeekend == "")
                    {
                        row.UnsocialGrandWeekend = "0";
                    }
                    if (row.OnCallDay == "")
                    {
                        row.OnCallDay = "0";
                    }
                    if (row.OnCallNight == "")
                    {
                        row.OnCallNight = "0";
                    }

                    for (int i = 0; i < create2VM.NumberOfSubAssistants; i++)
                    {
                        if (row.HoursSI[i] == "")
                        {
                            row.HoursSI[i] = "0";
                        }
                        if (row.UnsocialEveningSI[i] == "")
                        {
                            row.UnsocialEveningSI[i] = "0";
                        }
                        if (row.UnsocialNightSI[i] == "")
                        {
                            row.UnsocialNightSI[i] = "0";
                        }
                        if (row.UnsocialWeekendSI[i] == "")
                        {
                            row.UnsocialWeekendSI[i] = "0";
                        }
                        if (row.UnsocialGrandWeekendSI[i] == "")
                        {
                            row.UnsocialGrandWeekendSI[i] = "0";
                        }
                        if (row.OnCallDaySI[i] == "")
                        {
                            row.OnCallDaySI[i] = "0";
                        }
                        if (row.OnCallNightSI[i] == "")
                        {
                            row.OnCallNightSI[i] = "0";
                        }
                    }
                    idx++;
                }

                idx = 0;
                //Check that each entry has a correct format
                Regex regex = new Regex(@"^([0-9]|1[0-9]|2[0-5])?([,]|[,][0-9]|[,][0-9][0-9])?$");
                //Regex regex = new Regex(@"\d{0,2}(\,\d{0,2})?$");
                foreach (var row in create2VM.ScheduleRowList)
                {
                    if (row.Hours != null)
                    {
                        Match match = regex.Match(row.Hours);
                        if (!match.Success)
                        {
                            ModelState.AddModelError("ScheduleRowList[" + idx.ToString() + "].Hours", "Ogiltigt format eller värde.");
                        }
                    }
                    if (row.UnsocialEvening != null)
                    {
                        Match match = regex.Match(row.UnsocialEvening);
                        if (!match.Success)
                        {
                            ModelState.AddModelError("ScheduleRowList[" + idx.ToString() + "].UnsocialEvening", "Ogiltigt format eller värde.");
                        }
                    }
                    if (row.UnsocialNight != null)
                    {
                        Match match = regex.Match(row.UnsocialNight);
                        if (!match.Success)
                        {
                            ModelState.AddModelError("ScheduleRowList[" + idx.ToString() + "].UnsocialNight", "Ogiltigt format eller värde.");
                        }
                    }
                    if (row.UnsocialWeekend != null)
                    {
                        Match match = regex.Match(row.UnsocialWeekend);
                        if (!match.Success)
                        {
                            ModelState.AddModelError("ScheduleRowList[" + idx.ToString() + "].UnsocialWeekend", "Ogiltigt format eller värde.");
                        }
                    }
                    if (row.UnsocialGrandWeekend != null)
                    {
                        Match match = regex.Match(row.UnsocialGrandWeekend);
                        if (!match.Success)
                        {
                            ModelState.AddModelError("ScheduleRowList[" + idx.ToString() + "].UnsocialGrandWeekend", "Ogiltigt format eller värde.");
                        }
                    }
                    if (row.OnCallDay != null)
                    {
                        Match match = regex.Match(row.OnCallDay);
                        if (!match.Success)
                        {
                            ModelState.AddModelError("ScheduleRowList[" + idx.ToString() + "].OnCallDay", "Ogiltigt format eller värde.");
                        }
                    }
                    if (row.OnCallNight != null)
                    {
                        Match match = regex.Match(row.OnCallNight);
                        if (!match.Success)
                        {
                            ModelState.AddModelError("ScheduleRowList[" + idx.ToString() + "].OnCallNight", "Ogiltigt format eller värde.");
                        }
                    }

                    for (int i = 0; i < create2VM.NumberOfSubAssistants; i++)
                    {
                        if (row.HoursSI[i] != null)
                        {
                            Match match = regex.Match(row.HoursSI[i]);
                            if (!match.Success)
                            {
                                ModelState.AddModelError("ScheduleRowList[" + idx.ToString() + "].HoursSI[" + i.ToString() + "]", "Ogiltigt format eller värde.");
                            }
                        }
                    }
                    for (int i = 0; i < create2VM.NumberOfSubAssistants; i++)
                    {
                        if (row.UnsocialEveningSI[i] != null)
                        {
                            Match match = regex.Match(row.UnsocialEveningSI[i]);
                            if (!match.Success)
                            {
                                ModelState.AddModelError("ScheduleRowList[" + idx.ToString() + "].UnsocialEveningSI[" + i.ToString() + "]", "Ogiltigt format eller värde.");
                            }
                        }
                    }
                    for (int i = 0; i < create2VM.NumberOfSubAssistants; i++)
                    {
                        if (row.UnsocialNightSI[i] != null)
                        {
                            Match match = regex.Match(row.UnsocialNightSI[i]);
                            if (!match.Success)
                            {
                                ModelState.AddModelError("ScheduleRowList[" + idx.ToString() + "].UnsocialNightSI[" + i.ToString() + "]", "Ogiltigt format eller värde.");
                            }
                        }
                    }
                    for (int i = 0; i < create2VM.NumberOfSubAssistants; i++)
                    {
                        if (row.UnsocialWeekendSI[i] != null)
                        {
                            Match match = regex.Match(row.UnsocialWeekendSI[i]);
                            if (!match.Success)
                            {
                                ModelState.AddModelError("ScheduleRowList[" + idx.ToString() + "].UnsocialWeekendSI[" + i.ToString() + "]", "Ogiltigt format eller värde.");
                            }
                        }
                    }
                    for (int i = 0; i < create2VM.NumberOfSubAssistants; i++)
                    {
                        if (row.UnsocialGrandWeekendSI[i] != null)
                        {
                            Match match = regex.Match(row.UnsocialGrandWeekendSI[i]);
                            if (!match.Success)
                            {
                                ModelState.AddModelError("ScheduleRowList[" + idx.ToString() + "].UnsocialGrandWeekendSI[" + i.ToString() + "]", "Ogiltigt format eller värde.");
                            }
                        }
                    }
                    for (int i = 0; i < create2VM.NumberOfSubAssistants; i++)
                    {
                        if (row.OnCallDaySI[i] != null)
                        {
                            Match match = regex.Match(row.OnCallDaySI[i]);
                            if (!match.Success)
                            {
                                ModelState.AddModelError("ScheduleRowList[" + idx.ToString() + "].OnCallDaySI[" + i.ToString() + "]", "Ogiltigt format eller värde.");
                            }
                        }
                    }
                    for (int i = 0; i < create2VM.NumberOfSubAssistants; i++)
                    {
                        if (row.OnCallNightSI[i] != null)
                        {
                            Match match = regex.Match(row.OnCallNightSI[i]);
                            if (!match.Success)
                            {
                                ModelState.AddModelError("ScheduleRowList[" + idx.ToString() + "].OnCallNightSI[" + i.ToString() + "]", "Ogiltigt format eller värde.");
                            }
                        }
                    }
                    idx++;
                }

                idx = 0;
                if (ModelState.IsValid)
                {
                    //Check that no day has more than 25 hours of work
                    foreach (var row in create2VM.ScheduleRowList)
                    {
                        if (Convert.ToDecimal(create2VM.ScheduleRowList[idx].Hours) + Convert.ToDecimal(create2VM.ScheduleRowList[idx].OnCallDay) + Convert.ToDecimal(create2VM.ScheduleRowList[idx].OnCallNight) > 25)
                        {
                            ModelState.AddModelError("ScheduleRowList[" + idx.ToString() + "].Hours", "För många timmar.");
                        }
                        for (int i = 0; i < create2VM.NumberOfSubAssistants; i++)
                        {
                            if (Convert.ToDecimal(create2VM.ScheduleRowList[idx].HoursSI[i]) + Convert.ToDecimal(create2VM.ScheduleRowList[idx].OnCallDaySI[i]) + Convert.ToDecimal(create2VM.ScheduleRowList[idx].OnCallNightSI[i]) > 25)
                            {
                                ModelState.AddModelError("ScheduleRowList[" + idx.ToString() + "].HoursSI[" + i.ToString() + "]", "För många timmar.");
                            }
                        }
                        idx++;
                    }

                    //Check that no single item has more than 25 hours
                    idx = 0;
                    foreach (var row in create2VM.ScheduleRowList)
                    {
                        if (Convert.ToDecimal(create2VM.ScheduleRowList[idx].Hours) > 25)
                        {
                            ModelState.AddModelError("ScheduleRowList[" + idx.ToString() + "].Hours", "För många timmar.");
                        }
                        if (Convert.ToDecimal(create2VM.ScheduleRowList[idx].UnsocialEvening) > 25)
                        {
                            ModelState.AddModelError("ScheduleRowList[" + idx.ToString() + "].UnsocialEvening", "För många timmar.");
                        }
                        if (Convert.ToDecimal(create2VM.ScheduleRowList[idx].UnsocialNight) > 25)
                        {
                            ModelState.AddModelError("ScheduleRowList[" + idx.ToString() + "].UnsocialNight", "För många timmar.");
                        }
                        if (Convert.ToDecimal(create2VM.ScheduleRowList[idx].UnsocialWeekend) > 25)
                        {
                            ModelState.AddModelError("ScheduleRowList[" + idx.ToString() + "].UnsocialWeekend", "För många timmar.");
                        }
                        if (Convert.ToDecimal(create2VM.ScheduleRowList[idx].UnsocialGrandWeekend) > 25)
                        {
                            ModelState.AddModelError("ScheduleRowList[" + idx.ToString() + "].UnsocialGrandWeekend", "För många timmar.");
                        }
                        if (Convert.ToDecimal(create2VM.ScheduleRowList[idx].OnCallDay) > 25)
                        {
                            ModelState.AddModelError("ScheduleRowList[" + idx.ToString() + "].OnCallDay", "För många timmar.");
                        }
                        if (Convert.ToDecimal(create2VM.ScheduleRowList[idx].OnCallNight) > 25)
                        {
                            ModelState.AddModelError("ScheduleRowList[" + idx.ToString() + "].OnCallNight", "För många timmar.");
                        }
                        for (int i = 0; i < create2VM.NumberOfSubAssistants; i++)
                        {
                            if (Convert.ToDecimal(create2VM.ScheduleRowList[idx].HoursSI[i]) > 25)
                            {
                                ModelState.AddModelError("ScheduleRowList[" + idx.ToString() + "].HoursSI[" + i.ToString() + "]", "För många timmar.");
                            }
                        }
                        for (int i = 0; i < create2VM.NumberOfSubAssistants; i++)
                        {
                            if (Convert.ToDecimal(create2VM.ScheduleRowList[idx].UnsocialEveningSI[i]) > 25)
                            {
                                ModelState.AddModelError("ScheduleRowList[" + idx.ToString() + "].UnsocialEveningSI[" + i.ToString() + "]", "För många timmar.");
                            }
                        }
                        for (int i = 0; i < create2VM.NumberOfSubAssistants; i++)
                        {
                            if (Convert.ToDecimal(create2VM.ScheduleRowList[idx].UnsocialNightSI[i]) > 25)
                            {
                                ModelState.AddModelError("ScheduleRowList[" + idx.ToString() + "].UnsocialNightSI[" + i.ToString() + "]", "För många timmar.");
                            }
                        }
                        for (int i = 0; i < create2VM.NumberOfSubAssistants; i++)
                        {
                            if (Convert.ToDecimal(create2VM.ScheduleRowList[idx].UnsocialWeekendSI[i]) > 25)
                            {
                                ModelState.AddModelError("ScheduleRowList[" + idx.ToString() + "].UnsocialWeekendSI[" + i.ToString() + "]", "För många timmar.");
                            }
                        }
                        for (int i = 0; i < create2VM.NumberOfSubAssistants; i++)
                        {
                            if (Convert.ToDecimal(create2VM.ScheduleRowList[idx].UnsocialGrandWeekendSI[i]) > 25)
                            {
                                ModelState.AddModelError("ScheduleRowList[" + idx.ToString() + "].UnsocialGrandWeekendSI[" + i.ToString() + "]", "För många timmar.");
                            }
                        }
                        for (int i = 0; i < create2VM.NumberOfSubAssistants; i++)
                        {
                            if (Convert.ToDecimal(create2VM.ScheduleRowList[idx].OnCallDaySI[i]) > 25)
                            {
                                ModelState.AddModelError("ScheduleRowList[" + idx.ToString() + "].OnCallDaySI[" + i.ToString() + "]", "För många timmar.");
                            }
                        }
                        for (int i = 0; i < create2VM.NumberOfSubAssistants; i++)
                        {
                            if (Convert.ToDecimal(create2VM.ScheduleRowList[idx].OnCallNightSI[i]) > 25)
                            {
                                ModelState.AddModelError("ScheduleRowList[" + idx.ToString() + "].OnCallNightSI[" + i.ToString() + "]", "För många timmar.");
                            }
                        }
                        idx++;
                    }

                    //Check that the sum of hours for all substitute assistants is not greater than the number of hours for the regular assistant for each day
                    int numberOfCalendarDays = create2VM.ScheduleRowList.Count();
                    int numberOfSubAssistants = create2VM.NumberOfSubAssistants;
                    decimal[] subSum = new decimal[7];
                    for (int dayIdx = 0; dayIdx < numberOfCalendarDays; dayIdx++)
                    {
                        for (int i = 0; i < 7; i++)
                        {
                            subSum[i] = 0;
                        }
                        for (int subIdx = 0; subIdx < create2VM.NumberOfSubAssistants; subIdx++)
                        {
                            subSum[0] += Convert.ToDecimal(create2VM.ScheduleRowList[dayIdx].HoursSI[subIdx]);
                            subSum[1] += Convert.ToDecimal(create2VM.ScheduleRowList[dayIdx].UnsocialEveningSI[subIdx]);
                            subSum[2] += Convert.ToDecimal(create2VM.ScheduleRowList[dayIdx].UnsocialNightSI[subIdx]);
                            subSum[3] += Convert.ToDecimal(create2VM.ScheduleRowList[dayIdx].UnsocialWeekendSI[subIdx]);
                            subSum[4] += Convert.ToDecimal(create2VM.ScheduleRowList[dayIdx].UnsocialGrandWeekendSI[subIdx]);
                            subSum[5] += Convert.ToDecimal(create2VM.ScheduleRowList[dayIdx].OnCallDaySI[subIdx]);
                            subSum[6] += Convert.ToDecimal(create2VM.ScheduleRowList[dayIdx].OnCallNightSI[subIdx]);
                        }

                        if (subSum[0] > Convert.ToDecimal(create2VM.ScheduleRowList[dayIdx].Hours))
                        {
                            ModelState.AddModelError("ScheduleRowList[" + dayIdx.ToString() + "].Hours", "För många vikarietimmar.");
                        }
                        if (subSum[1] > Convert.ToDecimal(create2VM.ScheduleRowList[dayIdx].UnsocialEvening))
                        {
                            ModelState.AddModelError("ScheduleRowList[" + dayIdx.ToString() + "].UnsocialEvening", "För många vikarietimmar.");
                        }
                        if (subSum[2] > Convert.ToDecimal(create2VM.ScheduleRowList[dayIdx].UnsocialNight))
                        {
                            ModelState.AddModelError("ScheduleRowList[" + dayIdx.ToString() + "].UnsocialNight", "För många vikarietimmar.");
                        }
                        if (subSum[3] > Convert.ToDecimal(create2VM.ScheduleRowList[dayIdx].UnsocialWeekend))
                        {
                            ModelState.AddModelError("ScheduleRowList[" + dayIdx.ToString() + "].UnsocialWeekend", "För många vikarietimmar.");
                        }
                        if (subSum[4] > Convert.ToDecimal(create2VM.ScheduleRowList[dayIdx].UnsocialGrandWeekend))
                        {
                            ModelState.AddModelError("ScheduleRowList[" + dayIdx.ToString() + "].UnsocialGrandWeekend", "För många vikarietimmar.");
                        }
                        if (subSum[5] > Convert.ToDecimal(create2VM.ScheduleRowList[dayIdx].OnCallDay))
                        {
                            ModelState.AddModelError("ScheduleRowList[" + dayIdx.ToString() + "].OnCallDay", "För många vikarietimmar.");
                        }
                        if (subSum[6] > Convert.ToDecimal(create2VM.ScheduleRowList[dayIdx].OnCallNight))
                        {
                            ModelState.AddModelError("ScheduleRowList[" + dayIdx.ToString() + "].OnCallNight", "För många vikarietimmar.");
                        }
                    }

                    //Check that there are not more unsocial hours than working hours for each day
                    idx = 0;
                    foreach (var row in create2VM.ScheduleRowList)
                    {
                        if (Convert.ToDecimal(create2VM.ScheduleRowList[idx].UnsocialEvening) + Convert.ToDecimal(create2VM.ScheduleRowList[idx].UnsocialNight) + Convert.ToDecimal(create2VM.ScheduleRowList[idx].UnsocialWeekend) + Convert.ToDecimal(create2VM.ScheduleRowList[idx].UnsocialGrandWeekend) > Convert.ToDecimal(create2VM.ScheduleRowList[idx].Hours))
                        {
                            ModelState.AddModelError("ScheduleRowList[" + idx.ToString() + "].Hours", "För många OB-timmar.");
                        }
                        for (int i = 0; i < create2VM.NumberOfSubAssistants; i++)
                        {
                            if (Convert.ToDecimal(create2VM.ScheduleRowList[idx].UnsocialEveningSI[i]) + Convert.ToDecimal(create2VM.ScheduleRowList[idx].UnsocialNightSI[i]) + Convert.ToDecimal(create2VM.ScheduleRowList[idx].UnsocialWeekendSI[i]) + Convert.ToDecimal(create2VM.ScheduleRowList[idx].UnsocialGrandWeekendSI[i]) > Convert.ToDecimal(create2VM.ScheduleRowList[idx].HoursSI[i]))
                            {
                                ModelState.AddModelError("ScheduleRowList[" + idx.ToString() + "].HoursSI[" + i.ToString() + "]", "För många OB-timmar.");
                            }
                        }
                        idx++;
                    }

                    //Check that some working hours have been filled in for the regular and substitute assistants
                    bool hoursFound = false;
                    idx = 0;
                    do
                    {
                        if (!string.IsNullOrEmpty(create2VM.ScheduleRowList[idx].Hours) || !string.IsNullOrEmpty(create2VM.ScheduleRowList[idx].OnCallDay) || !string.IsNullOrEmpty(create2VM.ScheduleRowList[idx].OnCallNight))
                        {
                            hoursFound = true;
                        }
                        idx++;
                    } while (!hoursFound && idx < create2VM.ScheduleRowList.Count());
                    if (!hoursFound)
                    {
                        ModelState.AddModelError("ScheduleRowList[0].Hours", "Inga timmar ifyllda.");
                    }
                    for (int i = 0; i < create2VM.NumberOfSubAssistants; i++)
                    {
                        bool hoursSIFound = false;
                        idx = 0;
                        do
                        {
                            if (!string.IsNullOrEmpty(create2VM.ScheduleRowList[idx].HoursSI[i]) || !string.IsNullOrEmpty(create2VM.ScheduleRowList[idx].OnCallDaySI[i]) || !string.IsNullOrEmpty(create2VM.ScheduleRowList[idx].OnCallNightSI[i]))
                            {
                                hoursSIFound = true;
                            }
                            idx++;
                        } while (!hoursSIFound && idx < create2VM.ScheduleRowList.Count());
                        if (!hoursSIFound)
                        {
                            ModelState.AddModelError("ScheduleRowList[0].HoursSI[" + i.ToString() + "]", "Inga timmar ifyllda.");
                        }
                    }
                }

            }

            if (ModelState.IsValid)
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
                    var claim = db.Claims.Where(c => c.ReferenceNumber == refNumber).FirstOrDefault();
                    create2VM = LoadClaimCreate2VM(claim);
                    return View(create2VM);
                }
            }
            else
            {
                var claim = db.Claims.Where(c => c.ReferenceNumber == refNumber).FirstOrDefault();
                create2VM = LoadClaimCreate2VM(claim);
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
            decimal numberOfOrdinaryHours = 0;
            decimal numberOfUnsocialHours = 0;
            decimal numberOfOnCallHours = 0;
            decimal numberOfAbsenceHoursWithSI = 0;
            decimal numberOfOrdinaryHoursWithSI = 0;
            decimal numberOfUnsocialHoursSI = 0;
            decimal numberOfOnCallHoursSI = 0;

            DateTime claimDate = claim.FirstClaimDate;

            string numberOfHoursWithSIConcat = "";
            string numberOfOrdinaryHoursSIConcat = "";
            string numberOfUnsocialHoursSIConcat = "";
            string numberOfOnCallHoursSIConcat = "";

            decimal[] numberOfHoursWithSIArray = new decimal[20];
            decimal[] numberOfOrdinaryHoursSIArray = new decimal[20];
            decimal[] numberOfUnsocialHoursSIArray = new decimal[20];
            decimal[] numberOfOnCallHoursSIArray = new decimal[20];

            string HoursSIConcat = "";
            string UnsocialEveningSIConcat = "";
            string UnsocialNightSIConcat = "";
            string UnsocialWeekendSIConcat = "";
            string UnsocialGrandWeekendSIConcat = "";
            string OnCallDaySIConcat = "";
            string OnCallNightSIConcat = "";

            int sickDayCounter = 1;
            int qdIdx = 0;
            int day2Idx = 0;
            int day14Idx = 0;
            int day15Idx = 0;
            int lastDayIdx = 0;

            int dayIdx = 0;
            foreach (var day in create2VM.ScheduleRowList)
            {
                for (int i = 0; i < create2VM.NumberOfSubAssistants; i++)
                {
                    HoursSIConcat += day.HoursSI[i] + "+";
                    UnsocialEveningSIConcat += day.UnsocialEveningSI[i] + "+";
                    UnsocialNightSIConcat += day.UnsocialNightSI[i] + "+";
                    UnsocialWeekendSIConcat += day.UnsocialWeekendSI[i] + "+";
                    UnsocialGrandWeekendSIConcat += day.UnsocialGrandWeekendSI[i] + "+";
                    OnCallDaySIConcat += day.OnCallDaySI[i] + "+";
                    OnCallNightSIConcat += day.OnCallNightSI[i] + "+";
                }

                var claimDay = new ClaimDay
                {
                    ReferenceNumber = create2VM.ReferenceNumber,
                    DateString = day.ScheduleRowDateString,
                    Date = claimDate.AddDays(dayIdx),
                    CalendarDayNumber = dayIdx + 1,
                    Well = day.Well,
                    Hours = day.Hours,
                    UnsocialEvening = day.UnsocialEvening,
                    UnsocialNight = day.UnsocialNight,
                    UnsocialWeekend = day.UnsocialWeekend,
                    UnsocialGrandWeekend = day.UnsocialGrandWeekend,
                    OnCallDay = day.OnCallDay,
                    OnCallNight = day.OnCallNight,
                    HoursSI = HoursSIConcat,
                    UnsocialEveningSI = UnsocialEveningSIConcat,
                    UnsocialNightSI = UnsocialNightSIConcat,
                    UnsocialWeekendSI = UnsocialWeekendSIConcat,
                    UnsocialGrandWeekendSI = UnsocialGrandWeekendSIConcat,
                    OnCallDaySI = OnCallDaySIConcat,
                    OnCallNightSI = OnCallNightSIConcat
                };
                if (claimDay.Well)
                {
                    claimDay.SickDayNumber = null;
                }
                else
                {
                    switch (sickDayCounter)
                    {
                        case 1:
                            qdIdx = dayIdx;
                            break;
                        case 2:
                            day2Idx = dayIdx;
                            break;
                        case 14:
                            day14Idx = dayIdx;
                            break;
                        case 15:
                            day15Idx = dayIdx;
                            break;
                        default:
                            break;
                    }
                    claimDay.SickDayNumber = sickDayCounter;
                    lastDayIdx = dayIdx;
                    sickDayCounter++;
                }
                db.ClaimDays.Add(claimDay);

                claim.NumberOfSickDays = sickDayCounter - 1;
                claim.AdjustedNumberOfSickDays = sickDayCounter - 1;

                numberOfOrdinaryHours = numberOfOrdinaryHours + Convert.ToDecimal(day.Hours);
                numberOfUnsocialHours = numberOfUnsocialHours + Convert.ToDecimal(day.UnsocialEvening) + Convert.ToDecimal(day.UnsocialNight) + Convert.ToDecimal(day.UnsocialWeekend) + Convert.ToDecimal(day.UnsocialGrandWeekend);
                numberOfOnCallHours = numberOfOnCallHours + Convert.ToDecimal(day.OnCallDay) + Convert.ToDecimal(day.OnCallNight);
                numberOfAbsenceHours = numberOfAbsenceHours + Convert.ToDecimal(day.Hours) + Convert.ToDecimal(day.OnCallDay) + Convert.ToDecimal(day.OnCallNight);

                //numberOfOrdinaryHoursWithSI = numberOfOrdinaryHoursWithSI + Convert.ToDecimal(day.HoursSI);
                //numberOfUnsocialHoursSI = numberOfUnsocialHoursSI + Convert.ToDecimal(day.UnsocialEveningSI) + Convert.ToDecimal(day.UnsocialNightSI) + Convert.ToDecimal(day.UnsocialWeekendSI) + Convert.ToDecimal(day.UnsocialGrandWeekendSI);
                //numberOfOnCallHoursSI = numberOfOnCallHoursSI + Convert.ToDecimal(day.OnCallDaySI) + Convert.ToDecimal(day.OnCallNightSI);
                //numberOfAbsenceHoursWithSI = numberOfAbsenceHoursWithSI + Convert.ToDecimal(day.HoursSI) + Convert.ToDecimal(day.OnCallDaySI) + Convert.ToDecimal(day.OnCallNightSI);

                //The variables below are required in order to be able to handle more than one substitute assistants
                for (int i = 0; i < create2VM.NumberOfSubAssistants; i++)
                {
                    numberOfOrdinaryHoursSIArray[i] = numberOfOrdinaryHoursSIArray[i] + Convert.ToDecimal(day.HoursSI[i]);
                    numberOfUnsocialHoursSIArray[i] = numberOfUnsocialHoursSIArray[i] + Convert.ToDecimal(day.UnsocialEveningSI[i]) + Convert.ToDecimal(day.UnsocialNightSI[i]) + Convert.ToDecimal(day.UnsocialWeekendSI[i]) + Convert.ToDecimal(day.UnsocialGrandWeekendSI[i]);
                    numberOfOnCallHoursSIArray[i] = numberOfOnCallHoursSIArray[i] + Convert.ToDecimal(day.OnCallDaySI[i]) + Convert.ToDecimal(day.OnCallNightSI[i]);
                    numberOfHoursWithSIArray[i] = numberOfHoursWithSIArray[i] + Convert.ToDecimal(day.HoursSI[i]) + Convert.ToDecimal(day.OnCallDaySI[i]) + Convert.ToDecimal(day.OnCallNightSI[i]);
                }

                HoursSIConcat = "";
                UnsocialEveningSIConcat = "";
                UnsocialNightSIConcat = "";
                UnsocialWeekendSIConcat = "";
                UnsocialGrandWeekendSIConcat = "";
                OnCallDaySIConcat = "";
                OnCallNightSIConcat = "";
                dayIdx++;
            }

            db.SaveChanges();
            var claimDays = db.ClaimDays.Where(c => c.ReferenceNumber == create2VM.ReferenceNumber).ToList();
            //Identify qd, day2, day14, day15 and last day of sickness
            claim.QualifyingDayDate = claimDays[qdIdx].Date;
            claim.QualifyingDayDateAsString = claimDays[qdIdx].Date.ToShortDateString();

            if (day2Idx != 0)
            {
                claim.Day2OfSicknessDate = claimDays[day2Idx].Date;
                claim.Day2OfSicknessDateAsString = claimDays[day2Idx].Date.ToShortDateString();
            }
            if (day14Idx != 0)
            {
                claim.Day14OfSicknessDate = claimDays[day14Idx].Date;
                claim.Day14OfSicknessDateAsString = claimDays[day14Idx].Date.ToShortDateString();
            }
            if (day15Idx != 0)
            {
                claim.Day15OfSicknessDate = claimDays[day15Idx].Date;
                claim.Day15OfSicknessDateAsString = claimDays[day15Idx].Date.ToShortDateString();
            }
            claim.LastDayOfSicknessDate = claimDays[lastDayIdx].Date;
            claim.LastDayOfSicknessDateAsString = claimDays[lastDayIdx].Date.ToShortDateString();

            claim.NumberOfAbsenceHours = numberOfAbsenceHours;
            claim.NumberOfOrdinaryHours = numberOfOrdinaryHours;
            claim.NumberOfUnsocialHours = numberOfUnsocialHours;
            claim.NumberOfOnCallHours = numberOfOnCallHours;

            //claim.NumberOfHoursWithSI = numberOfAbsenceHoursWithSI;
            //claim.NumberOfOrdinaryHoursSI = numberOfOrdinaryHoursWithSI;
            //claim.NumberOfUnsocialHoursSI = numberOfUnsocialHoursSI;
            //claim.NumberOfOnCallHoursSI = numberOfOnCallHoursSI;

            for (int i = 0; i < create2VM.NumberOfSubAssistants; i++)
            {
                numberOfHoursWithSIConcat += numberOfHoursWithSIArray[i] + "+";
                numberOfOrdinaryHoursSIConcat += numberOfOrdinaryHoursSIArray[i] + "+";
                numberOfUnsocialHoursSIConcat += numberOfUnsocialHoursSIArray[i] + "+";
                numberOfOnCallHoursSIConcat += numberOfOnCallHoursSIArray[i] + "+";
            }

            claim.NumberOfHoursWithSIConcat = numberOfHoursWithSIConcat;
            claim.NumberOfOrdinaryHoursSIConcat = numberOfOrdinaryHoursSIConcat;
            claim.NumberOfUnsocialHoursSIConcat = numberOfUnsocialHoursSIConcat;
            claim.NumberOfOnCallHoursSIConcat = numberOfOnCallHoursSIConcat;




            if (claim.CompletionStage < 2)
            {
                claim.CompletionStage = 2;
            }
            db.Entry(claim).State = EntityState.Modified;
            db.SaveChanges();
        }

        // GET: Claims/Create3
        [HttpGet]
        [Authorize(Roles = "Ombud")]
        public ActionResult Create3(string refNumber)
        {
            Create3VM create3VM = new Create3VM();
            var claim = db.Claims.Where(c => c.ReferenceNumber == refNumber).FirstOrDefault();

            if (!demoMode || (demoMode && claim.CompletionStage >= 3)) //CompletionStage >= 3 means that stage 3 has been filled in earlier. This is an update
            {
                create3VM = LoadClaimCreate3VM(claim);
            }
            else if (!demoMode)    // This option was used before calculated amounts were filled in in the view
            {
                create3VM = LoadNewClaimCreate3VM(claim);
            }
            else //demoMode == true, Detta fall gäller ej Helsingborg 
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
            if (claim.CompletionStage >= 3) //CompletionStage >= 3 means that stage 3 has been filled in earlier. This is an update of stage 3
            {
                create3VM.SickPay = String.Format("{0:0.00}", claim.ClaimedSickPay);
                create3VM.HolidayPay = String.Format("{0:0.00}", claim.ClaimedHolidayPay);
                create3VM.SocialFees = String.Format("{0:0.00}", claim.ClaimedSocialFees);
                create3VM.PensionAndInsurance = String.Format("{0:0.00}", claim.ClaimedPensionAndInsurance);
                create3VM.ClaimSum = String.Format("{0:0.00}", claim.ClaimedSum);
            }
            else if (claim.CompletionStage < 3) // stage 3 has not been filled in earlier. Show calculated values according to Collective Agreement
            {
                decimal totalSickPayCalc = 0;
                decimal totalHolidayPayCalc = 0;
                decimal totalSocialFeesCalc = 0;
                decimal totalPensionAndInsuranceCalc = 0;

                //Calculate the model sum
                List<ClaimDay> claimDays = new List<ClaimDay>();
                claimDays = db.ClaimDays.Where(c => c.ReferenceNumber == create3VM.ClaimNumber).OrderBy(c => c.CalendarDayNumber).ToList();
                if (claimDays.Count() > 0)
                {
                    CalculateModelSum(claim, claimDays, null, null);
                }

                var claimCalculations = db.ClaimCalculations.Where(c => c.ReferenceNumber == claim.ReferenceNumber).OrderBy(c => c.StartDate).ToList();
                List<ClaimCalculation> claimCalcs = new List<ClaimCalculation>();

                for (int i = 0; i < claimCalculations.Count(); i++)
                {
                    if (i == 0)
                    {
                        //QUALIFYING DAY
                        totalSickPayCalc += Convert.ToDecimal(claimCalculations[i].SickPayQD);
                        totalHolidayPayCalc += Convert.ToDecimal(claimCalculations[i].HolidayPayQD);
                        totalSocialFeesCalc += Convert.ToDecimal(claimCalculations[i].SocialFeesQD);
                        totalPensionAndInsuranceCalc += Convert.ToDecimal(claimCalculations[i].PensionAndInsuranceQD);
                    }
                    //DAY 2 TO DAY 14
                    totalSickPayCalc += Convert.ToDecimal(claimCalculations[i].SickPayD2T14);
                    totalHolidayPayCalc += Convert.ToDecimal(claimCalculations[i].HolidayPayD2T14);
                    totalSocialFeesCalc += Convert.ToDecimal(claimCalculations[i].SocialFeesD2T14);
                    totalPensionAndInsuranceCalc += Convert.ToDecimal(claimCalculations[i].PensionAndInsuranceD2T14);
                }

                //Calculated values according to Collective Agreement should be shown in the View 
                create3VM.SickPay = String.Format("{0:0.00}", totalSickPayCalc);
                create3VM.HolidayPay = String.Format("{0:0.00}", totalHolidayPayCalc);
                create3VM.SocialFees = String.Format("{0:0.00}", totalSocialFeesCalc);
                create3VM.PensionAndInsurance = String.Format("{0:0.00}", totalPensionAndInsuranceCalc);
                create3VM.ClaimSum = String.Format("{0:0.00}", claim.TotalCostD1T14);
                create3VM.ShowCalculatedValues = true;
            }
            else   // This should never happen ?
            {
                create3VM.SickPay = "00,00";
                create3VM.HolidayPay = "00,00";
                create3VM.SocialFees = "00,00";
                create3VM.PensionAndInsurance = "00,00";
                create3VM.ClaimSum = "00,00";
            }
            return create3VM;
        }

        private Create3VM LoadDemoClaimCreate3VM(string refNumber)
        {
            Create3VM create3VM = new Create3VM();
            create3VM.ClaimNumber = refNumber;

            var claimDays = db.ClaimDays.Where(c => c.ReferenceNumber == refNumber).OrderBy(c => c.CalendarDayNumber).ToList();

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
        [Authorize(Roles = "Ombud")]
        public ActionResult Create3(Create3VM create3VM, string refNumber, string submitButton)
        {
            if (Convert.ToDecimal(create3VM.SickPay) == 0 && Convert.ToDecimal(create3VM.HolidayPay) == 0 && Convert.ToDecimal(create3VM.SocialFees) == 0 && Convert.ToDecimal(create3VM.PensionAndInsurance) == 0)
            {
                ModelState.AddModelError("ClaimSum", "Minst ett av beloppen måste fyllas i.");
            }

            if (Convert.ToDecimal(create3VM.SickPay) + Convert.ToDecimal(create3VM.HolidayPay) + Convert.ToDecimal(create3VM.SocialFees) + Convert.ToDecimal(create3VM.PensionAndInsurance) > 40000)
            {
                ModelState.AddModelError("ClaimSum", "För högt yrkat belopp.");
            }

            if (ModelState.IsValid)
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
            else
            {
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
        [Authorize(Roles = "Ombud")]
        public ActionResult Create4(string ClaimNumber)
        {
            //This get action needs to be updated to handle the case where attachments have been added to the claim but the claim was only saved with attachments, not submitted.
            var VM = new Create4VM();

            VM.ClaimNumber = ClaimNumber;
            var claim = db.Claims.Where(c => c.ReferenceNumber == ClaimNumber).FirstOrDefault();
            VM.NumberOfCalendarDays = claim.NumberOfCalendarDays;
            VM.NumberOfSickDays = claim.NumberOfSickDays;
            VM.NumberOfSubAssistants = claim.NumberOfSubAssistants;

            VM.SalaryAttachmentExists = CheckExistingDocument(claim, "SalaryAttachment");
            VM.TimeReportExists = CheckExistingDocument(claim, "TimeReport");
            VM.DoctorsCertificateExists = (CheckExistingDocument(claim, "DoctorsCertificate") && claim.NumberOfSickDays > 7);

            VM.TimeReportStandInExists = new List<bool>();

            for (int idx = 0; idx < VM.NumberOfSubAssistants; idx++)
            {
                VM.TimeReportStandInExists.Insert(idx, CheckExistingDocument(claim, "TimeReportStandIn[" + idx + "]"));
            }
            VM.RegAssistantSSNAndName = claim.RegAssistantSSN + ", " + claim.RegFirstName + " " + claim.RegLastName;

            string[] subAssistantSSN = new string[20];
            string[] subAssistantName = new string[20];
            string[] subAssistantSSNAndName = new string[20];

            subAssistantSSN = claim.SubAssistantsSSNConcat.Split('£').ToArray();
            subAssistantName = claim.SubAssistantsNameConcat.Split('£').ToArray();
            subAssistantSSNAndName[0] = claim.SubAssistantSSN + ", " + claim.SubFirstName + " " + claim.SubLastName;

            for (int i = 1; i < claim.NumberOfSubAssistants; i++)
            {
                subAssistantSSNAndName[i] = subAssistantSSN[i - 1] + ", " + subAssistantName[i - 1];
            }

            VM.SubAssistantSSNAndName = subAssistantSSNAndName;

            return View("Create4", VM);
        }

        //
        // POST: Claims/Create4
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Ombud")]
        public ActionResult Create4([Bind(Include = "ClaimNumber, SalaryAttachment, SickLeaveNotification, DoctorsCertificate, TimeReport, TimeReportStandIn, AssistantHasFile")]Create4VM model, string submitButton, int noOfSubAssistants)
        {
            if (submitButton == "Skicka in" || submitButton == "Spara")
            {
                if (!Directory.Exists(Server.MapPath("~/Uploads")))
                    Directory.CreateDirectory(Server.MapPath("~/Uploads"));
                if (!Directory.Exists(Server.MapPath($"~/Uploads/{model.ClaimNumber}")))
                    Directory.CreateDirectory(Server.MapPath($"~/Uploads/{model.ClaimNumber}"));

                var claim = db.Claims.Where(c => c.ReferenceNumber == model.ClaimNumber).FirstOrDefault();

                if (!CheckExistingDocument(claim, "SalaryAttachment", model.SalaryAttachment))
                    ModelState.AddModelError("SalaryAttachment", "Lönespecifikation för ordinarie assistent saknas.");

                //if (!CheckExistingDocument(claim, "SickLeaveNotification", model.SickLeaveNotification))
                //    ModelState.AddModelError("SickLeaveNotification", "Sjukfrånvaroanmälan saknas.");

                if (!CheckExistingDocument(claim, "DoctorsCertificate", model.DoctorsCertificate) && claim.NumberOfCalendarDays > 7)
                    ModelState.AddModelError("DoctorsCertificate", "Läkarintyg saknas.");

                if (!CheckExistingDocument(claim, "TimeReport", model.TimeReport))
                    ModelState.AddModelError("TimeReport", "Tidsredovisning för ordinarie assistent saknas.");

                if (model.TimeReportStandIn != null)
                {
                    for (int i = 0; i < noOfSubAssistants; i++)
                    {
                        if (model.AssistantHasFile[i] == false && !CheckExistingDocument(claim, "TimeReportStandIn[" + i.ToString() + "]"))
                        {
                            ModelState.AddModelError("TimeReportStandIn[" + i.ToString() + "]", "Tidsredovisning för vikarierande assistent saknas."); //This should hopefully never happen
                        }
                    }
                }
                else //Needs to be redone
                {
                    for (int i = 0; i < noOfSubAssistants; i++)
                    {
                        if (!CheckExistingDocument(claim, "TimeReportStandIn[" + i.ToString() + "]"))
                            ModelState.AddModelError("TimeReportStandIn[" + i.ToString() + "]", "Tidsredovisning för vikarierande assistent saknas.");
                    }
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        string path = Server.MapPath($"~/Uploads/{model.ClaimNumber}");

                        if (model.SalaryAttachment != null)
                            NewDocument(model.SalaryAttachment, path, "SalaryAttachment", claim);

                        //if (model.SickLeaveNotification != null)
                        //    NewDocument(model.SickLeaveNotification, path, "SickLeaveNotification", claim);

                        if (model.DoctorsCertificate != null)
                            NewDocument(model.DoctorsCertificate, path, "DoctorsCertificate", claim);

                        if (model.TimeReport != null)
                            NewDocument(model.TimeReport, path, "TimeReport", claim);

                        if (model.TimeReportStandIn != null)
                        {
                            int filesUploaded = 0;
                            for (int i = 0; i < noOfSubAssistants; i++)
                            {
                                if (model.AssistantHasFile[i] == true)
                                {
                                    CheckExistingDocument(claim, "TimeReportStandIn[" + i.ToString() + "]", model.TimeReportStandIn[filesUploaded]);
                                    NewDocument(model.TimeReportStandIn[filesUploaded], path, "TimeReportStandIn[" + i.ToString() + "]", claim);
                                    filesUploaded++;
                                }
                            }
                        }

                        if (claim.CompletionStage < 4)
                        {
                            claim.CompletionStage = 4;
                        }

                        if (submitButton == "Skicka in")
                        {
                            claim.ClaimStatusId = 4;
                            claim.StatusDate = DateTime.Now;
                            claim.SentInDate = DateTime.Now;

                            //Set default values for ivo and Procapita checks
                            claim.IVOCheck = false;
                            claim.IVOCheckMsg = "Kontroll ej utförd";
                            claim.ProxyCheck = false;
                            claim.ProxyCheckMsg = "Kontroll ej utförd";
                            claim.ProCapitaCheck = false;
                            claim.AssistanceCheckMsg = "Kontroll ej utförd";

                            //Set default values for attachment checks
                            claim.SalarySpecRegAssistantCheck = false;
                            claim.SalarySpecRegAssistantCheckMsg = "Kontroll ej utförd";

                            //claim.SalarySpecSubAssistantCheck = false;
                            //claim.SalarySpecSubAssistantCheckMsg = "Kontroll ej utförd";

                            //claim.SickleaveNotificationCheck = false;
                            //claim.SickleaveNotificationCheckMsg = "Kontroll ej utförd";

                            if (claim.NumberOfCalendarDays > 7)
                            {
                                claim.MedicalCertificateCheck = false;
                                claim.MedicalCertificateCheckMsg = "Kontroll ej utförd";
                            }
                            else
                            {
                                claim.MedicalCertificateCheck = true;
                                claim.MedicalCertificateCheckMsg = "Intyget krävs ej eftersom antalet sjukdagar är lägre än 8. Kontroll ej utförd.";
                            }

                            claim.FKRegAssistantCheck = false;
                            claim.FKRegAssistantCheckMsg = "Kontroll ej utförd";

                            claim.FKSubAssistantCheck = false;
                            claim.FKSubAssistantCheckMsg = "Kontroll ej utförd";

                            //FK attachment checks for substitute assistants 2 - 20
                            if (claim.NumberOfSubAssistants > 1)
                            {
                                string fkCheckBool = "";
                                string fkCheckMsg = "";

                                for (int i = 0; i < claim.NumberOfSubAssistants - 1; i++)
                                {
                                    fkCheckBool += "false£";
                                    fkCheckMsg += "Kontroll ej utförd£";
                                }
                                claim.FKSubAssistantCheckMsgConcat = fkCheckMsg;
                                claim.FKSubAssistantCheckBoolConcat = fkCheckBool;
                            }

                            //Set default values for transfers
                            claim.BasisForDecision = false;
                            claim.BasisForDecisionMsg = "Överföring ej utförd";

                            claim.Decision = false;
                            claim.DecisionMsg = "Överföring ej utförd";

                            db.Entry(claim).State = EntityState.Modified;
                            db.SaveChanges();
                            return RedirectToAction("ShowReceipt", new { model.ClaimNumber });
                        }
                        else
                        {
                            model.NumberOfSubAssistants = claim.NumberOfSubAssistants;

                            model.RegAssistantSSNAndName = claim.RegAssistantSSN + ", " + claim.RegFirstName + " " + claim.RegLastName;

                            string[] subAssistantSSN = new string[20];
                            string[] subAssistantName = new string[20];
                            string[] subAssistantSSNAndName = new string[20];

                            subAssistantSSN = claim.SubAssistantsSSNConcat.Split('£').ToArray();
                            subAssistantName = claim.SubAssistantsNameConcat.Split('£').ToArray();
                            subAssistantSSNAndName[0] = claim.SubAssistantSSN + ", " + claim.SubFirstName + " " + claim.SubLastName;

                            for (int i = 1; i < claim.NumberOfSubAssistants; i++)
                            {
                                subAssistantSSNAndName[i] = subAssistantSSN[i - 1] + ", " + subAssistantName[i - 1];
                            }

                            model.SubAssistantSSNAndName = subAssistantSSNAndName;
                            claim.StatusDate = DateTime.Now;
                            claim.CreationDate = DateTime.Now;
                            db.Entry(claim).State = EntityState.Modified;
                            db.SaveChanges();

                            model.SalaryAttachmentExists = CheckExistingDocument(claim, "SalaryAttachment");
                            model.TimeReportExists = CheckExistingDocument(claim, "TimeReport");
                            model.DoctorsCertificateExists = (CheckExistingDocument(claim, "DoctorsCertificate") && claim.NumberOfSickDays > 7);

                            model.TimeReportStandInExists = new List<bool>();
                            for (int idx = 0; idx < model.NumberOfSubAssistants; idx++)
                            {
                                model.TimeReportStandInExists.Insert(idx, CheckExistingDocument(claim, "TimeReportStandIn[" + idx + "]"));
                            }

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
                    model.NumberOfSubAssistants = claim.NumberOfSubAssistants;

                    model.RegAssistantSSNAndName = claim.RegAssistantSSN + ", " + claim.RegFirstName + " " + claim.RegLastName;

                    string[] subAssistantSSN = new string[20];
                    string[] subAssistantName = new string[20];
                    string[] subAssistantSSNAndName = new string[20];

                    subAssistantSSN = claim.SubAssistantsSSNConcat.Split('£').ToArray();
                    subAssistantName = claim.SubAssistantsNameConcat.Split('£').ToArray();
                    subAssistantSSNAndName[0] = claim.SubAssistantSSN + ", " + claim.SubFirstName + " " + claim.SubLastName;

                    for (int i = 1; i < claim.NumberOfSubAssistants; i++)
                    {
                        subAssistantSSNAndName[i] = subAssistantSSN[i - 1] + ", " + subAssistantName[i - 1];
                    }

                    model.SubAssistantSSNAndName = subAssistantSSNAndName;

                    model.SalaryAttachmentExists = CheckExistingDocument(claim, "SalaryAttachment");
                    model.TimeReportExists = CheckExistingDocument(claim, "TimeReport");
                    model.DoctorsCertificateExists = (CheckExistingDocument(claim, "DoctorsCertificate") && claim.NumberOfSickDays > 7);

                    model.TimeReportStandInExists = new List<bool>();
                    for (int idx = 0; idx < model.NumberOfSubAssistants; idx++)
                    {
                        model.TimeReportStandInExists.Insert(idx, CheckExistingDocument(claim, "TimeReportStandIn[" + idx + "]"));
                    }

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
                System.IO.File.Delete(linqQuery.FirstOrDefault().FilePath);
                db.Documents.Remove(linqQuery.FirstOrDefault());
                db.SaveChanges();
            }
            return true;
        }
        private bool CheckExistingDocument(Claim claim, string queryValue)
        {
            var linqQuery = claim.Documents.Where(d => d.Title == queryValue);
            if (!linqQuery.Any())
                return false;
            return true;
        }

        private void NewDocument(HttpPostedFileBase file, string path, string title, Claim claim)
        {
            var document = new Document();
            document.DateUploaded = DateTime.Now;
            document.Filename = $"{title}_{claim.ReferenceNumber}.pdf";
            document.FilePath = Path.Combine(path, $"{title}_{claim.ReferenceNumber}.pdf");
            document.FileSize = file.ContentLength;
            document.FileType = file.ContentType;
            document.Title = title;
            document.ReferenceNumber = claim.ReferenceNumber;
            db.Documents.Add(document);
            claim.Documents.Add(document);
            db.SaveChanges();
            file.SaveAs(Path.Combine(path, $"{title}_{claim.ReferenceNumber}.pdf"));
            return;
        }

        [Authorize(Roles = "Ombud")]
        public ActionResult ShowReceipt(string ClaimNumber)
        {
            var claim = db.Claims.Where(c => c.ReferenceNumber == ClaimNumber).FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(claim.OmbudEmail))
            {
                MailMessage message = new MailMessage();
                message.From = new MailAddress("ourrobotdemo@gmail.com");
                message.To.Add(new MailAddress(claim.OmbudEmail));
                message.Subject = "Ny ansökan med referensnummer: " + ClaimNumber;
                message.Body = "Vi har mottagit din ansökan med referensnummer " + ClaimNumber + ". Normalt får du ett beslut inom 1 - 3 dagar." + "\n" + "\n" +
                                                    "Med vänliga hälsningar, Vård- och omsorgsförvaltningen";

                //SendEmail(message); Remove comment after test
            }

            using (var writer = XmlWriter.Create(Server.MapPath("\\sjukloner" + "\\" + claim.ReferenceNumber + ".xml")))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("claiminformation");
                writer.WriteElementString("SSN", claim.CustomerSSN.Substring(2));
                writer.WriteElementString("OrgNumber", claim.OrganisationNumber);
                writer.WriteElementString("ReferenceNumber", claim.ReferenceNumber);
                writer.WriteElementString("ClaimId", claim.Id.ToString());
                writer.WriteElementString("OmbudName", $"{claim.OmbudFirstName} {claim.OmbudLastName}");
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Dispose();
            }
            var create3VM = new Create3VM();
            create3VM.ClaimNumber = claim.ReferenceNumber;
            create3VM.ClaimSum = String.Format("{0:0.00}", claim.ClaimedSum);
            return View("Receipt", create3VM);
        }

        // GET: Claims/Decide/5
        [Authorize(Roles = "Admin, AdministrativeOfficial")]
        public ActionResult Recommend(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            RecommendationVM recommendationVM = new RecommendationVM();
            var claim = db.Claims.Where(c => c.Id == id).FirstOrDefault();
            if (claim != null)
            {
                //Find ClaimDay records for the claim
                var claimDays = db.ClaimDays.Where(c => c.ReferenceNumber == claim.ReferenceNumber).OrderBy(c => c.CalendarDayNumber).ToList();

                //These check results are hardcoded for the demo. Need to be changed for the real solution.

                recommendationVM.IvoCheck = false;
                recommendationVM.IvoCheck = claim.IVOCheck;
                if (!recommendationVM.IvoCheck)
                {
                    recommendationVM.IvoCheckMsg = "Verksamheten saknas i Vårdgivarregistret på www.ivo.se.";
                }
                else
                {
                    recommendationVM.IvoCheckMsg = "Verksamheten finns i Vårdgivarregistret på www.ivo.se.";
                }

                recommendationVM.CompleteCheck = true; //All attachments will be included by default since the claim cannot be submitted without attachements
                if (!recommendationVM.CompleteCheck)
                {
                    recommendationVM.CompleteCheckMsg = "Bilaga saknas.";
                }
                else
                {
                    recommendationVM.CompleteCheckMsg = "Alla bilagor är med.";
                }

                recommendationVM.ProxyCheck = false;
                recommendationVM.ProxyCheck = claim.ProxyCheck;
                if (!recommendationVM.ProxyCheck)
                {
                    recommendationVM.ProxyCheckMsg = "Ombudet saknar giltig fullmakt.";
                }
                else
                {
                    recommendationVM.ProxyCheckMsg = "Ombudet har giltig fullmakt.";
                }

                bool partiallyCovered = false; //This variable is set to true if the decision about personal assistance only covers part of the sickleave period.
                recommendationVM.AssistanceCheck = false;
                recommendationVM.AssistanceCheck = claim.ProCapitaCheck;
                if (!recommendationVM.AssistanceCheck)
                {
                    recommendationVM.AssistanceCheckMsg = "Beslut om assistans saknas.";
                }
                else //There is a decision about personal assistance. Now it needs to be checked if it is valid for the whole sickleave period or parts of it or not at all.
                {
                    DateTime endOfAssistance = new DateTime();
                    if (!string.IsNullOrEmpty(claim.LastAssistanceDate))
                    {
                        //Check if claim.LastAssistanceDate is in the format YYYYMMDD
                        string tempDate = "";
                        if (claim.LastAssistanceDate.Length >= 10)
                        {
                            tempDate = claim.LastAssistanceDate.Substring(0, 10);
                        }
                        else
                        {
                            tempDate = claim.LastAssistanceDate;
                        }

                        Regex regex1 = new Regex(@"^([1-9][0-9]{3})(((0[13578]|1[02])(0[1-9]|[12][0-9]|3[01]))|((0[469]|11)(0[1-9]|[12][0-9]|30))|(02(0[1-9]|[12][0-9])))$");
                        Match match1 = regex1.Match(tempDate);
                        Regex regex2 = new Regex(@"^(((0[13578]/|1[02]/)(0[1-9]/|[12][0-9]/|3[01]/))|((0[469]/|11/)(0[1-9]/|[12][0-9]/|30/))|(02/(0[1-9]/|[12][0-9]/)))([1-9][0-9]{3})$");
                        Match match2 = regex2.Match(tempDate);
                        if (match1.Success)
                        {
                            claim.LastAssistanceDate = tempDate; //Ensures that there are no leading or trailing spaces in the string. 
                            db.Entry(claim).State = EntityState.Modified;
                            db.SaveChanges();
                            endOfAssistance = new DateTime(int.Parse(tempDate.Substring(0, 4)), int.Parse(tempDate.Substring(4, 2)), int.Parse(tempDate.Substring(6, 2)));
                        }
                        //Check if claim.LastAssistanceDate is a date in the format MM/DD/YYYY.
                        else if (match2.Success)
                        {
                            //Check if claim.LastAssistanceDate is a date in the format MM/DD/YYYY. In that case it must reformatted to YYYYMMDD.
                            claim.LastAssistanceDate = tempDate.Substring(6, 4) + tempDate.Substring(0, 2) + tempDate.Substring(3, 2);
                            db.Entry(claim).State = EntityState.Modified;
                            db.SaveChanges();
                            endOfAssistance = new DateTime(int.Parse(claim.LastAssistanceDate.Substring(0, 4)), int.Parse(claim.LastAssistanceDate.Substring(4, 2)), int.Parse(claim.LastAssistanceDate.Substring(6, 2)));
                        }
                        else
                        {
                            //Set a default value for endOfAssistance in case Robin did not set a value for claim.LastAssistanceDate. The default is big enough to ensure that the
                            //decision about personal assistance covers the whole sickleave period.
                            endOfAssistance = claim.LastClaimDate.Date.AddDays(20);
                        }
                    }
                    else
                    {
                        //Set a default value for endOfAssistance in case Robin did not set a value for claim.LastAssistanceDate. The default is big enough to ensure that the
                        //decision about personal assistance covers the whole sickleave period.
                        endOfAssistance = claim.LastClaimDate.Date.AddDays(20);
                    }

                    DateTime startOfAssistance = new DateTime();
                    if (!string.IsNullOrEmpty(claim.FirstAssistanceDate))
                    {
                        //Check if claim.FirstAssistanceDate is in the format YYYYMMDD
                        string tempDate = "";
                        if (claim.FirstAssistanceDate.Length >= 10)
                        {
                            tempDate = claim.FirstAssistanceDate.Substring(0, 10);
                        }
                        else
                        {
                            tempDate = claim.FirstAssistanceDate;
                        }

                        Regex regex1 = new Regex(@"^([1-9][0-9]{3})(((0[13578]|1[02])(0[1-9]|[12][0-9]|3[01]))|((0[469]|11)(0[1-9]|[12][0-9]|30))|(02(0[1-9]|[12][0-9])))$");
                        Match match1 = regex1.Match(tempDate);
                        Regex regex2 = new Regex(@"^(((0[13578]/|1[02]/)(0[1-9]/|[12][0-9]/|3[01]/))|((0[469]/|11/)(0[1-9]/|[12][0-9]/|30/))|(02/(0[1-9]/|[12][0-9]/)))([1-9][0-9]{3})$");
                        Match match2 = regex2.Match(tempDate);
                        if (match1.Success)
                        {
                            claim.FirstAssistanceDate = tempDate; //Ensures that there are no leading or trailing spaces in the string. 
                            db.Entry(claim).State = EntityState.Modified;
                            db.SaveChanges();
                            startOfAssistance = new DateTime(int.Parse(tempDate.Substring(0, 4)), int.Parse(tempDate.Substring(4, 2)), int.Parse(tempDate.Substring(6, 2)));
                        }
                        //Check if claim.FirstAssistanceDate is a date in the format MM/DD/YYYY.
                        else if (match2.Success)
                        {
                            //Check if claim.LastAssistanceDate is a date in the format MM/DD/YYYY. In that case it must reformatted to YYYYMMDD.
                            claim.FirstAssistanceDate = tempDate.Substring(6, 4) + tempDate.Substring(0, 2) + tempDate.Substring(3, 2);
                            db.Entry(claim).State = EntityState.Modified;
                            db.SaveChanges();
                            startOfAssistance = new DateTime(int.Parse(claim.FirstAssistanceDate.Substring(0, 4)), int.Parse(claim.FirstAssistanceDate.Substring(4, 2)), int.Parse(claim.FirstAssistanceDate.Substring(6, 2)));
                        }
                        else
                        {
                            //Set a default value for startOfAssistance in case Robin did not set a value for claim.FirstAssistanceDate. The default is small enough to ensure that the
                            //decision about personal assistance covers the whole sickleave period.
                            startOfAssistance = claim.FirstClaimDate.Date.AddDays(-20);
                        }
                    }
                    else
                    {
                        //Set a default value for startOfAssistance in case Robin did not set a value for claim.FirstAssistanceDate. The default is small enough to ensure that the
                        //decision about personal assistance covers the whole sickleave period.
                        startOfAssistance = claim.FirstClaimDate.Date.AddDays(-20);
                    }

                    //Check if the last day of approved personal assistance is equal to or greater than the first day of the sickperiod and earlier than the last day of the sickleave period. 
                    //In that case the model sum calculation which has been done prior to stage 3 in the claim process shall adjusted to only include those days for which personal assistance has been approved.
                    //Two edge cases have been implemented:
                    //1. The case where the last date of approved assistance is within the sickleave period 
                    //2. The case where the first date of approved assistance is within the sickleave period. 
                    //The case where both the qualifying date and last day of sickness are outside the approved personal assistance dates is not covered. It is a very unlikely case.
                    if (endOfAssistance.Date < claim.LastClaimDate.Date && endOfAssistance.Date >= claim.FirstClaimDate.Date)
                    {
                        //Calculate the number of claimdays to be removed from the model sum calcluation and the start index (zero-based) of the range of claimdays that shall be included in the model sum calculation
                        int numberOfDaysToRemove = (claim.LastClaimDate.Date - endOfAssistance.Date).Days;
                        int startIndex = claim.NumberOfCalendarDays - numberOfDaysToRemove;

                        //Calculate the model sum. Take into consideration that a number of claimdays must be excluded from the model sum calculation due to the fact that the decision about personal assistance
                        //does not cover the whole sickleave period.
                        if (claimDays.Count() - numberOfDaysToRemove > 0)
                        {
                            CalculateModelSum(claim, claimDays, startIndex, numberOfDaysToRemove);
                        }
                        recommendationVM.AssistanceCheckMsg = "Giltigt beslut om assistans finns t. o. m. " + claim.LastAssistanceDate + ", vilket endast täcker en del av sjukperioden";
                        partiallyCovered = true;
                    }
                    else if (startOfAssistance.Date > claim.FirstClaimDate.Date && startOfAssistance.Date <= claim.LastClaimDate.Date)
                    {
                        //Calculate the number of claimdays to be removed from the model sum calcluation and the start index (zero-based) of the range of claimdays that shall be included in the model sum calculation
                        int numberOfDaysToRemove = (startOfAssistance.Date - claim.FirstClaimDate.Date).Days;
                        int startIndex = 0;

                        //Calculate the model sum. Take into consideration that a number of claimdays must be excluded from the model sum calculation due to the fact that the decision about personal assistance
                        //does not cover the whole sickleave period.
                        if (claimDays.Count() - numberOfDaysToRemove > 0)
                        {
                            CalculateModelSum(claim, claimDays, startIndex, numberOfDaysToRemove);
                        }
                        recommendationVM.AssistanceCheckMsg = "Giltigt beslut om assistans finns fr. o. m. " + claim.FirstAssistanceDate + ", vilket endast täcker en del av sjukperioden";
                        partiallyCovered = true;
                    }
                    //Check if the last day of approved personal assistance is equal to or after the last day of the sickperiod. Claim.LastAssistanceDate is filled in by Robin. 
                    else if (endOfAssistance.Date >= claim.LastClaimDate.Date) //CHECK THIS OUT
                    {
                        recommendationVM.AssistanceCheckMsg = "Giltigt beslut om assistans finns.";
                    }
                    else
                    {
                        recommendationVM.AssistanceCheckMsg = "Beslut om assistans saknas för sjukperioden.";
                    }
                }

                db.Entry(claim).State = EntityState.Modified;
                db.SaveChanges();

                //Results of attachment checks
                recommendationVM.SalarySpecRegAssistantCheck = claim.SalarySpecRegAssistantCheck;
                recommendationVM.SalarySpecRegAssistantCheckMsg = claim.SalarySpecRegAssistantCheckMsg;

                //recommendationVM.SalarySpecSubAssistantCheck = claim.SalarySpecSubAssistantCheck;
                //recommendationVM.SalarySpecSubAssistantCheckMsg = claim.SalarySpecSubAssistantCheckMsg;

                //recommendationVM.SickleaveNotificationCheck = claim.SickleaveNotificationCheck;
                //recommendationVM.SickleaveNotificationCheckMsg = claim.SickleaveNotificationCheckMsg;

                recommendationVM.MedicalCertificateCheck = claim.MedicalCertificateCheck;
                recommendationVM.MedicalCertificateCheckMsg = claim.MedicalCertificateCheckMsg;

                recommendationVM.FKRegAssistantCheck = claim.FKRegAssistantCheck;
                recommendationVM.FKRegAssistantCheckMsg = claim.FKRegAssistantCheckMsg;

                recommendationVM.FKSubAssistantCheck = claim.FKSubAssistantCheck;
                recommendationVM.FKSubAssistantCheckMsg = claim.FKSubAssistantCheckMsg;

                if (claim.NumberOfSubAssistants > 1)
                {
                    //handle multiple substitute assistants 2 - 20
                    string[] fkAttachmentSubAssistantsAsString = new string[20];
                    bool[] fkAttachmentSubAssistants = new bool[20];
                    fkAttachmentSubAssistantsAsString = claim.FKSubAssistantCheckBoolConcat.Split('£');

                    for (int i = 0; i < claim.NumberOfSubAssistants - 1; i++)
                    {
                        if (fkAttachmentSubAssistantsAsString[i] == "true")
                        {
                            fkAttachmentSubAssistants[i] = true;
                        }
                        else if (fkAttachmentSubAssistantsAsString[i] == "false")
                        {
                            fkAttachmentSubAssistants[i] = false;
                        }
                    }

                    string[] fkAttachmentSubAssistantsMsg = new string[20];
                    fkAttachmentSubAssistantsMsg = claim.FKSubAssistantCheckMsgConcat.Split('£');

                    recommendationVM.FKSubAssistantCheckBoolArray = fkAttachmentSubAssistants;
                    recommendationVM.FKSubAssistantCheckMsgArray = fkAttachmentSubAssistantsMsg;
                }

                recommendationVM.NumberOfSubAssistants = claim.NumberOfSubAssistants;
                recommendationVM.NumberOfSickDays = claim.NumberOfSickDays;

                //Results of transfers
                recommendationVM.BasisForDecision = claim.BasisForDecision;
                recommendationVM.BasisForDecisionMsg = claim.BasisForDecisionMsg;

                recommendationVM.Decision = claim.Decision;
                recommendationVM.DecisionMsg = claim.DecisionMsg;

                recommendationVM.ClaimNumber = claim.ReferenceNumber;
                recommendationVM.ModelSum = claim.ModelSum;
                //recommendationVM.ModelSum = Convert.ToDecimal(claim.TotalCostD1T14);
                recommendationVM.ClaimSum = claim.ClaimedSum;

                if (claim.ClaimStatusId == 3 || claim.ClaimStatusId == 7)
                {
                    recommendationVM.BasisForDecisionMsg = "Överföring påbörjad " + claim.BasisForDecisionTransferStartTimeStamp.Date.ToShortDateString() + " kl " + claim.BasisForDecisionTransferStartTimeStamp.ToShortTimeString() + ".";
                    recommendationVM.BasisForDecision = false;
                }
                if (claim.ClaimStatusId == 6 || claim.ClaimStatusId == 1)
                {
                    recommendationVM.BasisForDecisionMsg = "Överföring avslutad " + claim.BasisForDecisionTransferFinishTimeStamp.Date.ToShortDateString() + " kl " + claim.BasisForDecisionTransferFinishTimeStamp.ToShortTimeString() + ".";
                    recommendationVM.BasisForDecision = true;
                }
                if (claim.ClaimStatusId == 1)
                {
                    recommendationVM.DecisionMsg = "Beslut upptäckt i Procapita " + claim.DecisionTransferTimeStamp.Date.ToShortDateString() + " kl " + claim.DecisionTransferTimeStamp.ToShortTimeString() + ".";
                }


                claim.BasisForDecisionMsg = recommendationVM.BasisForDecisionMsg;
                claim.IVOCheckMsg = recommendationVM.IvoCheckMsg;
                claim.ProxyCheckMsg = recommendationVM.ProxyCheckMsg;
                claim.AssistanceCheckMsg = recommendationVM.AssistanceCheckMsg;

                if (claim.ClaimStatusId == 5)   // Claim is in Inbox
                {
                    recommendationVM.InInbox = true;

                    //Handle FK attachments for multiple substitute assistants
                    bool FKSubAssistantAttachmentOK = true;
                    int index = 0;
                    while (index < claim.NumberOfSubAssistants - 1 && FKSubAssistantAttachmentOK)
                    {
                        if (recommendationVM.FKSubAssistantCheckBoolArray[index] == false)
                        {
                            FKSubAssistantAttachmentOK = false;
                        }
                        index++;
                    }

                    if (!recommendationVM.IvoCheck || !recommendationVM.CompleteCheck || !recommendationVM.ProxyCheck || !recommendationVM.AssistanceCheck || !recommendationVM.SalarySpecRegAssistantCheck ||
                        !recommendationVM.MedicalCertificateCheck || !recommendationVM.FKRegAssistantCheck || !recommendationVM.FKSubAssistantCheck || !FKSubAssistantAttachmentOK)
                    {
                        recommendationVM.ApprovedSum = "0,00";
                        recommendationVM.RejectedSum = recommendationVM.ClaimSum.ToString();
                    }
                    else
                    {
                        recommendationVM.ApprovedSum = recommendationVM.ModelSum.ToString();

                        if (recommendationVM.ModelSum > recommendationVM.ClaimSum)
                        {
                            recommendationVM.RejectedSum = "0,00";
                        }
                        else
                        {
                            recommendationVM.RejectedSum = (recommendationVM.ClaimSum - recommendationVM.ModelSum).ToString();
                        }
                    }

                    recommendationVM.RejectReason = RejectReason(claim, recommendationVM, partiallyCovered);
                    claim.RejectReason = recommendationVM.RejectReason;
                }
                else
                {
                    recommendationVM.InInbox = false;

                    recommendationVM.ApprovedSum = claim.ApprovedSum.ToString();
                    recommendationVM.RejectedSum = claim.RejectedSum.ToString();

                    recommendationVM.RejectReason = claim.RejectReason;
                }

                // Assign this Claim to the current Administrative Official               
                //if (User.IsInRole("AdministrativeOfficial"))
                //{
                //    var me = db.Users.Find(User.Identity.GetUserId());

                //    claim.AdmOffId = me.Id;
                //    claim.AdmOffName = me.FirstName + " " + me.LastName;
                //}

                db.Entry(claim).State = EntityState.Modified;
                db.SaveChanges();
                return View("Recommend", recommendationVM);
            }
            else
            {
                return View();
            }
        }

        // GET: Claims/RobotRecommend
        //This action is used by the robot when automatic transfer of claims is switched on by the admin.
        [HttpGet]
        [AllowAnonymous]
        public ActionResult RobotRecommend(string refNumber)
        {
            RecommendationVM recommendationVM = new RecommendationVM();
            var claim = db.Claims.Where(c => c.ReferenceNumber == refNumber).FirstOrDefault();
            if (claim != null)
            {
                //Find ClaimDay records for the claim
                var claimDays = db.ClaimDays.Where(c => c.ReferenceNumber == claim.ReferenceNumber).OrderBy(c => c.CalendarDayNumber).ToList();

                //These check results are hardcoded for the demo. Need to be changed for the real solution.

                recommendationVM.IvoCheck = false;
                recommendationVM.IvoCheck = claim.IVOCheck;
                if (!recommendationVM.IvoCheck)
                {
                    recommendationVM.IvoCheckMsg = "Verksamheten saknas i Vårdgivarregistret på www.ivo.se.";
                }
                else
                {
                    recommendationVM.IvoCheckMsg = "Verksamheten finns i Vårdgivarregistret på www.ivo.se.";
                }

                recommendationVM.CompleteCheck = true; //All attachments will be included by default since the claim cannot be submitted without attachements
                if (!recommendationVM.CompleteCheck)
                {
                    recommendationVM.CompleteCheckMsg = "Bilaga saknas.";
                }
                else
                {
                    recommendationVM.CompleteCheckMsg = "Alla bilagor är med.";
                }

                recommendationVM.ProxyCheck = false;
                recommendationVM.ProxyCheck = claim.ProxyCheck;
                if (!recommendationVM.ProxyCheck)
                {
                    recommendationVM.ProxyCheckMsg = "Ombudet saknar giltig fullmakt.";
                }
                else
                {
                    recommendationVM.ProxyCheckMsg = "Ombudet har giltig fullmakt.";
                }

                bool partiallyCovered = false; //This variable is set to true if the decision about personal assistance only covers part of the sickleave period.
                recommendationVM.AssistanceCheck = false;
                recommendationVM.AssistanceCheck = claim.ProCapitaCheck;
                if (!recommendationVM.AssistanceCheck)
                {
                    recommendationVM.AssistanceCheckMsg = "Beslut om assistans saknas.";
                }
                else //There is a decision about personal assistance. Now it needs to be checked if it is valid for the whole sickleave period or parts of it or not at all.
                {
                    DateTime endOfAssistance = new DateTime();
                    if (!string.IsNullOrEmpty(claim.LastAssistanceDate))
                    {
                        //Check if claim.LastAssistanceDate is in the format YYYYMMDD
                        string tempDate = "";
                        if (claim.LastAssistanceDate.Length >= 10)
                        {
                            tempDate = claim.LastAssistanceDate.Substring(0, 10);
                        }
                        else
                        {
                            tempDate = claim.LastAssistanceDate;
                        }

                        Regex regex1 = new Regex(@"^([1-9][0-9]{3})(((0[13578]|1[02])(0[1-9]|[12][0-9]|3[01]))|((0[469]|11)(0[1-9]|[12][0-9]|30))|(02(0[1-9]|[12][0-9])))$");
                        Match match1 = regex1.Match(tempDate);
                        Regex regex2 = new Regex(@"^(((0[13578]/|1[02]/)(0[1-9]/|[12][0-9]/|3[01]/))|((0[469]/|11/)(0[1-9]/|[12][0-9]/|30/))|(02/(0[1-9]/|[12][0-9]/)))([1-9][0-9]{3})$");
                        Match match2 = regex2.Match(tempDate);
                        if (match1.Success)
                        {
                            claim.LastAssistanceDate = tempDate; //Ensures that there are no leading or trailing spaces in the string. 
                            db.Entry(claim).State = EntityState.Modified;
                            db.SaveChanges();
                            endOfAssistance = new DateTime(int.Parse(tempDate.Substring(0, 4)), int.Parse(tempDate.Substring(4, 2)), int.Parse(tempDate.Substring(6, 2)));
                        }
                        //Check if claim.LastAssistanceDate is a date in the format MM/DD/YYYY.
                        else if (match2.Success)
                        {
                            //Check if claim.LastAssistanceDate is a date in the format MM/DD/YYYY. In that case it must reformatted to YYYYMMDD.
                            claim.LastAssistanceDate = tempDate.Substring(6, 4) + tempDate.Substring(0, 2) + tempDate.Substring(3, 2);
                            db.Entry(claim).State = EntityState.Modified;
                            db.SaveChanges();
                            endOfAssistance = new DateTime(int.Parse(claim.LastAssistanceDate.Substring(0, 4)), int.Parse(claim.LastAssistanceDate.Substring(4, 2)), int.Parse(claim.LastAssistanceDate.Substring(6, 2)));
                        }
                        else
                        {
                            //Set a default value for endOfAssistance in case Robin did not set a value for claim.LastAssistanceDate. The default is big enough to ensure that the
                            //decision about personal assistance covers the whole sickleave period.
                            endOfAssistance = claim.LastClaimDate.Date.AddDays(20);
                        }
                    }
                    else
                    {
                        //Set a default value for endOfAssistance in case Robin did not set a value for claim.LastAssistanceDate. The default is big enough to ensure that the
                        //decision about personal assistance covers the whole sickleave period.
                        endOfAssistance = claim.LastClaimDate.Date.AddDays(20);
                    }

                    DateTime startOfAssistance = new DateTime();
                    if (!string.IsNullOrEmpty(claim.FirstAssistanceDate))
                    {
                        //Check if claim.FirstAssistanceDate is in the format YYYYMMDD
                        string tempDate = "";
                        if (claim.FirstAssistanceDate.Length >= 10)
                        {
                            tempDate = claim.FirstAssistanceDate.Substring(0, 10);
                        }
                        else
                        {
                            tempDate = claim.FirstAssistanceDate;
                        }

                        Regex regex1 = new Regex(@"^([1-9][0-9]{3})(((0[13578]|1[02])(0[1-9]|[12][0-9]|3[01]))|((0[469]|11)(0[1-9]|[12][0-9]|30))|(02(0[1-9]|[12][0-9])))$");
                        Match match1 = regex1.Match(tempDate);
                        Regex regex2 = new Regex(@"^(((0[13578]/|1[02]/)(0[1-9]/|[12][0-9]/|3[01]/))|((0[469]/|11/)(0[1-9]/|[12][0-9]/|30/))|(02/(0[1-9]/|[12][0-9]/)))([1-9][0-9]{3})$");
                        Match match2 = regex2.Match(tempDate);
                        if (match1.Success)
                        {
                            claim.FirstAssistanceDate = tempDate; //Ensures that there are no leading or trailing spaces in the string. 
                            db.Entry(claim).State = EntityState.Modified;
                            db.SaveChanges();
                            startOfAssistance = new DateTime(int.Parse(tempDate.Substring(0, 4)), int.Parse(tempDate.Substring(4, 2)), int.Parse(tempDate.Substring(6, 2)));
                        }
                        //Check if claim.FirstAssistanceDate is a date in the format MM/DD/YYYY.
                        else if (match2.Success)
                        {
                            //Check if claim.LastAssistanceDate is a date in the format MM/DD/YYYY. In that case it must reformatted to YYYYMMDD.
                            claim.FirstAssistanceDate = tempDate.Substring(6, 4) + tempDate.Substring(0, 2) + tempDate.Substring(3, 2);
                            db.Entry(claim).State = EntityState.Modified;
                            db.SaveChanges();
                            startOfAssistance = new DateTime(int.Parse(claim.FirstAssistanceDate.Substring(0, 4)), int.Parse(claim.FirstAssistanceDate.Substring(4, 2)), int.Parse(claim.FirstAssistanceDate.Substring(6, 2)));
                        }
                        else
                        {
                            //Set a default value for startOfAssistance in case Robin did not set a value for claim.FirstAssistanceDate. The default is small enough to ensure that the
                            //decision about personal assistance covers the whole sickleave period.
                            startOfAssistance = claim.FirstClaimDate.Date.AddDays(-20);
                        }
                    }
                    else
                    {
                        //Set a default value for startOfAssistance in case Robin did not set a value for claim.FirstAssistanceDate. The default is small enough to ensure that the
                        //decision about personal assistance covers the whole sickleave period.
                        startOfAssistance = claim.FirstClaimDate.Date.AddDays(-20);
                    }

                    //Check if the last day of approved personal assistance is equal to or greater than the first day of the sickperiod and earlier than the last day of the sickleave period. 
                    //In that case the model sum calculation which has been done prior to stage 3 in the claim process shall adjusted to only include those days for which personal assistance has been approved.
                    //Two edge cases have been implemented:
                    //1. The case where the last date of approved assistance is within the sickleave period 
                    //2. The case where the first date of approved assistance is within the sickleave period. 
                    //The case where both the qualifying date and last day of sickness are outside the approved personal assistance dates is not covered. It is a very unlikely case.
                    if (endOfAssistance.Date < claim.LastClaimDate.Date && endOfAssistance.Date >= claim.FirstClaimDate.Date)
                    {
                        //Calculate the number of claimdays to be removed from the model sum calcluation and the start index (zero-based) of the range of claimdays that shall be included in the model sum calculation
                        int numberOfDaysToRemove = (claim.LastClaimDate.Date - endOfAssistance.Date).Days;
                        int startIndex = claim.NumberOfCalendarDays - numberOfDaysToRemove;

                        //Calculate the model sum. Take into consideration that a number of claimdays must be excluded from the model sum calculation due to the fact that the decision about personal assistance
                        //does not cover the whole sickleave period.
                        if (claimDays.Count() - numberOfDaysToRemove > 0)
                        {
                            CalculateModelSum(claim, claimDays, startIndex, numberOfDaysToRemove);
                        }
                        recommendationVM.AssistanceCheckMsg = "Giltigt beslut om assistans finns t. o. m. " + claim.LastAssistanceDate + ", vilket endast täcker en del av sjukperioden";
                        partiallyCovered = true;
                    }
                    else if (startOfAssistance.Date > claim.FirstClaimDate.Date && startOfAssistance.Date <= claim.LastClaimDate.Date)
                    {
                        //Calculate the number of claimdays to be removed from the model sum calcluation and the start index (zero-based) of the range of claimdays that shall be included in the model sum calculation
                        int numberOfDaysToRemove = (startOfAssistance.Date - claim.FirstClaimDate.Date).Days;
                        int startIndex = 0;

                        //Calculate the model sum. Take into consideration that a number of claimdays must be excluded from the model sum calculation due to the fact that the decision about personal assistance
                        //does not cover the whole sickleave period.
                        if (claimDays.Count() - numberOfDaysToRemove > 0)
                        {
                            CalculateModelSum(claim, claimDays, startIndex, numberOfDaysToRemove);
                        }
                        recommendationVM.AssistanceCheckMsg = "Giltigt beslut om assistans finns fr. o. m. " + claim.FirstAssistanceDate + ", vilket endast täcker en del av sjukperioden";
                        partiallyCovered = true;
                    }
                    //Check if the last day of approved personal assistance is equal to or after the last day of the sickperiod. Claim.LastAssistanceDate is filled in by Robin. 
                    else if (endOfAssistance.Date >= claim.LastClaimDate.Date) //CHECK THIS OUT
                    {
                        recommendationVM.AssistanceCheckMsg = "Giltigt beslut om assistans finns.";
                    }
                    else
                    {
                        recommendationVM.AssistanceCheckMsg = "Beslut om assistans saknas för sjukperioden.";
                    }
                }

                db.Entry(claim).State = EntityState.Modified;
                db.SaveChanges();

                //Results of attachment checks
                recommendationVM.SalarySpecRegAssistantCheck = claim.SalarySpecRegAssistantCheck;
                recommendationVM.SalarySpecRegAssistantCheckMsg = claim.SalarySpecRegAssistantCheckMsg;

                //recommendationVM.SalarySpecSubAssistantCheck = claim.SalarySpecSubAssistantCheck;
                //recommendationVM.SalarySpecSubAssistantCheckMsg = claim.SalarySpecSubAssistantCheckMsg;

                //recommendationVM.SickleaveNotificationCheck = claim.SickleaveNotificationCheck;
                //recommendationVM.SickleaveNotificationCheckMsg = claim.SickleaveNotificationCheckMsg;

                recommendationVM.MedicalCertificateCheck = claim.MedicalCertificateCheck;
                recommendationVM.MedicalCertificateCheckMsg = claim.MedicalCertificateCheckMsg;

                recommendationVM.FKRegAssistantCheck = claim.FKRegAssistantCheck;
                recommendationVM.FKRegAssistantCheckMsg = claim.FKRegAssistantCheckMsg;

                recommendationVM.FKSubAssistantCheck = claim.FKSubAssistantCheck;
                recommendationVM.FKSubAssistantCheckMsg = claim.FKSubAssistantCheckMsg;

                if (claim.NumberOfSubAssistants > 1)
                {
                    //handle multiple substitute assistants 2 - 20
                    string[] fkAttachmentSubAssistantsAsString = new string[20];
                    bool[] fkAttachmentSubAssistants = new bool[20];
                    fkAttachmentSubAssistantsAsString = claim.FKSubAssistantCheckBoolConcat.Split('£');

                    for (int i = 0; i < claim.NumberOfSubAssistants - 1; i++)
                    {
                        if (fkAttachmentSubAssistantsAsString[i] == "true")
                        {
                            fkAttachmentSubAssistants[i] = true;
                        }
                        else if (fkAttachmentSubAssistantsAsString[i] == "false")
                        {
                            fkAttachmentSubAssistants[i] = false;
                        }
                    }

                    string[] fkAttachmentSubAssistantsMsg = new string[20];
                    fkAttachmentSubAssistantsMsg = claim.FKSubAssistantCheckMsgConcat.Split('£');

                    recommendationVM.FKSubAssistantCheckBoolArray = fkAttachmentSubAssistants;
                    recommendationVM.FKSubAssistantCheckMsgArray = fkAttachmentSubAssistantsMsg;
                }

                recommendationVM.NumberOfSubAssistants = claim.NumberOfSubAssistants;
                recommendationVM.NumberOfSickDays = claim.NumberOfSickDays;

                //Results of transfers
                recommendationVM.BasisForDecision = claim.BasisForDecision;
                recommendationVM.BasisForDecisionMsg = claim.BasisForDecisionMsg;

                recommendationVM.Decision = claim.Decision;
                recommendationVM.DecisionMsg = claim.DecisionMsg;

                recommendationVM.ClaimNumber = claim.ReferenceNumber;
                recommendationVM.ModelSum = claim.ModelSum;
                //recommendationVM.ModelSum = Convert.ToDecimal(claim.TotalCostD1T14);
                recommendationVM.ClaimSum = claim.ClaimedSum;

                //Handle FK attachments for multiple substitute assistants
                bool FKSubAssistantAttachmentOK = true;
                int index = 0;
                while (index < claim.NumberOfSubAssistants - 1 && FKSubAssistantAttachmentOK)
                {
                    if (recommendationVM.FKSubAssistantCheckBoolArray[index] == false)
                    {
                        FKSubAssistantAttachmentOK = false;
                    }
                    index++;
                }

                if (!recommendationVM.IvoCheck || !recommendationVM.CompleteCheck || !recommendationVM.ProxyCheck || !recommendationVM.AssistanceCheck || !recommendationVM.SalarySpecRegAssistantCheck ||
                    !recommendationVM.MedicalCertificateCheck || !recommendationVM.FKRegAssistantCheck || !recommendationVM.FKSubAssistantCheck || !FKSubAssistantAttachmentOK)
                {
                    recommendationVM.ApprovedSum = "0,00";
                    claim.ApprovedSum = 0;
                    recommendationVM.RejectedSum = recommendationVM.ClaimSum.ToString();
                    claim.RejectedSum = recommendationVM.ClaimSum;

                }
                else
                {
                    recommendationVM.ApprovedSum = recommendationVM.ModelSum.ToString();
                    claim.ApprovedSum = recommendationVM.ModelSum;

                    if (recommendationVM.ModelSum > recommendationVM.ClaimSum)
                    {
                        recommendationVM.RejectedSum = "0,00";
                        claim.RejectedSum = 0;
                    }
                    else
                    {
                        recommendationVM.RejectedSum = (recommendationVM.ClaimSum - recommendationVM.ModelSum).ToString();
                        claim.RejectedSum = recommendationVM.ClaimSum - recommendationVM.ModelSum;
                    }
                }
                if (claim.ClaimStatusId == 3)
                {
                    recommendationVM.BasisForDecisionMsg = "Överföring påbörjad " + claim.BasisForDecisionTransferStartTimeStamp.Date.ToShortDateString() + " kl " + claim.BasisForDecisionTransferStartTimeStamp.ToShortTimeString();
                }

                if (claim.ClaimStatusId == 5)
                {
                    recommendationVM.InInbox = true;
                }

                claim.IVOCheckMsg = recommendationVM.IvoCheckMsg;
                claim.ProxyCheckMsg = recommendationVM.ProxyCheckMsg;
                claim.AssistanceCheckMsg = recommendationVM.AssistanceCheckMsg;

                recommendationVM.RejectReason = RejectReason(claim, recommendationVM, partiallyCovered);
                claim.RejectReason = recommendationVM.RejectReason;

                // Assign this Claim to the current Administrative Official               
                if (User.IsInRole("AdministrativeOfficial"))
                {
                    var me = db.Users.Find(User.Identity.GetUserId());

                    claim.AdmOffId = me.Id;
                    claim.AdmOffName = me.FirstName + " " + me.LastName;
                }

                claim.FirstClaimDateAsString = claim.FirstClaimDate.ToShortDateString().ToString().Remove(4, 1);
                claim.FirstClaimDateAsString = claim.FirstClaimDateAsString.Remove(6, 1);
                claim.LastClaimDateAsString = claim.LastClaimDate.ToShortDateString().ToString().Remove(4, 1);
                claim.LastClaimDateAsString = claim.LastClaimDateAsString.Remove(6, 1);
                claim.SentInDateAsString = DateTime.Now.ToShortDateString().ToString().Remove(4, 1);
                claim.SentInDateAsString = claim.SentInDateAsString.Remove(6, 1);
                claim.ClaimedSumAsString = String.Format("{0:0.00}", claim.ClaimedSum);
                claim.ModelSumAsString = String.Format("{0:0.00}", claim.ModelSum);
                claim.ApprovedSumAsString = String.Format("{0:0.00}", claim.ApprovedSum);
                claim.RejectedSumAsString = String.Format("{0:0.00}", claim.RejectedSum);

                claim.TransferToProcapitaString = "transferinfo" + claim.ReferenceNumber + "+" + claim.FirstClaimDateAsString + "+" + claim.LastClaimDateAsString + "+" + claim.SentInDateAsString + "+" + claim.RejectReason + "+" +
                    claim.ClaimedSumAsString + "+" + claim.ModelSumAsString + "+" + claim.ApprovedSumAsString + "+" + claim.RejectedSumAsString + "+" +
                    claim.IVOCheckMsg + "+" + claim.ProxyCheckMsg + "+" + claim.AssistanceCheckMsg + "+" + claim.SalarySpecRegAssistantCheckMsg + "+" + claim.SickleaveNotificationCheckMsg + "+" +
                    claim.MedicalCertificateCheckMsg + "+" + claim.FKRegAssistantCheckMsg + "+" + claim.FKSubAssistantCheckMsg + "+" + claim.NumberOfCalendarDays.ToString() + "+" +
                    claim.CustomerSSN.Substring(2) + "+" + claim.CustomerName;

                string[] subAssistantBools = claim.FKSubAssistantCheckBoolConcat.Split('£');
                string[] subAssistantCheckMsgs = claim.FKSubAssistantCheckMsgConcat.Split('£');
                for (int idx = 0; idx < subAssistantBools.Length; idx++)
                {
                    claim.TransferToProcapitaString = claim.TransferToProcapitaString + "+" + subAssistantBools[idx];
                }
                for (int idx = 0; idx < subAssistantBools.Length; idx++)
                {
                    claim.TransferToProcapitaString = claim.TransferToProcapitaString + "+" + subAssistantCheckMsgs[idx];
                }

                db.Entry(claim).State = EntityState.Modified;
                db.SaveChanges();

                //claim.ClaimStatusId = 6; //This should probably be done by the robot when the transfer to Procapita has been done.
                claim.BasisForDecisionTransferStartTimeStamp = DateTime.Now;
                db.Entry(claim).State = EntityState.Modified;
                db.SaveChanges();

                return View(); //Return the dummy view RobotRecommend
            }
            else
            {
                return View(); //Return the dummy view RobotRecommend
            }
        }

        [AllowAnonymous]
        private string RejectReason(Claim claim, RecommendationVM recommendationVM, bool partiallyCoveredSickleave)
        {
            string resultMsg = "";
            if (claim.ClaimedSum > claim.ModelSum)
            {
                resultMsg += "Det yrkade beloppet överstiger det beräknade beloppet. ";
            }
            if (!claim.IVOCheck)
            {
                resultMsg += "Verksamheten saknas i Vårdgivarregistret på www.ivo.se. ";
            }
            if (!claim.ProxyCheck)
            {
                resultMsg += "Ombudet saknar giltig fullmakt. ";
            }
            if (partiallyCoveredSickleave)
            {
                resultMsg += "Beslut om assistans finns för bara en del av sjukperioden. ";
            }
            else if (!claim.ProCapitaCheck)
            {
                resultMsg += "Beslut om assistans saknas. ";
            }
            if (!claim.SalarySpecRegAssistantCheck)
            {
                resultMsg += "Kontroll av ordinarie assistents lönespecifikation gav negativt resultat. ";
            }
            //if (!claim.SalarySpecSubAssistantCheck)
            //{
            //    resultMsg += "Kontroll av vikarierande assistents lönespecifikation gav negativt resultat. ";
            //}
            //if (!claim.SickleaveNotificationCheck)
            //{
            //    resultMsg += "Kontroll av sjukfrånvaroanmälan gav negativt resultat. ";
            //}
            if (!claim.MedicalCertificateCheck)
            {
                resultMsg += "Kontroll av sjukintyg gav negativt resultat. ";
            }
            if (!claim.FKRegAssistantCheck)
            {
                resultMsg += "Kontroll av tidsredovisning för ordinarie assistent gav negativt resultat. ";
            }
            if (claim.NumberOfSubAssistants == 1)
            {
                if (!claim.FKSubAssistantCheck)
                {
                    resultMsg += "Kontroll av tidsredovisning för vikarierande assistent gav negativt resultat. ";
                }
            }
            if (claim.NumberOfSubAssistants > 1)
            {
                if (!claim.FKSubAssistantCheck)
                {
                    resultMsg += "Kontroll av tidsredovisning för vikarierande assistent 1 gav negativt resultat. ";
                }

                string[] fkAttachmentSubAssistantsAsString = new string[20];
                fkAttachmentSubAssistantsAsString = claim.FKSubAssistantCheckBoolConcat.Split('£');

                for (int i = 0; i < claim.NumberOfSubAssistants - 1; i++)
                {
                    if (fkAttachmentSubAssistantsAsString[i] == "false")
                    {
                        resultMsg += "Kontroll av tidsredovisning för vikarierande assistent " + (i + 2).ToString() + " gav negativt resultat. ";
                    }
                }
            }
            return resultMsg;
        }

        //RobotDeleteClaim is used by the robot for auto-deletion of claims one year after the decision date
        //The robot needs to call this action once every night
        [AllowAnonymous]
        public ActionResult RobotDeleteClaim()
        {
            var date = DateTime.Now.AddYears(-1); //DateTime.Now.AddDays(-1)
            //Find claims that have a decision and where the decision date is one year old or older
            var claims = db.Claims.Where(c => c.ClaimStatusId == 1).Where(c => c.DecisionDate <= date).ToList(); //This is the production line
            //var claims = db.Claims.Where(c => c.ClaimStatusId == 1).Where(c => c.DecisionDate >= date).ToList(); //This exists mostly for test purposes

            foreach (var claim in claims)
            {
                db.ClaimDays.RemoveRange(db.ClaimDays.Where(c => c.ReferenceNumber == claim.ReferenceNumber));
                db.ClaimCalculations.RemoveRange(db.ClaimCalculations.Where(c => c.ReferenceNumber == claim.ReferenceNumber));

                if (claim.Documents.Count() > 0)
                {
                    db.Documents.RemoveRange(db.Documents.Where(d => d.ReferenceNumber == claim.ReferenceNumber));
                }

                db.Claims.Remove(claim);
                db.SaveChanges();
            }
            return View(); //Return the dummy view RobotDeleteClaim
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
        public ActionResult _ShowClaim(string refNumber, int? startIndex, int? numberOfDaysToRemove)
        {
            Claim claim = db.Claims.Include(c => c.ClaimStatus).Where(c => c.ReferenceNumber == refNumber).FirstOrDefault();
            //var currentId = User.Identity.GetUserId();
            //ApplicationUser ombud = db.Users.Where(u => u.Id == currentId).FirstOrDefault();

            ClaimDetailsOmbudVM claimDetailsOmbudVM = CreateVMClaimDetails(claim);

            return PartialView("_ClaimForOmbud", claimDetailsOmbudVM);
        }

        [HttpGet]
        public ActionResult ClaimDetailsAsPdf(string refNumber)
        {
            Claim claim = db.Claims.Include(c => c.ClaimStatus).Where(c => c.ReferenceNumber == refNumber).FirstOrDefault();

            // Create a pdf document with Claim Details
            CreateClaimPdf(claim);

            if (User.IsInRole("Ombud"))
            {
                // Check from which view the Pdf Request was made
                if (Request.UrlReferrer.ToString().Contains("Create1"))
                {
                    return RedirectToAction("Create1", new { refNumber });
                }
                else if (Request.UrlReferrer.ToString().Contains("Create2"))
                {
                    return RedirectToAction("Create2", new { refNumber });
                }
                else if (Request.UrlReferrer.ToString().Contains("Create3"))
                {
                    return RedirectToAction("Create3", new { refNumber });
                }
                else if (Request.UrlReferrer.ToString().Contains("Create4"))
                {
                    return RedirectToAction("Create4", new { ClaimNumber = refNumber });
                }
                else
                {
                    return RedirectToAction("IndexPageOmbud");
                }
            }

            // Logged in as Admin or AdmOff
            // Check if the Pdf Request was made from the Recommend view
            if (Request.UrlReferrer.ToString().Contains("Recommend"))
            {
                return RedirectToAction("Recommend", new { claim.Id });
            }

            return RedirectToAction("Index");
        }

        private ClaimDetailsOmbudVM CreateVMClaimDetails(Claim claim)
        {
            ClaimDetailsOmbudVM claimDetailsOmbudVM = new ClaimDetailsOmbudVM();

            claimDetailsOmbudVM.CompletionStage = claim.CompletionStage;
            if (claim.CompletionStage >= 1)
            {
                claimDetailsOmbudVM.ReferenceNumber = claim.ReferenceNumber;
                claimDetailsOmbudVM.StatusName = claim.ClaimStatus.Name;
                claimDetailsOmbudVM.DefaultCollectiveAgreement = claim.DefaultCollectiveAgreement;

                //Kommun
                claimDetailsOmbudVM.Council = "Helsingborgs kommun";
                claimDetailsOmbudVM.Administration = "Vård- och omsorgsförvaltningen";

                //Assistansberättigad
                claimDetailsOmbudVM.CustomerName = claim.CustomerName;
                claimDetailsOmbudVM.CustomerSSN = claim.CustomerSSN;
                //claimDetailsOmbudVM.CustomerAddress = claim.CustomerAddress;
                claimDetailsOmbudVM.CustomerPhoneNumber = claim.CustomerPhoneNumber;

                //Ombud/uppgiftslämnare
                claimDetailsOmbudVM.OmbudName = claim.OmbudFirstName + " " + claim.OmbudLastName;
                claimDetailsOmbudVM.OmbudPhoneNumber = claim.OmbudPhoneNumber;

                //Assistansanordnare
                claimDetailsOmbudVM.CompanyName = claim.CompanyName; ;
                claimDetailsOmbudVM.OrganisationNumber = claim.OrganisationNumber;
                claimDetailsOmbudVM.GiroNumber = claim.AccountNumber;
                claimDetailsOmbudVM.CompanyAddress = claim.StreetAddress;
                claimDetailsOmbudVM.CompanyPhoneNumber = claim.CompanyPhoneNumber;
                claimDetailsOmbudVM.CollectiveAgreement = claim.CollectiveAgreementName + ", " + claim.CollectiveAgreementSpecName;
                //claimDetailsOmbudVM.Workplace = "Björkängen, Birgittagården"; 

                //Insjuknad ordinarie assistent
                claimDetailsOmbudVM.RegAssistantName = claim.RegFirstName + " " + claim.RegLastName;
                claimDetailsOmbudVM.RegAssistantSSN = claim.RegAssistantSSN;
                claimDetailsOmbudVM.RegPhoneNumber = claim.RegPhoneNumber;
                claimDetailsOmbudVM.RegEmail = claim.RegEmail;
                claimDetailsOmbudVM.FirstClaimDate = claim.FirstClaimDate.ToShortDateString();
                claimDetailsOmbudVM.LastClaimDate = claim.LastClaimDate.ToShortDateString();

                //Vikarierande assistent 1
                claimDetailsOmbudVM.SubAssistantName = claim.SubFirstName + " " + claim.SubLastName;
                claimDetailsOmbudVM.SubAssistantSSN = claim.SubAssistantSSN;
                claimDetailsOmbudVM.SubPhoneNumber = claim.SubPhoneNumber;
                claimDetailsOmbudVM.SubEmail = claim.SubEmail;

                //Vikarierande assistent 2 - 20
                string[] Name = new string[20];
                string[] SSN = new string[20];
                string[] PhoneNumber = new string[20];
                string[] Email = new string[20];

                Name = claim.SubAssistantsNameConcat.Split('£');
                SSN = claim.SubAssistantsSSNConcat.Split('£');
                PhoneNumber = claim.SubAssistantsPhoneConcat.Split('£');
                Email = claim.SubAssistantsEmailConcat.Split('£');

                claimDetailsOmbudVM.SubstituteAssistantName = Name;
                claimDetailsOmbudVM.SubstituteAssistantSSN = SSN;
                claimDetailsOmbudVM.SubstituteAssistantPhoneNumber = PhoneNumber;
                claimDetailsOmbudVM.SubstituteAssistantEmail = Email;

                claimDetailsOmbudVM.NumberOfSubAssistants = claim.NumberOfSubAssistants;

                claimDetailsOmbudVM.NumberOfCalendarDays = claim.NumberOfCalendarDays;

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

                //Hours for substitute assistant including handling for multiple substitute assistants
                claimDetailsOmbudVM.NumberOfHoursWithSI = claim.NumberOfHoursWithSI;
                claimDetailsOmbudVM.NumberOfUnsocialHoursSI = claim.NumberOfUnsocialHoursSI;
                claimDetailsOmbudVM.NumberOfOnCallHoursSI = claim.NumberOfOnCallHoursSI;

                string[] hoursWithSI = new string[20];
                string[] ordinaryHoursSI = new string[20];
                string[] unsocialHoursSI = new string[20];
                string[] onCallHoursSI = new string[20];

                hoursWithSI = claim.NumberOfHoursWithSIConcat.Split('+').ToArray();
                ordinaryHoursSI = claim.NumberOfOrdinaryHoursSIConcat.Split('+').ToArray();
                unsocialHoursSI = claim.NumberOfUnsocialHoursSIConcat.Split('+').ToArray();
                onCallHoursSI = claim.NumberOfOnCallHoursSIConcat.Split('+').ToArray();

                claimDetailsOmbudVM.HoursWithSI = hoursWithSI;
                claimDetailsOmbudVM.OrdinaryHoursSI = ordinaryHoursSI;
                claimDetailsOmbudVM.UnsocialHoursSI = unsocialHoursSI;
                claimDetailsOmbudVM.OnCallHoursSI = onCallHoursSI;

                string[] hoursSIArrayPerDay = new string[20];
                string[] unsocialEveningSIArrayPerDay = new string[20];
                string[] unsocialNightSIArrayPerDay = new string[20];
                string[] unsocialWeekendSIArrayPerDay = new string[20];
                string[] unsocialGrandWeekendSIArrayPerDay = new string[20];
                string[] onCallDaySIArrayPerDay = new string[20];
                string[] onCallNightSIArrayPerDay = new string[20];

                string[,] hoursSIPerSubAndDay = new string[20, 70];
                string[,] unsocialEveningSIPerSubAndDay = new string[20, 70];
                string[,] unsocialNightSIPerSubAndDay = new string[20, 70];
                string[,] unsocialWeekendSIPerSubAndDay = new string[20, 70];
                string[,] unsocialGrandWeekendSIPerSubAndDay = new string[20, 70];
                string[,] onCallDaySIPerSubAndDay = new string[20, 70];
                string[,] onCallNightSIPerSubAndDay = new string[20, 70];

                var claimDays = db.ClaimDays.Where(c => c.ReferenceNumber == claim.ReferenceNumber).OrderBy(c => c.CalendarDayNumber).ToList();
                int index = 0;
                foreach (var day in claimDays)
                {
                    hoursSIArrayPerDay = day.HoursSI.Split('+').ToArray();
                    unsocialEveningSIArrayPerDay = day.UnsocialEveningSI.Split('+').ToArray();
                    unsocialNightSIArrayPerDay = day.UnsocialNightSI.Split('+').ToArray();
                    unsocialWeekendSIArrayPerDay = day.UnsocialWeekendSI.Split('+').ToArray();
                    unsocialGrandWeekendSIArrayPerDay = day.UnsocialGrandWeekendSI.Split('+').ToArray();
                    onCallDaySIArrayPerDay = day.OnCallDaySI.Split('+').ToArray();
                    onCallNightSIArrayPerDay = day.OnCallNightSI.Split('+').ToArray();

                    for (int i = 0; i < claim.NumberOfSubAssistants; i++)
                    {
                        hoursSIPerSubAndDay[i, index] = hoursSIArrayPerDay[i];
                        unsocialEveningSIPerSubAndDay[i, index] = unsocialEveningSIArrayPerDay[i];
                        unsocialNightSIPerSubAndDay[i, index] = unsocialNightSIArrayPerDay[i];
                        unsocialWeekendSIPerSubAndDay[i, index] = unsocialWeekendSIArrayPerDay[i];
                        unsocialGrandWeekendSIPerSubAndDay[i, index] = unsocialGrandWeekendSIArrayPerDay[i];
                        onCallDaySIPerSubAndDay[i, index] = onCallDaySIArrayPerDay[i];
                        onCallNightSIPerSubAndDay[i, index] = onCallNightSIArrayPerDay[i];
                    }
                    index++;
                }

                claimDetailsOmbudVM.HoursSIPerSubAndDay = hoursSIPerSubAndDay;
                claimDetailsOmbudVM.UnsocialEveningSIPerSubAndDay = unsocialEveningSIPerSubAndDay;
                claimDetailsOmbudVM.UnsocialNightSIPerSubAndDay = unsocialNightSIPerSubAndDay;
                claimDetailsOmbudVM.UnsocialWeekendSIPerSubAndDay = unsocialWeekendSIPerSubAndDay;
                claimDetailsOmbudVM.UnsocialGrandWeekendSIPerSubAndDay = unsocialGrandWeekendSIPerSubAndDay;
                claimDetailsOmbudVM.OnCallDaySIPerSubAndDay = onCallDaySIPerSubAndDay;
                claimDetailsOmbudVM.OnCallNightSIPerSubAndDay = onCallNightSIPerSubAndDay;

                //var claimDays = db.ClaimDays.Where(c => c.ReferenceNumber == claim.ReferenceNumber).OrderBy(c => c.CalendarDayNumber).ToList();
                claimDetailsOmbudVM.ClaimDays = claimDays;
                claimDetailsOmbudVM.NumberOfSickDays = claim.NumberOfSickDays;
                claimDetailsOmbudVM.AdjustedNumberOfSickDays = claim.NumberOfSickDays;
            }

            if (claim.CompletionStage >= 3)
            {
                claimDetailsOmbudVM.Sickpay = claim.ClaimedSickPay;
                claimDetailsOmbudVM.HolidayPay = claim.ClaimedHolidayPay;
                claimDetailsOmbudVM.SocialFees = claim.ClaimedSocialFees;
                claimDetailsOmbudVM.PensionAndInsurance = claim.ClaimedPensionAndInsurance;
                claimDetailsOmbudVM.ClaimSum = claim.ClaimedSum;

                claimDetailsOmbudVM.AdjustedNumberOfSickDays = claim.AdjustedNumberOfSickDays;
                claimDetailsOmbudVM.QualifyingDayDate = claim.QualifyingDayDate;
                claimDetailsOmbudVM.Day2OfSicknessDate = claim.Day2OfSicknessDate;
                claimDetailsOmbudVM.Day14OfSicknessDate = claim.Day14OfSicknessDate;
                claimDetailsOmbudVM.Day15OfSicknessDate = claim.Day15OfSicknessDate;
                claimDetailsOmbudVM.LastDayOfSicknessDate = claim.LastDayOfSicknessDate;

                claimDetailsOmbudVM.QualifyingDayDateAsString = claim.QualifyingDayDateAsString;
                claimDetailsOmbudVM.Day2OfSicknessDateAsString = claim.Day2OfSicknessDateAsString;
                claimDetailsOmbudVM.Day14OfSicknessDateAsString = claim.Day14OfSicknessDateAsString;
                claimDetailsOmbudVM.Day15OfSicknessDateAsString = claim.Day15OfSicknessDateAsString;
                claimDetailsOmbudVM.LastDayOfSicknessDateAsString = claim.LastDayOfSicknessDateAsString;
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
                //Add results from automated checks
                claimDetailsOmbudVM.IVOCheck = claim.IVOCheckMsg;
                claimDetailsOmbudVM.ProxyCheck = claim.ProxyCheckMsg;
                claimDetailsOmbudVM.AssistanceCheck = claim.AssistanceCheckMsg;
                claimDetailsOmbudVM.SalarySpecRegAssistantCheckMsg = claim.SalarySpecRegAssistantCheckMsg;

                claimDetailsOmbudVM.FKRegAssistantCheckMsg = claim.FKRegAssistantCheckMsg;
                claimDetailsOmbudVM.FKSubAssistantCheckMsg = claim.FKSubAssistantCheckMsg;

                if (claim.NumberOfSubAssistants > 1)
                {
                    //FK attachment checks for substitute assistants 2 - 20
                    string[] fkAttachmentSubAssistantsAsString = new string[20];
                    bool[] fkAttachmentSubAssistants = new bool[20];
                    fkAttachmentSubAssistantsAsString = claim.FKSubAssistantCheckBoolConcat.Split('£');

                    for (int i = 0; i < claim.NumberOfSubAssistants - 1; i++)
                    {
                        if (fkAttachmentSubAssistantsAsString[i] == "true")
                        {
                            fkAttachmentSubAssistants[i] = true;
                        }
                        else if (fkAttachmentSubAssistantsAsString[i] == "false")
                        {
                            fkAttachmentSubAssistants[i] = false;
                        }
                    }

                    string[] fkAttachmentSubAssistantsMsg = new string[20];
                    fkAttachmentSubAssistantsMsg = claim.FKSubAssistantCheckMsgConcat.Split('£');

                    claimDetailsOmbudVM.FKSubAssistantCheckBoolArray = fkAttachmentSubAssistants;
                    claimDetailsOmbudVM.FKSubAssistantCheckMsgArray = fkAttachmentSubAssistantsMsg;
                }
                //Not Used
                //claimDetailsOmbudVM.SickleaveNotificationCheckMsg = claim.SickleaveNotificationCheckMsg;

                claimDetailsOmbudVM.MedicalCertificateCheckMsg = claim.MedicalCertificateCheckMsg;
                claimDetailsOmbudVM.RejectReason = claim.RejectReason;

                //Add calculation and results from calculation
                List<ClaimDay> claimDays = new List<ClaimDay>();
                claimDays = db.ClaimDays.Where(c => c.ReferenceNumber == claim.ReferenceNumber).OrderBy(c => c.CalendarDayNumber).ToList();

                //Calculate the model sum
                //if (claimDays.Count() > 0)
                //{
                //    CalculateModelSum(claim, claimDays, startIndex, numberOfDaysToRemove);
                //}

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

                        //Sickpay for qualifying day (only if more than 8,00 hours on that day)
                        claimCalc.SalaryQD = claimCalculations[i].SalaryQD;
                        claimCalc.SalaryCalcQD = claimCalculations[i].SalaryCalcQD;

                        //Holiday pay for qualifying day
                        claimCalc.HolidayPayQD = claimCalculations[i].HolidayPayQD;
                        claimCalc.HolidayPayCalcQD = claimCalculations[i].HolidayPayCalcQD;

                        //Unsocial evening pay for qualifying day
                        claimCalc.UnsocialEveningPayQD = claimCalculations[i].UnsocialEveningPayQD;
                        claimCalc.UnsocialEveningPayCalcQD = claimCalculations[i].UnsocialEveningPayCalcQD;

                        //Unsocial night pay for qualifying day
                        claimCalc.UnsocialNightPayQD = claimCalculations[i].UnsocialNightPayQD;
                        claimCalc.UnsocialNightPayCalcQD = claimCalculations[i].UnsocialNightPayCalcQD;

                        //Unsocial weekend pay for qualifying day
                        claimCalc.UnsocialWeekendPayQD = claimCalculations[i].UnsocialWeekendPayQD;
                        claimCalc.UnsocialWeekendPayCalcQD = claimCalculations[i].UnsocialWeekendPayCalcQD;

                        //Unsocial grand weekend pay for qualifying day
                        claimCalc.UnsocialGrandWeekendPayQD = claimCalculations[i].UnsocialGrandWeekendPayQD;
                        claimCalc.UnsocialGrandWeekendPayCalcQD = claimCalculations[i].UnsocialGrandWeekendPayCalcQD;

                        //Unsocial sum pay for qualifying day
                        claimCalc.UnsocialSumPayQD = claimCalculations[i].UnsocialSumPayQD;
                        claimCalc.UnsocialSumPayCalcQD = claimCalculations[i].UnsocialSumPayCalcQD;

                        //On call day pay for qualifying day
                        claimCalc.OnCallDayPayQD = claimCalculations[i].OnCallDayPayQD;
                        claimCalc.OnCallDayPayCalcQD = claimCalculations[i].OnCallDayPayCalcQD;

                        //On call night pay for qualifying day
                        claimCalc.OnCallNightPayQD = claimCalculations[i].OnCallNightPayQD;
                        claimCalc.OnCallNightPayCalcQD = claimCalculations[i].OnCallNightPayCalcQD;

                        //On call sum pay for qualifying day
                        claimCalc.OnCallSumPayQD = claimCalculations[i].OnCallSumPayQD;
                        claimCalc.OnCallSumPayCalcQD = claimCalculations[i].OnCallSumPayCalcQD;

                        //Sick pay for qualifying day
                        claimCalc.SickPayQD = claimCalculations[i].SickPayQD;
                        claimCalc.SickPayCalcQD = claimCalculations[i].SickPayCalcQD;

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

                    if (claim.AdjustedNumberOfSickDays > 1)
                    {
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
                    }

                    if (claim.AdjustedNumberOfSickDays > 14)
                    {
                        //DAY 15 and beyond
                        claimCalc.HoursD15Plus = "0,00";
                        claimCalc.UnsocialEveningD15Plus = "0,00";
                        claimCalc.UnsocialNightD15Plus = "0,00";
                        claimCalc.UnsocialWeekendD15Plus = "0,00";
                        claimCalc.UnsocialGrandWeekendD15Plus = "0,00";
                        claimCalc.UnsocialSumD15Plus = "0,00";
                        claimCalc.OnCallDayD15Plus = "0,00";
                        claimCalc.OnCallNightD15Plus = "0,00";
                        claimCalc.OnCallSumD15Plus = "0,00";

                        claimCalc.HoursD15Plus = claimCalculations[i].HoursD15Plus;

                        claimCalc.UnsocialEveningD15Plus = claimCalculations[i].UnsocialEveningD15Plus;
                        claimCalc.UnsocialNightD15Plus = claimCalculations[i].UnsocialNightD15Plus;
                        claimCalc.UnsocialWeekendD15Plus = claimCalculations[i].UnsocialWeekendD15Plus;
                        claimCalc.UnsocialGrandWeekendD15Plus = claimCalculations[i].UnsocialGrandWeekendD15Plus;

                        claimCalc.OnCallDayD15Plus = claimCalculations[i].OnCallDayD15Plus;
                        claimCalc.OnCallNightD15Plus = claimCalculations[i].OnCallNightD15Plus;

                        claimCalc.UnsocialSumD15Plus = claimCalculations[i].UnsocialSumD15Plus;
                        claimCalc.OnCallSumD15Plus = claimCalculations[i].OnCallSumD15Plus;

                        //Load the money by category for day 15 and beyond
                        //Sickpay for day 15 and beyond
                        claimCalc.SalaryD15Plus = claimCalculations[i].SalaryD15Plus;
                        claimCalc.SalaryCalcD15Plus = claimCalculations[i].SalaryCalcD15Plus;

                        //Holiday pay for day 15 and beyond
                        claimCalc.HolidayPayD15Plus = claimCalculations[i].HolidayPayD15Plus;
                        claimCalc.HolidayPayCalcD15Plus = claimCalculations[i].HolidayPayCalcD15Plus;

                        //Unsocial evening pay for day 15 and beyond
                        claimCalc.UnsocialEveningPayD15Plus = claimCalculations[i].UnsocialEveningPayD15Plus;
                        claimCalc.UnsocialEveningPayCalcD15Plus = claimCalculations[i].UnsocialEveningPayCalcD15Plus;

                        //Unsocial night pay for day 15 and beyond
                        claimCalc.UnsocialNightPayD15Plus = claimCalculations[i].UnsocialNightPayD15Plus;
                        claimCalc.UnsocialNightPayCalcD15Plus = claimCalculations[i].UnsocialNightPayCalcD15Plus;

                        //Unsocial weekend pay for day 15 and beyond
                        claimCalc.UnsocialWeekendPayD15Plus = claimCalculations[i].UnsocialWeekendPayD15Plus;
                        claimCalc.UnsocialWeekendPayCalcD15Plus = claimCalculations[i].UnsocialWeekendPayCalcD15Plus;

                        //Unsocial grand weekend pay for day 15 and beyond
                        claimCalc.UnsocialGrandWeekendPayD15Plus = claimCalculations[i].UnsocialGrandWeekendPayD15Plus;
                        claimCalc.UnsocialGrandWeekendPayCalcD15Plus = claimCalculations[i].UnsocialGrandWeekendPayCalcD15Plus;

                        //Unsocial sum pay for day 15 and beyond
                        claimCalc.UnsocialSumPayD15Plus = claimCalculations[i].UnsocialSumPayD15Plus;
                        claimCalc.UnsocialSumPayCalcD15Plus = claimCalculations[i].UnsocialSumPayCalcD15Plus;

                        //On call day pay for day 15 and beyond
                        claimCalc.OnCallDayPayD15Plus = claimCalculations[i].OnCallDayPayD15Plus;
                        claimCalc.OnCallDayPayCalcD15Plus = claimCalculations[i].OnCallDayPayCalcD15Plus;

                        //On call night pay for day 15 and beyond
                        claimCalc.OnCallNightPayD15Plus = claimCalculations[i].OnCallNightPayD15Plus;
                        claimCalc.OnCallNightPayCalcD15Plus = claimCalculations[i].OnCallNightPayCalcD15Plus;

                        //On call sum pay for day 15 and beyond
                        claimCalc.OnCallSumPayD15Plus = claimCalculations[i].OnCallSumPayD15Plus;
                        claimCalc.OnCallSumPayCalcD15Plus = claimCalculations[i].OnCallSumPayCalcD15Plus;

                        //Sick pay for day 15 and beyond
                        //claimCalc.SickPayD15Plus = claimCalculations[i].SickPayD15Plus;
                        //claimCalc.SickPayCalcD15Plus = claimCalculations[i].SickPayCalcD15Plus;

                        //Social fees for day 15 and beyond
                        claimCalc.SocialFeesD15Plus = claimCalculations[i].SocialFeesD15Plus;
                        claimCalc.SocialFeesCalcD15Plus = claimCalculations[i].SocialFeesCalcD15Plus;

                        //Pensions and insurances for day 15 and beyond
                        claimCalc.PensionAndInsuranceD15Plus = claimCalculations[i].PensionAndInsuranceD15Plus;
                        claimCalc.PensionAndInsuranceCalcD15Plus = claimCalculations[i].PensionAndInsuranceCalcD15Plus;

                        //Sum for day 15 and beyond
                        claimCalc.CostD15Plus = claimCalculations[i].CostD15Plus;
                        claimCalc.CostCalcD15Plus = claimCalculations[i].CostCalcD15Plus;
                    }

                    claimCalcs.Add(claimCalc);
                }
                claimDetailsOmbudVM.ClaimCalculations = claimCalcs;

                if (claim.AdjustedNumberOfSickDays < 15)
                {
                    //Total sum for day 1 to day 14
                    claimDetailsOmbudVM.TotalCostD1T14 = claim.TotalCostD1T14;
                    claimDetailsOmbudVM.TotalCostCalcD1T14 = claim.TotalCostCalcD1T14;
                }
                else
                {
                    //Total sum for day 15 and beyond
                    claimDetailsOmbudVM.TotalCostD1Plus = claim.TotalCostD1Plus;
                    claimDetailsOmbudVM.TotalCostCalcD1Plus = claim.TotalCostCalcD1Plus;
                }
            }
            return claimDetailsOmbudVM;
        }

        private void CreateClaimPdf(Claim claim)
        {
            ClaimDetailsOmbudVM claimDetailsOmbudVM = CreateVMClaimDetails(claim);

            FileStream fileStream = null;
            try
            {
                // Specify parameters for Page footers in the generated Pdf File 
                string footer = "--footer-right \"Datum: [date] [time]\" " + "--footer-center \"Sida: [page] av [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

                var viewPdf = new Rotativa.ViewAsPdf("ClaimDetailsPdf", claimDetailsOmbudVM)
                {
                    CustomSwitches = footer
                };

                byte[] byteArray = viewPdf.BuildFile(ControllerContext);

                // Save the pdf-document in the same directory as the Claim attachments 
                if (!Directory.Exists(Server.MapPath("~/Uploads")))
                    Directory.CreateDirectory(Server.MapPath("~/Uploads"));
                if (!Directory.Exists(Server.MapPath($"~/Uploads/{claim.ReferenceNumber}")))
                    Directory.CreateDirectory(Server.MapPath($"~/Uploads/{claim.ReferenceNumber}"));

                string path = Server.MapPath($"~/Uploads/{claim.ReferenceNumber}");
                string title = "Ansökan";

                fileStream = new FileStream(Path.Combine(path, $"{title}_{claim.ReferenceNumber}.pdf"), FileMode.Create, FileAccess.Write);
                fileStream.Write(byteArray, 0, byteArray.Length);

                TempData["PdfSuccess"] = "Pdf-dokument har skapats för ansökan med Referensnummer: " + claim.ReferenceNumber;
            }
            catch (Exception ex)
            {
                TempData["PdfFail"] = "Det misslyckades att skapa Pdf-dokument. Exception: " + ex.ToString();
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        //public ActionResult ShowClaimDetails(string referenceNumber)
        //{
        //    var claim = db.Claims.Include(c => c.ClaimStatus).Where(c => c.ReferenceNumber == referenceNumber).FirstOrDefault();

        //    List<ClaimDay> claimDays = new List<ClaimDay>();
        //    claimDays = db.ClaimDays.Where(c => c.ReferenceNumber == referenceNumber).OrderBy(c => c.CalendarDayNumber).ToList();

        //    //Calculate the model sum
        //    if (claimDays.Count() > 0)
        //    {
        //        CalculateModelSum(claim, claimDays, null, null);
        //    }

        //    ClaimDetailsVM claimDetailsVM = new ClaimDetailsVM();

        //    claimDetailsVM.ReferenceNumber = referenceNumber;
        //    claimDetailsVM.StatusName = claim.ClaimStatus.Name;
        //    claimDetailsVM.DefaultCollectiveAgreement = claim.DefaultCollectiveAgreement;

        //    //Kommun
        //    claimDetailsVM.Council = "Helsingborgs kommun";
        //    claimDetailsVM.Administration = "Vård- och omsorgsförvaltningen";

        //    //Assistansberättigad
        //    claimDetailsVM.CustomerName = claim.CustomerName;
        //    claimDetailsVM.CustomerSSN = claim.CustomerSSN;
        //    //claimDetailsVM.CustomerAddress = claim.CustomerAddress;
        //    claimDetailsVM.CustomerPhoneNumber = claim.CustomerPhoneNumber;

        //    //Ombud/uppgiftslämnare
        //    claimDetailsVM.OmbudName = claim.OmbudFirstName + " " + claim.OmbudLastName;
        //    claimDetailsVM.OmbudPhoneNumber = claim.OmbudPhoneNumber;

        //    //Assistansanordnare
        //    claimDetailsVM.CompanyName = claim.CompanyName; ;
        //    claimDetailsVM.OrganisationNumber = claim.OrganisationNumber;
        //    claimDetailsVM.GiroNumber = claim.AccountNumber;
        //    claimDetailsVM.CompanyAddress = claim.StreetAddress;
        //    claimDetailsVM.CompanyPhoneNumber = claim.CompanyPhoneNumber;
        //    claimDetailsVM.CollectiveAgreement = claim.CollectiveAgreementName + ", " + claim.CollectiveAgreementSpecName;
        //    claimDetailsVM.Workplace = "Björkängen, Birgittagården"; //This can probably be removed

        //    //Insjuknad ordinarie assistent
        //    //Källa till belopp: https://assistanskoll.se/Guider-Att-arbeta-som-personlig-assistent.html (Vårdföretagarna)
        //    claimDetailsVM.AssistantName = claim.RegFirstName + " " + claim.RegLastName;
        //    claimDetailsVM.AssistantSSN = claim.RegAssistantSSN;
        //    claimDetailsVM.QualifyingDayDate = claim.QualifyingDate.ToShortDateString();
        //    claimDetailsVM.LastDayOfSicknessDate = claim.LastDayOfSicknessDate.ToShortDateString();

        //    claimDetailsVM.NumberOfSickDays = claim.NumberOfSickDays;

        //    claimDetailsVM.Salary = claim.HourlySalary;  //This property is used either as an hourly salary or as a monthly salary in claimDetailsVM.cs.
        //    claimDetailsVM.HourlySalary = claim.HourlySalary;    //This property is used as the hourly salary in calculations.
        //    claimDetailsVM.Sickpay = claim.ClaimedSickPay;
        //    claimDetailsVM.HolidayPay = claim.ClaimedHolidayPay;
        //    claimDetailsVM.SocialFees = claim.ClaimedSocialFees;
        //    claimDetailsVM.PensionAndInsurance = claim.ClaimedPensionAndInsurance;

        //    claimDetailsVM.NumberOfAbsenceHours = claim.NumberOfAbsenceHours;
        //    claimDetailsVM.NumberOfOrdinaryHours = claim.NumberOfOrdinaryHours;
        //    claimDetailsVM.NumberOfUnsocialHours = claim.NumberOfUnsocialHours;
        //    claimDetailsVM.NumberOfOnCallHours = claim.NumberOfOnCallHours;

        //    claimDetailsVM.NumberOfHoursWithSI = claim.NumberOfHoursWithSI;
        //    claimDetailsVM.NumberOfOrdinaryHoursSI = claim.NumberOfOrdinaryHoursSI;
        //    claimDetailsVM.NumberOfUnsocialHoursSI = claim.NumberOfUnsocialHoursSI;
        //    claimDetailsVM.NumberOfOnCallHoursSI = claim.NumberOfOnCallHoursSI;

        //    claimDetailsVM.ClaimSum = claim.ClaimedSum;

        //    claimDetailsVM.DecisionMade = false;
        //    if (claim.ClaimStatus.Name == "Beslutad")
        //    {
        //        claimDetailsVM.ApprovedSum = claim.ApprovedSum;
        //        claimDetailsVM.RejectedSum = claim.RejectedSum;
        //        claimDetailsVM.DecisionMade = true;
        //    }

        //    //Underlag lönekostnader
        //    claimDetailsVM.PerHourUnsocialEvening = claim.PerHourUnsocialEvening;
        //    claimDetailsVM.PerHourUnsocialNight = claim.PerHourUnsocialNight;
        //    claimDetailsVM.PerHourUnsocialWeekend = claim.PerHourUnsocialWeekend;
        //    claimDetailsVM.PerHourUnsocialHoliday = claim.PerHourUnsocialHoliday;
        //    claimDetailsVM.PerHourOnCallWeekday = claim.PerHourOnCallWeekday;
        //    claimDetailsVM.PerHourOnCallWeekend = claim.PerHourOnCallWeekend;


        //    claimDetailsVM.HolidayPayRate = claim.HolidayPayRate;
        //    claimDetailsVM.SocialFeeRate = claim.SocialFeeRate;
        //    claimDetailsVM.PensionAndInsuranceRate = claim.PensionAndInsuranceRate;
        //    claimDetailsVM.SickPayRate = claim.SickPayRate;

        //    claimDetailsVM.SickPayRateAsString = claim.SickPayRateAsString;
        //    claimDetailsVM.HolidayPayRateAsString = claim.HolidayPayRateAsString;
        //    claimDetailsVM.SocialFeeRateAsString = claim.SocialFeeRateAsString;
        //    claimDetailsVM.PensionAndInsuranceRateAsString = claim.PensionAndInsuranceRateAsString;
        //    claimDetailsVM.HourlySalaryAsString = claim.HourlySalaryAsString;

        //    var claimCalculations = db.ClaimCalculations.Where(c => c.ReferenceNumber == referenceNumber).OrderBy(c => c.StartDate).ToList();
        //    List<ClaimCalculation> claimCalcs = new List<ClaimCalculation>();
        //    for (int i = 0; i < claimCalculations.Count(); i++)
        //    {
        //        ClaimCalculation claimCalc = new ClaimCalculation();
        //        claimCalc.StartDate = claimCalculations[i].StartDate.Date;
        //        claimCalc.EndDate = claimCalculations[i].EndDate.Date;

        //        claimCalc.PerHourUnsocialEvening = claimCalculations[i].PerHourUnsocialEvening;
        //        claimCalc.PerHourUnsocialNight = claimCalculations[i].PerHourUnsocialNight;
        //        claimCalc.PerHourUnsocialWeekend = claimCalculations[i].PerHourUnsocialWeekend;
        //        claimCalc.PerHourUnsocialHoliday = claimCalculations[i].PerHourUnsocialHoliday;
        //        claimCalc.PerHourOnCallWeekday = claimCalculations[i].PerHourOnCallWeekday;
        //        claimCalc.PerHourOnCallWeekend = claimCalculations[i].PerHourOnCallWeekend;

        //        if (i == 0)
        //        {
        //            //QUALIFYING DAY

        //            //Hours for qualifying day
        //            claimCalc.HoursQD = claimCalculations[i].HoursQD;

        //            //Holiday pay for qualifying day
        //            claimCalc.HolidayPayQD = claimCalculations[i].HolidayPayQD;
        //            claimCalc.HolidayPayCalcQD = claimCalculations[i].HolidayPayCalcQD;

        //            //Social fees for qualifying day
        //            claimCalc.SocialFeesQD = claimCalculations[i].SocialFeesQD;
        //            claimCalc.SocialFeesCalcQD = claimCalculations[i].SocialFeesCalcQD;

        //            //Pension and insurance for qualifying day
        //            claimCalc.PensionAndInsuranceQD = claimCalculations[i].PensionAndInsuranceQD;
        //            claimCalc.PensionAndInsuranceCalcQD = claimCalculations[i].PensionAndInsuranceCalcQD;

        //            //Sum for qualifying day (sum of the three previous items)
        //            claimCalc.CostQD = claimCalculations[i].CostQD;
        //            claimCalc.CostCalcQD = claimCalculations[i].CostCalcQD;
        //        }

        //        //DAY 2 TO DAY 14
        //        claimCalc.HoursD2T14 = "0,00";
        //        claimCalc.UnsocialEveningD2T14 = "0,00";
        //        claimCalc.UnsocialNightD2T14 = "0,00";
        //        claimCalc.UnsocialWeekendD2T14 = "0,00";
        //        claimCalc.UnsocialGrandWeekendD2T14 = "0,00";
        //        claimCalc.UnsocialSumD2T14 = "0,00";
        //        claimCalc.OnCallDayD2T14 = "0,00";
        //        claimCalc.OnCallNightD2T14 = "0,00";
        //        claimCalc.OnCallSumD2T14 = "0,00";

        //        claimCalc.HoursD2T14 = claimCalculations[i].HoursD2T14;

        //        claimCalc.UnsocialEveningD2T14 = claimCalculations[i].UnsocialEveningD2T14;
        //        claimCalc.UnsocialNightD2T14 = claimCalculations[i].UnsocialNightD2T14;
        //        claimCalc.UnsocialWeekendD2T14 = claimCalculations[i].UnsocialWeekendD2T14;
        //        claimCalc.UnsocialGrandWeekendD2T14 = claimCalculations[i].UnsocialGrandWeekendD2T14;

        //        claimCalc.OnCallDayD2T14 = claimCalculations[i].OnCallDayD2T14;
        //        claimCalc.OnCallNightD2T14 = claimCalculations[i].OnCallNightD2T14;

        //        claimCalc.UnsocialSumD2T14 = claimCalculations[i].UnsocialSumD2T14;
        //        claimCalc.OnCallSumD2T14 = claimCalculations[i].OnCallSumD2T14;

        //        //These numbers go to the assistant's part of the view
        //        claimDetailsVM.NumberOfAbsenceHours = claim.NumberOfAbsenceHours;
        //        claimDetailsVM.NumberOfOrdinaryHours = claim.NumberOfOrdinaryHours;
        //        claimDetailsVM.NumberOfUnsocialHours = claim.NumberOfUnsocialHours;
        //        claimDetailsVM.NumberOfOnCallHours = claim.NumberOfOnCallHours;

        //        //Load the money by category for day 2 to day 14
        //        //Sickpay for day 2 to day 14
        //        claimCalc.SalaryD2T14 = claimCalculations[i].SalaryD2T14;
        //        claimCalc.SalaryCalcD2T14 = claimCalculations[i].SalaryCalcD2T14;

        //        //Holiday pay for day 2 to day 14
        //        claimCalc.HolidayPayD2T14 = claimCalculations[i].HolidayPayD2T14;
        //        claimCalc.HolidayPayCalcD2T14 = claimCalculations[i].HolidayPayCalcD2T14;

        //        //Unsocial evening pay for day 2 to day 14
        //        claimCalc.UnsocialEveningPayD2T14 = claimCalculations[i].UnsocialEveningPayD2T14;
        //        claimCalc.UnsocialEveningPayCalcD2T14 = claimCalculations[i].UnsocialEveningPayCalcD2T14;

        //        //Unsocial night pay for day 2 to day 14
        //        claimCalc.UnsocialNightPayD2T14 = claimCalculations[i].UnsocialNightPayD2T14;
        //        claimCalc.UnsocialNightPayCalcD2T14 = claimCalculations[i].UnsocialNightPayCalcD2T14;

        //        //Unsocial weekend pay for day 2 to day 14
        //        claimCalc.UnsocialWeekendPayD2T14 = claimCalculations[i].UnsocialWeekendPayD2T14;
        //        claimCalc.UnsocialWeekendPayCalcD2T14 = claimCalculations[i].UnsocialWeekendPayCalcD2T14;

        //        //Unsocial grand weekend pay for day 2 to day 14
        //        claimCalc.UnsocialGrandWeekendPayD2T14 = claimCalculations[i].UnsocialGrandWeekendPayD2T14;
        //        claimCalc.UnsocialGrandWeekendPayCalcD2T14 = claimCalculations[i].UnsocialGrandWeekendPayCalcD2T14;

        //        //Unsocial sum pay for day 2 to day 14
        //        claimCalc.UnsocialSumPayD2T14 = claimCalculations[i].UnsocialSumPayD2T14;
        //        claimCalc.UnsocialSumPayCalcD2T14 = claimCalculations[i].UnsocialSumPayCalcD2T14;

        //        //On call day pay for day 2 to day 14
        //        claimCalc.OnCallDayPayD2T14 = claimCalculations[i].OnCallDayPayD2T14;
        //        claimCalc.OnCallDayPayCalcD2T14 = claimCalculations[i].OnCallDayPayCalcD2T14;

        //        //On call night pay for day 2 to day 14
        //        claimCalc.OnCallNightPayD2T14 = claimCalculations[i].OnCallNightPayD2T14;
        //        claimCalc.OnCallNightPayCalcD2T14 = claimCalculations[i].OnCallNightPayCalcD2T14;

        //        //On call sum pay for day 2 to day 14
        //        claimCalc.OnCallSumPayD2T14 = claimCalculations[i].OnCallSumPayD2T14;
        //        claimCalc.OnCallSumPayCalcD2T14 = claimCalculations[i].OnCallSumPayCalcD2T14;

        //        //Sick pay for day 2 to day 14
        //        claimCalc.SickPayD2T14 = claimCalculations[i].SickPayD2T14;
        //        claimCalc.SickPayCalcD2T14 = claimCalculations[i].SickPayCalcD2T14;

        //        //Social fees for day 2 to day 14
        //        claimCalc.SocialFeesD2T14 = claimCalculations[i].SocialFeesD2T14;
        //        claimCalc.SocialFeesCalcD2T14 = claimCalculations[i].SocialFeesCalcD2T14;

        //        //Pensions and insurances for day 2 to day 14
        //        claimCalc.PensionAndInsuranceD2T14 = claimCalculations[i].PensionAndInsuranceD2T14;
        //        claimCalc.PensionAndInsuranceCalcD2T14 = claimCalculations[i].PensionAndInsuranceCalcD2T14;

        //        //Sum for day 2 to day 14
        //        claimCalc.CostD2T14 = claimCalculations[i].CostD2T14;
        //        claimCalc.CostCalcD2T14 = claimCalculations[i].CostCalcD2T14;

        //        claimCalcs.Add(claimCalc);
        //    }
        //    claimDetailsVM.ClaimCalculations = claimCalcs;

        //    //Total sum for day 1 to day 14
        //    claimDetailsVM.TotalCostD1T14 = claim.TotalCostD1T14;
        //    claimDetailsVM.TotalCostCalcD1T14 = claim.TotalCostCalcD1T14;

        //    claimDetailsVM.Documents = claim.Documents;

        //    claimDetailsVM.messages = db.Messages.Where(c => c.ClaimId == claim.Id).ToList();

        //    return View("ClaimDetails", claimDetailsVM);
        //}

        // POST: Claims/Decide/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "AdministrativeOfficial")]
        public ActionResult Recommend(RecommendationVM recommendationVM)
        {
            var claim = db.Claims.Where(c => c.ReferenceNumber == recommendationVM.ClaimNumber).FirstOrDefault();
            if (claim != null)
            {
                claim.ApprovedSum = Convert.ToDecimal(recommendationVM.ApprovedSum);
                claim.RejectedSum = Convert.ToDecimal(recommendationVM.RejectedSum);
                claim.RejectReason = recommendationVM.RejectReason;

                // Assign this Claim to the current Administrative Official               
                if (User.IsInRole("AdministrativeOfficial"))
                {
                    var me = db.Users.Find(User.Identity.GetUserId());

                    claim.AdmOffId = me.Id;
                    claim.AdmOffName = me.FirstName + " " + me.LastName;
                }

                db.Entry(claim).State = EntityState.Modified;
                db.SaveChanges();

                ConfirmTransferVM confirmTransferVM = new ConfirmTransferVM();
                confirmTransferVM.ClaimId = claim.Id;
                confirmTransferVM.ReferenceNumber = claim.ReferenceNumber;
                confirmTransferVM.CustomerSSN = claim.CustomerSSN;
                confirmTransferVM.FirstClaimDate = claim.FirstClaimDate;
                confirmTransferVM.LastClaimDate = claim.LastClaimDate;
                confirmTransferVM.ClaimedSum = claim.ClaimedSum;
                confirmTransferVM.ModelSum = claim.ModelSum;
                confirmTransferVM.ApprovedSum = claim.ApprovedSum;
                confirmTransferVM.RejectedSum = claim.RejectedSum;
                confirmTransferVM.RejectReason = claim.RejectReason;

                return View("ConfirmTransfer", confirmTransferVM);
            }
            return RedirectToAction("IndexPageAdmOff", "Claims");

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
                writer.Dispose();
            }
            return View("RecommendationReceipt", recommendationVM);
        }

        // GET: Claims/Transfer
        [HttpGet]
        [Authorize(Roles = "AdministrativeOfficial")]
        public ActionResult Transfer(string refNumber)
        {
            var claim = db.Claims.Where(rn => rn.ReferenceNumber == refNumber).FirstOrDefault();
            ConfirmTransferVM confirmTransferVM = new ConfirmTransferVM();
            confirmTransferVM.ClaimId = claim.Id;
            confirmTransferVM.ReferenceNumber = claim.ReferenceNumber;
            confirmTransferVM.CustomerSSN = claim.CustomerSSN;
            confirmTransferVM.FirstClaimDate = claim.FirstClaimDate;
            confirmTransferVM.LastClaimDate = claim.LastClaimDate;
            confirmTransferVM.ClaimedSum = claim.ClaimedSum;
            confirmTransferVM.ModelSum = claim.ModelSum;
            confirmTransferVM.ApprovedSum = claim.ApprovedSum;
            confirmTransferVM.RejectedSum = claim.RejectedSum;
            confirmTransferVM.RejectReason = claim.RejectReason;

            return View("ConfirmTransfer", confirmTransferVM);
        }

        // POST: Claims/ConfirmTransfer
        [HttpPost, ActionName("Transfer")]
        [Authorize(Roles = "AdministrativeOfficial")]
        public ActionResult ConfirmTransfer(int id, string submitButton)
        {
            var claim = db.Claims.Find(id);
            string refNumber = claim.ReferenceNumber;
            if (submitButton == "Bekräfta")
            {
                string sentInDate = claim.SentInDate.ToString().Substring(2, 8).Replace("-", "");

                using (var writer = XmlWriter.Create(Server.MapPath("\\sjukloner" + "\\" + "transfer" + refNumber + ".xml")))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("claiminformation");
                    writer.WriteElementString("ReferenceNumber", refNumber);
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }

                claim.FirstClaimDateAsString = claim.FirstClaimDate.ToShortDateString().ToString().Remove(4, 1);
                claim.FirstClaimDateAsString = claim.FirstClaimDateAsString.Remove(6, 1);
                claim.LastClaimDateAsString = claim.LastClaimDate.ToShortDateString().ToString().Remove(4, 1);
                claim.LastClaimDateAsString = claim.LastClaimDateAsString.Remove(6, 1);
                claim.SentInDateAsString = DateTime.Now.ToShortDateString().ToString().Remove(4, 1);
                claim.SentInDateAsString = claim.SentInDateAsString.Remove(6, 1);
                claim.ClaimedSumAsString = String.Format("{0:0.00}", claim.ClaimedSum);
                claim.ModelSumAsString = String.Format("{0:0.00}", claim.ModelSum);
                claim.ApprovedSumAsString = String.Format("{0:0.00}", claim.ApprovedSum);
                claim.RejectedSumAsString = String.Format("{0:0.00}", claim.RejectedSum);

                claim.TransferToProcapitaString = "transferinfo" + claim.ReferenceNumber + "+" + claim.FirstClaimDateAsString + "+" + claim.LastClaimDateAsString + "+" + claim.SentInDateAsString + "+" + claim.RejectReason + "+" +
                    claim.ClaimedSumAsString + "+" + claim.ModelSumAsString + "+" + claim.ApprovedSumAsString + "+" + claim.RejectedSumAsString + "+" +
                    claim.IVOCheckMsg + "+" + claim.ProxyCheckMsg + "+" + claim.AssistanceCheckMsg + "+" + claim.SalarySpecRegAssistantCheckMsg + "+" + claim.SickleaveNotificationCheckMsg + "+" +
                    claim.MedicalCertificateCheckMsg + "+" + claim.FKRegAssistantCheckMsg + "+" + claim.FKSubAssistantCheckMsg + "+" + claim.NumberOfCalendarDays.ToString() + "+" +
                    claim.CustomerSSN.Substring(2) + "+" + claim.CustomerName;

                string[] subAssistantBools = claim.FKSubAssistantCheckBoolConcat.Split('£');
                string[] subAssistantCheckMsgs = claim.FKSubAssistantCheckMsgConcat.Split('£');
                for (int idx = 0; idx < subAssistantBools.Length; idx++)
                {
                    claim.TransferToProcapitaString = claim.TransferToProcapitaString + "+" + subAssistantBools[idx];
                }
                for (int idx = 0; idx < subAssistantBools.Length; idx++)
                {
                    claim.TransferToProcapitaString = claim.TransferToProcapitaString + "+" + subAssistantCheckMsgs[idx];
                }

                claim.ClaimStatusId = 7;      // Transfer to Procapita started
                claim.BasisForDecisionTransferStartTimeStamp = DateTime.Now;
                claim.StatusDate = DateTime.Now;

                db.Entry(claim).State = EntityState.Modified;
                db.SaveChanges();
                //return RedirectToAction("IndexPageAdmOff", "Claims");
                return RedirectToAction("Recommend", new { id });

            }
            return RedirectToAction("Recommend", new { id });
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
            decisionVM.FirstClaimDate = claim.FirstClaimDate;
            decisionVM.LastClaimDate = claim.LastClaimDate;
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
            claim.DecisionDate = DateTime.Now;
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

                //SendEmail(message);
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
                writer.Dispose();
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
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return RedirectToAction("Index", "Home");
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
        [Authorize(Roles = "Ombud, Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return RedirectToAction("Index", "Home");
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
        [Authorize(Roles = "Ombud, Admin")]
        public ActionResult DeleteConfirmed(string refNumber, string submitButton)
        {
            if (submitButton == "Bekräfta")
            {
                Claim claim = db.Claims.Where(c => c.ReferenceNumber == refNumber).FirstOrDefault();

                if (User.IsInRole("Admin"))
                {
                    // Only Applications with StatusId == 1,3,5 can be removed by Admin
                    if (claim.ClaimStatusId == 1 | claim.ClaimStatusId == 3 | claim.ClaimStatusId == 5)
                    {
                        db.ClaimDays.RemoveRange(db.ClaimDays.Where(c => c.ReferenceNumber == claim.ReferenceNumber));
                        db.ClaimCalculations.RemoveRange(db.ClaimCalculations.Where(c => c.ReferenceNumber == claim.ReferenceNumber));

                        if (claim.Documents.Count() > 0)
                        {
                            db.Documents.RemoveRange(db.Documents.Where(d => d.ReferenceNumber == refNumber));
                        }

                        db.Claims.Remove(claim);
                        db.SaveChanges();
                    }
                }

                if (User.IsInRole("Ombud"))
                {
                    // Only Applications with StatusId == 2 can be removed by Ombud
                    if (claim.ClaimStatusId == 2)
                    {
                        var me = db.Users.Find(User.Identity.GetUserId());
                        // Only Applications from same Company can be removed by Ombud
                        if (claim.CareCompanyId == me.CareCompanyId)
                        {
                            if (claim.CompletionStage > 1)
                            {
                                db.ClaimDays.RemoveRange(db.ClaimDays.Where(c => c.ReferenceNumber == claim.ReferenceNumber));
                            }
                            if (claim.CompletionStage >= 2)
                            {
                                db.ClaimCalculations.RemoveRange(db.ClaimCalculations.Where(c => c.ReferenceNumber == claim.ReferenceNumber));
                            }
                            if (claim.CompletionStage >= 3)
                            {
                                if (claim.Documents.Count() > 0)
                                {
                                    db.Documents.RemoveRange(db.Documents.Where(d => d.ReferenceNumber == refNumber));
                                }
                            }
                            db.Claims.Remove(claim);
                            db.SaveChanges();
                        }
                    }
                }

            }
            return RedirectToAction("Index");
        }

        private bool OverlappingClaim(string referenceNumber, DateTime firstDayOfSicknessDate, DateTime lastDayOfSicknessDate, string CustomerSSN, string AssistantSSN)
        {
            //Check if a claim for overlapping dates already exists for the same customer.
            List<Claim> claims = new List<Claim>();
            if (referenceNumber == null) //New claim
            {
                claims = db.Claims.Where(c => c.CustomerSSN == CustomerSSN).Where(c => c.RegAssistantSSN == AssistantSSN).ToList();
            }
            else //Update of existing claim. The already stored claim must be excluded from the list of claims to be checked for overlaps
            {
                claims = db.Claims.Where(c => c.CustomerSSN == CustomerSSN).Where(c => c.ReferenceNumber != referenceNumber).ToList();
            }
            if (claims != null)
            {
                foreach (var claim in claims)
                {
                    if (firstDayOfSicknessDate.Date <= claim.LastClaimDate.Date && firstDayOfSicknessDate.Date >= claim.FirstClaimDate.Date)
                    {
                        return true;
                    }
                    if (lastDayOfSicknessDate.Date <= claim.LastClaimDate.Date && lastDayOfSicknessDate.Date >= claim.FirstClaimDate.Date)
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

        [AllowAnonymous]
        private void CalculateModelSum(Claim claim, List<ClaimDay> claimDays, int? startIndex, int? numberOfDaysToRemove)
        {
            //The optional parameters startIndex and numberOfDaysToRemove are only needed if the decision about personal assistance does not cover the whole sickleave period.
            //In that case only a subset of the claimdays shall be included in the model sum calculation.
            //The parameter startIndex tells from which (zero based) index claimdays in the claimDays list shall be excluded from the calculation.
            //The parameter numberOfDaysToRemove tells how many claimdays shall be removed from the calculation of the model sum (starting from startIndex).

            //These declarations are needed in order for this method to be able to handle edge cases where the decision about personal assistance only
            //covers a part of the sickleave period
            //DateTime adjustedLastDayOfSickness = new DateTime();
            //adjustedLastDayOfSickness = claim.LastClaimDate;
            //DateTime adjustedQualifyingDay = new DateTime();
            //adjustedQualifyingDay = claim.FirstClaimDate;

            DateTime adjustedLastClaimDate = new DateTime();
            adjustedLastClaimDate = claim.LastClaimDate;
            DateTime adjustedFirstClaimDate = new DateTime();
            adjustedFirstClaimDate = claim.FirstClaimDate;
            int adjustedNumberOfCalendarDays = claim.NumberOfCalendarDays;
            int adjustedNumberOfSickdays = claim.NumberOfSickDays;
            int calendarDayNumberOffset = 0;

            int adjustedStartCalendarDayIdx = 0;
            int adjustedStopCalendarDayIdx = claim.NumberOfCalendarDays - 1;

            bool day1T14InThisCollAgreement = false;
            bool day15PlusInThisCollAgreement = false;

            //if (startIndex != null && numberOfDaysToRemove != null)
            ////This is only true if the decision about personal assistance only covers a part of the sickleave period. In that case not all claimdays shall be included in the model sum calculation.
            //{
            //    claimDays.RemoveRange((int)startIndex, (int)numberOfDaysToRemove);
            //    db.SaveChanges();
            //    if (startIndex > 0) //This means that the decision about personal assistance covers one or more days of the sickleave period starting from claim.QualifyingDate (1st day of sickness), but it does not cover the whole sickleave period.
            //    {
            //        adjustedLastDayOfSickness = claim.LastClaimDate.AddDays(-(int)numberOfDaysToRemove);
            //    }
            //    else //This means that the decision about personal assistance covers one or more days at the end of the sickleave period, but not the whole period.
            //    {
            //        adjustedQualifyingDay = claim.FirstClaimDate.AddDays((int)numberOfDaysToRemove);
            //        calendarDayNumberOffset = (int)numberOfDaysToRemove;
            //    }
            //    adjustedNumberOfCalendarDays = claim.NumberOfCalendarDays - (int)numberOfDaysToRemove;

            //    //Update SickDayNumber in claimdays. Very likely the previous numbering is not valid after removing days the sickleave period.
            //    var adjustedClaimDays = db.ClaimDays.Where(c => c.ReferenceNumber == claim.ReferenceNumber).OrderBy(c => c.CalendarDayNumber).ToList();
            //    claim.NumberOfCalendarDays = adjustedClaimDays.Count(); //Is this line needed/necessary?
            //    db.Entry(claim).State = EntityState.Modified;

            //    int sickDayCounter = 1;
            //    foreach (var day in adjustedClaimDays)
            //    {
            //        if (!day.Well)
            //        {
            //            day.SickDayNumber = sickDayCounter;
            //            sickDayCounter++;
            //            db.Entry(day).State = EntityState.Modified;
            //        }
            //        else
            //        {
            //            day.SickDayNumber = null;
            //        }
            //    }
            //    db.SaveChanges();
            //}

            int qdIdx = 0; //index of the qualifying day
            int day2Idx = 0;
            int day14Idx = 0;
            int day15Idx = 0;
            int lastDayIdx = 0;

            if (startIndex != null && numberOfDaysToRemove != null)
            //This is only true if the decision about personal assistance only covers a part of the sickleave period. In that case not all claimdays shall be included in the model sum calculation.
            {
                //claimDays.RemoveRange((int)startIndex, (int)numberOfDaysToRemove);
                //db.SaveChanges();
                if (startIndex > 0) //This means that the decision about personal assistance covers one or more days of the sickleave period starting from claim.QualifyingDate (1st day of sickness), but it does not cover the whole sickleave period.
                {
                    adjustedLastClaimDate = claim.LastClaimDate.AddDays(-(int)numberOfDaysToRemove);
                }
                else //This means that the decision about personal assistance covers one or more days at the end of the sickleave period, but not the whole period.
                {
                    adjustedFirstClaimDate = claim.FirstClaimDate.AddDays((int)numberOfDaysToRemove);
                    calendarDayNumberOffset = (int)numberOfDaysToRemove;
                }
                adjustedNumberOfCalendarDays = claim.NumberOfCalendarDays - (int)numberOfDaysToRemove;

                //Update SickDayNumber in claimdays. Very likely the previous numbering is not valid after removing days the sickleave period.
                //var adjustedClaimDays = db.ClaimDays.Where(c => c.ReferenceNumber == claim.ReferenceNumber).OrderBy(c => c.CalendarDayNumber).ToList();
                //claim.NumberOfCalendarDays = adjustedClaimDays.Count(); //Is this line needed/necessary?
                db.Entry(claim).State = EntityState.Modified;


                if (startIndex == 0)
                {
                    adjustedStartCalendarDayIdx = (int)numberOfDaysToRemove;
                    adjustedStopCalendarDayIdx = claim.NumberOfCalendarDays - 1;
                }
                else
                {
                    adjustedStartCalendarDayIdx = 0;
                    adjustedStopCalendarDayIdx = claim.NumberOfCalendarDays - (int)numberOfDaysToRemove - 1;
                }

                //Reset the sickday numbering in claimDays
                foreach (var day in claimDays)
                {
                    day.SickDayNumber = null;
                    db.Entry(day).State = EntityState.Modified;
                }
                db.SaveChanges();

                //Update claimDays with adjusted sickday numbering
                int sickDayCounter = 1;
                int dayIdx = 0;
                for (int i = adjustedStartCalendarDayIdx; i <= adjustedStopCalendarDayIdx; i++)
                {
                    if (!claimDays[i].Well)
                    {
                        switch (sickDayCounter)
                        {
                            case 1:
                                qdIdx = dayIdx;
                                break;
                            case 2:
                                day2Idx = dayIdx;
                                break;
                            case 14:
                                day14Idx = dayIdx;
                                break;
                            case 15:
                                day15Idx = dayIdx;
                                break;
                            default:
                                break;
                        }
                        lastDayIdx = dayIdx;
                        claimDays[i].SickDayNumber = sickDayCounter;
                        sickDayCounter++;
                        db.Entry(claimDays[i]).State = EntityState.Modified;
                    }
                }
                adjustedNumberOfSickdays = lastDayIdx;

                //Update indexes for sickdays in claim record
                claim.QualifyingDayDate = claimDays[qdIdx].Date;
                claim.QualifyingDayDateAsString = claimDays[qdIdx].Date.ToShortDateString();

                if (day2Idx != 0)
                {
                    claim.Day2OfSicknessDate = claimDays[day2Idx].Date;
                    claim.Day2OfSicknessDateAsString = claimDays[day2Idx].Date.ToShortDateString();
                }
                if (day14Idx != 0)
                {
                    claim.Day14OfSicknessDate = claimDays[day14Idx].Date;
                    claim.Day14OfSicknessDateAsString = claimDays[day14Idx].Date.ToShortDateString();
                }
                if (day15Idx != 0)
                {
                    claim.Day15OfSicknessDate = claimDays[day15Idx].Date;
                    claim.Day2OfSicknessDateAsString = claimDays[day15Idx].Date.ToShortDateString();
                }
                claim.LastDayOfSicknessDate = claimDays[lastDayIdx].Date;
                claim.LastDayOfSicknessDateAsString = claimDays[lastDayIdx].Date.ToShortDateString();

                claim.AdjustedNumberOfSickDays = adjustedNumberOfSickdays;
                db.Entry(claim).State = EntityState.Modified;
                db.SaveChanges();
            }

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
                claimDays.OrderBy(c => c.CalendarDayNumber); //Should this be adjustedClaimDays?
                do
                {
                    claimDayDate = adjustedFirstClaimDate.AddDays(claimDays[claimDayIdx].CalendarDayNumber - 1 - calendarDayNumberOffset); //Should this be adjustedClaimDays?
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
                } while (claimDayIdx < adjustedNumberOfCalendarDays && !useDefaultCollectiveAgreement);
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
                    if (collectiveAgreementInfos[idx].EndDate.Date >= adjustedLastClaimDate.Date)
                    {
                        applicableSickDays = 1 + (adjustedLastClaimDate.Date - adjustedFirstClaimDate).Days - prevSickDayIdx;
                    }
                    else
                    {
                        applicableSickDays = 1 + (collectiveAgreementInfos[idx].EndDate - adjustedFirstClaimDate).Days - prevSickDayIdx;
                    }
                    prevSickDayIdx = prevSickDayIdx + applicableSickDays;
                }
                if (applicableSickDays < adjustedNumberOfCalendarDays)
                {
                    useDefaultCollectiveAgreement = true;
                }
            }
            applicableSickDays = 0;
            prevSickDayIdx = 0;

            //Repeat the calculation for each CollectiveAgreementInfo that applies to the sickleave period
            int startIdx;
            int stopIdx;
            //These two variables are used if the number of sickdays is less than 15
            decimal totalCostD1D14 = 0;
            string totalCostCalcD1D14 = "";

            //These two variables are used for the total cost if the numberr of sickdays is greater than 14
            decimal totalCostD1Plus = 0;
            string totalCostCalcD1Plus = "";

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
                    if (collectiveAgreementInfos[idx].EndDate.Date >= adjustedLastClaimDate.Date)
                    {
                        applicableSickDays = 1 + (adjustedLastClaimDate.Date - adjustedFirstClaimDate).Days - prevSickDayIdx;
                    }
                    else
                    {
                        applicableSickDays = 1 + (collectiveAgreementInfos[idx].EndDate - adjustedFirstClaimDate).Days - prevSickDayIdx;
                    }
                    claimCalculation.StartDate = adjustedFirstClaimDate.AddDays(prevSickDayIdx);
                    claimCalculation.EndDate = adjustedFirstClaimDate.AddDays(prevSickDayIdx + applicableSickDays - 1);
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

                    claimCalculation.StartDate = adjustedFirstClaimDate;
                    claimCalculation.EndDate = adjustedLastClaimDate;
                    applicableSickDays = adjustedNumberOfCalendarDays;
                }

                if (idx == 0) //Include qualifying day only in first ClaimCalculation record
                {
                    // QUALIFYING DAY

                    //Set defaults
                    claimCalculation.PaidHoursQD = "0,00";
                    claimCalculation.PaidOnCallDayHoursQD = "0,00";
                    claimCalculation.PaidOnCallNightHoursQD = "0,00";
                    claimCalculation.PaidUnsocialEveningHoursQD = "0,00";
                    claimCalculation.PaidUnsocialNightHoursQD = "0,00";
                    claimCalculation.PaidUnsocialWeekendHoursQD = "0,00";
                    claimCalculation.PaidUnsocialGrandWeekendHoursQD = "0,00";



                    //new code for 5 day-rule
                    //find the claimday index of the first day in the period where the assistant was sick
                    bool firstClaimDayFound = false;
                    int claimDayIdx = adjustedStartCalendarDayIdx;
                    while (!firstClaimDayFound && claimDayIdx < adjustedStartCalendarDayIdx + adjustedNumberOfCalendarDays)
                    {
                        if (!claimDays[claimDayIdx].Well)
                        {
                            firstClaimDayFound = true;
                        }
                        claimDayIdx++;
                    }
                    qdIdx = claimDayIdx - 1;



                    //Format hours for qualifying day
                    claimCalculation.HoursQD = String.Format("{0:0.00}", Convert.ToDecimal("0,00") + Convert.ToDecimal(claimDays[qdIdx].Hours)); //Should this be adjustedClaimDays?
                    claimCalculation.OnCallDayHoursQD = String.Format("{0:0.00}", Convert.ToDecimal("0,00") + Convert.ToDecimal(claimDays[qdIdx].OnCallDay));
                    claimCalculation.OnCallNightHoursQD = String.Format("{0:0.00}", Convert.ToDecimal("0,00") + Convert.ToDecimal(claimDays[qdIdx].OnCallNight));
                    claimCalculation.UnsocialEveningHoursQD = String.Format("{0:0.00}", Convert.ToDecimal("0,00") + Convert.ToDecimal(claimDays[qdIdx].UnsocialEvening));
                    claimCalculation.UnsocialNightHoursQD = String.Format("{0:0.00}", Convert.ToDecimal("0,00") + Convert.ToDecimal(claimDays[qdIdx].UnsocialNight));
                    claimCalculation.UnsocialWeekendHoursQD = String.Format("{0:0.00}", Convert.ToDecimal("0,00") + Convert.ToDecimal(claimDays[qdIdx].UnsocialWeekend));
                    claimCalculation.UnsocialGrandWeekendHoursQD = String.Format("{0:0.00}", Convert.ToDecimal("0,00") + Convert.ToDecimal(claimDays[qdIdx].UnsocialGrandWeekend));

                    //These numbers go to the assistant's part of the view
                    claim.NumberOfAbsenceHours = Convert.ToDecimal(claimCalculation.HoursQD) + Convert.ToDecimal(claimDays[qdIdx].OnCallDay) + Convert.ToDecimal(claimDays[qdIdx].OnCallNight);
                    claim.NumberOfOrdinaryHours = Convert.ToDecimal(claimCalculation.HoursQD);
                    claim.NumberOfUnsocialHours = Convert.ToDecimal(claimDays[qdIdx].UnsocialEvening) + Convert.ToDecimal(claimDays[qdIdx].UnsocialNight) + Convert.ToDecimal(claimDays[qdIdx].UnsocialWeekend) + Convert.ToDecimal(claimDays[qdIdx].UnsocialGrandWeekend);
                    claim.NumberOfOnCallHours = Convert.ToDecimal(claimDays[qdIdx].OnCallDay) + Convert.ToDecimal(claimDays[qdIdx].OnCallNight);

                    //Calculate number of hours exceeding 8. Only those hours shall be paid.
                    if (Convert.ToDecimal(claimCalculation.HoursQD) > 8)
                    {
                        claimCalculation.PaidHoursQD = String.Format("{0:0.00}", Convert.ToDecimal(claimCalculation.HoursQD) - 8);
                        claimCalculation.PaidOnCallDayHoursQD = claimCalculation.OnCallDayHoursQD;
                        claimCalculation.PaidOnCallNightHoursQD = claimCalculation.OnCallNightHoursQD;
                    }
                    else if (Convert.ToDecimal(claimCalculation.HoursQD) + Convert.ToDecimal(claimCalculation.OnCallDayHoursQD) > 8)
                    {
                        claimCalculation.PaidOnCallDayHoursQD = String.Format("{0:0.00}", Convert.ToDecimal(claimCalculation.HoursQD) + Convert.ToDecimal(claimCalculation.OnCallDayHoursQD) - 8);
                        claimCalculation.PaidOnCallNightHoursQD = claimCalculation.OnCallNightHoursQD;
                    }
                    else if (Convert.ToDecimal(claimCalculation.HoursQD) + Convert.ToDecimal(claimCalculation.OnCallDayHoursQD) + Convert.ToDecimal(claimCalculation.OnCallNightHoursQD) > 8)
                    {
                        claimCalculation.PaidOnCallNightHoursQD = String.Format("{0:0.00}", Convert.ToDecimal(claimCalculation.HoursQD) + Convert.ToDecimal(claimCalculation.OnCallDayHoursQD) + Convert.ToDecimal(claimCalculation.OnCallNightHoursQD) - 8);
                    }

                    //Calculate number of unsocial hours exceeding 8. Only those hours shall be paid.
                    if (Convert.ToDecimal(claimCalculation.UnsocialEveningHoursQD) > 8)
                    {
                        claimCalculation.PaidUnsocialEveningHoursQD = String.Format("{0:0.00}", Convert.ToDecimal(claimCalculation.UnsocialEveningHoursQD) - 8);
                        claimCalculation.PaidUnsocialNightHoursQD = claimCalculation.UnsocialNightHoursQD;
                        claimCalculation.PaidUnsocialWeekendHoursQD = claimCalculation.UnsocialWeekendHoursQD;
                        claimCalculation.PaidUnsocialGrandWeekendHoursQD = claimCalculation.UnsocialGrandWeekendHoursQD;
                    }
                    else if (Convert.ToDecimal(claimCalculation.UnsocialEveningHoursQD) + Convert.ToDecimal(claimCalculation.UnsocialNightHoursQD) > 8)
                    {
                        claimCalculation.PaidUnsocialNightHoursQD = String.Format("{0:0.00}", Convert.ToDecimal(claimCalculation.UnsocialEveningHoursQD) + Convert.ToDecimal(claimCalculation.UnsocialNightHoursQD) - 8);
                        claimCalculation.PaidUnsocialWeekendHoursQD = claimCalculation.UnsocialWeekendHoursQD;
                        claimCalculation.PaidUnsocialGrandWeekendHoursQD = claimCalculation.UnsocialGrandWeekendHoursQD;
                    }
                    else if (Convert.ToDecimal(claimCalculation.UnsocialEveningHoursQD) + Convert.ToDecimal(claimCalculation.UnsocialNightHoursQD) + Convert.ToDecimal(claimCalculation.UnsocialWeekendHoursQD) > 8)
                    {
                        claimCalculation.PaidUnsocialWeekendHoursQD = String.Format("{0:0.00}", Convert.ToDecimal(claimCalculation.UnsocialEveningHoursQD) + Convert.ToDecimal(claimCalculation.UnsocialNightHoursQD) + Convert.ToDecimal(claimCalculation.UnsocialWeekendHoursQD) - 8);
                        claimCalculation.PaidUnsocialGrandWeekendHoursQD = claimCalculation.UnsocialGrandWeekendHoursQD;
                    }
                    else if (Convert.ToDecimal(claimCalculation.UnsocialEveningHoursQD) + Convert.ToDecimal(claimCalculation.UnsocialNightHoursQD) + Convert.ToDecimal(claimCalculation.UnsocialWeekendHoursQD) + Convert.ToDecimal(claimCalculation.UnsocialGrandWeekendHoursQD) > 8)
                    {
                        claimCalculation.PaidUnsocialGrandWeekendHoursQD = String.Format("{0:0.00}", Convert.ToDecimal(claimCalculation.UnsocialEveningHoursQD) + Convert.ToDecimal(claimCalculation.UnsocialNightHoursQD) + Convert.ToDecimal(claimCalculation.UnsocialWeekendHoursQD) + Convert.ToDecimal(claimCalculation.UnsocialGrandWeekendHoursQD) - 8);

                    }

                    //Sickpay for qualifying day
                    claimCalculation.SalaryQD = String.Format("{0:0.00}", (Convert.ToDecimal(claim.SickPayRateAsString) * Convert.ToDecimal(claimCalculation.PaidHoursQD) * Convert.ToDecimal(claim.HourlySalaryAsString) / 100));
                    claimCalculation.SalaryCalcQD = claim.SickPayRateAsString + " % x " + claimCalculation.PaidHoursQD + " timmar x " + claim.HourlySalaryAsString + " Kr";

                    //Holiday pay for qualifying day
                    claimCalculation.HolidayPayQD = String.Format("{0:0.00}", (Convert.ToDecimal(claim.HolidayPayRateAsString) * Convert.ToDecimal(claimCalculation.HoursQD) * Convert.ToDecimal(claim.HourlySalaryAsString) / 100));
                    claimCalculation.HolidayPayCalcQD = claim.HolidayPayRateAsString + " % x " + claimCalculation.HoursQD + " timmar x " + claim.HourlySalaryAsString + " Kr";

                    //Unsocial evening pay for qualifying day
                    claimCalculation.UnsocialEveningPayQD = String.Format("{0:0.00}", (Convert.ToDecimal(claim.SickPayRateAsString) * Convert.ToDecimal(claimCalculation.PaidUnsocialEveningHoursQD) * Convert.ToDecimal(claimCalculation.PerHourUnsocialEveningAsString) / 100));
                    claimCalculation.UnsocialEveningPayCalcQD = claim.SickPayRateAsString + " % x " + claimCalculation.PaidUnsocialEveningHoursQD + " timmar x " + claimCalculation.PerHourUnsocialEveningAsString + " Kr";

                    //Unsocial night pay for qualifying day
                    claimCalculation.UnsocialNightPayQD = String.Format("{0:0.00}", (Convert.ToDecimal(claim.SickPayRateAsString) * Convert.ToDecimal(claimCalculation.PaidUnsocialNightHoursQD) * Convert.ToDecimal(claimCalculation.PerHourUnsocialNightAsString) / 100));
                    claimCalculation.UnsocialNightPayCalcQD = claim.SickPayRateAsString + " % x " + claimCalculation.PaidUnsocialNightHoursQD + " timmar x " + claimCalculation.PerHourUnsocialNightAsString + " Kr";

                    //Unsocial weekend pay for qualifying day
                    claimCalculation.UnsocialWeekendPayQD = String.Format("{0:0.00}", (Convert.ToDecimal(claim.SickPayRateAsString) * Convert.ToDecimal(claimCalculation.PaidUnsocialWeekendHoursQD) * Convert.ToDecimal(claimCalculation.PerHourUnsocialWeekendAsString) / 100));
                    claimCalculation.UnsocialWeekendPayCalcQD = claim.SickPayRateAsString + " % x " + claimCalculation.PaidUnsocialWeekendHoursQD + " timmar x " + claimCalculation.PerHourUnsocialWeekendAsString + " Kr";

                    //Unsocial grand weekend pay for qualifying day
                    claimCalculation.UnsocialGrandWeekendPayQD = String.Format("{0:0.00}", (Convert.ToDecimal(claim.SickPayRateAsString) * Convert.ToDecimal(claimCalculation.PaidUnsocialGrandWeekendHoursQD) * Convert.ToDecimal(claimCalculation.PerHourUnsocialHolidayAsString) / 100));
                    claimCalculation.UnsocialGrandWeekendPayCalcQD = claim.SickPayRateAsString + " % x " + claimCalculation.PaidUnsocialGrandWeekendHoursQD + " timmar x " + claimCalculation.PerHourUnsocialHolidayAsString + " Kr";

                    //Unsocial sum pay for qualifying day
                    claimCalculation.UnsocialSumPayQD = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.UnsocialEveningPayQD) + Convert.ToDecimal(claimCalculation.UnsocialNightPayQD) + Convert.ToDecimal(claimCalculation.UnsocialWeekendPayQD) + Convert.ToDecimal(claimCalculation.UnsocialGrandWeekendPayQD)));
                    claimCalculation.UnsocialSumPayCalcQD = claimCalculation.UnsocialEveningPayQD + " Kr + " + claimCalculation.UnsocialNightPayQD + " Kr + " + claimCalculation.UnsocialWeekendPayQD + " Kr + " + claimCalculation.UnsocialGrandWeekendPayQD + " Kr";

                    //On call day pay for qualifying day
                    claimCalculation.OnCallDayPayQD = String.Format("{0:0.00}", (Convert.ToDecimal(claim.SickPayRateAsString) * Convert.ToDecimal(claimCalculation.PaidOnCallDayHoursQD) * Convert.ToDecimal(claimCalculation.PerHourOnCallDayAsString) / 100));
                    claimCalculation.OnCallDayPayCalcQD = claim.SickPayRateAsString + " % x " + claimCalculation.PaidOnCallDayHoursQD + " timmar x " + claimCalculation.PerHourOnCallDayAsString + " Kr";

                    //On call night pay for qualifying day
                    claimCalculation.OnCallNightPayQD = String.Format("{0:0.00}", (Convert.ToDecimal(claim.SickPayRateAsString) * Convert.ToDecimal(claimCalculation.PaidOnCallNightHoursQD) * Convert.ToDecimal(claimCalculation.PerHourOnCallNightAsString) / 100));
                    claimCalculation.OnCallNightPayCalcQD = claim.SickPayRateAsString + " % x " + claimCalculation.PaidOnCallNightHoursQD + " timmar x " + claimCalculation.PerHourOnCallNightAsString + " Kr";

                    //On call sum pay for qualifying day
                    claimCalculation.OnCallSumPayQD = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.OnCallDayPayQD) + Convert.ToDecimal(claimCalculation.OnCallNightPayQD)));
                    claimCalculation.OnCallSumPayCalcQD = claimCalculation.OnCallDayPayQD + " Kr + " + claimCalculation.OnCallNightPayQD + " Kr";

                    //Sick pay for qualifying day
                    claimCalculation.SickPayQD = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.SalaryQD) + Convert.ToDecimal(claimCalculation.UnsocialSumPayQD) + Convert.ToDecimal(claimCalculation.OnCallSumPayQD)));
                    claimCalculation.SickPayCalcQD = claimCalculation.SalaryQD + " Kr + " + claimCalculation.UnsocialSumPayQD + " Kr + " + claimCalculation.OnCallSumPayQD + " Kr";

                    //Social fees for qualifying day
                    claimCalculation.SocialFeesQD = String.Format("{0:0.00}", (Convert.ToDecimal(claim.SocialFeeRateAsString) * (Convert.ToDecimal(claimCalculation.SickPayQD) + Convert.ToDecimal(claimCalculation.HolidayPayQD)) / 100));
                    claimCalculation.SocialFeesCalcQD = claim.SocialFeeRateAsString + " % x (" + claimCalculation.SickPayQD + " + " + claimCalculation.HolidayPayQD + ") Kr";

                    //Pension and insurance for qualifying day
                    claimCalculation.PensionAndInsuranceQD = String.Format("{0:0.00}", (Convert.ToDecimal(claim.PensionAndInsuranceRateAsString) * (Convert.ToDecimal(claimCalculation.SickPayQD) + Convert.ToDecimal(claimCalculation.HolidayPayQD)) / 100));
                    claimCalculation.PensionAndInsuranceCalcQD = claim.PensionAndInsuranceRateAsString + " % x (" + claimCalculation.SickPayQD + " + " + claimCalculation.HolidayPayQD + ") Kr";

                    //Sum for qualifying day (sum of the three previous items)
                    claimCalculation.CostQD = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.SickPayQD) + Convert.ToDecimal(claimCalculation.HolidayPayQD) + Convert.ToDecimal(claimCalculation.SocialFeesQD) + Convert.ToDecimal(claimCalculation.PensionAndInsuranceQD)));
                    claimCalculation.CostCalcQD = claimCalculation.SickPayQD + " Kr + " + claimCalculation.HolidayPayQD + " Kr + " + claimCalculation.SocialFeesQD + " Kr + " + claimCalculation.PensionAndInsuranceQD + " Kr";
                }

                //DAY 2 TO DAY 14
                if (adjustedNumberOfSickdays > 1)
                {
                    claimCalculation.HoursD2T14 = "0,00";
                    claimCalculation.UnsocialEveningD2T14 = "0,00";
                    claimCalculation.UnsocialNightD2T14 = "0,00";
                    claimCalculation.UnsocialWeekendD2T14 = "0,00";
                    claimCalculation.UnsocialGrandWeekendD2T14 = "0,00";
                    claimCalculation.UnsocialSumD2T14 = "0,00";
                    claimCalculation.OnCallDayD2T14 = "0,00";
                    claimCalculation.OnCallNightD2T14 = "0,00";
                    claimCalculation.OnCallSumD2T14 = "0,00";
                }

                //new code for day 15 and beyond
                //DAY 15 and plus
                if (adjustedNumberOfSickdays > 14)
                {
                    claimCalculation.HoursD15Plus = "0,00";
                    claimCalculation.UnsocialEveningD15Plus = "0,00";
                    claimCalculation.UnsocialNightD15Plus = "0,00";
                    claimCalculation.UnsocialWeekendD15Plus = "0,00";
                    claimCalculation.UnsocialGrandWeekendD15Plus = "0,00";
                    claimCalculation.UnsocialSumD15Plus = "0,00";
                    claimCalculation.OnCallDayD15Plus = "0,00";
                    claimCalculation.OnCallNightD15Plus = "0,00";
                    claimCalculation.OnCallSumD15Plus = "0,00";
                }

                //Sum up hours by category
                if (idx == 0) //check this line perhaps startIdx should be set to claimDayIdx or qdIdx?
                {
                    //startIdx = 1;
                    startIdx = qdIdx + 1;
                    stopIdx = startIdx + applicableSickDays - 1;
                }
                else
                {
                    startIdx = prevSickDayIdx;
                    stopIdx = startIdx + applicableSickDays; //needs to be adjusted. Need to figure out which claimday idx X has got SickDayNumber = 14. If startIdx + applicableSickDays is greater than X, then stopIdx should be set to X. 
                }
                for (int i = startIdx; i < stopIdx; i++)
                {
                    if (!claimDays[i].Well && claimDays[i].SickDayNumber < 15) //new if-statement for 5-day rule
                    {
                        claimCalculation.HoursD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.HoursD2T14) + Convert.ToDecimal(claimDays[i].Hours)));

                        claimCalculation.UnsocialEveningD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.UnsocialEveningD2T14) + Convert.ToDecimal(claimDays[i].UnsocialEvening)));
                        claimCalculation.UnsocialNightD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.UnsocialNightD2T14) + Convert.ToDecimal(claimDays[i].UnsocialNight)));
                        claimCalculation.UnsocialWeekendD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.UnsocialWeekendD2T14) + Convert.ToDecimal(claimDays[i].UnsocialWeekend)));
                        claimCalculation.UnsocialGrandWeekendD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.UnsocialGrandWeekendD2T14) + Convert.ToDecimal(claimDays[i].UnsocialGrandWeekend)));

                        claimCalculation.OnCallDayD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.OnCallDayD2T14) + Convert.ToDecimal(claimDays[i].OnCallDay)));
                        claimCalculation.OnCallNightD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.OnCallNightD2T14) + Convert.ToDecimal(claimDays[i].OnCallNight)));
                        day1T14InThisCollAgreement = true;
                    }
                    else if (!claimDays[i].Well && claimDays[i].SickDayNumber > 14) //new if-statement for 5-day rule
                    {
                        claimCalculation.HoursD15Plus = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.HoursD15Plus) + Convert.ToDecimal(claimDays[i].Hours)));

                        claimCalculation.UnsocialEveningD15Plus = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.UnsocialEveningD15Plus) + Convert.ToDecimal(claimDays[i].UnsocialEvening)));
                        claimCalculation.UnsocialNightD15Plus = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.UnsocialNightD15Plus) + Convert.ToDecimal(claimDays[i].UnsocialNight)));
                        claimCalculation.UnsocialWeekendD15Plus = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.UnsocialWeekendD15Plus) + Convert.ToDecimal(claimDays[i].UnsocialWeekend)));
                        claimCalculation.UnsocialGrandWeekendD15Plus = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.UnsocialGrandWeekendD15Plus) + Convert.ToDecimal(claimDays[i].UnsocialGrandWeekend)));

                        claimCalculation.OnCallDayD15Plus = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.OnCallDayD15Plus) + Convert.ToDecimal(claimDays[i].OnCallDay)));
                        claimCalculation.OnCallNightD15Plus = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.OnCallNightD15Plus) + Convert.ToDecimal(claimDays[i].OnCallNight)));
                        day15PlusInThisCollAgreement = true;
                    }
                }
                if (day1T14InThisCollAgreement)
                {

                    claimCalculation.UnsocialSumD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.UnsocialEveningD2T14) + Convert.ToDecimal(claimCalculation.UnsocialNightD2T14) + Convert.ToDecimal(claimCalculation.UnsocialWeekendD2T14) + Convert.ToDecimal(claimCalculation.UnsocialGrandWeekendD2T14)));
                    claimCalculation.OnCallSumD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.OnCallDayD2T14) + Convert.ToDecimal(claimCalculation.OnCallNightD2T14)));

                    //These numbers go to the assistant's part of the view
                    claim.NumberOfAbsenceHours = claim.NumberOfAbsenceHours + Convert.ToDecimal(claimCalculation.HoursD2T14) + Convert.ToDecimal(claimCalculation.OnCallDayD2T14) + Convert.ToDecimal(claimCalculation.OnCallNightD2T14);
                    claim.NumberOfOrdinaryHours = claim.NumberOfOrdinaryHours + Convert.ToDecimal(claimCalculation.HoursD2T14);
                    claim.NumberOfUnsocialHours = claim.NumberOfUnsocialHours + Convert.ToDecimal(claimCalculation.UnsocialEveningD2T14) + Convert.ToDecimal(claimCalculation.UnsocialNightD2T14) + Convert.ToDecimal(claimCalculation.UnsocialWeekendD2T14) + Convert.ToDecimal(claimCalculation.UnsocialGrandWeekendD2T14);
                    claim.NumberOfOnCallHours = claim.NumberOfOnCallHours + Convert.ToDecimal(claimCalculation.OnCallDayD2T14) + Convert.ToDecimal(claimCalculation.OnCallNightD2T14);
                    //Code maybe should be added here to calculate the number of hours for the SI assistant..

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
                    claimCalculation.SocialFeesD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claim.SocialFeeRateAsString) * (Convert.ToDecimal(claimCalculation.SickPayD2T14) + Convert.ToDecimal(claimCalculation.HolidayPayD2T14)) / 100));
                    //claimCalculation.SocialFeesD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claim.SocialFeeRateAsString) * (Convert.ToDecimal(claimCalculation.SalaryD2T14) + Convert.ToDecimal(claimCalculation.UnsocialSumPayD2T14) + Convert.ToDecimal(claimCalculation.OnCallSumPayD2T14) + Convert.ToDecimal(claimCalculation.HolidayPayD2T14)) / 100));
                    claimCalculation.SocialFeesCalcD2T14 = claim.SocialFeeRateAsString + " % x (" + claimCalculation.SickPayD2T14 + " Kr + " + claimCalculation.HolidayPayD2T14 + " Kr)";

                    //Pensions and insurances for day 2 to day 14
                    claimCalculation.PensionAndInsuranceD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claim.PensionAndInsuranceRateAsString) * (Convert.ToDecimal(claimCalculation.SickPayD2T14) + Convert.ToDecimal(claimCalculation.HolidayPayD2T14)) / 100));
                    //claimCalculation.PensionAndInsuranceD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claim.PensionAndInsuranceRateAsString) * (Convert.ToDecimal(claimCalculation.SalaryD2T14) + Convert.ToDecimal(claimCalculation.HolidayPayD2T14) + Convert.ToDecimal(claimCalculation.UnsocialSumPayD2T14) + Convert.ToDecimal(claimCalculation.OnCallSumPayD2T14)) / 100));
                    claimCalculation.PensionAndInsuranceCalcD2T14 = claim.PensionAndInsuranceRateAsString + " % x (" + claimCalculation.SickPayD2T14 + " Kr + " + claimCalculation.HolidayPayD2T14 + " Kr)";

                    //Sum for day 2 to day 14
                    claimCalculation.CostD2T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.SickPayD2T14) + Convert.ToDecimal(claimCalculation.HolidayPayD2T14) + Convert.ToDecimal(claimCalculation.SocialFeesD2T14) + Convert.ToDecimal(claimCalculation.PensionAndInsuranceD2T14)));
                    claimCalculation.CostCalcD2T14 = claimCalculation.SickPayD2T14 + " Kr + " + claimCalculation.HolidayPayD2T14 + " Kr + " + claimCalculation.SocialFeesD2T14 + " Kr + " + claimCalculation.PensionAndInsuranceD2T14 + " Kr";
                }

                //new line for day 15 and beyond
                //prevSickDayIdx = prevSickDayIdx + applicableSickDays;

                if (day15PlusInThisCollAgreement)
                {
                    //new code for day 15 and beyond
                    //DAY 15 and plus
                    //claimCalculation.HoursD15Plus = "0,00";
                    //claimCalculation.UnsocialEveningD15Plus = "0,00";
                    //claimCalculation.UnsocialNightD15Plus = "0,00";
                    //claimCalculation.UnsocialWeekendD15Plus = "0,00";
                    //claimCalculation.UnsocialGrandWeekendD15Plus = "0,00";
                    //claimCalculation.UnsocialSumD15Plus = "0,00";
                    //claimCalculation.OnCallDayD15Plus = "0,00";
                    //claimCalculation.OnCallNightD15Plus = "0,00";
                    //claimCalculation.OnCallSumD15Plus = "0,00";

                    ////Sum up hours by category for day 15 and beyond
                    //startIdx = prevSickDayIdx;
                    //stopIdx = startIdx + applicableSickDays;

                    //for (int i = startIdx; i < stopIdx; i++)
                    //{
                    //    if (!claimDays[i].Well && claimDays[i].SickDayNumber > 14) //new if-statement for 5-day rule
                    //    {
                    //        claimCalculation.HoursD15Plus = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.HoursD15Plus) + Convert.ToDecimal(claimDays[i].Hours)));

                    //        claimCalculation.UnsocialEveningD15Plus = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.UnsocialEveningD15Plus) + Convert.ToDecimal(claimDays[i].UnsocialEvening)));
                    //        claimCalculation.UnsocialNightD15Plus = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.UnsocialNightD15Plus) + Convert.ToDecimal(claimDays[i].UnsocialNight)));
                    //        claimCalculation.UnsocialWeekendD15Plus = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.UnsocialWeekendD15Plus) + Convert.ToDecimal(claimDays[i].UnsocialWeekend)));
                    //        claimCalculation.UnsocialGrandWeekendD15Plus = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.UnsocialGrandWeekendD15Plus) + Convert.ToDecimal(claimDays[i].UnsocialGrandWeekend)));

                    //        claimCalculation.OnCallDayD15Plus = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.OnCallDayD15Plus) + Convert.ToDecimal(claimDays[i].OnCallDay)));
                    //        claimCalculation.OnCallNightD15Plus = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.OnCallNightD15Plus) + Convert.ToDecimal(claimDays[i].OnCallNight)));
                    //    }
                    //}

                    claimCalculation.UnsocialSumD15Plus = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.UnsocialEveningD15Plus) + Convert.ToDecimal(claimCalculation.UnsocialNightD15Plus) + Convert.ToDecimal(claimCalculation.UnsocialWeekendD15Plus) + Convert.ToDecimal(claimCalculation.UnsocialGrandWeekendD15Plus)));
                    claimCalculation.OnCallSumD15Plus = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.OnCallDayD15Plus) + Convert.ToDecimal(claimCalculation.OnCallNightD15Plus)));

                    //These numbers go to the assistant's part of the view
                    claim.NumberOfAbsenceHours = claim.NumberOfAbsenceHours + Convert.ToDecimal(claimCalculation.HoursD15Plus) + Convert.ToDecimal(claimCalculation.OnCallDayD15Plus) + Convert.ToDecimal(claimCalculation.OnCallNightD15Plus);
                    claim.NumberOfOrdinaryHours = claim.NumberOfOrdinaryHours + Convert.ToDecimal(claimCalculation.HoursD15Plus);
                    claim.NumberOfUnsocialHours = claim.NumberOfUnsocialHours + Convert.ToDecimal(claimCalculation.UnsocialEveningD15Plus) + Convert.ToDecimal(claimCalculation.UnsocialNightD15Plus) + Convert.ToDecimal(claimCalculation.UnsocialWeekendD15Plus) + Convert.ToDecimal(claimCalculation.UnsocialGrandWeekendD15Plus);
                    claim.NumberOfOnCallHours = claim.NumberOfOnCallHours + Convert.ToDecimal(claimCalculation.OnCallDayD15Plus) + Convert.ToDecimal(claimCalculation.OnCallNightD15Plus);
                    //Code maybe should be added here to calculate the number of hours for the SI assistant..

                    //Calculate the money by category for day 15 and beyond
                    //Sickpay for day 15 and beyond
                    claimCalculation.SalaryD15Plus = String.Format("{0:0.00}", (Convert.ToDecimal(claim.SickPayRateAsString) * Convert.ToDecimal(claimCalculation.HoursD15Plus) * Convert.ToDecimal(claim.HourlySalaryAsString) / 100));
                    claimCalculation.SalaryCalcD15Plus = claim.SickPayRateAsString + " % x " + claimCalculation.HoursD15Plus + " timmar x " + claim.HourlySalaryAsString + " Kr";

                    //Holiday pay for day 15 and beyond
                    claimCalculation.HolidayPayD15Plus = String.Format("{0:0.00}", (Convert.ToDecimal(claim.HolidayPayRateAsString) * Convert.ToDecimal(claimCalculation.SalaryD15Plus) / 100));
                    claimCalculation.HolidayPayCalcD15Plus = claim.HolidayPayRateAsString + " % x " + claimCalculation.SalaryD15Plus + " Kr";

                    //Unsocial evening pay for day 15 and beyond
                    claimCalculation.UnsocialEveningPayD15Plus = String.Format("{0:0.00}", (Convert.ToDecimal(claim.SickPayRateAsString) * Convert.ToDecimal(claimCalculation.UnsocialEveningD15Plus) * Convert.ToDecimal(claimCalculation.PerHourUnsocialEveningAsString) / 100));
                    claimCalculation.UnsocialEveningPayCalcD15Plus = claim.SickPayRateAsString + " % x " + claimCalculation.UnsocialEveningD15Plus + " timmar x " + claimCalculation.PerHourUnsocialEveningAsString + " Kr";

                    //Unsocial night pay for day 15 and beyond
                    claimCalculation.UnsocialNightPayD15Plus = String.Format("{0:0.00}", (Convert.ToDecimal(claim.SickPayRateAsString) * Convert.ToDecimal(claimCalculation.UnsocialNightD15Plus) * Convert.ToDecimal(claimCalculation.PerHourUnsocialNightAsString) / 100));
                    claimCalculation.UnsocialNightPayCalcD15Plus = claim.SickPayRateAsString + " % x " + claimCalculation.UnsocialNightD15Plus + " timmar x " + claimCalculation.PerHourUnsocialNightAsString + " Kr";

                    //Unsocial weekend pay for day 15 and beyond
                    claimCalculation.UnsocialWeekendPayD15Plus = String.Format("{0:0.00}", (Convert.ToDecimal(claim.SickPayRateAsString) * Convert.ToDecimal(claimCalculation.UnsocialWeekendD15Plus) * Convert.ToDecimal(claimCalculation.PerHourUnsocialWeekendAsString) / 100));
                    claimCalculation.UnsocialWeekendPayCalcD15Plus = claim.SickPayRateAsString + " % x " + claimCalculation.UnsocialWeekendD15Plus + " timmar x " + claimCalculation.PerHourUnsocialWeekendAsString + " Kr";

                    //Unsocial grand weekend pay for day 15 and beyond
                    claimCalculation.UnsocialGrandWeekendPayD15Plus = String.Format("{0:0.00}", (Convert.ToDecimal(claim.SickPayRateAsString) * Convert.ToDecimal(claimCalculation.UnsocialGrandWeekendD15Plus) * Convert.ToDecimal(claimCalculation.PerHourUnsocialHolidayAsString) / 100));
                    claimCalculation.UnsocialGrandWeekendPayCalcD15Plus = claim.SickPayRateAsString + " % x " + claimCalculation.UnsocialGrandWeekendD15Plus + " timmar x " + claimCalculation.PerHourUnsocialHolidayAsString + " Kr";

                    //Unsocial sum pay for day 15 and beyond
                    claimCalculation.UnsocialSumPayD15Plus = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.UnsocialEveningPayD15Plus) + Convert.ToDecimal(claimCalculation.UnsocialNightPayD15Plus) + Convert.ToDecimal(claimCalculation.UnsocialWeekendPayD15Plus) + Convert.ToDecimal(claimCalculation.UnsocialGrandWeekendPayD15Plus)));
                    claimCalculation.UnsocialSumPayCalcD15Plus = claimCalculation.UnsocialEveningPayD15Plus + " Kr + " + claimCalculation.UnsocialNightPayD15Plus + " Kr + " + claimCalculation.UnsocialWeekendPayD15Plus + " Kr + " + claimCalculation.UnsocialGrandWeekendPayD15Plus;

                    //On call day pay for day 15 and beyond
                    claimCalculation.OnCallDayPayD15Plus = String.Format("{0:0.00}", (Convert.ToDecimal(claim.SickPayRateAsString) * Convert.ToDecimal(claimCalculation.OnCallDayD15Plus) * Convert.ToDecimal(claimCalculation.PerHourOnCallDayAsString) / 100));
                    claimCalculation.OnCallDayPayCalcD15Plus = claim.SickPayRateAsString + " % x " + claimCalculation.OnCallDayD15Plus + " timmar x " + claimCalculation.PerHourOnCallDayAsString + " Kr";

                    //On call night pay for day 15 and beyond
                    claimCalculation.OnCallNightPayD15Plus = String.Format("{0:0.00}", (Convert.ToDecimal(claim.SickPayRateAsString) * Convert.ToDecimal(claimCalculation.OnCallNightD15Plus) * Convert.ToDecimal(claimCalculation.PerHourOnCallNightAsString) / 100));
                    claimCalculation.OnCallNightPayCalcD15Plus = claim.SickPayRateAsString + " % x " + claimCalculation.OnCallNightD15Plus + " timmar x " + claimCalculation.PerHourOnCallNightAsString + " Kr";

                    //On call sum pay for day 15 and beyond
                    claimCalculation.OnCallSumPayD15Plus = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.OnCallDayPayD15Plus) + Convert.ToDecimal(claimCalculation.OnCallNightPayD15Plus)));
                    claimCalculation.OnCallSumPayCalcD15Plus = claimCalculation.OnCallDayPayD15Plus + " Kr + " + claimCalculation.OnCallNightPayD15Plus;

                    //Sick pay for day 15 and beyond
                    claimCalculation.SickPayD15Plus = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.SalaryD15Plus) + Convert.ToDecimal(claimCalculation.UnsocialSumPayD15Plus) + Convert.ToDecimal(claimCalculation.OnCallSumPayD15Plus)));
                    claimCalculation.SickPayCalcD15Plus = claimCalculation.SalaryD15Plus + " Kr + " + claimCalculation.UnsocialSumPayD15Plus + " Kr + " + claimCalculation.OnCallSumPayD15Plus + " Kr";

                    //Social fees for day 15 and beyond
                    claimCalculation.SocialFeesD15Plus = String.Format("{0:0.00}", (Convert.ToDecimal(claim.SocialFeeRateAsString) * (Convert.ToDecimal(claimCalculation.SickPayD15Plus) + Convert.ToDecimal(claimCalculation.HolidayPayD15Plus)) / 100));
                    //claimCalculation.SocialFeesD15Plus = String.Format("{0:0.00}", (Convert.ToDecimal(claim.SocialFeeRateAsString) * (Convert.ToDecimal(claimCalculation.SalaryD15Plus) + Convert.ToDecimal(claimCalculation.UnsocialSumPayD15Plus) + Convert.ToDecimal(claimCalculation.OnCallSumPayD15Plus) + Convert.ToDecimal(claimCalculation.HolidayPayD15Plus)) / 100));
                    claimCalculation.SocialFeesCalcD15Plus = claim.SocialFeeRateAsString + " % x (" + claimCalculation.SickPayD15Plus + " Kr + " + claimCalculation.HolidayPayD15Plus + " Kr)";

                    //Pensions and insurances for day 15 and beyond
                    claimCalculation.PensionAndInsuranceD15Plus = String.Format("{0:0.00}", (Convert.ToDecimal(claim.PensionAndInsuranceRateAsString) * (Convert.ToDecimal(claimCalculation.SickPayD15Plus) + Convert.ToDecimal(claimCalculation.HolidayPayD15Plus)) / 100));
                    //claimCalculation.PensionAndInsuranceD15Plus = String.Format("{0:0.00}", (Convert.ToDecimal(claim.PensionAndInsuranceRateAsString) * (Convert.ToDecimal(claimCalculation.SalaryD15Plus) + Convert.ToDecimal(claimCalculation.HolidayPayD15Plus) + Convert.ToDecimal(claimCalculation.UnsocialSumPayD15Plus) + Convert.ToDecimal(claimCalculation.OnCallSumPayD15Plus)) / 100));
                    claimCalculation.PensionAndInsuranceCalcD15Plus = claim.PensionAndInsuranceRateAsString + " % x (" + claimCalculation.SickPayD15Plus + " Kr + " + claimCalculation.HolidayPayD15Plus + " Kr)";

                    //Sum for day 15 and beyond
                    //claimCalculation.CostD15Plus = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.SickPayD15Plus) + Convert.ToDecimal(claimCalculation.HolidayPayD15Plus) + Convert.ToDecimal(claimCalculation.SocialFeesD15Plus) + Convert.ToDecimal(claimCalculation.PensionAndInsuranceD15Plus)));
                    //claimCalculation.CostCalcD15Plus = claimCalculation.SickPayD15Plus + " Kr + " + claimCalculation.HolidayPayD15Plus + " Kr + " + claimCalculation.SocialFeesD15Plus + " Kr + " + claimCalculation.PensionAndInsuranceD15Plus + " Kr";
                    claimCalculation.CostD15Plus = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.HolidayPayD15Plus) + Convert.ToDecimal(claimCalculation.SocialFeesD15Plus) + Convert.ToDecimal(claimCalculation.PensionAndInsuranceD15Plus)));
                    claimCalculation.CostCalcD15Plus = claimCalculation.HolidayPayD15Plus + " Kr + " + claimCalculation.SocialFeesD15Plus + " Kr + " + claimCalculation.PensionAndInsuranceD15Plus + " Kr";
                }

                claim.StatusDate = DateTime.Now;
                prevSickDayIdx = prevSickDayIdx + applicableSickDays;

                if (claim.AdjustedNumberOfSickDays <= 14)
                {
                    //Total sum for day 1 to day 14
                    claimCalculation.TotalCostD1T14 = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.CostQD) + Convert.ToDecimal(claimCalculation.CostD2T14)));
                    claimCalculation.TotalCostCalcD1T14 = claimCalculation.CostQD + " Kr + " + claimCalculation.CostD2T14;

                    db.ClaimCalculations.Add(claimCalculation);
                    db.SaveChanges();
                    totalCostD1D14 = totalCostD1D14 + Convert.ToDecimal(claimCalculation.TotalCostD1T14);
                    if (adjustedNumberOfCalendarDays == 1)
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
                    claim.ModelSum = totalCostD1D14;
                    claim.TotalCostD1T14 = String.Format("{0:0.00}", totalCostD1D14);
                    claim.TotalCostCalcD1T14 = totalCostCalcD1D14;
                }
                else //more than 14 sickdays in the claim
                {
                    //Total sum for all days
                    claimCalculation.TotalCostD1Plus = String.Format("{0:0.00}", (Convert.ToDecimal(claimCalculation.CostQD) + Convert.ToDecimal(claimCalculation.CostD2T14) + +Convert.ToDecimal(claimCalculation.CostD15Plus)));
                    claimCalculation.TotalCostCalcD1Plus = claimCalculation.CostQD + " Kr + " + claimCalculation.CostD2T14 + " Kr + " + claimCalculation.CostD15Plus;

                    db.ClaimCalculations.Add(claimCalculation);
                    db.SaveChanges();
                    totalCostD1Plus = totalCostD1Plus + Convert.ToDecimal(claimCalculation.TotalCostD1Plus);
                    if (adjustedNumberOfCalendarDays == 1)
                    {
                        totalCostCalcD1Plus = claimCalculation.CostQD;
                    }
                    else if (idx == 0)
                    {
                        totalCostCalcD1Plus = claimCalculation.CostQD + " Kr + " + claimCalculation.CostD2T14 + " Kr + " + claimCalculation.CostD15Plus;
                    }
                    else
                    {
                        totalCostCalcD1Plus = totalCostCalcD1Plus + " Kr " + claimCalculation.CostD2T14 + " Kr " + claimCalculation.CostD15Plus;
                    }
                    claim.ModelSum = totalCostD1Plus;
                    claim.TotalCostD1Plus = String.Format("{0:0.00}", totalCostD1Plus);
                    claim.TotalCostCalcD1Plus = totalCostCalcD1Plus;
                }
                day1T14InThisCollAgreement = false;
                day15PlusInThisCollAgreement = false;
            }
            db.Entry(claim).State = EntityState.Modified;
            db.SaveChanges();
        }

        private static void SendEmail(MailMessage message)
        {
            int smtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
            string smtpHost = ConfigurationManager.AppSettings["SMTPServer"];
            SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort);
            NetworkCredential credentials = new NetworkCredential(ConfigurationManager.AppSettings["mailAccount"], ConfigurationManager.AppSettings["mailPassword"]);
            smtpClient.Credentials = credentials;
            //smtpClient.UseDefaultCredentials = true;

            if (smtpHost == "smtp.gmail.com")
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
