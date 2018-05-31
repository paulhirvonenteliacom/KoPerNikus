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

        //[RegularExpression(@"\d{0,2}(\,\d{0,2})?$", ErrorMessage = "Fel format eller ogiltigt antal.")]
        public string[] HoursSI { get; set; }

        //[RegularExpression(@"\d{0,2}(\,\d{0,2})?$", ErrorMessage = "Fel format eller ogiltigt antal.")]
        public string[] UnsocialEveningSI { get; set; }

        //[RegularExpression(@"\d{0,2}(\,\d{0,2})?$", ErrorMessage = "Fel format eller ogiltigt antal.")]
        public string[] UnsocialNightSI { get; set; }

        //[RegularExpression(@"\d{0,2}(\,\d{0,2})?$", ErrorMessage = "Fel format eller ogiltigt antal.")]
        public string[] UnsocialWeekendSI { get; set; }

        //[RegularExpression(@"\d{0,2}(\,\d{0,2})?$", ErrorMessage = "Fel format eller ogiltigt antal.")]
        public string[] UnsocialGrandWeekendSI { get; set; }

        //[RegularExpression(@"\d{0,2}(\,\d{0,2})?$", ErrorMessage = "Fel format eller ogiltigt antal.")]
        public string[] OnCallDaySI { get; set; }

        //[RegularExpression(@"\d{0,2}(\,\d{0,2})?$", ErrorMessage = "Fel format eller ogiltigt antal.")]
        public string[] OnCallNightSI { get; set; }

        //[RegularExpression(@"^([0-9]|1[0-9]|2[0-5])?([,]|[,][0-9]|[,][0-9][0-9])?$", ErrorMessage = "Fel format eller ogiltigt antal.")]
        //public List<string> Test { get; set; }
    }
}