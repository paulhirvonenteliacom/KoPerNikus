using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Sjuklöner.Models;

namespace Sjuklöner.Viewmodels
{
    public class OmbudIndexVM
    {
        public string CurrentUserId { get; set; }

        public int CareCompanyId { get; set; }

        [Required]
        [Display(Name = "Efternamn")]
        public string CareCompanyName { get; set; }

        public List<OmbudForVM> OmbudForVMList { get; set; }

        public class OmbudForVM
        {
            [Required]
            public string Id { get; set; }

            [Required]
            [Display(Name = "Förnamn")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Efternamn")]
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
}
