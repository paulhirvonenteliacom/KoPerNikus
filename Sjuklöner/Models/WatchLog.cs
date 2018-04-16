using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sjuklöner.Models
{
    public class WatchLog
    {
        public int Id { get; set; }
        public int LogCode { get; set; }
        public DateTime LogDate{ get; set; }
        public String Robot { get; set; }
        public String LogMsg { get; set; }
    }
}