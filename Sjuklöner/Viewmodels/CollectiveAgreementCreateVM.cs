using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sjuklöner.Viewmodels
{
    public class CollectiveAgreementCreateVM
    {
        [Display(Name = "Kollektivavtal")]
        public string Name { get; set; }

        public bool NewAgreement { get; set; }

        public int HeaderId { get; set; }

        [Required]
        [Display(Name = "Startdatum")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Slutdatum")]
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