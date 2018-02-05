using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sjuklöner.Models
{
    public class ClaimDay
    {
        public int Id { get; set; }

        public string ReferenceNumber { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ClaimDayDate { get; set; }

        public string DateString { get; set; }

        public int CollectiveAgreementInfoId { get; set; }

        public int SickDayNumber { get; set; }

        //public string StartHour { get; set; }

        //public string StartMinute { get; set; }

        //public string StopHour { get; set; }

        //public string StopMinute { get; set; }

        //public float NumberOfHours { get; set; }

        //public float NumberOfUnsocialHours { get; set; }

        //public float NumberOfUnsocialHoursNight { get; set; }

        //public float NumberOfUnsocialHoursEvening { get; set; }

        //public string StartHourOnCall { get; set; }

        //public string StartMinuteOnCall { get; set; }

        //public string StopHourOnCall { get; set; }

        //public string StopMinuteOnCall { get; set; }

        //public float NumberOfOnCallHours { get; set; }

        //public float NumberOfOnCallHoursNight { get; set; }

        //public float NumberOfOnCallHoursEvening { get; set; }


        //public string StartHourSI { get; set; }

        //public string StartMinuteSI { get; set; }

        //public string StopHourSI { get; set; }

        //public string StopMinuteSI { get; set; }

        //public float NumberOfHoursSI { get; set; }

        //public float NumberOfUnsocialHoursSI { get; set; }

        //public float NumberOfUnsocialHoursNightSI { get; set; }

        //public float NumberOfUnsocialHoursEveningSI { get; set; }

        //public string StartHourOnCallSI { get; set; }

        //public string StartMinuteOnCallSI { get; set; }

        //public string StopHourOnCallSI { get; set; }

        //public string StopMinuteOnCallSI { get; set; }

        //public float NumberOfOnCallHoursSI { get; set; }

        //public float NumberOfOnCallHoursNightSI { get; set; }

        //public float NumberOfOnCallHoursEveningSI { get; set; }

        public string Hours { get; set; }

        public string UnsocialEvening { get; set; }

        public string UnsocialNight { get; set; }

        public string UnsocialWeekend { get; set; }

        public string UnsocialGrandWeekend { get; set; }

        public string OnCallDay { get; set; }

        public string OnCallNight { get; set; }

        public string HoursSI { get; set; }

        public string UnsocialEveningSI { get; set; }

        public string UnsocialNightSI { get; set; }

        public string UnsocialWeekendSI { get; set; }

        public string UnsocialGrandWeekendSI { get; set; }

        public string OnCallDaySI { get; set; }

        public string OnCallNightSI { get; set; }

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
    }
}