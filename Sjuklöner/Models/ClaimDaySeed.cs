using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sjuklöner.Models
{
    public class ClaimDaySeed
    {
        public int Id { get; set; }

        public string ReferenceNumber { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ClaimDayDate { get; set; }

        public string DateString { get; set; }

        public int SickDayNumber { get; set; }

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