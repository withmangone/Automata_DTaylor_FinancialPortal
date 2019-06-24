using Automata_DTaylor_FinancialPortal.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Automata_DTaylor_FinancialPortal.Models
{
    public class BankAccount
    {
        public int Id { get; set; }
        public int HouseholdId { get; set; }
        [Display(Name = "Account Name")]
        public string AccountName { get; set; }
        [Display(Name = "Account Type")]
        public AccountType AccountType { get; set; }
        [Display(Name = "Starting Balance")]
        public decimal StartingBalance { get; set; }
        [Display(Name = "Low Balance Level")]
        public decimal LowBalanceLevel { get; set; }
        [Display(Name = "Current Balance")]
        public decimal CurrentBalance { get; set; }

        public virtual Household Household { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }

        public BankAccount()
        {
            Transactions = new HashSet<Transaction>();
        }
    }
}