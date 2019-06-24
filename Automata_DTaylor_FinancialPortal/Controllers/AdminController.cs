using Automata_DTaylor_FinancialPortal.ExtensionMethods;
using Automata_DTaylor_FinancialPortal.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Automata_DTaylor_FinancialPortal.Controllers
{
    [RequireHttps]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin
        [Authorize(Roles ="Admin")]
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var currentUser = db.Users.Find(userId);
            var currentRole = currentUser.GetRole();
            switch(currentRole)
            {
                case "Admin":
                    return View();
                case "HeadOfHouse":
                case "Resident":
                    return View("Dashboard", "Households");
                case "Lobbyist":
                    return View("Index", "Home");
                default:
                    return View("Login", "Account");
            }
        }
    }
}