using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sjuklöner.Viewmodels
{
    public class AssistantCreateVM
    {
        [Required]
        [Display(Name = "Förnamn")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Efternamn")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Personnummer")]
        [RegularExpression(@"(((20)((0[0-9])|(1[0-7])))|(([1][^0-8])?\d{2}))((0[1-9])|1[0-2])((0[1-9])|(1[0-9])|(2[0-9])|(3[01]))[-]?\d{4}$", ErrorMessage = "Ej giltigt personnummer. Formaten YYYYMMDD-XXXX och YYYYMMDDXXXX är giltiga.")]
        public string AssistantSSN { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Telefonnummer")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Timlön (Kr)")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public string HourlySalary { get; set; }

        [Required]
        [Display(Name = "Semesterersättning (%)")]
        [DisplayFormat(DataFormatString = "{0:####}", ApplyFormatInEditMode = true)]
        public string HolidayPayRate { get; set; }

        [Required]
        [Display(Name = "Arbetsgivaravgift (%)")]
        [DisplayFormat(DataFormatString = "{0:####}", ApplyFormatInEditMode = true)]
        public string PayrollTaxRate { get; set; }

        [Required]
        [Display(Name = "Avtalspension och försäkring (%)")]
        [DisplayFormat(DataFormatString = "{0:###}", ApplyFormatInEditMode = true)]
        public string PensionAndInsuranceRate { get; set; }
    }
}
