using Sjuklöner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sjuklöner.Viewmodels
{
    public class CareCompanyIndexVM
    {
        public List<ApplicationUser> UserList { get; set; }
        public List<CareCompany> CareCompanyList { get; set; }
    }
}