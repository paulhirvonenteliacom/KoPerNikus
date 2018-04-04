using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sjuklöner.Viewmodels
{
    public class AdminIndexVM
    {
        public int NumberOfClaims { get; set; }

        public int NumberOfAdmOffs { get; set; }

        public int NumberOfOmbuds { get; set; }

        public int NumberOfCareCompanies { get; set; }

        public int NumberOfAssistants { get; set; }

        public int NumberOfCollectiveAgreements { get; set; }

    }
}
