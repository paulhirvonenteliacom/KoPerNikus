using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sjuklöner.Models
{
    public class CollectiveAgreementInfo
    {
        public int Id { get; set; }

        public int CollectiveAgreementHeaderId { get; set; }

        [Required]
        [Display(Name = "Från och med")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Till och med")]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        [Display(Name = "OB, kväll (Kr)")]
        //[RegularExpression(@"\d{0,3}(\,\d{0,2})?$", ErrorMessage = "Fel format eller för stort belopp.")]
        //[DisplayFormat(DataFormatString = "{0:f2}")]
        public string PerHourUnsocialEvening { get; set; }

        [Required]
        [Display(Name = "OB, natt (Kr)")]
        //[RegularExpression(@"\d{0,3}(\,\d{0,2})?$", ErrorMessage = "Fel format eller för stort belopp.")]
        //[DisplayFormat(DataFormatString = "{0:f2}")]
        public string PerHourUnsocialNight { get; set; }

        [Required]
        [Display(Name = "OB, helg (Kr)")]
        //[RegularExpression(@"\d{0,3}(\,\d{0,2})?$", ErrorMessage = "Fel format eller för stort belopp.")]
        //[DisplayFormat(DataFormatString = "{0:f2}")]
        public string PerHourUnsocialWeekend { get; set; }

        [Required]
        [Display(Name = "OB, storhelg (Kr)")]
        //[RegularExpression(@"\d{0,3}(\,\d{0,2})?$", ErrorMessage = "Fel format eller för stort belopp.")]
        //[DisplayFormat(DataFormatString = "{0:f2}")]
        public string PerHourUnsocialHoliday { get; set; }

        [Required]
        [Display(Name = "Jour, vardag (Kr)")]
        //[RegularExpression(@"\d{0,3}(\,\d{0,2})?$", ErrorMessage = "Fel format eller för stort belopp.")]
        //[DisplayFormat(DataFormatString = "{0:f2}")]
        public string PerHourOnCallWeekday { get; set; }

        [Required]
        [Display(Name = "Jour, natt/helg (Kr)")]
        //[RegularExpression(@"\d{0,3}(\,\d{0,2})?$", ErrorMessage = "Fel format eller för stort belopp.")]
        //[DisplayFormat(DataFormatString = "{0:f2}")]
        public string PerHourOnCallWeekend { get; set; }
    }
}