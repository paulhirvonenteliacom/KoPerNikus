using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Sjuklöner.Models;

namespace Sjuklöner.Viewmodels
{
    public class OmbudDeleteVM
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
