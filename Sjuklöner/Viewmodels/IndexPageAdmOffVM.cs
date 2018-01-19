using Sjuklöner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sjuklöner.Viewmodels
{
    public class IndexPageAdmOffVM
    {
        public List<Claim> DecidedClaims  { get; set; }  //Beslutade, old "Avslagna"

        public List<Claim> InInboxClaims { get; set; }  //I inkorgen

        public List<Claim> UnderReviewClaims { get; set; }  //Inskickade, under handläggning

        //public List<Claim> ApprovedClaims { get; set; } //Godkända
    }
}