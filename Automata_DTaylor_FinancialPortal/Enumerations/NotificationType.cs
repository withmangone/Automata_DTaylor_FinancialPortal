using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Automata_DTaylor_FinancialPortal.Enumerations
{
    public enum NotificationType
    {
        [Display(Name = "Low Budget Level")]
        LowBudget,
        Overdraft
    }
}