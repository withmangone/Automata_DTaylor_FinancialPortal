using Automata_DTaylor_FinancialPortal.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Automata_DTaylor_FinancialPortal.ExtensionMethods
{
    public static class InvitationExtension
    {
        public static ApplicationDbContext db = new ApplicationDbContext();

        public static async Task SendAsync(this Invitation invitation)
        {
            try
            {
                var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                var callbackUrl = urlHelper.Action("InviteRegister", "Account", new { email = invitation.RecipientEmail, code = invitation.Code, householdId = invitation.HouseholdId }, protocol: HttpContext.Current.Request.Url.Scheme);
                var house = db.Households.Find(invitation.HouseholdId);
                var from = "Automata_DTaylor_FinancialPortal<smtp.drew@gmail.com>";
                var email = new MailMessage(from, invitation.RecipientEmail)
                {
                    Subject = $"You have been invited to join {house.Name}.",
                    Body = $"<p>You've been invited by {invitation.SentBy} to plan your finances together on DT Financial Portal!<p><br><br><p>Click <a href={callbackUrl}>here</a> to register as a new user and accept the invite!<p><br> Or, if you're already a registered user, use this code in the Join Household panel of the lobby:{invitation.Code}",
                    IsBodyHtml = true
                };
                var svc = new PersonalEmail();
                await svc.SendAsync(email);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Task.FromResult(0);
            }
        }
    }
}