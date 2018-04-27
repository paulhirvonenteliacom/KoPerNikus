using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sjuklöner.Viewmodels
{
    public class ConfirmTransferVM
    {
        public int ClaimId { get; set; }

        [Display(Name = "Referensnummer")]
        public string ReferenceNumber { get; set; }

        [Display(Name = "Kundens personnummer")]
        public string CustomerSSN { get; set; }

        [Display(Name = "Första sjukdag")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime QualifyingDate { get; set; }

        [Display(Name = "Sista sjukdag")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime LastDayOfSicknessDate { get; set; }

        [Display(Name = "Yrkat belopp (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal ClaimedSum { get; set; }

        [Display(Name = "Modellbelopp (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal ModelSum { get; set; }

        [Display(Name = "Godkänt belopp (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal ApprovedSum { get; set; }

        [Display(Name = "Avslaget belopp (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal RejectedSum { get; set; }

        [Display(Name = "Motivering för avslag")]
        public string RejectReason { get; set; }
    }
}