using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sjuklöner.Viewmodels
{
    public class DecisionVM
    {
        [Display(Name = "Referensnummer")]
        public string ClaimNumber { get; set; }

        [Display(Name = "Assistansbolag")]
        public string CareCompany { get; set; }

        [Display(Name = "Assistentens personnummer")]
        public string AssistantSSN { get; set; }

        [Display(Name = "Första sjukdag")]
        public DateTime FirstClaimDate { get; set; }

        [Display(Name = "Sista sjukdag")]
        public DateTime LastClaimDate { get; set; }

        [Display(Name = "Yrkat belopp (Kr)")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal ClaimSum { get; set; }

        [Display(Name = "Godkänt belopp (Kr)")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string ApprovedSum { get; set; }

        [Display(Name = "Avslaget belopp (Kr)")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string RejectedSum { get; set; }

        [Display(Name = "Kommentar")]
        public string Comment { get; set; }
    }
}