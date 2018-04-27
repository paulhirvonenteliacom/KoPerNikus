using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sjuklöner.Models
{
    public class WatchLog
    {
        public int Id { get; set; }
        [Display(Name = "Kod")]
        public int LogCode { get; set; }
        [Display(Name = "Datum")]
        public DateTime LogDate{ get; set; }
        public String Robot { get; set; }
        [Display(Name = "Meddelande")]
        public String LogMsg { get; set; }
    }
}