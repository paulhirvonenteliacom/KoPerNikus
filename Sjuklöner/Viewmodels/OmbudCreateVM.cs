using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Sjuklöner.Models;
using static Sjuklöner.Viewmodels.OmbudIndexVM;

namespace Sjuklöner.Viewmodels
{
    public class OmbudCreateVM
    {
        public string Id { get; set; }

        public int CareCompanyId { get; set; }

        [Required]
        [Display(Name = "Bolagsnamn")]
        public string CareCompanyName { get; set; }

        public OmbudForVM Ombud { get; set; }
    }
}
