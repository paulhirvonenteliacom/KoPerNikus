using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Sjuklöner.Models;
using Sjuklöner.BankIDService;
using System.Collections.Generic;
using Sjuklöner.Viewmodels;
using System.Text.RegularExpressions;
using System.Data.Entity;
using static Sjuklöner.Models.AdmOffIndexVM;
using System.Net;
using static Sjuklöner.Models.IndexAllOmbudsVM;

namespace Sjuklöner.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        private ApplicationDbContext db = new ApplicationDbContext();       

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
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
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Ogiltigt användarnamn eller lösenord.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Ogiltig kod.");
                    return View(model);
            }
        }

        // GET: Acount/DeleteOmbud
        [Authorize(Roles = "Admin, Ombud")]       
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

            OmbudDeleteVM ombudVM = new OmbudDeleteVM();
            ombudVM.Id = id;
            ombudVM.FirstName = applicationUser.FirstName;
            ombudVM.LastName = applicationUser.LastName;
            ombudVM.SSN = applicationUser.SSN;
            ombudVM.PhoneNumber = applicationUser.PhoneNumber;
            ombudVM.Email = applicationUser.Email;
            return View("DeleteOmbud", ombudVM);
        }
     
        // POST: Account/DeleteOmbud/5
        [HttpPost, ActionName("DeleteOmbud")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Ombud")]
        public ActionResult DeleteOmbud(string id, string submitButton)
        {
            if (submitButton == "Bekräfta")
            {
                var myId = User.Identity.GetUserId();
                var me = db.Users.Where(u => u.Id == myId).FirstOrDefault();
                ApplicationUser applicationUser = db.Users.Find(id);
                if (applicationUser != me)
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

            return RedirectToAction("IndexAllOmbuds");
        }               

        // GET: /Account/IndexAdmOff
        [Authorize(Roles = "Admin")]
        public ActionResult IndexAdmOff()
        {
            var role = db.Roles.SingleOrDefault(m => m.Name == "AdministrativeOfficial");
            AdmOffIndexVM admOffIndexVM = new AdmOffIndexVM();
            admOffIndexVM.AdmOffsExist = false;
            if (role != null)
            {
                var admOffs = db.Users.Where(m => m.Roles.Any(r => r.RoleId == role.Id)).OrderBy(m => m.LastName);

                if (admOffs.Count() > 0)
                {
                    List<AdmOffForVM> admOffForVMList = new List<AdmOffForVM>();
                    foreach (var admOff in admOffs)
                    {
                        AdmOffForVM AdmOffForVM = new AdmOffForVM
                        {
                            Id = admOff.Id,
                            FirstName = admOff.FirstName,
                            LastName = admOff.LastName,
                            SSN = admOff.SSN,
                            Email = admOff.Email,
                            PhoneNumber = admOff.PhoneNumber
                        };
                        admOffForVMList.Add(AdmOffForVM);
                    }
                    admOffIndexVM.AdmOffForVMList = admOffForVMList;
                    admOffIndexVM.AdmOffsExist = true;
                }
            }
            return View("IndexAdmOff", admOffIndexVM);
        }

        // GET: /Account/NewAdmOff
        [Authorize(Roles = "Admin, AdministrativeOfficial")]
        public ActionResult NewAdmOff()
        {
            return View();
        }

        // POST: /Account/NewAdmOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, AdministrativeOfficial")]
        public async Task<ActionResult> NewAdmOff(NewAdmOffVM vm)
        {
            var currentId = User.Identity.GetUserId();
            ApplicationUser currentUser = UserManager.Users.Where(u => u.Id == currentId).FirstOrDefault();

            bool errorFound = false;
            //Check that the SSN has the correct format
            if (!string.IsNullOrWhiteSpace(vm.SSN))
            {
                vm.SSN = vm.SSN.Trim();
                Regex regex = new Regex(@"^([1-9][0-9]{3})(((0[13578]|1[02])(0[1-9]|[12][0-9]|3[01]))|((0[469]|11)(0[1-9]|[12][0-9]|30))|(02(0[1-9]|[12][0-9])))[-]?\d{4}$");
                Match match = regex.Match(vm.SSN);
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

            //Check that the administrative official is born in the 20th or 21st century
            if (!errorFound)
            {
                if (int.Parse(vm.SSN.Substring(0, 2)) != 19 && int.Parse(vm.SSN.Substring(0, 2)) != 20)
                {
                    ModelState.AddModelError("SSN", "Handläggaren måste vara född på 1900- eller 2000-talet.");
                    errorFound = true;
                }
            }

            //Check that the administrative official is at least 18 years old and was not born in the future:-)
            if (!errorFound)
            {
                DateTime admOffBirthday = new DateTime(int.Parse(vm.SSN.Substring(0, 4)), int.Parse(vm.SSN.Substring(4, 2)), int.Parse(vm.SSN.Substring(6, 2)));
                if (admOffBirthday.Date > DateTime.Now.Date)
                {
                    ModelState.AddModelError("SSN", "Födelsedatumet får inte vara senare än idag.");
                    errorFound = true;
                }
                else if (admOffBirthday > DateTime.Now.AddYears(-18))
                {
                    ModelState.AddModelError("SSN", "Handläggaren måste vara minst 18 år.");
                    errorFound = true;
                }
            }

            //Check if there is an administrative official with the same SSN already.
            if (!errorFound)
            {
                if (UserManager.Users.Where(u => u.SSN == vm.SSN).Any())
                {
                    ModelState.AddModelError("SSN", "Det finns redan en användare med det personnummret");
                    errorFound = true;
                }
            }

            if (!errorFound)
            {
                if (vm.SSN.Length == 12)
                {
                    vm.SSN = vm.SSN.Insert(8, "-");
                }
            }

            if (UserManager.Users.Where(u => u.Email == vm.Email).Any())
                ModelState.AddModelError("Email", "Det finns redan en användare med den e-postaddressen");
            if (ModelState.IsValid) //&& vm.SSN == vm.ConfirmSSN
            {
                var user = new ApplicationUser
                {
                    //UserName = $"{vm.FirstName} {vm.LastName}", //For use with BankID
                    UserName = vm.Email,
                    Email = vm.Email,
                    FirstName = vm.FirstName,
                    LastName = vm.LastName,
                    PhoneNumber = vm.PhoneNumber,
                    LastLogon = DateTime.Now,
                    SSN = vm.SSN
                };
                var result = await UserManager.CreateAsync(user, vm.Password);
                if (result.Succeeded)
                {
                    UserManager.AddToRole(user.Id, "AdministrativeOfficial");
                    return RedirectToAction("IndexAdmOff", "Account");
                }
                AddErrors(result);
            }
            return View(vm);
        }

        // GET: AdmOff/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult EditAdmOff(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser admOff = db.Users.Where(u => u.Id == id).FirstOrDefault();
            if (admOff == null)
            {
                return HttpNotFound();
            }
            ApplicationUser currentUser = db.Users.Where(u => u.Id == id).FirstOrDefault();
            AdmOffEditVM admOffEditVM = new AdmOffEditVM();
            admOffEditVM.Id = id;
            admOffEditVM.FirstName = currentUser.FirstName;
            admOffEditVM.LastName = currentUser.LastName;
            admOffEditVM.PhoneNumber = currentUser.PhoneNumber;
            admOffEditVM.Email = currentUser.Email;
            admOffEditVM.SSN = currentUser.SSN;
            return View("EditAdmOff", admOffEditVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditAdmOff(AdmOffEditVM admOffEditVM)
        {
            var currentId = User.Identity.GetUserId();
            ApplicationUser currentUser = UserManager.Users.Where(u => u.Id == currentId).FirstOrDefault();

            bool errorFound = false;
            //Check that the SSN has the correct format
            if (!string.IsNullOrWhiteSpace(admOffEditVM.SSN))
            {
                admOffEditVM.SSN = admOffEditVM.SSN.Trim();
                Regex regex = new Regex(@"^([1-9][0-9]{3})(((0[13578]|1[02])(0[1-9]|[12][0-9]|3[01]))|((0[469]|11)(0[1-9]|[12][0-9]|30))|(02(0[1-9]|[12][0-9])))[-]?\d{4}$");
                Match match = regex.Match(admOffEditVM.SSN);
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

            //Check that the administrative official is born in the 20th or 21st century
            if (!errorFound)
            {
                if (int.Parse(admOffEditVM.SSN.Substring(0, 2)) != 19 && int.Parse(admOffEditVM.SSN.Substring(0, 2)) != 20)
                {
                    ModelState.AddModelError("SSN", "Handläggaren måste vara född på 1900- eller 2000-talet.");
                    errorFound = true;
                }
            }

            //Check that the administrative official is at least 18 years old and was not born in the future:-)
            if (!errorFound)
            {
                DateTime admOffBirthday = new DateTime(int.Parse(admOffEditVM.SSN.Substring(0, 4)), int.Parse(admOffEditVM.SSN.Substring(4, 2)), int.Parse(admOffEditVM.SSN.Substring(6, 2)));
                if (admOffBirthday.Date > DateTime.Now.Date)
                {
                    ModelState.AddModelError("SSN", "Födelsedatumet får inte vara senare än idag.");
                    errorFound = true;
                }
                else if (admOffBirthday > DateTime.Now.AddYears(-18))
                {
                    ModelState.AddModelError("SSN", "Handläggaren måste vara minst 18 år.");
                    errorFound = true;
                }
            }

            //Check if there is an administrative official with the same SSN already.
            if (!errorFound)
            {
                if (UserManager.Users.Where(u => u.SSN == admOffEditVM.SSN).Where(u => u.Id != admOffEditVM.Id).Any())
                {
                    ModelState.AddModelError("SSN", "Det finns redan en användare med det personnummret");
                    errorFound = true;
                }
            }

            if (!errorFound)
            {
                if (admOffEditVM.SSN.Length == 12)
                {
                    admOffEditVM.SSN = admOffEditVM.SSN.Insert(8, "-");
                }
            }

            if (UserManager.Users.Where(u => u.Email == admOffEditVM.Email).Where(u => u.Id != admOffEditVM.Id).Any())
                ModelState.AddModelError("Email", "Det finns redan en användare med den e-postaddressen");
            if (ModelState.IsValid) //&& admOffEditVM.SSN == admOffEditVM.ConfirmSSN
            {
                var user = db.Users.Find(admOffEditVM.Id);
                user.UserName = admOffEditVM.Email;
                user.Email = admOffEditVM.Email;
                user.FirstName = admOffEditVM.FirstName;
                user.LastName = admOffEditVM.LastName;
                user.PhoneNumber = admOffEditVM.PhoneNumber;
                user.SSN = admOffEditVM.SSN;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexAdmOff");

                //var user = new ApplicationUser
                //{
                //    //UserName = $"{admOffEditVM.FirstName} {admOffEditVM.LastName}", //For use with BankID
                //    UserName = admOffEditVM.Email,
                //    Email = admOffEditVM.Email,
                //    FirstName = admOffEditVM.FirstName,
                //    LastName = admOffEditVM.LastName,
                //    PhoneNumber = admOffEditVM.PhoneNumber,
                //    LastLogon = DateTime.Now,
                //    SSN = admOffEditVM.SSN
                //};
                //var result = await UserManager.UpdateAsync(user);
                //if (result.Succeeded)
                //{
                //    UserManager.AddToRole(user.Id, "AdministrativeOfficial");
                //    return RedirectToAction("IndexAdmOff", "Account");
                //}
                //AddErrors(result);
                //return View(admOffEditVM);
            }
            return View("EditAdmOff", admOffEditVM);          
        }

        // GET: Account/DetailsAdmOff
        [Authorize(Roles = "Admin")]
        public ActionResult DetailsAdmOff(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            DetailsAdmOffVM detailsAdmOffVM = new DetailsAdmOffVM();
            detailsAdmOffVM.FirstName = user.FirstName;
            detailsAdmOffVM.LastName = user.LastName;
            detailsAdmOffVM.SSN = user.SSN;
            detailsAdmOffVM.Email = user.Email;
            detailsAdmOffVM.PhoneNumber = user.PhoneNumber;

            return View("DetailsAdmOff", detailsAdmOffVM);
        }

        // GET: Acount/DeleteAdmOff
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteAdmOff(string id)
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
            AdmOffDeleteVM admOffVM = new AdmOffDeleteVM();
            admOffVM.Id = id;
            admOffVM.FirstName = applicationUser.FirstName;
            admOffVM.LastName = applicationUser.LastName;
            admOffVM.SSN = applicationUser.SSN;
            admOffVM.PhoneNumber = applicationUser.PhoneNumber;
            admOffVM.Email = applicationUser.Email;
            return View("DeleteAdmOff", admOffVM);
        }

        // POST: Account/DeleteAdmOff/5
        [HttpPost, ActionName("DeleteAdmOff")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmedAdmOff(string id, string submitButton)
        {
            if (submitButton == "Bekräfta")
            {
                var myId = User.Identity.GetUserId();
                var me = db.Users.Where(u => u.Id == myId).FirstOrDefault();

                ApplicationUser applicationUser = db.Users.Find(id);
               
                if (applicationUser != me)
                {
                    var claimsForAdmOff = db.Claims.Where(c => c.AdmOffId == id).ToList();
                    foreach (var claim in claimsForAdmOff)
                    {
                        claim.AdmOffId = null;
                        claim.AdmOffName = "-";

                        db.Entry(claim).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    db.Users.Remove(applicationUser);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("IndexAdmOff");
        }

        // GET: Ombud for all companies
        [Authorize(Roles = "Admin")]
        public ActionResult IndexAllOmbuds()
        {
            IndexAllOmbudsVM ombudIndexVM = new IndexAllOmbudsVM();

            var role = db.Roles.SingleOrDefault(r => r.Name == "Ombud");
            if (role != null)
            {
                var ombuds = db.Users.Where(m => m.Roles.Any(r => r.RoleId == role.Id)).OrderBy(m => m.LastName).ToList();

                //if (ombuds.Count() > 0)
                //{
                    List<OmbudForVM> ombudForVMList = new List<OmbudForVM>();
                    foreach (var ombud in ombuds)
                    {
                        OmbudForVM ombudForVM = new OmbudForVM();
                        ombudForVM.Id = ombud.Id;
                        ombudForVM.CareCompanyId = (int)ombud.CareCompanyId;
                        ombudForVM.FirstName = ombud.FirstName;
                        ombudForVM.LastName = ombud.LastName;
                        ombudForVM.SSN = ombud.SSN;
                        ombudForVM.Email = ombud.Email;
                        ombudForVM.PhoneNumber = ombud.PhoneNumber;
                        ombudForVMList.Add(ombudForVM);
                    }
                    ombudIndexVM.OmbudList = ombudForVMList;
                //}
            }

            var companies = db.CareCompanies.OrderBy(c => c.Id).ToList();

            ombudIndexVM.CareCompanyList = companies;

            return View(ombudIndexVM);
        }

        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            var vm = new RegisterViewModel();
            List<SelectListItem> collectiveAgreements = new List<SelectListItem>();
            collectiveAgreements = new ApplicationDbContext().CollectiveAgreementHeaders.ToList().ConvertAll(c => new SelectListItem
            {
                Value = $"{c.Id}",
                Text = c.Name
            });
            vm.CollectiveAgreements = new SelectList(collectiveAgreements, "Value", "Text");
            return View(vm);
        }

        // Used with BankID
        // GET: /Account/RegisterConfirm
        //[AllowAnonymous]
        //public ActionResult RegisterConfirm(RegisterViewModel model)
        //{
        //    return View(model);
        //}

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            var currentId = User.Identity.GetUserId();
            ApplicationUser currentUser = UserManager.Users.Where(u => u.Id == currentId).FirstOrDefault();

            bool errorFound = false;
            //Check that the SSN has the correct format
            if (!string.IsNullOrWhiteSpace(model.SSN))
            {
                model.SSN = model.SSN.Trim();
                Regex regex = new Regex(@"^([1-9][0-9]{3})(((0[13578]|1[02])(0[1-9]|[12][0-9]|3[01]))|((0[469]|11)(0[1-9]|[12][0-9]|30))|(02(0[1-9]|[12][0-9])))[-]?\d{4}$");
                Match match = regex.Match(model.SSN);
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
                if (int.Parse(model.SSN.Substring(0, 2)) != 19 && int.Parse(model.SSN.Substring(0, 2)) != 20)
                {
                    ModelState.AddModelError("SSN", "Ombudet måste vara fött på 1900- eller 2000-talet.");
                    errorFound = true;
                }
            }

            //Check that the ombud is at least 18 years old and was not born in the future:-)
            if (!errorFound)
            {
                DateTime ombudBirthday = new DateTime(int.Parse(model.SSN.Substring(0, 4)), int.Parse(model.SSN.Substring(4, 2)), int.Parse(model.SSN.Substring(6, 2)));
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

            ApplicationDbContext db = new ApplicationDbContext();
            //Check if there is an ombud with the same SSN already in the company. The same ombud is allowed in another company.
            if (!errorFound)
            {
                var twinOmbud = UserManager.Users.Where(u => u.SSN == model.SSN).FirstOrDefault();
                if (twinOmbud != null && db.CareCompanies.Where(c => c.Id == twinOmbud.CareCompanyId).FirstOrDefault().OrganisationNumber == model.CompanyOrganisationNumber)
                {
                    ModelState.AddModelError("SSN", "Det finns redan ett ombud med detta personnummer på samma bolag.");
                    errorFound = true;
                }
            }

            if (!errorFound)
            {
                if (model.SSN.Length == 12)
                {
                    model.SSN = model.SSN.Insert(8, "-");
                }
            }

            if (db.CareCompanies.Where(c => c.OrganisationNumber == model.CompanyOrganisationNumber).Any())
                ModelState.AddModelError("CompanyOrganisationNumber", "Det finns redan ett bolag med det organisationsnumret.");
            if (UserManager.Users.Where(u => u.Email == model.Email).Any())
                ModelState.AddModelError("Email", "Det finns redan ett konto med den e-postadressen.");

            if (ModelState.IsValid)
            {
                /*System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;


                using (var client = new RpServicePortTypeClient())
                {
                    var authRequest = new AuthenticateRequestType();
                    authRequest.personalNumber = model.SSN;
                    if (model.Type == "Mobilt")
                    {
                        RequirementType conditions = new RequirementType
                        {
                            condition = new[]
                            {
                                new ConditionType()
                                {
                                    key = "certificatePolicies",
                                    value = new[] {"1.2.3.4.25"},
                                }
                            }
                        };
                        authRequest.requirementAlternatives = new[] { conditions };
                    }
                
                
                    OrderResponseType response = client.Authenticate(authRequest);
                
                    CollectResponseType registerCollectResult;
                
                    do
                    {
                        try
                        {
                            registerCollectResult = client.Collect(response.orderRef);
                        }
                        catch
                        {
                            return View("RegisterConfirm", model);
                        }
                        System.Threading.Thread.Sleep(1000);
                    } while (registerCollectResult.progressStatus != ProgressStatusType.COMPLETE);*/

                CareCompany company = new CareCompany()
                {
                    IsActive = true,
                    CompanyPhoneNumber = model.CompanyPhoneNumber,
                    Postcode = model.Postcode,
                    City = model.City,
                    OrganisationNumber = model.CompanyOrganisationNumber,
                    StreetAddress = model.StreetAddress,
                    SelectedCollectiveAgreementId = model.SelectedCollectiveAgreementId,
                    CollectiveAgreementSpecName = model.CollectiveAgreementSpecName,
                    AccountNumber = model.AccountNumber,
                    CompanyName = model.CompanyName
                };
                db.CareCompanies.Add(company);
                var user = new ApplicationUser
                {
                    //UserName = $"{registerCollectResult.name} {registerCollectResult.surname}", For use with BankId
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.OmbudPhoneNumber,
                    LastLogon = DateTime.Now,
                    SSN = model.SSN
                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {

                    db.SaveChanges();
                    user.CareCompanyId = company.Id;
                    UserManager.AddToRole(user.Id, "Ombud");
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Claims");
                }
                string errorMessagePassword = "";
                foreach (var error in result.Errors)
                {
                    if (error.Contains("Passwords must have at least one digit ('0'-'9')."))
                    {
                        errorMessagePassword = errorMessagePassword + "Lösenordet behöver innehålla åtminstone en siffra.";
                    }
                    if (error.Contains("Passwords must have at least one non letter or digit character."))
                    {
                        errorMessagePassword = errorMessagePassword + " Lösenordet måste innehålla åtminstone ett specialtecken.";
                    }
                    if (error.Contains("Passwords must have at least one uppercase ('A'-'Z')."))
                    {
                        errorMessagePassword = errorMessagePassword + " Lösenordet måste innehålla åtminstone en stor bokstav.";
                    }
                    if (error.Contains("Passwords must have at least one lowercase ('a'-'z')."))
                    {
                        errorMessagePassword = errorMessagePassword + " Lösenordet måste innehålla åtminstone en liten bokstav.";
                    }
                }
                if (!string.IsNullOrWhiteSpace(errorMessagePassword))
                    ModelState.AddModelError("Password", errorMessagePassword.Trim());
                //AddErrors(result);
                //}
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
        }

        // GET: Ombud/Create
        [Authorize(Roles = "Admin, Ombud")]
        public ActionResult CreateOmbud()
        {
            OmbudCreateVM ombudCreateVM = new OmbudCreateVM();

            if (User.IsInRole("Admin"))
            {
                var carecompanies = db.CareCompanies.Where(c => c.IsActive == true).ToList();
                //List<int> carecompanyIds = new List<int>(); //This list is required in order to be able to map the selected ddl ids to Assistant records in the db.
                var carecompanyDdlString = new List<SelectListItem>();
                for (int i = 0; i < carecompanies.Count(); i++)
                {
                    carecompanyDdlString.Add(new SelectListItem() { Text = carecompanies[i].CompanyName, Value = carecompanies[i].Id.ToString() });
                    //carecompanyIds.Add(carecompanies[i].Id);
                }
                ombudCreateVM.CareCompanies = carecompanyDdlString;
                //ombudCreateVM.CareCompanyIds = carecompanyIds;
            }

            return View("CreateOmbud", ombudCreateVM);
        }

        // POST: Ombud/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Ombud")]
        //public ActionResult CreateOmbud([Bind(Include = "Id,FirstName,LastName,LastLogon,CareCompanyId,SSN,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser applicationUser)
        //public ActionResult CreateOmbud([Bind(Include = "Id,FirstName,LastName,CareCompanyId,CareCompanyName,SSN,Email,PhoneNumber")] OmbudCreateVM ombudCreateVM)
        public async Task<ActionResult> CreateOmbud(OmbudCreateVM vm)
        {
            var currentId = User.Identity.GetUserId();
            ApplicationUser currentUser = UserManager.Users.Where(u => u.Id == currentId).FirstOrDefault();

            bool errorFound = false;
            //Check that the SSN has the correct format
            if (!string.IsNullOrWhiteSpace(vm.SSN))
            {
                vm.SSN = vm.SSN.Trim();
                Regex regex = new Regex(@"^([1-9][0-9]{3})(((0[13578]|1[02])(0[1-9]|[12][0-9]|3[01]))|((0[469]|11)(0[1-9]|[12][0-9]|30))|(02(0[1-9]|[12][0-9])))[-]?\d{4}$");
                Match match = regex.Match(vm.SSN);
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
                if (int.Parse(vm.SSN.Substring(0, 2)) != 19 && int.Parse(vm.SSN.Substring(0, 2)) != 20)
                {
                    ModelState.AddModelError("SSN", "Ombudet måste vara fött på 1900- eller 2000-talet.");
                    errorFound = true;
                }
            }

            //Check that the ombud is at least 18 years old and was not born in the future:-)
            if (!errorFound)
            {
                DateTime ombudBirthday = new DateTime(int.Parse(vm.SSN.Substring(0, 4)), int.Parse(vm.SSN.Substring(4, 2)), int.Parse(vm.SSN.Substring(6, 2)));
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
                var twinOmbud = UserManager.Users.Where(u => u.SSN == vm.SSN).FirstOrDefault();
                if (twinOmbud != null && twinOmbud.CareCompanyId == currentUser.CareCompanyId)
                {
                    ModelState.AddModelError("SSN", "Det finns redan ett ombud med detta personnummer");
                    errorFound = true;
                }
            }

            if (!errorFound)
            {
                if (vm.SSN.Length == 12)
                {
                    vm.SSN = vm.SSN.Insert(8, "-");
                }
            }

            //Check if a carecompany has been selected
            if (!errorFound && vm.SelectedCareCompanyId == null && User.IsInRole("Admin"))
            {
                ModelState.AddModelError("CareCompanies", "Assistansbolag måste väljas.");
            }

            if (UserManager.Users.Where(u => u.Email == vm.Email).Any())
                ModelState.AddModelError("Email", "Det finns redan en användare med den e-postadressen");

            if (ModelState.IsValid)
            {
                //var currentUserId = User.Identity.GetUserId();
                //var currentUser = UserManager.Users.Where(u => u.Id == currentUserId).FirstOrDefault();
                if (User.IsInRole("Ombud"))
                {
                    ApplicationUser newOmbud = new ApplicationUser
                    {
                        UserName = vm.Email,
                        FirstName = vm.FirstName,
                        LastName = vm.LastName,
                        CareCompanyId = currentUser.CareCompanyId,
                        Email = vm.Email,
                        PhoneNumber = vm.PhoneNumber,
                        SSN = vm.SSN,
                        LastLogon = DateTime.Now
                    };
                    var result = await UserManager.CreateAsync(newOmbud, vm.Password);
                    if (result.Succeeded)
                    {
                        UserManager.AddToRole(newOmbud.Id, "Ombud");
                        return RedirectToAction("IndexOmbud", "CareCompanies");
                    }
                    AddErrors(result);
                }
                else if (User.IsInRole("Admin"))
                {
                    ApplicationUser newOmbud = new ApplicationUser
                    {
                        UserName = vm.Email,
                        FirstName = vm.FirstName,
                        LastName = vm.LastName,
                        CareCompanyId = vm.SelectedCareCompanyId,
                        Email = vm.Email,
                        PhoneNumber = vm.PhoneNumber,
                        SSN = vm.SSN,
                        LastLogon = DateTime.Now
                    };
                    var result = await UserManager.CreateAsync(newOmbud, vm.Password);
                    if (result.Succeeded)
                    {
                        UserManager.AddToRole(newOmbud.Id, "Ombud");
                        return RedirectToAction("IndexAllOmbuds", "Account");
                    }
                    AddErrors(result);
                }
            }
            if (User.IsInRole("Admin"))
            {
                var carecompanies = db.CareCompanies.Where(c => c.IsActive == true).ToList();              
                //List<int> carecompanyIds = new List<int>(); //This list is required in order to be able to map the selected ddl ids to Assistant records in the db.
                var carecompanyDdlString = new List<SelectListItem>();
                for (int i = 0; i < carecompanies.Count(); i++)
                {
                    carecompanyDdlString.Add(new SelectListItem() { Text = carecompanies[i].CompanyName, Value = carecompanies[i].Id.ToString() });
                    //carecompanyIds.Add(carecompanies[i].Id);
                }
                vm.CareCompanies = carecompanyDdlString;
                //ombudCreateVM.CareCompanyIds = carecompanyIds;
            }
            return View(vm);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/BankIDLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult BankIDLogin(string ssn, string ReturnUrl, string type)
        {

            return RedirectToAction("BankIDWaitScreen", new { SSN = ssn, returnUrl = ReturnUrl, Type = type });
        }

        // GET: /Account/BankIDWaitScreen
        [AllowAnonymous]
        public ActionResult BankIDWaitScreen(string SSN, string returnUrl, string Type)
        {
            IDLoginVM VM = new IDLoginVM
            {
                ssn = SSN,
                type = Type,
                ReturnUrl = returnUrl
            };

            return View("BankIDWaitScreen", VM);
        }

        //
        // POST: /Account/BankIDWaitScreen
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BankIDWaitScreen(IDLoginVM model)
        {
            if (UserManager.Users.Where(u => u.SSN == model.ssn).Any())
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;


                using (var client = new RpServicePortTypeClient())
                {
                    var authRequest = new AuthenticateRequestType();
                    authRequest.personalNumber = model.ssn;
                    if (model.type == "Mobilt")
                    {
                        RequirementType conditions = new RequirementType
                        {
                            condition = new[]
                            {
                                new ConditionType()
                                {
                                    key = "certificatePolicies",
                                    value = new[] {"1.2.3.4.25"}
                                }
                            }
                        };
                        authRequest.requirementAlternatives = new[] { conditions };
                    }


                    OrderResponseType response = client.Authenticate(authRequest);

                    CollectResponseType result;

                    do
                    {
                        try
                        {
                            result = client.Collect(response.orderRef);
                        }
                        catch
                        {
                            return View("Login");
                        }
                        System.Threading.Thread.Sleep(1000);
                    } while (result.progressStatus != ProgressStatusType.COMPLETE);

                    var user = UserManager.Users.Where(u => u.SSN == model.ssn).FirstOrDefault();
                    await SignInManager.SignInAsync(user, true, true);
                }

                if (!string.IsNullOrWhiteSpace(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);

                return RedirectToAction("Index", "Claims");
            }
            return View("BankIDWaitScreen");
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, model.ReturnUrl, model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}