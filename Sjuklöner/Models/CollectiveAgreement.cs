using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sjuklöner.Models
{
    public class CollectiveAgreement
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [Display(Name = "OB-ersättning, kväll (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double PerHourUnsocialEvening { get; set; }

        [Display(Name = "OB-ersättning, natt (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double PerHourUnsocialNight { get; set; }

        [Display(Name = "OB-ersättning, helg (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double PerHourUnsocialWeekend { get; set; }

        [Display(Name = "OB-ersättning, storhelg (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double PerHourUnsocialHoliday { get; set; }

        [Display(Name = "Jour-ersättning, vardag (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double PerHourOnCallWeekday { get; set; }

        [Display(Name = "Jour-ersättning, helg (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double PerHourOnCallWeekend { get; set; }

        [Display(Name = "Beredskapsersättning, vardag (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double PerHourPreparedWeekday { get; set; }

        [Display(Name = "Beredsakpsersättning, natt/helg (Kr):")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double PerHourPreparedWeekend { get; set; }

        [Display(Name = "Övertidsfaktor:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double OvertimeFactor { get; set; }

        [Display(Name = "Mertidsfaktor:")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public double ExtraHOurFactor { get; set; }
    }
}