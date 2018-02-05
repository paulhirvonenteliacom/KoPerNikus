using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sjuklöner.Models
{
    public class CareCompany
    {
        public int Id { get; set; }

        public int OmbudId { get; set; }

        [Display(Name = "Företagets namn")]
        public string CompanyName { get; set; }

        [Display(Name = "Företagets organisationsnummer")]
        public string CompanyOrganisationNumber { get; set; }

        [Display(Name = "Gatuadress")]
        public string StreetAddress { get; set; }

        [Display(Name = "Postnummer")]
        public string Postcode { get; set; }

        [Display(Name = "Ort")]
        public string City { get; set; }

        [Display(Name = "Bank-/Postgironummer")]
        public string AccountNumber { get; set; }

        [Display(Name = "Telefonnummer (inkl. riktnummer)")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Kollektivavtal")]
        public string CollectiveAgreement { get; set; }

        [Display(Name = "Medgivande om lagring av uppgifter")]
        public bool StorageApproval { get; set; }
    }
}