using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sjuklöner.Viewmodels
{
    public class AssistantClaimVM
    {
        public string Heading { get; set; }

        public string ClaimReference { get; set; }

        [Display(Name = "Vårdgivarens organisationsnummer")]
        [RegularExpression(@"[0-9]{6}-[0-9]{4}$")]
        public string OrganisationNumber { get; set; }

        [Display(Name = "Mottagare av beslut (e-post)")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Kundens personnummer")]
        [RegularExpression(@"(((20)((0[0 - 9])|(1[0 - 7])))|(([1][^ 0 - 8])?\d{2}))((0[1-9])|1[0-2])((0[1-9])|(1[0-9])|(2[0-9])|(3[01]))[-]?\d{4}$")]
        public string CustomerSSN { get; set; }

        //[Required]
        [Display(Name = "Assistentens personnummer")]
        [RegularExpression(@"(((20)((0[0 - 9])|(1[0 - 7])))|(([1][^ 0 - 8])?\d{2}))((0[1-9])|1[0-2])((0[1-9])|(1[0-9])|(2[0-9])|(3[01]))[-]?\d{4}$")]
        public string AssistantSSN { get; set; }

        //[Required]
        [Display(Name = "Vikarie")]
        [RegularExpression(@"(((20)((0[0 - 9])|(1[0 - 7])))|(([1][^ 0 - 8])?\d{2}))((0[1-9])|1[0-2])((0[1-9])|(1[0-9])|(2[0-9])|(3[01]))[-]?\d{4}$")]
        public string StandInSSN { get; set; }

        [Required]
        [Display(Name = "Första sjukdag")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime FirstDayOfSicknessDate { get; set; }

        [Required]
        [Display(Name = "Sista sjukdag")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime LastDayOfSicknessDate { get; set; }

        [Display(Name = "Ordinarie assistents förnamn")]
        public string AssistantFirstName { get; set; }

        [Display(Name = "Ordinarie assistents efternamn")]
        public string AssistantLastNameFirstLetter { get; set; }

        public bool Rejected { get; set; }

        public string RejectReason { get; set; }
    }
}