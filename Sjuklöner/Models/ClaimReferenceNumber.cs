using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sjuklöner.Models
{
    public class ClaimReferenceNumber
    {
        public int Id { get; set; }

        public int LatestYear { get; set; }

        public int LatestReferenceNumber { get; set; }
    }
}