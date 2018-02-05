using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Script.Serialization;

namespace Sjuklöner.Viewmodels
{
    public class CollectiveAgreementIndexVM
    {
        public List<CollAgreementHeader> CollectiveAgreementHeader { get; set; }

        public List<CollAgreementInfo> CollectiveAgreementInfo { get; set; }

        public int NumberOfCollectiveAgreements { get; set; }
    }

    public class CollAgreementHeader
    {
        public int Id { get; set; }

        [Display(Name = "Kollektivavtal")]
        public string Name { get; set; }

        public int Counter { get; set; }
    }

    public class CollAgreementInfo
    {
        public int Id { get; set; }

        [Display(Name = "Startdatum")]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[DataType(DataType.DateTime)]
        public String StartDate { get; set; }

        [Display(Name = "Slutdatum")]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[DataType(DataType.DateTime)]
        public String EndDate { get; set; }

        [Display(Name = "OB, kväll (Kr)")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string PerHourUnsocialEvening { get; set; }

        [Display(Name = "OB, natt (Kr)")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string PerHourUnsocialNight { get; set; }

        [Display(Name = "OB, helg (Kr)")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string PerHourUnsocialWeekend { get; set; }

        [Display(Name = "OB, storhelg (Kr)")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string PerHourUnsocialHoliday { get; set; }

        [Display(Name = "Jour, vardag (Kr)")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string PerHourOnCallWeekday { get; set; }

        [Display(Name = "Jour, natt/helg (Kr)")]
        [DisplayFormat(DataFormatString = "{0:f2}")]
        public string PerHourOnCallWeekend { get; set; }
    }
}