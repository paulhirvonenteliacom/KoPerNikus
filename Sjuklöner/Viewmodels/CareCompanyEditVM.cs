using Sjuklöner.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sjuklöner.Viewmodels
{
    public class CareCompanyEditVM
    {
        public int CareCompanyId { get; set; }

        public CareCompany CareCompany { get; set; }

        public int? SelectedCollectiveAgreementId { get; set; }
        [Display(Name = "Kollektivavtal")]
        public IEnumerable<SelectListItem> CollectiveAgreement { get; set; }
    }
}