using Sjuklöner.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sjuklöner.Viewmodels
{
    public class CareCompanyCreateVM
    {
        [Required]
        [Display(Name = "Bolagets namn")]
        public string CompanyName { get; set; }

        [Required]
        [Display(Name = "Organisationsnummer")]
        [RegularExpression(@"[0-9]{6}-[0-9]{4}$", ErrorMessage = "Formatet på organisationsnummret ska vara XXXXXX-XXXX där alla X är siffror.")]
        public string OrganisationNumber { get; set; }

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
    }
}




