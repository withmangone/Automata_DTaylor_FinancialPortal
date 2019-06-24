using Automata_DTaylor_FinancialPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Automata_DTaylor_FinancialPortal.ViewModels
{
    public class DashboardViewModel
    {
        //the household the dashboard references
        public Household Household = new Household();
        //needed for form submission
        public BankAccount BankAccount = new BankAccount();
        public BudgetCategory BudgetCategory = new BudgetCategory();
        public BudgetCategoryItem BudgetCategoryItem = new BudgetCategoryItem();
    }
}