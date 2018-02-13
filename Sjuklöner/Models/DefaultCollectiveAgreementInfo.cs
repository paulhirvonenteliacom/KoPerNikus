using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sjuklöner.Models
{
    public class DefaultCollectiveAgreementInfo
    {
        public int Id { get; set; }

        public int CollectiveAgreementHeaderId { get; set; }

        [Required]
        [Display(Name = "Från och med")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Till och med")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }

        [Required]
        [Display(Name = "OB, kväll (Kr)")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string PerHourUnsocialEvening { get; set; }

        [Required]
        [Display(Name = "OB, natt (Kr)")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string PerHourUnsocialNight { get; set; }

        [Required]
        [Display(Name = "OB, helg (Kr)")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string PerHourUnsocialWeekend { get; set; }

        [Required]
        [Display(Name = "OB, storhelg (Kr)")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string PerHourUnsocialHoliday { get; set; }

        [Required]
        [Display(Name = "Jour, vardag (Kr)")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string PerHourOnCallWeekday { get; set; }

        [Required]
        [Display(Name = "Jour, natt/helg (Kr)")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string PerHourOnCallWeekend { get; set; }
    }
}