using Automata_DTaylor_FinancialPortal.Models;
using Automata_DTaylor_FinancialPortal.ExtensionMethods;
using Automata_DTaylor_FinancialPortal.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace Automata_DTaylor_FinancialPortal.Controllers
{
    [Authorize(Roles ="HeadOfHouse,Resident")]
    public class DashboardController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public JsonResult BuildBudgetData()
        {
            var userId = User.Identity.GetUserId();
            var houseId = db.Users.AsNoTracking().FirstOrDefault(u => u.Id == userId).HouseholdId;

            var myBudgets = db.BudgetCategories.Where(b => b.HouseholdId == houseId).ToList();
            var barDataList = new List<BudgetBarData>();

            foreach(var budget in myBudgets)
            {
                var barData = new BudgetBarData();
                barData.budget = budget.BudgetCategoryName;
                barData.target = budget.TargetAmount;

                var aMonthAgo = DateTimeOffset.UtcNow.ToLocalTime().AddMonths(-1);
                foreach (var item in budget.BudgetCategoryItems.ToList())
                {
                    barData.actual += db.Transactions.Where(t => t.BudgetCategoryItemId == item.Id && t.Created > aMonthAgo).ToList().Sum(i => i.Amount);
                }

                barDataList.Add(barData);
            }

            return Json(barDataList);
        }

        public JsonResult BuildAccountBudgetData()
        {
            var userId = User.Identity.GetUserId();
            var houseId = db.Users.AsNoTracking().FirstOrDefault(u => u.Id == userId).HouseholdId;

            var myAccounts = db.BankAccounts.Where(b => b.HouseholdId == houseId).ToList();
            var barDataList = new List<BudgetBarData>();

            foreach (var account in myAccounts)
            {
                var barData = new BudgetBarData();
                barData.budget = account.AccountName;
                barData.target = account.LowBalanceLevel;
                barData.actual = account.CurrentBalance;
                barDataList.Add(barData);
            }

            return Json(barDataList);
        }
    }
}