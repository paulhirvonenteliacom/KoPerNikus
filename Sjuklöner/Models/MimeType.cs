using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sjuklöner.Models
{
    public class MimeType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DefaultExtension { get; set; }
        public string IconURL { get; set; }
        public string Description { get; set; }

        public MimeType() { }
        public MimeType(string name, string defaultExtension, string iconUrl, string description)
        {
            Name = name;
            DefaultExtension = defaultExtension;
            IconURL = iconUrl;
            Description = description;
        }
    }
}