using Sjuklöner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sjuklöner.Viewmodels
{
    public class IndexPageOmbudVM
    {
        public int NumberOfDecided { get; set; }

        public int NumberOfDraft { get; set; }

        public int NumberOfReview { get; set; }

        public int NumberOfDecidedFiltered { get; set; }

        public int NumberOfDraftFiltered { get; set; }

        public int NumberOfReviewFiltered { get; set; }

        public bool MyClaims { get; set; }

        public int? SelectedTimePeriodId { get; set; }

        public IEnumerable<SelectListItem> TimePeriods { get; set; }

        public int? SelectedKeyId { get; set; }

        public IEnumerable<SelectListItem> Keys { get; set; }

        public string SearchString { get; set; }

        public string FilterTextDecided { get; set; }

        public string FilterTextDraft { get; set; }

        public string FilterTextReview { get; set; }

        public bool AssistantsExist { get; set; }

        public string CompanyName { get; set; }

        public List<Claim> DecidedClaims  { get; set; }  //Avslagna

        public List<Claim> DraftClaims { get; set; }  //Ej inskickade, utkast

        public List<Claim> UnderReviewClaims { get; set; }  //Inskickade, under handläggning

        //public List<Claim> ApprovedClaims { get; set; } //Godkända
    }
}