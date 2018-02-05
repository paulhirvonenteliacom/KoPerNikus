using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sjuklöner.Models
{
    public class Message
    {
        public int Id { get; set; }

        public int ClaimId { get; set; }
        
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Datum")]
        public DateTime CommentDate { get; set; }

        [DataType(DataType.MultilineText)]
        [Required]
        public string Comment { get; set; }

        public virtual ApplicationUser applicationUser { get; set; }
    }
}