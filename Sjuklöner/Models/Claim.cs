using Sjuklöner.Viewmodels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sjuklöner.Models
{
    public class Claim
    {
        public int Id { get; set; }

        public int ClaimStatusId { get; set; }

        [Display(Name = "Referensnummer")]
        public string ReferenceNumber { get; set; }

        public int? CompletionStage { get; set; }

        [Display(Name = "Senaste statusändring")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? StatusDate { get; set; }

        /* DeadlineDate is Obsolete 
        [DisplayFormat(DataFormatString = "{0: yyyy-MM-dd}")]
        [DataType(DataType.DateTime)]
        public DateTime? DeadlineDate { get; set; }
        */

        [DisplayFormat(DataFormatString = "{0: yyyy-MM-dd}")]
        [DataType(DataType.DateTime)]
        public DateTime? CreationDate { get; set; }
        
        [DisplayFormat(DataFormatString = "{0: yyyy-MM-dd}")]
        [DataType(DataType.DateTime)]
        public DateTime? DecisionDate { get; set; }

        [DisplayFormat(DataFormatString = "{0: yyyy-MM-dd}")]
        [DataType(DataType.DateTime)]
        public DateTime? SentInDate { get; set; }

        public bool DefaultCollectiveAgreement { get; set; }

        //COMPANY INFORMATION
        public int CareCompanyId { get; set; }

        [Display(Name = "Bolagets namn")]
        public string CompanyName { get; set; }

        [Display(Name = "Organiationsnummer")]
        public string OrganisationNumber { get; set; }

        [Display(Name = "Gatuadress")]
        public string StreetAddress { get; set; }

        [Display(Name = "Postnummer")]
        public string Postcode { get; set; }

        [Display(Name = "Ort")]
        public string City { get; set; }

        [Display(Name = "Bank-/Postgironummer")]
        public string AccountNumber { get; set; }

        [Display(Name = "Tel.nummer (inkl. riktnr.)")]
        public string CompanyPhoneNumber { get; set; }

        [Display(Name = "Kollektivavtal")]
        public string CollectiveAgreementName { get; set; }

        [Display(Name = "Kollektivavtalets branschbeteckning")]
        public string CollectiveAgreementSpecName { get; set; }

        //HANDLÄGGARE INFORMATION
        public string AdmOffId { get; set; }

        [Display(Name = "Handläggarens namn")]
        public string AdmOffName { get; set; }

        //OMBUD INFORMATION
        public string OwnerId { get; set; }

        [Display(Name = "Ombudets förnamn")]
        public string OmbudFirstName { get; set; }

        [Display(Name = "Ombudets efternamn")]
        public string OmbudLastName { get; set; }

        [Display(Name = "Ombudets telefonnummer")]
        public string OmbudPhoneNumber { get; set; }

        [Display(Name = "Ombudets e-postadress")]
        public string OmbudEmail { get; set; }

        //CUSTOMER INFORMATION
        [Required]
        [Display(Name = "Kundens förnamn")]
        public string CustomerName { get; set; }

        [Required]
        [Display(Name = "Kundens personnummer")]
        //[RegularExpression(@"2[0-9]|([1]$")]
        //[RegularExpression(@"(((20)((0[0-9])|(1[0-7])))|(([1][^0-8])?\d{2}))((0[1-9])|1[0-2])((0[1-9])|(1[0-9])|(2[0-9])|(3[01]))[-]?\d{4}$", ErrorMessage = "Ej giltigt personnummer. Formaten YYYYMMDD-NNNN och YYYYMMDDNNNN är giltiga.")]
        public string CustomerSSN { get; set; }

        //This property will never be used
        //[Display(Name = "Kundens adress")]
        //public string CustomerAddress { get; set; }

        [Required]
        [Display(Name = "Kundens telefonnummer")]
        public string CustomerPhoneNumber { get; set; }

        //REGULAR ASSISTANT INFORMATION
        public int? SelectedRegAssistantId { get; set; }

        [Display(Name = "Personnummer")]
        public string RegAssistantSSN { get; set; }

        [Display(Name = "Förnamn")]
        public string RegFirstName { get; set; }

        [Display(Name = "Efternamn")]
        public string RegLastName { get; set; }

        [EmailAddress]
        [Display(Name = "E-postadress")]
        public string RegEmail { get; set; }

        [Display(Name = "Telefonnummer")]
        public string RegPhoneNumber { get; set; }

        [Display(Name = "Timlön (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string HourlySalaryAsString { get; set; }

        [Display(Name = "Sjuklön (%):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string SickPayRateAsString { get; set; }

        [Display(Name = "Semesterersättning (%):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string HolidayPayRateAsString { get; set; }

        [Display(Name = "Sociala avgifter (%):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string SocialFeeRateAsString { get; set; }

        [Display(Name = "Pensioner/försäkringar (%):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string PensionAndInsuranceRateAsString { get; set; }

        //SUBSTITUTE ASSISTANT INFORMATION
        //public int NumberOfSubAssistants { get; set; }

        public int? SelectedSubAssistantId { get; set; }

        [Display(Name = "Personnummer")]
        public string SubAssistantSSN { get; set; }

        [Display(Name = "Förnamn")]
        public string SubFirstName { get; set; }

        [Display(Name = "Efternamn")]
        public string SubLastName { get; set; }

        [EmailAddress]
        [Display(Name = "E-postadress")]
        public string SubEmail { get; set; }

        [Display(Name = "Telefonnummer")]
        public string SubPhoneNumber { get; set; }

        public bool IVOCheck { get; set; }
        public string IVOCheckMsg { get; set; }

        public bool ProCapitaCheck { get; set; }
        public string AssistanceCheckMsg { get; set; }

        public string LastAssistanceDate { get; set; } //Last date of personal assistance according to current decision. Wanted format from Robin: YYYY-MM-DD

        public string FirstAssistanceDate { get; set; } //First date of personal assistance according to current decision. Wanted format from Robin: YYYY-MM-DD

        public bool ProxyCheck { get; set; }
        public string ProxyCheckMsg { get; set; }

        public bool SalarySpecRegAssistantCheck { get; set; }
        public string SalarySpecRegAssistantCheckMsg { get; set; }

        //public bool SalarySpecSubAssistantCheck { get; set; }
        //public string SalarySpecSubAssistantCheckMsg { get; set; }

        public bool SickleaveNotificationCheck { get; set; }
        public string SickleaveNotificationCheckMsg { get; set; }

        public bool MedicalCertificateCheck { get; set; }
        public string MedicalCertificateCheckMsg { get; set; }

        public bool FKRegAssistantCheck { get; set; }
        public string FKRegAssistantCheckMsg { get; set; }

        public bool FKSubAssistantCheck { get; set; }
        public string FKSubAssistantCheckMsg { get; set; }

        public bool BasisForDecision { get; set; }
        public string BasisForDecisionMsg { get; set; }
        public DateTime BasisForDecisionTransferStartTimeStamp { get; set; }
        public DateTime BasisForDecisionTransferFinishTimeStamp { get; set; }

        public bool Decision { get; set; }
        public string DecisionMsg { get; set; }
        public DateTime DecisionTransferTimeStamp { get; set; }

        [Display(Name = "Motivering för avslag")]
        public string RejectReason { get; set; }

        //[Required]
        [Display(Name = "Vikarierande assistents personnummer")]
        //[RegularExpression(@"(((20)((0[0-9])|(1[0-7])))|(([1][^ 0-8])?\d{2}))((0[1-9])|1[0-2])((0[1-9])|(1[0-9])|(2[0-9])|(3[01]))[-]?\d{4}$")]
        public string StandInSSN { get; set; }

        [Display(Name = "Första sjukdag")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]        
        public DateTime QualifyingDate { get; set; }

        [Display(Name = "Sista sjukdag")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime LastDayOfSicknessDate { get; set; }

        [Display(Name = "Antal sjukdagar")]
        public int NumberOfSickDays { get; set; }

        [Display(Name = "Antal timmar frånvaro")]
        public decimal NumberOfAbsenceHours { get; set; }

        [Display(Name = "Antal ordinarie timmar frånvaro")]
        public decimal NumberOfOrdinaryHours { get; set; }

        [Display(Name = "Antal OB-timmar frånvaro")]
        public decimal NumberOfUnsocialHours { get; set; }

        [Display(Name = "Antal jour-timmar frånvaro")]
        public decimal NumberOfOnCallHours { get; set; }

        [Display(Name = "Antal timmar med vikarie")]
        public decimal NumberOfHoursWithSI { get; set; }

        [Display(Name = "Antal ordinarie timmar med vikarie")]
        public decimal NumberOfOrdinaryHoursSI { get; set; }

        [Display(Name = "Antal OB-timmar med vikarie")]
        public decimal NumberOfUnsocialHoursSI { get; set; }

        [Display(Name = "Antal jour-timmar med vikarie")]
        public decimal NumberOfOnCallHoursSI { get; set; }

        [Display(Name = "Sjuklön (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal ClaimedSickPay { get; set; }

        [Display(Name = "Semesterersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal ClaimedHolidayPay { get; set; }

        [Display(Name = "Sociala avgifter (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal ClaimedSocialFees { get; set; }

        [Display(Name = "Övriga avtalsbundna kostnader (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal ClaimedPensionAndInsurance { get; set; }

        [Display(Name = "Yrkat belopp (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal ClaimedSum { get; set; }

        [Display(Name = "Modellbelopp (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal ModelSum { get; set; }

        [Display(Name = "Beslutat belopp (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal DecidedSum { get; set; }

        [Display(Name = "Godkänt belopp (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal ApprovedSum { get; set; }

        [Display(Name = "Avslaget belopp (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal RejectedSum { get; set; }

        //These string properties are used by the robot when it transfers information to Procapita
        public string QualifyingDateAsString { get; set; }

        public string LastDayOfSicknessDateAsString { get; set; }

        public string SentInDateAsString { get; set; }

        public string ClaimedSumAsString { get; set; }

        public string ModelSumAsString { get; set; }

        public string ApprovedSumAsString { get; set; }

        public string RejectedSumAsString { get; set; }

        public string TransferToProcapitaString { get; set; }

        //These properties were added for the case when hours only (not working time) are given as input by the ombud.
        [Display(Name = "Sjuklön (%):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal SickPayRate { get; set; }

        [Display(Name = "Semesterersättning (%):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal HolidayPayRate { get; set; }

        [Display(Name = "Sociala avgifter (%):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal SocialFeeRate { get; set; }

        [Display(Name = "Pensioner/försäkringar (%):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal PensionAndInsuranceRate { get; set; }

        [Display(Name = "Ordinarie timlön (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal HourlySalary { get; set; }

        [Display(Name = "OB-ersättning, kväll (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal PerHourUnsocialEvening { get; set; }

        [Display(Name = "OB-ersättning, natt (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal PerHourUnsocialNight { get; set; }

        [Display(Name = "OB-ersättning, helg (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal PerHourUnsocialWeekend { get; set; }

        [Display(Name = "OB-ersättning, storhelg (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal PerHourUnsocialHoliday { get; set; }

        [Display(Name = "Jour-ersättning, vardag (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal PerHourOnCallWeekday { get; set; }

        [Display(Name = "Jour-ersättning, helg (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal PerHourOnCallWeekend { get; set; }

        [Display(Name = "Kostnad för sjukperioden (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string TotalCostD1T14 { get; set; }

        [Display(Name = "Kostnad för sjukperioden (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string TotalCostCalcD1T14 { get; set; }

        public virtual CareCompany CareCompany { get; set; }

        public virtual ClaimStatus ClaimStatus { get; set; }

        public virtual List<Document> Documents { get; set; }

        public virtual List<Message> Messages { get; set; }
    }
}