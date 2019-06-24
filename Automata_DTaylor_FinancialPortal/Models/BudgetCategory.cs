using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Automata_DTaylor_FinancialPortal.Models
{
    public class BudgetCategory
    {
        public int Id { get; set; }
        public int HouseholdId { get; set; }
        [Display(Name = "Budget Category Name")]
        public string BudgetCategoryName { get; set; }
        [Display(Name = "Target Amount")]
        public decimal TargetAmount { get; set; }

        public virtual Household Household { get; set; }

        public virtual ICollection<BudgetCategoryItem> BudgetCategoryItems { get; set; }

        public BudgetCategory()
        {
            BudgetCategoryItems = new HashSet<BudgetCategoryItem>();
        }
    }
}