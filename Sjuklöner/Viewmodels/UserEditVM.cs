using Sjuklöner.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sjuklöner.Viewmodels
{
    public class UserEditVM
    {
        public string UserId { get; set; }

        [Display(Name = "Förnamn")]
        public string FirstName { get; set; }

        [Display(Name = "Efternamn")]
        public string LastName { get; set; }

        [Display(Name = "Tel.nummer (inkl. riktnr.)")]
        public string PhoneNumber { get; set; }

        [Display(Name = "E-postadress")]
        public string Email { get; set; }
    }
}