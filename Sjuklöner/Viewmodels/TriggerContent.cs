using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sjuklöner.Viewmodels
{
    public class TriggerContent
    {
        public string ClaimInfo { get; set; }

        public string ReferenceNumber { get; set; }

        public string QualifyingDate { get; set; }

        public string LastDayOfSicknessDate { get; set; }

        public string RejectReason { get; set; }

        public string ModelSum { get; set; }

        public string ClaimedSum { get; set; }

        public string ApprovedSum { get; set; }

        public string RejectedSum { get; set; }

        public string IVOCheckMsg { get; set; }

        public string ProxyCheckMsg { get; set; }

        public string AssistanceCheckMsg { get; set; }

        public string SalarySpecRegAssistantCheckMsg { get; set; }

        public string SalarySpecSubAssistantCheckMsg { get; set; }

        public string SickleaveNotificationCheckMsg { get; set; }

        public string MedicalCertificateCheckMsg { get; set; }

        public string FKRegAssistantCheckMsg { get; set; }

        public string FKSubAssistantCheckMsg { get; set; }

        public string sentInDate { get; set; }

        public string NumberOfSickDays { get; set; }
    }
}