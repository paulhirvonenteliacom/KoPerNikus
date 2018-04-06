using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sjuklöner.Models
{
    public class CareCompany
    {
        public int Id { get; set; }

        //public string OmbudId { get; set; }

        //[Display(Name = "Ombudets förnamn")]
        //public string FirstName { get; set; }

        //[Display(Name = "Ombudets Efternamn")]
        //public string LastName { get; set; }

        //[Display(Name = "Ombudets telefonnummer (inkl. riktnr)")]
        //public string OmbudPhoneNumber { get; set; }

        [Required]
        [Display(Name = "Status")]
        public bool IsActive  { get; set; }

        [Required]
        [Display(Name = "Bolagets namn")]
        public string CompanyName { get; set; }

        [Required]
        [Display(Name = "Organisationsnummer")]
        [RegularExpression(@"[0-9]{6}-[0-9]{4}$", ErrorMessage = "Formatet på organisationsnummret ska vara XXXXXX-XXXX där alla X är siffror.")]
        public string OrganisationNumber { get; set; }

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
        [Display(Name = "Tel.nummer (inkl. riktnr.)")]
        public string CompanyPhoneNumber { get; set; }

        [Required]
        public int? SelectedCollectiveAgreementId { get; set; }

        [Display(Name = "Kollektivavtal")]
        public string CollectiveAgreementName { get; set; }

        [Display(Name = "Kollektivavtalets branschbeteckning")]
        public string CollectiveAgreementSpecName { get; set; }
    }
}