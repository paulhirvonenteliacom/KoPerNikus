using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sjuklöner.Viewmodels
{
    public class MessageVM
    {
        public MessageVM(DateTime date, string comment, string name)
        {
            Date = date;
            Comment = comment;
            Name = name;
        }

        public DateTime Date { get; set; }
        public string Comment { get; set; }
        public string Name { get; set; }

    }
}