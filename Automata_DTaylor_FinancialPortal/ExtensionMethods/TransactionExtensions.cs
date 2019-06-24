using Automata_DTaylor_FinancialPortal.Enumerations;
using Automata_DTaylor_FinancialPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Automata_DTaylor_FinancialPortal.ExtensionMethods
{
    public static class TransactionExtension
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        public static void UpdateAccountBalance(this Transaction transaction)
        {
            //Get Bank Account
            var account = db.BankAccounts.Find(transaction.BankAccountId);
            if (transaction.TransactionType.ToString() == "Withdrawal" || transaction.TransactionType.ToString() == "Adjustment down")
                account.CurrentBalance -= transaction.Amount;
            else
                account.CurrentBalance += transaction.Amount;

            db.SaveChanges();
        }

        public static void RevertAccountBalance(this Transaction transaction)
        {
            //Get Bank Account
            var account = db.BankAccounts.Find(transaction.BankAccountId);
            if (transaction.TransactionType.ToString() == "Withdrawal" || transaction.TransactionType.ToString() == "Adjustment down")
                account.CurrentBalance += transaction.Amount;
            else
                account.CurrentBalance -= transaction.Amount;

            db.SaveChanges();
        }

        public static void NotifyOnBalanceIssues(this Transaction transaction)
        {
            var bankAccount = db.BankAccounts.Find(transaction.BankAccountId);
            if (bankAccount.CurrentBalance < 0)
                transaction.SendOverDraftNotification(bankAccount);
            else if (bankAccount.CurrentBalance < bankAccount.LowBalanceLevel)
                transaction.SendLowBalanceNotification(bankAccount);
        }

        public static void SendOverDraftNotification(this Transaction transaction, BankAccount account)
        {
            var notification = new Notification
            {
                Created = DateTimeOffset.UtcNow.ToLocalTime(),
                HouseholdId = account.HouseholdId,
                NotificationBody = $"Your account '{account.AccountName}' has been overdrafted. Your most recent transaction in the amount of ${transaction.Amount} has resulted in a balance of ${account.CurrentBalance}.",
                NotificationType = NotificationType.Overdraft
            };
            db.Notifications.Add(notification);
            db.SaveChanges();
        }

        public static void SendLowBalanceNotification(this Transaction transaction, BankAccount account)
        {
            var notification = new Notification
            {
                Created = DateTimeOffset.UtcNow.ToLocalTime(),
                HouseholdId = account.HouseholdId,
                NotificationBody = $"Your account '{account.AccountName}' has reached it's low-level balance. Your most recent transaction in the amount of ${transaction.Amount} has resulted in a balance of ${account.CurrentBalance}.",
                NotificationType = NotificationType.LowBudget
            };
            db.Notifications.Add(notification);
            db.SaveChanges();
        }
    }
}