using Automata_DTaylor_Bugtracker.Helpers;
using Automata_DTaylor_FinancialPortal.Enumerations;
using Automata_DTaylor_FinancialPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Automata_DTaylor_FinancialPortal.Helpers
{
    public class HouseholdHelper
    {
        ApplicationDbContext db = new ApplicationDbContext();
        RoleHelper roleHelper = new RoleHelper();

        public void RemoveResidentFromHousehold(string userId)
        {
            var user = db.Users.Find(userId);
            user.HouseholdId = db.Households.FirstOrDefault(h => h.Name == "The Lobby").Id;
            roleHelper.RemoveUserFromRole(userId, PortalRole.Resident);
            roleHelper.AddUserToRole(userId, PortalRole.Lobbyist);
            db.SaveChanges();
        }

        public void PromoteToHOH(string userId, string targetUserId)
        {
            roleHelper.RemoveUserFromRole(userId, PortalRole.HeadOfHouse);
            roleHelper.AddUserToRole(userId, PortalRole.Resident);
            roleHelper.RemoveUserFromRole(targetUserId, PortalRole.Resident);
            roleHelper.AddUserToRole(targetUserId, PortalRole.HeadOfHouse);
            db.SaveChanges();
        }
    }
}