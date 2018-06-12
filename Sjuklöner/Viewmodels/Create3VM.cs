using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sjuklöner.Viewmodels
{
    public class Create3VM
    {
        [Display(Name = "Referensnummer")]
        public string ClaimNumber { get; set; }

        //[Required]
        [Display(Name = "Sjuklön (Kr):")]
        [RegularExpression(@"\d{0,5}(\,\d{0,2})?$", ErrorMessage = "Ogiltigt format eller värde.")]
        public string SickPay { get; set; }

        //[Required]
        [Display(Name = "Semesterersättning (Kr):")]
        [RegularExpression(@"\d{0,5}(\,\d{0,2})?$", ErrorMessage = "Ogiltigt format eller värde.")]
        public string HolidayPay { get; set; }

        //[Required]
        [Display(Name = "Sociala avgifter (Kr):")]
        [RegularExpression(@"\d{0,5}(\,\d{0,2})?$", ErrorMessage = "Ogiltigt format eller värde.")]
        public string SocialFees { get; set; }

        //[Required]
        [Display(Name = "Övriga avtalsbundna kostnader (Kr):")]
        [RegularExpression(@"\d{0,5}(\,\d{0,2})?$", ErrorMessage = "Ogiltigt format eller värde.")]
        public string PensionAndInsurance { get; set; }

        [Display(Name = "Yrkat belopp (Kr):")]
        [RegularExpression(@"\d{0,5}(\,\d{0,2})?$", ErrorMessage = "Ogiltigt format eller värde.")]
        public string ClaimSum { get; set; }

        public bool ShowCalculatedValues { get; internal set; }

        //[Required]
        //[Display(Name = "Sjuklön (Kr):")]
        //[DisplayFormat(DataFormatString = "{0:f2}")]
        //public decimal SickPay { get; set; }

        //[Required]
        //[Display(Name = "Semesterersättning (Kr):")]
        //[DisplayFormat(DataFormatString = "{0:f2}")]
        //public decimal HolidayPay { get; set; }

        //[Required]
        //[Display(Name = "Sociala avgifter (Kr):")]
        //[DisplayFormat(DataFormatString = "{0:f2}")]
        //public decimal SocialFees { get; set; }

        //[Required]
        //[Display(Name = "Övriga avtalsbundna kostnader (Kr):")]
        //[DisplayFormat(DataFormatString = "{0:f2}")]
        //public decimal PensionAndInsurance { get; set; }

        //[Required]
        //[Display(Name = "Yrkat belopp (Kr):")]
        //[DisplayFormat(DataFormatString = "{0:f2}")]
        //public decimal ClaimSum { get; set; }
    }
}