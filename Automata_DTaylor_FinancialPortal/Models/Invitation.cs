using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Automata_DTaylor_FinancialPortal.Models
{
    public class Invitation
    {
        public int Id { get; set; }
        public int HouseholdId { get; set; }
        public bool Used { get; set; }
        public Guid Code { get; set; }
        public string RecipientEmail { get; set; }
        public string SentBy { get; set; }

        public virtual Household Household { get; set; }
    }
}