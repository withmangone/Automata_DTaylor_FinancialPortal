using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Automata_DTaylor_FinancialPortal.Models
{
    public class BudgetCategoryItem
    {
        public int Id { get; set; }
        public int BudgetCategoryId { get; set; }
        [Display(Name = "Item Name")]
        public string ItemName { get; set; }

        public virtual BudgetCategory BudgetCategory { get; set; }
    }
}