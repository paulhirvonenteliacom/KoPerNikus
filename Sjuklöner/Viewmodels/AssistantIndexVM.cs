using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Sjuklöner.Models;

namespace Sjuklöner.Viewmodels
{
    public class AssistantIndexVM
    {
        public string CareCompanyId { get; set; }

        public string CareCompanyName { get; set; }

        public List<Assistant> AssistantList { get; set; }

        //public class AssistantForVM
        //{
        //    public int AssistantId { get; set; }

        //    [Display(Name = "Efternamn")]
        //    public string FirstName { get; set; }

        //    [Display(Name = "Förnamn")]
        //    public string LastName { get; set; }

        //    [EmailAddress]
        //    [Display(Name = "E-post")]
        //    public string Email { get; set; }

        //    [Display(Name = "Telefonnummer")]
        //    public string PhoneNumber { get; set; }
        //}
    }
}
