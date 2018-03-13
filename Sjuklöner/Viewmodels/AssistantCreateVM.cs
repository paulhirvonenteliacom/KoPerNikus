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
        //[RegularExpression(@"^((19|20)[0-9]{2})(((0[13578]|1[02])(0[1-9]|[12][0-9]|3[01]))|((0[469]|11)(0[1-9]|[12][0-9]|30))|(02(0[1-9]|[12][0-9])))[-]?\d{4}$", ErrorMessage = "Ej giltigt personnummer. Formaten YYYYMMDD-NNNN och YYYYMMDDNNNN är giltiga.")]
        public string AssistantSSN { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Tel.nr. (inkl. riktnr.)")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Timlön (Kr)")]
        [RegularExpression(@"\d{0,3}(\,\d{0,2})?$", ErrorMessage = "Fel format eller för högt värde.")]
        public string HourlySalary { get; set; }

        [Required]
        [Display(Name = "Semesterersättning (%)")]
        [RegularExpression(@"\d{0,2}(\,\d{0,2})?$", ErrorMessage = "Fel format eller för högt värde.")]
        public string HolidayPayRate { get; set; }

        [Required]
        [Display(Name = "Arbetsgivaravgift (%)")]
        [RegularExpression(@"\d{0,2}(\,\d{0,2})?$", ErrorMessage = "Fel format eller för högt värde.")]
        public string PayrollTaxRate { get; set; }

        [Required]
        [Display(Name = "Avtalspension och försäkring (%)")]
        [RegularExpression(@"\d{0,2}(\,\d{0,2})?$", ErrorMessage = "Fel format eller för högt värde.")]
        public string PensionAndInsuranceRate { get; set; }
    }
}
