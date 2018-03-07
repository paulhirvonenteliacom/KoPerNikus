using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sjuklöner.Viewmodels
{
    public class Create4VM
    {
        [Required]
        public string ClaimNumber { get; set; }

        [Display(Name = "Lönespecifikation, ordinarie assistent")]
        public HttpPostedFileBase SalaryAttachment { get; set; }

        [Display(Name = "Lönespecifikation, vikarierande assistent")]
        public HttpPostedFileBase SalaryAttachmentStandIn { get; set; }

        [Display(Name = "Sjukfrånvaroanmälan")]
        public HttpPostedFileBase SickLeaveNotification { get; set; }

        [Display(Name = "Läkarintyg")]
        public HttpPostedFileBase DoctorsCertificate { get; set; }

        [Display(Name = "Tidsredovisning, ordinarie assistent")]
        public HttpPostedFileBase TimeReport { get; set; }
        
        [Display(Name = "Tidsredovisning, vikarierande assistent")]
        public HttpPostedFileBase TimeReportStandIn { get; set; }
    }
}