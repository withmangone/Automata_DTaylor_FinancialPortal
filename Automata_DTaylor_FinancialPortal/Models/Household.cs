using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Automata_DTaylor_FinancialPortal.Models
{
    public class Household
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Greeting { get; set; }
        public DateTimeOffset Created { get; set; }
        public bool IsConfigured { get; set; }

        public virtual ICollection<ApplicationUser> Members { get; set; }
        public virtual ICollection<BankAccount> BankAccounts { get; set; }
        public virtual ICollection<BudgetCategory> BudgetCategories { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Invitation> Invitations { get; set; }

        public Household()
        {
            Members = new HashSet<ApplicationUser>();
            BankAccounts = new HashSet<BankAccount>();
            BudgetCategories = new HashSet<BudgetCategory>();
            Notifications = new HashSet<Notification>();
            Invitations = new HashSet<Invitation>();
        }        
    }
}