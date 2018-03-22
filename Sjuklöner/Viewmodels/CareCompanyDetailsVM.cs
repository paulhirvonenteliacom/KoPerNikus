using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sjuklöner.Viewmodels
{  
    public class CareCompanyDetailsVM
    {       
        [Display(Name = "Bolagets namn")]
        public string CompanyName { get; set; }       
        
        [Display(Name = "Organisationsnummer")]
        [RegularExpression(@"[0-9]{6}-[0-9]{4}$", ErrorMessage = "Formatet på organisationsnummret ska vara XXXXXX-XXXX där alla X är siffror.")]
        public string OrganisationNumber { get; set; }       
       
        [Display(Name = "Tel. nr. (inkl. riktnr.)")]
        public string CompanyPhoneNumber { get; set; }

        [Display(Name = "Gatuadress")]
        public string StreetAddress { get; set; }
       
        [Display(Name = "Postnummer")]
        public string Postcode { get; set; }
       
        [Display(Name = "Ort")]
        public string City { get; set; }
      
        [Display(Name = "Bank-/Postgironummer")]
        public string AccountNumber { get; set; }

        [Display(Name = "Kollektivavtal")]
        public string CollectiveAgreementName { get; set; }

        [Display(Name = "Kollektivavtalets branschbeteckning")]
        public string CollectiveAgreementSpecName { get; set; }
    }
}