using Automata_DTaylor_FinancialPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Automata_DTaylor_FinancialPortal.Helpers
{
    public class DashboardHelper
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        public static decimal TargetAmountUsedThisMonth(BudgetCategory budgetCategory)
        {
            decimal amountSpent = 0;
            var aMonthAgo = DateTimeOffset.UtcNow.ToLocalTime().AddMonths(-1);
            var childItemIds = "";
            foreach(var item in budgetCategory.BudgetCategoryItems)
            {
                childItemIds += (item.Id + ", ");
            }

            foreach (var transaction in db.Transactions.Where(t => childItemIds.Contains(t.BudgetCategoryItemId.ToString()) && t.Created > aMonthAgo))
            {
                amountSpent += transaction.Amount;
            }
            decimal amountUsed = amountSpent / budgetCategory.TargetAmount;

            if (amountUsed > 1)
                amountUsed = 1;
            
            return amountUsed;
        }
    }
}