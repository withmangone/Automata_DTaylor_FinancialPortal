using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Automata_DTaylor_FinancialPortal.Enumerations
{
    public enum PortalRole
    {
        [Display(Name = "Head of House")]
        HeadOfHouse,
        Resident,
        Lobbyist,
        Admin
    }
}