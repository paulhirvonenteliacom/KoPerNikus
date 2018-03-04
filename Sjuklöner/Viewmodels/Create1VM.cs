using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sjuklöner.Viewmodels
{
    public class Create1VM
    {
        public string Heading { get; set; }

        public string ClaimNumber { get; set; }

        public int CompletionStage { get; set; }

        [Required]
        [Display(Name = "Vårdgivarens organisationsnummer")]
        [RegularExpression(@"[0-9]{6}-[0-9]{4}$", ErrorMessage = "Inte ett giltigt organisationsnummer.")]
        public string OrganisationNumber { get; set; }

        [Display(Name = "Mottagare av beslut (e-post)")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Kundens personnummer")]
        //[RegularExpression(@"^((19|20)[0-9]{2})(((0[13578]|1[02])(0[1-9]|[12][0-9]|3[01]))|((0[469]|11)(0[1-9]|[12][0-9]|30))|(02(0[1-9]|[12][0-9])))[-]?([a-zåäö]|[0-9]){4}$", ErrorMessage = "Ej giltigt personnummer. Formaten YYYYMMDD-NNNN och YYYYMMDDNNNN är giltiga.")]
        public string CustomerSSN { get; set; }

        [Required]
        [Display(Name = "Kundens för- och efternamn")]
        public string CustomerName { get; set; }

        [Required]
        [Display(Name = "Kundens adress")]
        public string CustomerAddress { get; set; }

        [Required]
        [Display(Name = "Kundens tel.nr (inkl. riktnr.")]
        public string CustomerPhoneNumber { get; set; }

        [Required]
        [Display(Name = "Första sjukdag")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime FirstDayOfSicknessDate { get; set; }

        [Required]
        [Display(Name = "Sista sjukdag")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime LastDayOfSicknessDate { get; set; }

        //[Required]
        [Range(typeof(int), "1", "999999", ErrorMessage = "En ordinarie assistent måste väljas")]
        public int? SelectedRegAssistantId { get; set; }

        //[Required]
        [Display(Name = "Ordinarie assistent")]
        public IEnumerable<SelectListItem> RegularAssistants { get; set; }

        //[Required]
        [Range(typeof(int), "1", "999999", ErrorMessage = "En vikarierande assistent måste väljas")]
        public int? SelectedSubAssistantId { get; set; }

        //[Required]
        [Display(Name = "Vikarierande assistent")]
        public IEnumerable<SelectListItem> SubstituteAssistants { get; set; }

        public List<int> AssistantIds { get; set; } //This list is required in order to be able to map the selected ddl ids to Assistant records in the db.

        //public bool Rejected { get; set; }

        //public string RejectReason { get; set; }
    }
}