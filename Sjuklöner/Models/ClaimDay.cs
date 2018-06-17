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

        public int? SickDayNumber { get; set; }

        public int CalendarDayNumber { get; set; }

        public bool Well { get; set; } //Well = true betyder att ordinarie assistent varit frisk den dagen. Well = false betyder att ordinarie assistent varit sjuk den dagen.

        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string Hours { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialEvening { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialNight { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialWeekend { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialGrandWeekend { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string OnCallDay { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string OnCallNight { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string HoursSI { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialEveningSI { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialNightSI { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialWeekendSI { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string UnsocialGrandWeekendSI { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string OnCallDaySI { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}")]
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