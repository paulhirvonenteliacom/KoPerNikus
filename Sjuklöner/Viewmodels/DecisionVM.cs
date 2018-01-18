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

        [Display(Name = "Komplett ansökan")]
        public bool CompleteCheck { get; set; }

        public string CompleteCheckMsg { get; set; }

        [Display(Name = "Kontroll av fullmakt i ProCapita")]
        public bool ProxyCheck { get; set; }

        public string ProxyCheckMsg { get; set; }

        [Display(Name = "Vårdgivarregistret, IVO")]
        public bool IvoCheck { get; set; }

        public string IvoCheckMsg { get; set; }

        [Display(Name = "Beslut om assistans i ProCapita")]
        public bool AssistanceCheck { get; set; }

        public string AssistanceCheckMsg { get; set; }

        [Display(Name = "Yrkat belopp (Kr)")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal ClaimSum { get; set; }

        [Display(Name = "Modellbelopp (Kr)")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal ModelSum { get; set; }

        [Display(Name = "Diff. mellan yrkat belopp och modellbelopp (Kr)")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal DiffClaimModel { get; set; }

        [Display(Name = "Diff. mellan yrkat belopp och beslutat belopp (Kr)")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal DiffClaimDecided { get; set; }

        [Display(Name = "Beslutat belopp (Kr)")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal DecidedSum { get; set; }

        //public List<CheckBox> OfficialsCheck { get; set; }

        //public class CheckBox
        //{
        //    public string Text { get; set; }
        //    public bool Value { get; set; }
        //}

        //public DecisionVM()
        //{
        //    new CheckBox { Text = "Vårdgivaren finns i IVO's Vårdgivarregister" };

        //    new CheckBox { Text = "Aktuellt beslut om assistans finns i Procapita" }
        //}
    }
}