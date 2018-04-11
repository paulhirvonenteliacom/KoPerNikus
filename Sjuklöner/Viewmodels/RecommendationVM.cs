using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sjuklöner.Viewmodels
{
    public class RecommendationVM
    {
        [Display(Name = "Referensnummer:")]
        public string ClaimNumber { get; set; }

        [Display(Name = "Komplett ansökan")]
        public bool CompleteCheck { get; set; }
        public string CompleteCheckMsg { get; set; }

        [Display(Name = "Kontroll av fullmakt i Procapita:")]
        public bool ProxyCheck { get; set; }
        public string ProxyCheckMsg { get; set; }

        [Display(Name = "Vårdgivarregistret, IVO:")]
        public bool IvoCheck { get; set; }
        public string IvoCheckMsg { get; set; }

        [Display(Name = "Beslut om assistans i Procapita:")]
        public bool AssistanceCheck { get; set; }
        public string AssistanceCheckMsg { get; set; }

        [Display(Name = "Lönespecifikation, ordinarie assistent:")]
        public bool SalarySpecRegAssistantCheck { get; set; }
        public string SalarySpecRegAssistantCheckMsg { get; set; }

        [Display(Name = "Lönespecifikation, vikarierande assistent:")]
        public bool SalarySpecSubAssistantCheck { get; set; }
        public string SalarySpecSubAssistantCheckMsg { get; set; }

        [Display(Name = "Sjukfrånvaroanmälan:")]
        public bool SickleaveNotificationCheck { get; set; }
        public string SickleaveNotificationCheckMsg { get; set; }

        [Display(Name = "Läkarintyg:")]
        public bool MedicalCertificateCheck { get; set; }
        public string MedicalCertificateCheckMsg { get; set; }

        public int NumberOfSickDays { get; set; }

        [Display(Name = "Tidsredovisning FK, ordinarie assistent:")]
        public bool FKRegAssistantCheck { get; set; }
        public string FKRegAssistantCheckMsg { get; set; }

        [Display(Name = "Tidsredovisning FK, vikarierande assistent:")]
        public bool FKSubAssistantCheck { get; set; }
        public string FKSubAssistantCheckMsg { get; set; }

        [Display(Name = "Beslutsunderlag till Procapita:")]
        public bool BasisForDecision { get; set; }
        public string BasisForDecisionMsg { get; set; }

        public DateTime BasisForDecisionTransferStartTimeStamp { get; set; }
        public DateTime BasisForDecisionTransferFinishTimeStamp { get; set; }

        [Display(Name = "Beslut från Procapita:")]
        public bool Decision { get; set; }
        public string DecisionMsg { get; set; }

        public DateTime DecisionTransferTimeStamp { get; set; }

        [Display(Name = "Yrkat belopp (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal ClaimSum { get; set; }

        [Display(Name = "Beräknat belopp (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal ModelSum { get; set; }

        [Display(Name = "Diff. mellan yrkat belopp och modellbelopp (Kr)")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal DiffClaimModel { get; set; }

        [Display(Name = "Diff. mellan yrkat belopp och beslutat belopp (Kr)")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal DiffClaimDecided { get; set; }

        //[Display(Name = "Beslutat belopp (Kr)")]
        //[DisplayFormat(DataFormatString = "{0:f2}")]
        //public decimal DecidedSum { get; set; }
        
        //[Display(Name = "Rekommenderat belopp (Kr)")]
        //[DisplayFormat(DataFormatString = "{0:f2}")]
        //public decimal ApprovedSum { get; set; }

        //[Display(Name = "Rekommenderat avslag (Kr)")]
        //[DisplayFormat(DataFormatString = "{0:f2}")]
        //public decimal RejectedSum { get; set; }

        [Display(Name = "Rekommenderat belopp (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        [RegularExpression(@"\d{0,5}(\,\d{0,2})?$", ErrorMessage = "Fel format eller för stort belopp.")]
        public string ApprovedSum { get; set; }

        [Display(Name = "Rekommenderat avslag (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        [RegularExpression(@"\d{0,5}(\,\d{0,2})?$", ErrorMessage = "Fel format eller för stort belopp.")]
        public string RejectedSum { get; set; }

        [Display(Name = "Motivering för avslag:")]
        public string RejectReason { get; set; }

        //public int? StartIndex { get; set; }

        //public int? NumberOfDaysToRemove { get; set; }

        //public List<CheckBox> OfficialsCheck { get; set; }

        //public class CheckBox
        //{
        //    public string Text { get; set; }
        //    public bool Value { get; set; }
        //}

        //public recommendationVM()
        //{
        //    new CheckBox { Text = "Vårdgivaren finns i IVO's Vårdgivarregister" };

        //    new CheckBox { Text = "Aktuellt beslut om assistans finns i Procapita" }
        //}
    }
}