using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sjuklöner.Models
{
    public class AppAdmin
    {
        //This class contains properties that are valid for the whole application and that can be changed by the admin only (only one so far)
        //There shall be only one instance of this class

        public int Id { get; set; }

        public bool AutomaticTransferToProcapita { get; set; }
    }
}