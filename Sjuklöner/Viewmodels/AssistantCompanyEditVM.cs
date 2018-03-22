using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sjuklöner.Viewmodels
{
    public class AssistantCompanyEditVM
    {
        public int Id { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [Required]
        [Display(Name = "Förnamn")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Efternamn")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Personnummer")]
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