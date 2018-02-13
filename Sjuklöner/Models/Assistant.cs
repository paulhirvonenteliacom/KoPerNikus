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
        [RegularExpression(@"(((20)((0[0 - 9])|(1[0 - 7])))|(([1][^ 0 - 8])?\d{2}))((0[1-9])|1[0-2])((0[1-9])|(1[0-9])|(2[0-9])|(3[01]))[-]?\d{4}$")]
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

        [Display(Name = "Avtalspension och försäkring (%)")]
        [DisplayFormat(DataFormatString = "{0:###}", ApplyFormatInEditMode = true)]
        public string PensionAndInsuranceRate { get; set; }
    }
}