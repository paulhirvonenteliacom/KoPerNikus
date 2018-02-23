using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Sjuklöner.Models;
using static Sjuklöner.Viewmodels.OmbudIndexVM;

namespace Sjuklöner.Viewmodels
{
    public class OmbudEditVM
    {
        public string Id { get; set; }

        public int CareCompanyId { get; set; }

        [Display(Name = "Bolagsnamn")]
        public string CareCompanyName { get; set; }

        [Required]
        [Display(Name = "Efternamn")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Förnamn")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Personnummer")]
        [RegularExpression(@"(((20)((0[0-9])|(1[0-7])))|(([1][^0-8])?\d{2}))((0[1-9])|1[0-2])((0[1-9])|(1[0-9])|(2[0-9])|(3[01]))?\d{4}$")]
        public string SSN { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Telefonnummer")]
        public string PhoneNumber { get; set; }
    }
}
