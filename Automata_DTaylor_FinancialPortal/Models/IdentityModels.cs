using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Automata_DTaylor_FinancialPortal.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [StringLength(40, MinimumLength = 3, ErrorMessage = "The First Name must be between 3 and 40 characters long.")]
        public string FirstName { get; set; }
        [StringLength(40, MinimumLength = 3, ErrorMessage = "The Last Name must be between 3 and 40 characters long.")]
        public string LastName { get; set; }
        public string ProfilePic { get; set; }
        public int HouseholdId { get; set; }

        public virtual Household Household { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            userIdentity.AddClaim(new Claim("HouseholdId", HouseholdId.ToString()));
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Household> Households { get; set; }
        public DbSet<BudgetCategory> BudgetCategories { get; set; }
        public DbSet<BudgetCategoryItem> BudgetCategoryItems { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}