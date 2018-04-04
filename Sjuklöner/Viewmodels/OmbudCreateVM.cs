using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Sjuklöner.Models;
using static Sjuklöner.Viewmodels.OmbudIndexVM;
using System.Web.Mvc;

namespace Sjuklöner.Viewmodels
{
    public class OmbudCreateVM
    {
        [Display(Name = "Bolagsnamn")]
        public string CareCompanyName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0}et måste vara minst {2} tecken långt.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Lösenord")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta lösenord")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Lösenordet och det bekräftade lösenordet är inte lika.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Förnamn")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Efternamn")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Personnummer")]
        public string SSN { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Telefonnummer")]
        public string PhoneNumber { get; set; }

        [Range(typeof(int), "1", "999999", ErrorMessage = "Ett assistansbolag måste väljas")]
        public int? SelectedCareCompanyId { get; set; }

        //[Required]
        [Display(Name = "Assistansbolag")]
        public IEnumerable<SelectListItem> CareCompanies { get; set; }

        public List<int> CareCompanyIds { get; set; } //This list is required in order to be able to map the selected ddl ids to CareCompany records in the db.
    }
}
