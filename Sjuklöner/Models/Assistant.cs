using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sjuklöner.Models
{
    public class Assistant:ApplicationUser
    {
        public string UserId { get; set; }

        [Display(Name = "Godkänd av handläggare")]
        public bool Approved { get; set; }

        [Display(Name = "Företagsnamn")]
        public string CompanyName { get; set; }

        [Display(Name = "Gatuadress")]
        public string StreetAddress { get; set; }

        [Display(Name = "Postnummer")]
        public string Postcode { get; set; }

        [Display(Name = "Ort")]
        public string City { get; set; }

        [Display(Name = "Clearingnummer")]
        public string ClearingNumber { get; set; }

        [Display(Name = "Kontonummer")]
        public string AccountNumber { get; set; }

        [Display(Name = "Ordinarie timlön")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal HourlySalary { get; set; }

        [Display(Name = "Semesterersättning (%)")]
        [DisplayFormat(DataFormatString = "{0:####}", ApplyFormatInEditMode = true)]
        public decimal HolidayPayRate { get; set; }

        [Display(Name = "Arbetsgivaravgift (%)")]
        [DisplayFormat(DataFormatString = "{0:####}", ApplyFormatInEditMode = true)]
        public decimal PayrollTaxRate { get; set; }

        [Display(Name = "Avtalsförsäkring (%)")]
        [DisplayFormat(DataFormatString = "{0:###}", ApplyFormatInEditMode = true)]
        public decimal InsuranceRate { get; set; }

        [Display(Name = "Kollektivavtalad pension (%)")]
        [DisplayFormat(DataFormatString = "{0:###}", ApplyFormatInEditMode = true)]
        public decimal PensionRate { get; set; }

        [Display(Name = "Medgivande om lagring av uppgifter")]
        public bool StorageApproval { get; set; }
    }
}