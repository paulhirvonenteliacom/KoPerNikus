using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Script.Serialization;

namespace Sjuklöner.Viewmodels
{
    public class Create2VM
    {
        public List<ScheduleRow> ScheduleRowList { get; set; }

        public string ReferenceNumber { get; set; }

        public string RegAssistantSSNAndName { get; set; }

        public string[] SubAssistantSSNAndName { get; set; }

        public int NumberOfSubAssistants { get; set; }
    }

    public class ScheduleRow
    {
        public bool Well { get; set; } //Frisk, true/false för denna property avgör om en dag räknas som en sjukdag eller inte

        public string ScheduleRowDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DayDate { get; set; }

        public string ScheduleRowDateString { get; set; }

        public string ScheduleRowWeekDay { get; set; }

        [RegularExpression(@"^([0-9]|1[0-9]|2[0-5])?([,]|[,][0-9]|[,][0-9][0-9])?$", ErrorMessage = "Ogiltigt format eller värde.")]
        public string Hours { get; set; }

        [RegularExpression(@"^([0-9]|1[0-9]|2[0-5])?([,]|[,][0-9]|[,][0-9][0-9])?$", ErrorMessage = "Ogiltigt format eller värde.")]
        public string UnsocialEvening { get; set; }

        [RegularExpression(@"^([0-9]|1[0-9]|2[0-5])?([,]|[,][0-9]|[,][0-9][0-9])?$", ErrorMessage = "Ogiltigt format eller värde.")]
        public string UnsocialNight { get; set; }

        [RegularExpression(@"^([0-9]|1[0-9]|2[0-5])?([,]|[,][0-9]|[,][0-9][0-9])?$", ErrorMessage = "Ogiltigt format eller värde.")]
        public string UnsocialWeekend { get; set; }

        [RegularExpression(@"^([0-9]|1[0-9]|2[0-5])?([,]|[,][0-9]|[,][0-9][0-9])?$", ErrorMessage = "Ogiltigt format eller värde.")]
        public string UnsocialGrandWeekend { get; set; }

        [RegularExpression(@"^([0-9]|1[0-9]|2[0-5])?([,]|[,][0-9]|[,][0-9][0-9])?$", ErrorMessage = "Ogiltigt format eller värde.")]
        public string OnCallDay { get; set; }

        [RegularExpression(@"^([0-9]|1[0-9]|2[0-5])?([,]|[,][0-9]|[,][0-9][0-9])?$", ErrorMessage = "Ogiltigt format eller värde.")]
        public string OnCallNight { get; set; }

        public string[] HoursSI { get; set; }

        public string[] UnsocialEveningSI { get; set; }

        public string[] UnsocialNightSI { get; set; }

        public string[] UnsocialWeekendSI { get; set; }

        public string[] UnsocialGrandWeekendSI { get; set; }

        public string[] OnCallDaySI { get; set; }

        public string[] OnCallNightSI { get; set; }
    }
}