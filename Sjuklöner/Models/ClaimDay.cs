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

        public string ClaimDayDate { get; set; }

        public string DateString { get; set; }

        public int SickDayNumber { get; set; }

        public string StartHour { get; set; }

        public string StartMinute { get; set; }

        public string StopHour { get; set; }

        public string StopMinute { get; set; }

        public float NumberOfHours { get; set; }

        public float NumberOfUnsocialHours { get; set; }

        public float NumberOfUnsocialHoursNight { get; set; }

        public float NumberOfUnsocialHoursEvening { get; set; }

        public string StartHourOnCall { get; set; }

        public string StartMinuteOnCall { get; set; }

        public string StopHourOnCall { get; set; }

        public string StopMinuteOnCall { get; set; }

        public float NumberOfOnCallHours { get; set; }

        public float NumberOfOnCallHoursNight { get; set; }

        public float NumberOfOnCallHoursEvening { get; set; }


        public string StartHourSI { get; set; }

        public string StartMinuteSI { get; set; }

        public string StopHourSI { get; set; }

        public string StopMinuteSI { get; set; }

        public float NumberOfHoursSI { get; set; }

        public float NumberOfUnsocialHoursSI { get; set; }

        public float NumberOfUnsocialHoursNightSI { get; set; }

        public float NumberOfUnsocialHoursEveningSI { get; set; }

        public string StartHourOnCallSI { get; set; }

        public string StartMinuteOnCallSI { get; set; }

        public string StopHourOnCallSI { get; set; }

        public string StopMinuteOnCallSI { get; set; }

        public float NumberOfOnCallHoursSI { get; set; }

        public float NumberOfOnCallHoursNightSI { get; set; }

        public float NumberOfOnCallHoursEveningSI { get; set; }
    }
}