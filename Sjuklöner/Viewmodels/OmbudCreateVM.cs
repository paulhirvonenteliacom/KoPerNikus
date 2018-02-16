using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Sjuklöner.Models;
using static Sjuklöner.Viewmodels.OmbudIndexVM;

namespace Sjuklöner.Viewmodels
{
    public class OmbudCreateVM
    {
        public int? CareCompanyId { get; set; }

        [Display(Name = "Förnamn")]
        public string FirstName { get; set; }

        [Display(Name = "Efternamn")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Telefonnummer")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Personnummer")]
        public string SSN { get; set; }
    }
}
