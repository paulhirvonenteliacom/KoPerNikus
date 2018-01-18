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

        public string OwnerId { get; set; }

        public int ClaimStatusId { get; set; }

        public int CareCompanyId { get; set; }

        public bool IVOCheck { get; set; }

        public bool ProCapitaCheck { get; set; }

        [Display(Name = "Referensnummer")]
        public string ReferenceNumber { get; set; }

        [Display(Name = "Senaste statusändring")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? StatusDate { get; set; }

        [DisplayFormat(DataFormatString = "{0: yyyy-MM-dd}")]
        [DataType(DataType.DateTime)]
        public DateTime? DeadlineDate { get; set; }

        [Display(Name = "Kundens förnamn")]
        public string CustomerFirstName { get; set; }

        [Display(Name = "Kundens efternamn")]
        public string CustomerLastName { get; set; }

        //[Required]
        [Display(Name = "Organiationsnummer")]
        [RegularExpression(@"[0-9]{6}-[0-9]{4}$")]
        public string OrganisationNumber { get; set; }

        [Required]
        [Display(Name = "Kundens personnummer")]
        [RegularExpression(@"(((20)((0[0 - 9])|(1[0 - 7])))|(([1][^ 0 - 8])?\d{2}))((0[1-9])|1[0-2])((0[1-9])|(2[0-9])|(3[01]))[-]?\d{4}$")]
        public string CustomerSSN { get; set; }

        //[Required]
        [Display(Name = "Ordinarie assistents personnummer")]
        [RegularExpression(@"(((20)((0[0 - 9])|(1[0 - 7])))|(([1][^ 0 - 8])?\d{2}))((0[1-9])|1[0-2])((0[1-9])|(2[0-9])|(3[01]))[-]?\d{4}$")]
        public string AssistantSSN { get; set; }

        //[Required]
        //[Display(Name = "Vikarierande assistents personnummer")]
        //[RegularExpression(@"(((20)((0[0 - 9])|(1[0 - 7])))|(([1][^ 0 - 8])?\d{2}))((0[1-9])|1[0-2])((0[1-9])|(2[0-9])|(3[01]))[-]?\d{4}$")]
        //public string StandInSSN { get; set; }

        [Display(Name = "Första sjukdag")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]        
        public DateTime QualifyingDate { get; set; }

        [Display(Name = "Sista sjukdag")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime LastDayOfSicknessDate { get; set; }

        [Display(Name = "Mottagare av beslut (e-post)")]
        public string Email { get; set; }

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
        public decimal SickPay { get; set; }

        [Display(Name = "Semesterersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal HolidayPay { get; set; }

        [Display(Name = "Sociala avgifter (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal SocialFees { get; set; }

        [Display(Name = "Övriga avtalsbundna kostnader (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal PensionAndInsurance { get; set; }

        [Display(Name = "Yrkat belopp (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal ClaimSum { get; set; }

        [Display(Name = "Modellbelopp (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal ModelSum { get; set; }

        [Display(Name = "Beslutat belopp (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal DecidedSum { get; set; }

        [Display(Name = "Antal timmar som skulle arbetats karensdagen")]
        public decimal HoursQualifyingDay { get; set; }

        [Display(Name = "Semesterersättning för karensdag")]
        public decimal HolidayPayQualDay { get; set; }

        [Display(Name = "Arbetsgivaravgift för karensdag")]
        public decimal PayrollTaxQualDay { get; set; }

        [Display(Name = "Avtalsförsäkring för karensdag")]
        public decimal InsuranceQualDay { get; set; }

        [Display(Name = "Kollektivavtalad pension för karensdag")]
        public decimal PensionQualDay { get; set; }

        [Display(Name = "Ersättningsanspråk för karensdag")]
        public decimal ClaimQualDay { get; set; }

        [Display(Name = "Antal timmar dag 2 - dag 14, inkl. jour/beredskap")]
        public decimal HoursDay2To14 { get; set; }

        [Display(Name = "Sjuklön per timme (80% av ordinarie timlön)")]
        public decimal HourlySickPay { get; set; }

        [Display(Name = "Sjuklön dag 2 - dag 14")]
        public decimal SickPayDay2To14 { get; set; }

        [Display(Name = "Semesterersättning dag 2 - dag 14")]
        public decimal HolidayPayDay2To14 { get; set; }

        [Display(Name = "OB-ersättning dag 2 - dag 14")]
        public decimal UnsocialHoursPayDay2To14 { get; set; }

        [Display(Name = "Jour/beredskap dag 2 - dag 14")]
        public decimal OnCallHoursPayDay2To14 { get; set; }

        [Display(Name = "Arbetsgivaravgift dag 2 - dag 14")]
        public decimal PayrollTaxDay2To14 { get; set; }

        [Display(Name = "Avtalsförsäkring dag 2 - dag 14")]
        public decimal InsuranceDay2To14 { get; set; }

        [Display(Name = "Kollektivavtalad pension dag 2 - dag 14")]
        public decimal PensionDay2To14 { get; set; }

        [Display(Name = "Ersättningsanspråk dag 2 - dag 14")]
        public decimal ClaimDay2To14 { get; set; }

        //public bool Step1Saved { get; set; }
        //public bool Step1Complete { get; set; }

        //public bool Step2Saved { get; set; }
        //public bool Step2Complete { get; set; }

        //public bool Step3Saved { get; set; }
        //public bool Step3Complete { get; set; }

        //[Display(Name = "Yrkat belopp")]
        //public decimal ClaimSum { get; set; }

        //public List<string> StartHour { get; set; }
        //public List<string> StartMinute { get; set; }
        //public List<string> StopHour { get; set; }
        //public List<string> StopMinute { get; set; }
        //public List<string> CalculatedHours { get; set; }
        //public List<string> CalculatedUnsocialHours { get; set; }

        //public List<string> StartHourOnCall { get; set; }
        //public List<string> StartMinuteOnCall { get; set; }
        //public List<string> StopHourOnCall { get; set; }
        //public List<string> StopMinuteOnCall { get; set; }
        //public List<string> CalculatedHoursOnCall { get; set; }

        public virtual CareCompany CareCompany { get; set; }

        public virtual ClaimStatus ClaimStatus { get; set; }

        public virtual List<Document> Documents { get; set; }

        public virtual List<Message> Messages { get; set; }
    }
}