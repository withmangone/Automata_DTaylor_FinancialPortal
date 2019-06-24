using Automata_DTaylor_FinancialPortal.Models;
using Automata_DTaylor_FinancialPortal.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Automata_DTaylor_FinancialPortal.Helpers
{
    public class BadgeHelper
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        public static int AccountsOverBudget(DashboardViewModel dbvm)
        {
            var total = dbvm.Household.BankAccounts.Where(b => b.CurrentBalance < 0).Count();
            return total;
        }

        public static int LowBalanceAccounts(DashboardViewModel dbvm)
        {
            var total = dbvm.Household.BankAccounts.Where(b => b.CurrentBalance >= 0 && b.CurrentBalance <= b.LowBalanceLevel).Count();
            return total;
        }

        public static int CategoriesOverBudget(DashboardViewModel dbvm)
        {
            var tally = 0;
            foreach (var category in db.BudgetCategories.Where(i => i.HouseholdId == dbvm.Household.Id).ToList())
            {
                decimal total = 0;
                foreach(var categoryitem in category.BudgetCategoryItems.ToList())
                {
                    var aMonthAgo = DateTimeOffset.UtcNow.ToLocalTime().AddMonths(-1);
                    foreach (var transaction in db.Transactions.Where(t => t.BudgetCategoryItemId == categoryitem.Id && t.Created > aMonthAgo).ToList())
                    {
                        if(transaction.TransactionType == Enumerations.TransactionType.AdjustmentDown || transaction.TransactionType == Enumerations.TransactionType.Withdrawal)
                        {
                            total += transaction.Amount;
                        }
                        else
                        {
                            total -= transaction.Amount;
                        }
                    }
                }
                if(total > category.TargetAmount)
                {
                    tally += 1;
                }
                //    var childItemIds = "";
                //    foreach (var item in category.BudgetCategoryItems)
                //    {
                //        childItemIds += (item.Id + ", ");
                //    }

                //    var myTransactions = db.Transactions.Where(t => childItemIds.Contains(t.BudgetCategoryItemId.ToString()));
                //    decimal myTransactionsTotal = 0;
                //    foreach(var transaction in myTransactions)
                //    {
                //        if(transaction.TransactionType == Enumerations.TransactionType.Withdrawal || transaction.TransactionType == Enumerations.TransactionType.AdjustmentDown)
                //        {
                //            myTransactionsTotal += transaction.Amount;
                //        }
                //        else
                //        {
                //            myTransactionsTotal -= transaction.Amount;
                //        }
                //    }

                //    if (category.TargetAmount < myTransactionsTotal)
                //    {
                //        total += 1;
                //    }
            }
            return tally;

        }
    }
}