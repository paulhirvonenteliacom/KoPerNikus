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
        public List<ScheduleRow> ScheduleRowList { get; set; }

        public string ReferenceNumber { get; set; }
    }

    public class ScheduleRow
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

        public string StartTimeHourOnCall { get; set; }

        public string StartTimeMinuteOnCall { get; set; }

        public string StopTimeHourOnCall { get; set; }

        public string StopTimeMinuteOnCall { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        public float NumberOfOnCallHours { get; set; }

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
        public float NumberOfOnCallHoursEveningSI { get; set; }
    }
}