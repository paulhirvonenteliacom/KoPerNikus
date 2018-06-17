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

        public string RegAssistantSSNAndName { get; set; }

        [Display(Name = "Lönespecifikation, ordinarie assistent")]
        public HttpPostedFileBase SalaryAttachment { get; set; }

        public bool SalaryAttachmentExists { get; set; }

        //[Display(Name = "Lönespecifikation, vikarierande assistent")]
        //public HttpPostedFileBase SalaryAttachmentStandIn { get; set; }

        //[Display(Name = "Sjukfrånvaroanmälan")]
        //public HttpPostedFileBase SickLeaveNotification { get; set; }

        [Display(Name = "Läkarintyg")]
        public HttpPostedFileBase DoctorsCertificate { get; set; }

        public bool DoctorsCertificateExists { get; set; }

        [Display(Name = "Tidsredovisning, ordinarie assistent")]
        public HttpPostedFileBase TimeReport { get; set; }
        public bool TimeReportExists { get; set; }

        public string[] SubAssistantSSNAndName { get; set; }

        [Display(Name = "Tidsredovisning, vikarierande assistent")]
        public HttpPostedFileBase[] TimeReportStandIn { get; set; }
        public List<bool> TimeReportStandInExists { get; set; }
        public bool[] AssistantHasFile { get; set; }

        public int NumberOfSickDays { get; set; }

        public int NumberOfCalendarDays { get; set; }

        public int NumberOfSubAssistants { get; set; }
    }
}