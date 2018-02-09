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

        [Required]
        public HttpPostedFileBase SalaryAttachment { get; set; }

        [Required]
        public HttpPostedFileBase SalaryAttachmentStandIn { get; set; }

        [Required]
        public HttpPostedFileBase SickLeaveNotification { get; set; }

        [Required]
        public HttpPostedFileBase DoctorsCertificate { get; set; }

        [Required]
        public HttpPostedFileBase TimeReport { get; set; }
        
        [Required]
        public HttpPostedFileBase TimeReportStandIn { get; set; }
    }
}