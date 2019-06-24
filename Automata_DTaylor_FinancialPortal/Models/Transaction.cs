using Automata_DTaylor_FinancialPortal.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Automata_DTaylor_FinancialPortal.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int BankAccountId { get; set; }
        public int? BudgetCategoryItemId { get; set; }
        public string CreatedById { get; set; }
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public string Payee { get; set; }
        public string Memo { get; set; }
        public DateTimeOffset Created { get; set; }
        public bool Reconciled { get; set; }
        public DateTimeOffset? ReconciledDate { get; set; }

        public virtual BankAccount BankAccount { get; set; }
        public virtual BudgetCategoryItem BudgetCategoryItem { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }
    }
}