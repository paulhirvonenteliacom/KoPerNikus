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

namespace Sjuklöner.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

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
                    ModelState.AddModelError("", "Invalid login attempt.");
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
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/NewAdmOff
        public ActionResult NewAdmOff()
        {
            return View();
        }

        //
        // POST: /Account/NewAdmOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> NewAdmOff(NewAdmOffVM vm)
        {
            if (ModelState.IsValid && !UserManager.Users.Where(u => u.SSN == vm.SSN).Any()) //&& vm.SSN == vm.ConfirmSSN
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
                UserManager.AddToRole(user.Id, "AdministrativeOfficial");
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Claims");
                }
                AddErrors(result);
            }
            return View(vm);
        }

        //
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
            var db = new ApplicationDbContext();
            if (db.CareCompanies.Where(c => c.OrganisationNumber == model.CompanyOrganisationNumber).Any())
                ModelState.AddModelError("CompanyOrganisationError", "Det finns redan ett bolag med det organisationsnumret.");
            if (UserManager.Users.Where(u => u.SSN == model.SSN).Any())
                ModelState.AddModelError("SSN", "Det finns redan ett konto med det personnumret");

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

                CareCompany company = new CareCompany();
                company.CompanyPhoneNumber = model.CompanyPhoneNumber;
                company.Postcode = model.Postcode;
                company.City = model.City;
                company.OrganisationNumber = model.CompanyOrganisationNumber;
                company.StreetAddress = model.StreetAddress;
                company.SelectedCollectiveAgreementId = model.SelectedCollectiveAgreementId;
                company.AccountNumber = model.AccountNumber;
                company.CompanyName = model.CompanyName;
                db.CareCompanies.Add(company);
                db.SaveChanges();
                //Find the id of the just saved company
                company = db.CareCompanies.OrderBy(c => c.Id).ToList().Last();
                var user = new ApplicationUser
                {
                    //UserName = $"{registerCollectResult.name} {registerCollectResult.surname}", For use with BankId
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.OmbudPhoneNumber,
                    CareCompanyId = company.Id,
                    LastLogon = DateTime.Now,
                    SSN = model.SSN
                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {

                    db.SaveChanges();
                    UserManager.AddToRole(user.Id, "Ombud");
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Claims");
                }
                AddErrors(result);
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
        public ActionResult CreateOmbud()
        {
            return View("CreateOmbud");
        }

        // POST: Ombud/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult CreateOmbud([Bind(Include = "Id,FirstName,LastName,LastLogon,CareCompanyId,SSN,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser applicationUser)
        //public ActionResult CreateOmbud([Bind(Include = "Id,FirstName,LastName,CareCompanyId,CareCompanyName,SSN,Email,PhoneNumber")] OmbudCreateVM ombudCreateVM)
        public async Task<ActionResult> CreateOmbud(OmbudCreateVM vm)
        {
            //ModelState.Remove(nameof(OmbudCreateVM.Id));
            if (UserManager.Users.Where(u => u.Email == vm.Email).Any())
                ModelState.AddModelError("Email", "Det finns redan en användare med den e-postadressen");
            if (ModelState.IsValid)
            {
                var currentUserId = User.Identity.GetUserId();
                var currentUser = UserManager.Users.Where(u => u.Id == currentUserId).FirstOrDefault();
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
            IDLoginVM VM = new IDLoginVM();
            VM.ssn = SSN;
            VM.type = Type;
            VM.ReturnUrl = returnUrl;

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
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
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