using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Sjuklöner.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "E-post")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "E-post")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Lösenord")]
        public string Password { get; set; }

        [Display(Name = "Kom ihåg mig?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"(((20)((0[0 - 9])|(1[0 - 7])))|(([1][^ 0 - 8])?\d{2}))((0[1-9])|1[0-2])((0[1-9])|(1[0-9])|(2[0-9])|(3[01]))?\d{4}$")]
        [Display(Name = "Personnummer")]
        public string SSN { get; set; }

        [Display(Name = "Telefonnummer (inkl. riktnummer)")]
        public string OmbudPhoneNumber { get; set; }

        [Display(Name = "Företagets namn")]
        public string CompanyName { get; set; }

        [Display(Name = "Företagets organisationsnummer")]
        public string CompanyOrganisationNumber { get; set; }

        [Display(Name = "Telefonnummer (inkl. riktnummer)")]
        public string CompanyPhoneNumber { get; set; }

        [Display(Name = "Gatuadress")]
        public string StreetAddress { get; set; }

        [Display(Name = "Postnummer")]
        public string Postcode { get; set; }

        [Display(Name = "Ort")]
        public string City { get; set; }

        [Display(Name = "Bank-/Postgironummer")]
        public string AccountNumber { get; set; }

        public int? SelectedCollectiveAgreementId { get; set; }

        [Display(Name = "Kollektivavtal")]
        public IEnumerable<SelectListItem> CollectiveAgreements { get; set; }

        public string Type { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Lösenord")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta lösenord")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-post")]
        public string Email { get; set; }
    }

    public class IDLoginVM
    {
        public string ssn { get; set; }
        public string type { get; set; }
        public string ReturnUrl { get; set; }
    }
}
