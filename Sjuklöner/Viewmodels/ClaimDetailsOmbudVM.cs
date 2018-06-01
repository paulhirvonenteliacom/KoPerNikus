using Sjuklöner.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sjuklöner.Viewmodels
{
    public class ClaimDetailsOmbudVM
    {
        [Display(Name = "Referensnummer")]
        public string ReferenceNumber { get; set; }

        public int? CompletionStage { get; set; }

        [Display(Name = "Status")]
        public string StatusName { get; set; }

        //Kommun
        [Display(Name = "Kommun:")]
        public string Council { get; set; }

        [Display(Name = "Förvaltning:")]
        public string Administration { get; set; }


        //Assistansberättigad
        [Display(Name = "Namn:")]
        public string CustomerName { get; set; }

        [Display(Name = "Personnummer:")]
        public string CustomerSSN { get; set; }

        //CustomerAddress will never be used
        //[Display(Name = "Adress:")]
        //public string CustomerAddress { get; set; }

        [Display(Name = "Telefonnummer:")]
        public string CustomerPhoneNumber { get; set; }


        //Ombud/uppgiftslämnare
        [Display(Name = "Namn:")]
        public string OmbudName { get; set; }

        [Display(Name = "Telefonnummer:")]
        public string OmbudPhoneNumber { get; set; }


        //Assistansanordnare
        [Display(Name = "Företagets namn:")]
        public string CompanyName { get; set; }

        [Display(Name = "Organisationsnummer:")]
        public string OrganisationNumber { get; set; }

        [Display(Name = "Bank-/plusgironummer:")]
        public string GiroNumber { get; set; }

        [Display(Name = "Företagets adress:")]
        public string CompanyAddress { get; set; }

        [Display(Name = "Telefonnummer:")]
        public string CompanyPhoneNumber { get; set; }

        [Display(Name = "Kollektivavtal:")]
        public string CollectiveAgreement { get; set; } //Avtal, fackförbund

        //1:a Vikarierande assistent
        [Display(Name = "Namn:")]
        public string SubAssistantName { get; set; }

        [Display(Name = "Personnummer:")]
        public string SubAssistantSSN { get; set; }

        [Display(Name = "Telefonnummer (inkl. riktnr.):")]
        public string SubPhoneNumber { get; set; }

        [Display(Name = "E-postadress:")]
        public string SubEmail { get; set; }

        //Övriga vikarierande assistenter (2 - 20)
        [Display(Name = "Namn:")]
        public string[] SubstituteAssistantName { get; set; }

        [Display(Name = "Personnummer:")]
        public string[] SubstituteAssistantSSN { get; set; }

        [Display(Name = "Telefonnummer (inkl. riktnr.):")]
        public string[] SubstituteAssistantPhoneNumber { get; set; }

        [Display(Name = "E-postadress:")]
        public string[] SubstituteAssistantEmail { get; set; }

        public int NumberOfSubAssistants { get; set; }

        //Insjuknad ordinarie assistent
        [Display(Name = "Namn:")]
        public string RegAssistantName { get; set; }

        [Display(Name = "Personnummer:")]
        public string RegAssistantSSN { get; set; }

        [Display(Name = "Telefonnummer (inkl. riktnr.):")]
        public string RegPhoneNumber { get; set; }

        [Display(Name = "E-postadress:")]
        public string RegEmail { get; set; }

        [Display(Name = "Karensdag, datum:")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[DataType(DataType.DateTime)]
        public string QualifyingDayDate { get; set; }

        [Display(Name = "Sista sjukdag, datum:")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[DataType(DataType.DateTime)]
        public string LastDayOfSicknessDate { get; set; }

        [Display(Name = "Lön, tim- eller månadslön (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal Salary { get; set; }

        [Display(Name = "Sjuklön (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal Sickpay { get; set; }

        [Display(Name = "OB-ersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal UnsocialHoursPay { get; set; }

        [Display(Name = "Jour-ersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal OnCallHoursPay { get; set; }

        [Display(Name = "Semesterersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal HolidayPay { get; set; }
    
        [Display(Name = "Sociala avgifter (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal SocialFees { get; set; }

        [Display(Name = "Övriga avtalsbundna kostnader (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal PensionAndInsurance { get; set; }

        [Display(Name = "Totalt antal timmar med frånvaro:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal NumberOfAbsenceHours { get; set; }

        [Display(Name = "Varav ordinarie timmar:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal NumberOfOrdinaryHours { get; set; }

        [Display(Name = "Varav timmar med OB-tillägg:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal NumberOfUnsocialHours { get; set; }

        [Display(Name = "Varav timmar med jour:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal NumberOfOnCallHours { get; set; }

        [Display(Name = "Totalt antal timmar med vikarie:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal NumberOfHoursWithSI { get; set; }

        [Display(Name = "Varav ordinarie timmar:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal NumberOfOrdinaryHoursSI { get; set; }

        [Display(Name = "Varav timmar med OB-tillägg:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal NumberOfUnsocialHoursSI { get; set; }

        [Display(Name = "Varav timmar med jour:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal NumberOfOnCallHoursSI { get; set; }

        //The following 11 properties handle multiple substitute assistants in the same claim (subs 2 - 20)
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string[] HoursWithSI { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string[] OrdinaryHoursSI { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string[] UnsocialHoursSI { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string[] OnCallHoursSI { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string[,] HoursSIPerSubAndDay { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string[,] UnsocialEveningSIPerSubAndDay { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string[,] UnsocialNightSIPerSubAndDay { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string[,] UnsocialWeekendSIPerSubAndDay { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string[,] UnsocialGrandWeekendSIPerSubAndDay { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string[,] OnCallDaySIPerSubAndDay { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string[,] OnCallNightSIPerSubAndDay { get; set; }


        [Display(Name = "Yrkat belopp, total (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal ClaimSum { get; set; }

        [Display(Name = "Godkänt belopp (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal ApprovedSum { get; set; }

        [Display(Name = "Avslaget belopp (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal RejectedSum { get; set; }

        public bool DecisionMade { get; set; }

        [Display(Name = "Arbetsställe:")]
        public string Workplace { get; set; }

        [Display(Name = "Uppgiftslämnare/ombud:")]
        public string NameOfOmbud { get; set; }

        [Display(Name = "Uppgiftslämnarens tel.nummer:")]
        public string PhonenUmberOfOmbud { get; set; }

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

        
        //For hours only (not working time)
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

        public bool DefaultCollectiveAgreement { get; set; }

        [Display(Name = "Antal sjukdagar:")]
        public int NumberOfSickDays { get; set; }

        [Display(Name = "Kostnad för sjukperioden (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string TotalCostD1T14 { get; set; }

        [Display(Name = "Kostnad för sjukperioden (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string TotalCostCalcD1T14 { get; set; }

        [Display(Name = "Kontroll av organisationsnummer:")]
        public string IVOCheck { get; set; }

        [Display(Name = "Kontroll av fullmakt:")]
        public string ProxyCheck { get; set; }

        [Display(Name = "Kontroll av beslut om personlig assistans:")]
        public string AssistanceCheck { get; set; }

        public bool SalarySpecRegAssistantCheck { get; set; }
        [Display(Name = "Kontroll av lönespecifikation, ordinarie assistent:")]
        public string SalarySpecRegAssistantCheckMsg { get; set; }

        public bool SalarySpecSubAssistantCheck { get; set; }
        [Display(Name = "Kontroll av lönespecifikation, vikarierande assistent:")]
        public string SalarySpecSubAssistantCheckMsg { get; set; }

        public bool SickleaveNotificationCheck { get; set; }
        [Display(Name = "Kontroll av sjukfrånvaroanmälan:")]
        public string SickleaveNotificationCheckMsg { get; set; }

        public bool MedicalCertificateCheck { get; set; }
        [Display(Name = "Kontroll av läkarintyg:")]
        public string MedicalCertificateCheckMsg { get; set; }

        public bool FKRegAssistantCheck { get; set; }
        [Display(Name = "Kontroll av tidsredovisning Försäkringskassan, ordinarie assistent:")]
        public string FKRegAssistantCheckMsg { get; set; }

        public bool FKSubAssistantCheck { get; set; }
        [Display(Name = "Kontroll av tidsredovisning Försäkringskassan, vikarierande assistent:")]
        public string FKSubAssistantCheckMsg { get; set; }

        [Display(Name = "Motivering för avslag:")]
        public string RejectReason { get; set; }

        public List<ClaimDay> ClaimDays { get; set; }

        public List<ClaimCalculation> ClaimCalculations { get; set; }

        public List<Document> Documents { get; set; }

        [Display(Name = "Kommentarer")]
        public List<Message> messages { get; set; }
    }
}