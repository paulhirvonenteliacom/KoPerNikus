using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sjuklöner.Viewmodels
{
    public class ClaimFormVM
    {
        [Display(Name = "Referensnummer")]
        public string ReferenceNumber { get; set; }

        //Kommun
        [Display(Name = "Kommun:")]
        public string Council { get; set; }

        [Display(Name = "Förvaltning:")]
        public string Administration { get; set; }


        //Assistansberättigad
        [Display(Name = "Namn:")]
        public string CustomerName { get; set; }

        [Display(Name = "Personnummer:")]
        [RegularExpression(@"(((20)((0[0-9])|(1[0-7])))|(([1][^0-8])?\d{2}))((0[1-9])|1[0-2])((0[1-9])|(1[0-9])|(2[0-9])|(3[01]))[-]?\d{4}$", ErrorMessage = "Ej giltigt personnummer. Formaten YYYYMMDD-XXXX och YYYYMMDDXXXX är giltiga.")]
        public string CustomerSSN { get; set; }

        [Display(Name = "Adress:")]
        public string CustomerAddress { get; set; }

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

        [Display(Name = "Bank-/postgironummer:")]
        public string GiroNumber { get; set; }

        [Display(Name = "Företagets adress:")]
        public string CompanyAddress { get; set; }

        [Display(Name = "Telefonnummer:")]
        public string CompanyPhoneNumber { get; set; }

        [Display(Name = "Kollektivavtal:")]
        public string CollectiveAgreement { get; set; } //Avtal, fackförbund


        //Insjuknad ordinarie assistent
        [Display(Name = "Namn:")]
        public string AssistantName { get; set; }

        [Display(Name = "Personnummer:")]
        [RegularExpression(@"(((20)((0[0 - 9])|(1[0 - 7])))|(([1][^ 0 - 8])?\d{2}))((0[1-9])|1[0-2])((0[1-9])|(2[0-9])|(3[01]))[-]?\d{4}$", ErrorMessage = "Ej giltigt personnummer")]
        public string AssistantSSN { get; set; }

        [Display(Name = "Karensdag, datum:")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[DataType(DataType.DateTime)]
        public string QualifyingDayDate { get; set; }

        [Display(Name = "Sista sjukdag, datum:")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime LastDayOfSicknessDate { get; set; }

        [Display(Name = "Lön, tim- eller månadslön (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double Salary { get; set; }

        [Display(Name = "Sjuklön (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double Sickpay { get; set; }

        [Display(Name = "OB-ersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double UnsocialHoursPay { get; set; }

        [Display(Name = "Jour-ersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double OnCallHoursPay { get; set; }

        [Display(Name = "Semesterersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double HolidayPayDay { get; set; }
    
        [Display(Name = "Sociala avgifter (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double SocialFees { get; set; }

        [Display(Name = "Övriga avtalsbundna kostnader (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double PensionAndInsurance { get; set; }

        [Display(Name = "Totalt antal timmar med vikarie:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double NumberOfHours { get; set; }

        [Display(Name = "Varav timmar med OB-tillägg:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double NumberOfUnsocialHours { get; set; }

        [Display(Name = "Varav timmar med jour:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double NumberOfOnCallHours { get; set; }

        [Display(Name = "Yrkat belopp (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double ClaimSum { get; set; }



        [Display(Name = "Arbetsställe:")]
        public string Workplace { get; set; }

        [Display(Name = "Uppgiftslämnare/ombud:")]
        public string NameOfOmbud { get; set; }

        [Display(Name = "Uppgiftslämnarens tel.nummer:")]
        public string PhonenUmberOfOmbud { get; set; }

        
       

        [Display(Name = "Sjuklön (%):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double SickPayRate { get; set; }

        [Display(Name = "Semesterersättning (%):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double HolidayPayRate { get; set; }

        [Display(Name = "Sociala avgifter (%):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double SocialFeeRate { get; set; }

        [Display(Name = "Pensioner/försäkringar (%):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double PensionAndInsuranceRate { get; set; }


        [Display(Name = "Ordinarie timlön (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double HourlySalary { get; set; }

        [Display(Name = "OB-ersättning, kväll (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double PerHourUnsocialEvening { get; set; }

        [Display(Name = "OB-ersättning, natt (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double PerHourUnsocialNight { get; set; }

        [Display(Name = "OB-ersättning, helg (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double PerHourUnsocialWeekend { get; set; }

        [Display(Name = "OB-ersättning, storhelg (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double PerHourUnsocialHoliday { get; set; }

        [Display(Name = "Jour-ersättning, vardag (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double PerHourOnCallWeekday { get; set; }

        [Display(Name = "Jour-ersättning, helg (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double PerHourOnCallWeekend { get; set; }

        

        [Display(Name = "Antal timmar:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double NumberOfHoursQualifyingDay { get; set; }

        [Display(Name = "Grundlön (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double SalaryQualifyingDay { get; set; }

        [Display(Name = "Semesterersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double HolidayPayQualifyingDay { get; set; }

        [Display(Name = "OB-ersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double UnsocialHoursPayQualifyingDay { get; set; }

        [Display(Name = "Jourersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double OnCallHoursPayQualifyingDay { get; set; }

        [Display(Name = "Sociala avgifter enligt lag (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double SocialFeesQualifyingDay { get; set; }

        [Display(Name = "Pensioner och försäkringar enligt kollektivavtal (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double PensionAndInsuranceQualifyingDay { get; set; }

        [Display(Name = "Kostnad för dag 1, karensdagen (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double CostQualifyingDay { get; set; }


        [Display(Name = "Datum, dag 2:")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime Day2Date { get; set; }

        

        [Display(Name = "Antal timmar:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double NumberOfHoursDay2To14 { get; set; }

        [Display(Name = "Grundlön (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double SalaryDay2To14 { get; set; }

        [Display(Name = "Semesterersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double HolidayPayDay2To14 { get; set; }

        [Display(Name = "Antal timmar:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double NumberOfUnsocialHoursDay2To14 { get; set; }

        [Display(Name = "Ersättning obekväm arbetstid (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double UnsocialHoursPayDay2To14 { get; set; }

        [Display(Name = "Antal timmar:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double NumberOfOnCallHoursDay2To14 { get; set; }

        [Display(Name = "Jourersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double OnCallHoursPayDay2To14 { get; set; }

        [Display(Name = "Sociala avgifter enligt lag (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double SocialFeesDay2To14 { get; set; }

        [Display(Name = "Pensioner och försäkringar enligt kollektivavtal (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double PensionAndInsuranceDay2To14 { get; set; }

        [Display(Name = "Kostnad för dag 2 - 14 (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double CostDay2To14 { get; set; }


        [Display(Name = "Total kostnad för sjuklöneperioden dag 1 - 14 (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double CostDay1To14 { get; set; }
    }
}