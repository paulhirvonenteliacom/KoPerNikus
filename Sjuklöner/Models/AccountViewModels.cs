using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        [RegularExpression(@"^([1-9][0-9]{3})(((0[13578]|1[02])(0[1-9]|[12][0-9]|3[01]))|((0[469]|11)(0[1-9]|[12][0-9]|30))|(02(0[1-9]|[12][0-9])))[-]?\d{4}$", ErrorMessage = "Ej giltigt personnummer. Formaten YYYYMMDD-NNNN och YYYYMMDDNNNN är giltiga.")]
        [Display(Name = "Personnummer")]
        public string SSN { get; set; }

        [Required]
        [Display(Name = "Förnamn")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Efternamn")]
        public string LastName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0}et måste vara minst {2} tecken långt.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Lösenord")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta lösenord")]
        [Compare("Password", ErrorMessage = "Lösenordet och det bekräftade lösenordet är inte lika.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Tel. nr. (inkl. riktnr.)")]
        public string OmbudPhoneNumber { get; set; }

        [Required]
        [Display(Name = "Bolagets namn")]
        public string CompanyName { get; set; }

        [Required]
        [Display(Name = "Organisationsnummer")]
        [RegularExpression(@"[0-9]{6}-[0-9]{4}$", ErrorMessage = "Formatet på organisationsnummret ska vara XXXXXX-XXXX där alla X är siffror.")]
        public string CompanyOrganisationNumber { get; set; }

        [Required]
        [Display(Name = "Tel. nr. (inkl. riktnr.)")]
        public string CompanyPhoneNumber { get; set; }

        [Required]
        [Display(Name = "Gatuadress")]
        public string StreetAddress { get; set; }

        [Required]
        [Display(Name = "Postnummer")]
        public string Postcode { get; set; }

        [Required]
        [Display(Name = "Ort")]
        public string City { get; set; }

        [Required]
        [Display(Name = "Bank-/Postgironummer")]
        public string AccountNumber { get; set; }

        [Required]
        public int? SelectedCollectiveAgreementId { get; set; }

        [Display(Name = "Kollektivavtal")]
        public IEnumerable<System.Web.Mvc.SelectListItem> CollectiveAgreements { get; set; }

        [Display(Name = "Kollektivavtalets branschbeteckning")]
        public string CollectiveAgreementSpecName { get; set; }

        public string Type { get; set; }
    }

    public class NewAdmOffVM
    {
        [Required]
        [Display(Name = "Personnummer")]
        //[RegularExpression(@"^((19|20)[0-9]{2})(((0[13578]|1[02])(0[1-9]|[12][0-9]|3[01]))|((0[469]|11)(0[1-9]|[12][0-9]|30))|(02(0[1-9]|[12][0-9])))[-]?\d{4}$", ErrorMessage = "Ej giltigt personnummer. Formaten YYYYMMDD-NNNN och YYYYMMDDNNNN är giltiga.")]
        public string SSN { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0}et måste vara minst {2} tecken långt.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Lösenord")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta lösenord")]
        [Compare("Password", ErrorMessage = "Lösenordet och det bekräftade lösenordet är inte lika.")]
        public string ConfirmPassword { get; set; }

        //[Required]        
        //public string ConfirmSSN { get; set; }

        [Required]
        [Display(Name = "Förnamn")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Efternamn")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Tel. nr. (inkl. riktnr.)")]
        public string PhoneNumber { get; set; }
    }

    public class AdmOffEditVM
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "Personnummer")]
        //[RegularExpression(@"^((19|20)[0-9]{2})(((0[13578]|1[02])(0[1-9]|[12][0-9]|3[01]))|((0[469]|11)(0[1-9]|[12][0-9]|30))|(02(0[1-9]|[12][0-9])))[-]?\d{4}$", ErrorMessage = "Ej giltigt personnummer. Formaten YYYYMMDD-NNNN och YYYYMMDDNNNN är giltiga.")]
        public string SSN { get; set; }

        //[Required]
        //[StringLength(100, ErrorMessage = "{0}et måste vara minst {2} tecken långt.", MinimumLength = 6)]
        //[DataType(DataType.Password)]
        //[Display(Name = "Lösenord")]
        //public string Password { get; set; }

        //[Required]
        //[DataType(DataType.Password)]
        //[Display(Name = "Bekräfta lösenord")]
        //[Compare("Password", ErrorMessage = "Lösenordet och det bekräftade lösenordet är inte lika.")]
        //public string ConfirmPassword { get; set; }

        //[Required]        
        //public string ConfirmSSN { get; set; }

        [Required]
        [Display(Name = "Förnamn")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Efternamn")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Tel. nr. (inkl. riktnr.)")]
        public string PhoneNumber { get; set; }
    }

    public class AdmOffIndexVM
    {
        public List<AdmOffForVM> AdmOffForVMList { get; set; }

        public bool AdmOffsExist { get; set; }

        public class AdmOffForVM
        {
            public string Id { get; set; }

            [Display(Name = "Förnamn")]
            public string FirstName { get; set; }

            [Display(Name = "Efternamn")]
            public string LastName { get; set; }

            [Display(Name = "Personnummer")]
            //[RegularExpression(@"^((19|20)[0-9]{2})(((0[13578]|1[02])(0[1-9]|[12][0-9]|3[01]))|((0[469]|11)(0[1-9]|[12][0-9]|30))|(02(0[1-9]|[12][0-9])))[-]?\d{4}$", ErrorMessage = "Ej giltigt personnummer. Formaten YYYYMMDD-NNNN och YYYYMMDDNNNN är giltiga.")]
            public string SSN { get; set; }

            //[DataType(DataType.Password)]
            //[Display(Name = "Bekräfta lösenord")]
            //[Compare("Password", ErrorMessage = "Lösenordet och det bekräftade lösenordet är inte lika.")]
            //public string ConfirmPassword { get; set; }

            //[Required]        
            //public string ConfirmSSN { get; set; }

            [EmailAddress]
            [Display(Name = "E-post")]
            public string Email { get; set; }

            [Display(Name = "Tel. nr. (inkl. riktnr.)")]
            public string PhoneNumber { get; set; }

            //[StringLength(100, ErrorMessage = "{0}et måste vara minst {2} tecken långt.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Lösenord")]
            public string Password { get; set; }
        }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0}et måste vara minst {2} tecken långt.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Lösenord")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta lösenord")]
        [Compare("Password", ErrorMessage = "Lösenordet och det bekräftade lösenordet är inte lika.")]
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
