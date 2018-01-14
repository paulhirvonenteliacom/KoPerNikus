using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sjuklöner.Viewmodels
{
    public class DecisionVM
    {
        [Display(Name = "Vårdgivarregistret, IVO:")]
        public string IvoCheck { get; set; }

        [Display(Name = "Beslut om assistans i Procapita:")]
        public string AssistanceCheck { get; set; }

        [Display(Name = "Yrkat belopp (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double ClaimSum { get; set; }

        [Display(Name = "Modellbelopp (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double ModelSum { get; set; }

        [Display(Name = "Diff. mellan yrkat belopp och modellbelopp (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double DiffClaimModel { get; set; }

        [Display(Name = "Diff. mellan yrkat belopp och beslutat belopp (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double DiffClaimDecided { get; set; }

        [Display(Name = "Beslutat belopp (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double DecidedSum { get; set; }

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