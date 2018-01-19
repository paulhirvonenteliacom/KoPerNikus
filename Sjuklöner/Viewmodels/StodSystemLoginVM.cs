using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sjuklöner.Viewmodels
{
    public class StodSystemLoginVM
    {
        [Display(Name = "Användarnamn")]
        //[EmailAddress]
        public string Email { get; set; }

        //[DataType(DataType.Password)]
        [Display(Name = "Lösenord")]
        public string Lösenord { get; set; }

        [Display(Name = "Kom ihåg mig?")]
        public bool RememberMe { get; set; }

        public string ReferenceNumber { get; set; }
    }
}