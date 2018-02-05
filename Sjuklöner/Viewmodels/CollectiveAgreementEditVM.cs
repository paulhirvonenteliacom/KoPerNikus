using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sjuklöner.Viewmodels
{
    public class CollectiveAgreementEditVM
    {
        public CollAgreementHeader CollAgreementHeader { get; set; }

        public List<CollAgreementInfo> CollAgreementInfo { get; set; }

        public string Type { get; set; }
    }
}