using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Automata_DTaylor_FinancialPortal.Enumerations
{
    public enum AccountType
    {
        Checking,
        Savings,
        [Display(Name = "Money Market")]
        MoneyMarket,
        Credit
    }
}