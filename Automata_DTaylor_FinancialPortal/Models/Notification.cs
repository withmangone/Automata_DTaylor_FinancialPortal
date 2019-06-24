using Automata_DTaylor_FinancialPortal.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Automata_DTaylor_FinancialPortal.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int HouseholdId { get; set; }
        public string NotificationBody { get; set; }
        public DateTimeOffset Created { get; set; }
        public NotificationType NotificationType { get; set; }

        public virtual Household Household { get; set; }
    }
}