using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sjuklöner.Models
{
    public class Assistant
    {
        public int Id { get; set; }

        public int CareCompanyId { get; set; }

        [Display(Name = "Förnamn")]
        public string FirstName { get; set; }

        [Display(Name = "Efternamn")]
        public string LastName { get; set; }

        [Display(Name = "Personnummer")]
        public string AssistantSSN { get; set; }

        [EmailAddress]
        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Display(Name = "Telefonnummer")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Timlön (Kr)")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public string HourlySalary { get; set; }

        [Display(Name = "Semesterersättning (%)")]
        [DisplayFormat(DataFormatString = "{0:####}", ApplyFormatInEditMode = true)]
        public string HolidayPayRate { get; set; }

        [Display(Name = "Arbetsgivaravgift (%)")]
        [DisplayFormat(DataFormatString = "{0:####}", ApplyFormatInEditMode = true)]
        public string PayrollTaxRate { get; set; }

        [Display(Name = "Avtalsförsäkring (%)")]
        [DisplayFormat(DataFormatString = "{0:###}", ApplyFormatInEditMode = true)]
        public string InsuranceRate { get; set; }

        [Display(Name = "Kollektivavtalad pension (%)")]
        [DisplayFormat(DataFormatString = "{0:###}", ApplyFormatInEditMode = true)]
        public string PensionRate { get; set; }
    }
}