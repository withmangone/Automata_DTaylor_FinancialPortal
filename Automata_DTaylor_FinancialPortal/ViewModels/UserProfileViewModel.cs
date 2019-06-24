using Automata_DTaylor_FinancialPortal.Enumerations;
using Automata_DTaylor_FinancialPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Automata_DTaylor_FinancialPortal.ViewModels
{
    public class UserProfileViewModel
    {
        public IndexViewModel IndexViewModel { get; set; }
        public UserViewModel UserViewModel { get; set; }
        public ChangePasswordViewModel ChangePasswordViewModel { get; set; }
        public AccountType AccountType { get; set; }
    }
}