using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Script.Serialization;

namespace Sjuklöner.Viewmodels
{
    public class ScheduleVM
    {
        public List<ScheduleRow2> ScheduleRowList { get; set; }

        public string ReferenceNumber { get; set; }
    }

    public class ScheduleRow2
    {
        public string ScheduleRowDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DayDate { get; set; }

        public string ScheduleRowDateString { get; set; }

        public string ScheduleRowWeekDay { get; set; }

        public string StartTimeHour { get; set; }

        public string StartTimeMinute { get; set; }

        public string StopTimeHour { get; set; }

        public string StopTimeMinute { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        public float NumberOfHours { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        public float NumberOfUnsocialHours { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        public float NumberOfUnsocialHoursNight { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        public float NumberOfUnsocialHoursEvening { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        public float NumberOfUnsocialHoursWeekend { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        public float NumberOfUnsocialHoursGrandWeekend { get; set; }

        public string StartTimeHourOnCall { get; set; }

        public string StartTimeMinuteOnCall { get; set; }

        public string StopTimeHourOnCall { get; set; }

        public string StopTimeMinuteOnCall { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        public float NumberOfOnCallHours { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        public float NumberOfOnCallHoursDay { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        public float NumberOfOnCallHoursNight { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        public float NumberOfOnCallHoursEvening { get; set; }

        public string StartTimeHourSI { get; set; }

        public string StartTimeMinuteSI { get; set; }

        public string StopTimeHourSI { get; set; }

        public string StopTimeMinuteSI { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        public float NumberOfHoursSI { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        public float NumberOfUnsocialHoursSI { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        public float NumberOfUnsocialHoursNightSI { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        public float NumberOfUnsocialHoursEveningSI { get; set; }

        public string StartTimeHourOnCallSI { get; set; }

        public string StartTimeMinuteOnCallSI { get; set; }

        public string StopTimeHourOnCallSI { get; set; }

        public string StopTimeMinuteOnCallSI { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        public float NumberOfOnCallHoursSI { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        public float NumberOfOnCallHoursNightSI { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        public float NumberOfOnCallHoursDaySI { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        public float NumberOfOnCallHoursEveningSI { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        public float NumberOfUnsocialHoursWeekendSI { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        public float NumberOfUnsocialHoursGrandWeekendSI { get; set; }


        //The properties below are used if only number of hours shall be given as input 
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
    }
}