using Sjuklöner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sjuklöner.Viewmodels
{
    public class IndexPageAdmOffVM
    {
        public int NumberOfDecided { get; set; }

        public int NumberOfInbox { get; set; }

        public int NumberOfReview { get; set; }

        public int NumberOfDecidedFiltered { get; set; }

        public int NumberOfInboxFiltered { get; set; }

        public int NumberOfReviewFiltered { get; set; }

        public bool MyClaims { get; set; }

        public int? SelectedTimePeriodId { get; set; }

        public IEnumerable<SelectListItem> TimePeriods { get; set; }

        public int? SelectedKeyId { get; set; }

        public IEnumerable<SelectListItem> Keys { get; set; }

        public string SearchString { get; set; }

        public string FilterTextDecided { get; set; }

        public string FilterTextInbox { get; set; }

        public string FilterTextReview { get; set; }

        public List<Claim> DecidedClaims  { get; set; }  //Beslutade, old "Avslagna"

        public List<Claim> InInboxClaims { get; set; }  //I inkorgen

        public List<Claim> UnderReviewClaims { get; set; }  //Inskickade, under handläggning
    }
}