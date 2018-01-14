using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sjuklöner.Viewmodels
{
    public class ClaimAmountVM
    {
        [Display(Name = "Referensnummer")]
        public string ClaimNumber { get; set; }

        [Required]
        [Display(Name = "Sjuklön (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double SickPay { get; set; }

        [Required]
        [Display(Name = "Semesterersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double HolidayPay { get; set; }

        [Required]
        [Display(Name = "Sociala avgifter (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double SocialFees { get; set; }

        [Required]
        [Display(Name = "Övriga avtalsbundna kostnader (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double PensionAndInsurance { get; set; }

        [Required]
        [Display(Name = "Yrkat belopp (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double ClaimSum { get; set; }
    }
}