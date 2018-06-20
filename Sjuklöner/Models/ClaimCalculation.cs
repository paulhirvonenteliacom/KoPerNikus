using Sjuklöner.Viewmodels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sjuklöner.Models
{
    public class ClaimCalculation
    {
        public int Id { get; set; }

        [Display(Name = "Referensnummer")]
        public string ReferenceNumber { get; set; }

        [Display(Name = "Fr. o. m.")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]        
        public DateTime StartDate { get; set; }

        [Display(Name = "T. o. m.")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        [Display(Name = "Antal sjukdagar")]
        public int NumberOfSickDays { get; set; }

        [Display(Name = "Modellbelopp (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public decimal ModelSum { get; set; }

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

        [Display(Name = "Timlön (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string HourlySalaryAsString { get; set; }

        [Display(Name = "OB-ersättning, kväll (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string PerHourUnsocialEveningAsString { get; set; }

        [Display(Name = "OB-ersättning, natt (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string PerHourUnsocialNightAsString { get; set; }

        [Display(Name = "OB-ersättning, helg (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string PerHourUnsocialWeekendAsString { get; set; }

        [Display(Name = "OB-ersättning, storhelg (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string PerHourUnsocialHolidayAsString { get; set; }

        [Display(Name = "Jour-ersättning, dag (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string PerHourOnCallDayAsString { get; set; }

        [Display(Name = "Jour-ersättning, natt (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string PerHourOnCallNightAsString { get; set; }

        //Qualifying day, if hours only given as input by the ombud
        [Display(Name = "Antal arbetstimmar:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string HoursQD { get; set; }

        [Display(Name = "Antal OB-timmar, kväll:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialEveningHoursQD { get; set; }

        [Display(Name = "Antal OB-timmar, natt:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialNightHoursQD { get; set; }

        [Display(Name = "Antal OB-timmar, helg:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialWeekendHoursQD { get; set; }

        [Display(Name = "Antal OB-timmar, storhelg:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialGrandWeekendHoursQD { get; set; }

        [Display(Name = "Antal jourtimmar, dag:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string OnCallDayHoursQD { get; set; }

        [Display(Name = "Antal jourtimmar, helg/natt:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string OnCallNightHoursQD { get; set; }

        [Display(Name = "Antal arbetstimmar efter avdrag för karensdag:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string PaidHoursQD { get; set; }

        [Display(Name = "Antal OB-timmar, kväll, efter avdrag för karensdag:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string PaidUnsocialEveningHoursQD { get; set; }

        [Display(Name = "Antal OB-timmar, natt, efter avdrag för karensdag:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string PaidUnsocialNightHoursQD { get; set; }

        [Display(Name = "Antal OB-timmar, helg, efter avdrag för karensdag:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string PaidUnsocialWeekendHoursQD { get; set; }

        [Display(Name = "Antal OB-timmar, storhelg, efter avdrag för karensdag:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string PaidUnsocialGrandWeekendHoursQD { get; set; }

        [Display(Name = "Antal jourtimmar, dag, efter avdrag för karensdag:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string PaidOnCallDayHoursQD { get; set; }

        [Display(Name = "Antal jourtimmar, helg/natt, efter avdrag för karensdag:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string PaidOnCallNightHoursQD { get; set; }

        [Display(Name = "Lön, 80% av ordinarie timlön:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string SalaryQD { get; set; }

        [Display(Name = "Lön, 80% av ordinarie timlön:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string SalaryCalcQD { get; set; }

        [Display(Name = "Sjuklön (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string SickPayQD { get; set; }

        [Display(Name = "Sjuklön (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string SickPayCalcQD { get; set; }

        [Display(Name = "Semesterersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string SalaryBaseQD { get; set; }

        [Display(Name = "Semesterersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string SalaryBaseCalcQD { get; set; }

        [Display(Name = "Semesterersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string HolidayPayQD { get; set; }

        [Display(Name = "Semesterersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string HolidayPayCalcQD { get; set; }

        [Display(Name = "OB-ersättning, kväll (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialEveningPayQD { get; set; }

        [Display(Name = "OB-ersättning, kväll (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialEveningPayCalcQD { get; set; }

        [Display(Name = "OB-ersättning, kväll (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialNightPayQD { get; set; }

        [Display(Name = "OB-ersättning, kväll (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialNightPayCalcQD { get; set; }

        [Display(Name = "OB-ersättning, kväll (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialWeekendPayQD { get; set; }

        [Display(Name = "OB-ersättning, kväll (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialWeekendPayCalcQD { get; set; }

        [Display(Name = "OB-ersättning, kväll (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialGrandWeekendPayQD { get; set; }

        [Display(Name = "OB-ersättning, kväll (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialGrandWeekendPayCalcQD { get; set; }

        [Display(Name = "Summa OB-ersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialSumPayQD { get; set; }

        [Display(Name = "Summa OB-ersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialSumPayCalcQD { get; set; }

        [Display(Name = "Jour-ersättning, dag (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string OnCallDayPayQD { get; set; }

        [Display(Name = "Jour-ersättning, dag (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string OnCallDayPayCalcQD { get; set; }

        [Display(Name = "Jour-ersättning, natt/helg (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string OnCallNightPayQD { get; set; }

        [Display(Name = "Jour-ersättning, natt/helg (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string OnCallNightPayCalcQD { get; set; }

        [Display(Name = "Summa Jour-ersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string OnCallSumPayQD { get; set; }

        [Display(Name = "Summa Jour-ersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string OnCallSumPayCalcQD { get; set; }

        [Display(Name = "Sociala avgifter enligt lag (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string SocialFeesQD { get; set; }

        [Display(Name = "Sociala avgifter enligt lag (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string SocialFeesCalcQD { get; set; }

        [Display(Name = "Pensioner och försäkringar enligt kollektivavtal (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string PensionAndInsuranceQD { get; set; }

        [Display(Name = "Pensioner och försäkringar enligt kollektivavtal (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string PensionAndInsuranceCalcQD { get; set; }

        [Display(Name = "Kostnad för dag 1, karensdagen (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string CostQD { get; set; }

        [Display(Name = "Kostnad för dag 1, karensdagen (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string CostCalcQD { get; set; }

        //Day 2 to day 14, if hours only given as input by the ombud
        [Display(Name = "Antal timmar:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string HoursD2T14 { get; set; }

        [Display(Name = "Lön, 80% av ordinarie timlön:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string SalaryD2T14 { get; set; }

        [Display(Name = "Lön, 80% av ordinarie timlön:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string SalaryCalcD2T14 { get; set; }

        [Display(Name = "Sjuklön (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string SickPayD2T14 { get; set; }

        [Display(Name = "Sjuklön (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string SickPayCalcD2T14 { get; set; }

        [Display(Name = "Semesterersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string SalaryBaseD2T14 { get; set; }

        [Display(Name = "Semesterersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string SalaryBaseCalcD2T14 { get; set; }

        [Display(Name = "Semesterersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string HolidayPayD2T14 { get; set; }

        [Display(Name = "Semesterersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string HolidayPayCalcD2T14 { get; set; }

        [Display(Name = "OB-ersättning, kväll (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialEveningD2T14 { get; set; }

        [Display(Name = "OB-ersättning, kväll (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialEveningPayD2T14 { get; set; }

        [Display(Name = "OB-ersättning, kväll (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialEveningPayCalcD2T14 { get; set; }

        [Display(Name = "OB-ersättning, natt (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialNightD2T14 { get; set; }

        [Display(Name = "OB-ersättning, natt (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialNightPayD2T14 { get; set; }

        [Display(Name = "OB-ersättning, natt (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialNightPayCalcD2T14 { get; set; }

        [Display(Name = "OB-ersättning, helg (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialWeekendD2T14 { get; set; }

        [Display(Name = "OB-ersättning, helg (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialWeekendPayD2T14 { get; set; }

        [Display(Name = "OB-ersättning, helg (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialWeekendPayCalcD2T14 { get; set; }

        [Display(Name = "OB-ersättning, storhelg (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialGrandWeekendD2T14 { get; set; }

        [Display(Name = "OB-ersättning, storhelg (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialGrandWeekendPayD2T14 { get; set; }

        [Display(Name = "OB-ersättning, storhelg (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialGrandWeekendPayCalcD2T14 { get; set; }

        [Display(Name = "Summa OB-ersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialSumD2T14 { get; set; }

        [Display(Name = "Summa OB-ersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialSumPayD2T14 { get; set; }

        [Display(Name = "Summa OB-ersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialSumPayCalcD2T14 { get; set; }

        [Display(Name = "Jour-ersättning, dag (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string OnCallDayD2T14 { get; set; }

        [Display(Name = "Jour-ersättning, dag (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string OnCallDayPayD2T14 { get; set; }

        [Display(Name = "Jour-ersättning, dag (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string OnCallDayPayCalcD2T14 { get; set; }

        [Display(Name = "Jour-ersättning, natt/helg (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string OnCallNightD2T14 { get; set; }

        [Display(Name = "Jour-ersättning, natt/helg (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string OnCallNightPayD2T14 { get; set; }

        [Display(Name = "Jour-ersättning, natt/helg (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string OnCallNightPayCalcD2T14 { get; set; }

        [Display(Name = "Summa Jour-ersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string OnCallSumD2T14 { get; set; }

        [Display(Name = "Summa Jour-ersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string OnCallSumPayD2T14 { get; set; }

        [Display(Name = "Summa Jour-ersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string OnCallSumPayCalcD2T14 { get; set; }

        [Display(Name = "Sociala avgifter enligt lag (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string SocialFeesD2T14 { get; set; }

        [Display(Name = "Sociala avgifter enligt lag (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string SocialFeesCalcD2T14 { get; set; }

        [Display(Name = "Pensioner och försäkringar enligt kollektivavtal (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string PensionAndInsuranceD2T14 { get; set; }

        [Display(Name = "Pensioner och försäkringar enligt kollektivavtal (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string PensionAndInsuranceCalcD2T14 { get; set; }

        [Display(Name = "Kostnad för dag 1, karensdagen (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string CostD2T14 { get; set; }

        [Display(Name = "Kostnad för dag 1, karensdagen (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string CostCalcD2T14 { get; set; }


        //Day 15 and beyond
        [Display(Name = "Antal timmar:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string HoursD15Plus { get; set; }

        [Display(Name = "Lön, 80% av ordinarie timlön:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string SalaryD15Plus { get; set; }

        [Display(Name = "Lön, 80% av ordinarie timlön:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string SalaryCalcD15Plus { get; set; }

        [Display(Name = "Sjuklön (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string SickPayD15Plus { get; set; }

        [Display(Name = "Sjuklön (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string SickPayCalcD15Plus { get; set; }

        [Display(Name = "Semesterersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string SalaryBaseD15Plus { get; set; }

        [Display(Name = "Semesterersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string SalaryBaseCalcD15Plus { get; set; }

        [Display(Name = "Semesterersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string HolidayPayD15Plus { get; set; }

        [Display(Name = "Semesterersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string HolidayPayCalcD15Plus { get; set; }

        [Display(Name = "OB-ersättning, kväll (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialEveningD15Plus { get; set; }

        [Display(Name = "OB-ersättning, kväll (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialEveningPayD15Plus { get; set; }

        [Display(Name = "OB-ersättning, kväll (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialEveningPayCalcD15Plus { get; set; }

        [Display(Name = "OB-ersättning, natt (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialNightD15Plus { get; set; }

        [Display(Name = "OB-ersättning, natt (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialNightPayD15Plus { get; set; }

        [Display(Name = "OB-ersättning, natt (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialNightPayCalcD15Plus { get; set; }

        [Display(Name = "OB-ersättning, helg (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialWeekendD15Plus { get; set; }

        [Display(Name = "OB-ersättning, helg (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialWeekendPayD15Plus { get; set; }

        [Display(Name = "OB-ersättning, helg (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialWeekendPayCalcD15Plus { get; set; }

        [Display(Name = "OB-ersättning, storhelg (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialGrandWeekendD15Plus { get; set; }

        [Display(Name = "OB-ersättning, storhelg (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialGrandWeekendPayD15Plus { get; set; }

        [Display(Name = "OB-ersättning, storhelg (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialGrandWeekendPayCalcD15Plus { get; set; }

        [Display(Name = "Summa OB-ersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialSumD15Plus { get; set; }

        [Display(Name = "Summa OB-ersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialSumPayD15Plus { get; set; }

        [Display(Name = "Summa OB-ersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialSumPayCalcD15Plus { get; set; }

        [Display(Name = "Jour-ersättning, dag (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string OnCallDayD15Plus { get; set; }

        [Display(Name = "Jour-ersättning, dag (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string OnCallDayPayD15Plus { get; set; }

        [Display(Name = "Jour-ersättning, dag (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string OnCallDayPayCalcD15Plus { get; set; }

        [Display(Name = "Jour-ersättning, natt/helg (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string OnCallNightD15Plus { get; set; }

        [Display(Name = "Jour-ersättning, natt/helg (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string OnCallNightPayD15Plus { get; set; }

        [Display(Name = "Jour-ersättning, natt/helg (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string OnCallNightPayCalcD15Plus { get; set; }

        [Display(Name = "Summa Jour-ersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string OnCallSumD15Plus { get; set; }

        [Display(Name = "Summa Jour-ersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string OnCallSumPayD15Plus { get; set; }

        [Display(Name = "Summa Jour-ersättning (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string OnCallSumPayCalcD15Plus { get; set; }

        [Display(Name = "Sociala avgifter enligt lag (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string SocialFeesD15Plus { get; set; }

        [Display(Name = "Sociala avgifter enligt lag (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string SocialFeesCalcD15Plus { get; set; }

        [Display(Name = "Pensioner och försäkringar enligt kollektivavtal (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string PensionAndInsuranceD15Plus { get; set; }

        [Display(Name = "Pensioner och försäkringar enligt kollektivavtal (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string PensionAndInsuranceCalcD15Plus { get; set; }

        [Display(Name = "Kostnad för dag 1, karensdagen (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string CostD15Plus { get; set; }

        [Display(Name = "Kostnad för dag 1, karensdagen (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string CostCalcD15Plus { get; set; }

        //The two properties below are used for the total cost if the number of sickdays in the claim is less than 15
        [Display(Name = "Kostnad för sjukperioden (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string TotalCostD1T14 { get; set; }

        [Display(Name = "Kostnad för sjukperioden (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string TotalCostCalcD1T14 { get; set; }

        //The two properties below are used for the total cost if the number of sickdays in the claim is greater than 14
        [Display(Name = "Kostnad för sjukperioden (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string TotalCostD1Plus { get; set; }

        [Display(Name = "Kostnad för sjukperioden (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string TotalCostCalcD1Plus { get; set; }

        public virtual ClaimStatus ClaimStatus { get; set; }

    }
}