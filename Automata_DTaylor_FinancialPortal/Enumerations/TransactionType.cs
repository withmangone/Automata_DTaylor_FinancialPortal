using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Automata_DTaylor_FinancialPortal.Enumerations
{
    public enum TransactionType
    {
        Deposit,
        Withdrawal,
        [Display(Name = "Adjustment Up")]
        AdjustmentUp,
        [Display(Name = "Adjustment Down")]
        AdjustmentDown,
        Reconciliation
    }
}