using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sjuklöner.Models
{
    public class Document
    {
        public int Id { get; set; }

        public int? MimeTypeId { get; set; }

        public int? DocStatusId { get; set; }

        public int? PurposeId { get; set; }

        //public string OwnerId { get; set; }

        [Required]
        [Display(Name = "Filnamn")]
        public string Filename { get; set; }

        public int FileSize { get; set; }

        public string Title { get; set; }

        [Display(Name = "Filtyp")]
        public string FileType { get; set; }

        [Required]
        [Display(Name = "Uppladdningsdatum")]
        public DateTime DateUploaded { get; set; }

        public virtual MimeType MimeType { get; set; }

        public virtual DocStatus DocStatus { get; set; }

        public virtual Purpose Purpose { get; set; }

        //public virtual ApplicationUser Owner { get; set; }
    }
}