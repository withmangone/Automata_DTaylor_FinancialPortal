using Automata_DTaylor_FinancialPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Automata_DTaylor_FinancialPortal.ExtensionMethods
{
    public static class HouseholdExtensions
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        public static bool HasAccounts(this Household house)
        {
            return house.BankAccounts.Count() > 0;
        }
    }
}