using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Sjuklöner.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Display(Name = "Last Logon")]
        [DataType(DataType.DateTime)]
        public System.DateTime LastLogon { get; set; }

        [Display(Name = "Assistansbolag")]
        public int? CareCompanyId { get; set; }

        public virtual List<Document> Documents { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Claim> Claims { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ClaimDay> ClaimDays { get; set; }
        public DbSet<MimeType> MimeTypes { get; set; }
        public DbSet<Purpose> Purposes { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocStatus> DocStatuses { get; set; }
        public DbSet<ClaimReferenceNumber> ClaimReferenceNumbers { get; set; }
        public DbSet<ClaimStatus> ClaimStatuses { get; set; }
        public DbSet<CollectiveAgreement> CollectiveAgreements { get; set; }
        public DbSet<CareCompany> CareCompanies { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        //public System.Data.Entity.DbSet<Sjuklöner.Models.Assistant> ApplicationUsers { get; set; }
    }
}