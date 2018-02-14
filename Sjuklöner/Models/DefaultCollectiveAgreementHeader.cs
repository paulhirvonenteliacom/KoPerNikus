using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sjuklöner.Models
{
    public class DefaultCollectiveAgreementHeader
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Kollektivavtal")]
        public string Name { get; set; }

        public int Counter { get; set; }
    }
}