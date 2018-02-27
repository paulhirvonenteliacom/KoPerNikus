using Sjuklöner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sjuklöner.Viewmodels
{
    public class IndexPageOmbudVM
    {
        public bool AssistantsExist { get; set; }

        public List<Claim> DecidedClaims  { get; set; }  //Avslagna

        public List<Claim> DraftClaims { get; set; }  //Ej inskickade, utkast

        public List<Claim> UnderReviewClaims { get; set; }  //Inskickade, under handläggning

        //public List<Claim> ApprovedClaims { get; set; } //Godkända
    }
}