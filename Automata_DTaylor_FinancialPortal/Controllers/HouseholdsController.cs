using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Automata_DTaylor_Bugtracker.Helpers;
using Automata_DTaylor_FinancialPortal.Enumerations;
using Automata_DTaylor_FinancialPortal.ExtensionMethods;
using Automata_DTaylor_FinancialPortal.Helpers;
using Automata_DTaylor_FinancialPortal.Models;
using Automata_DTaylor_FinancialPortal.ViewModels;
using Microsoft.AspNet.Identity;

namespace Automata_DTaylor_FinancialPortal.Controllers
{
    [RequireHttps]
    public class HouseholdsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private RoleHelper roleHelper = new RoleHelper();
        private HouseholdHelper householdHelper = new HouseholdHelper();

        // GET: Households Dashboard
        [Authorize(Roles ="HeadOfHouse, Resident")]
        public ActionResult Dashboard()
        {
            var userId = User.Identity.GetUserId();
            var houseId = db.Users.AsNoTracking().FirstOrDefault(u => u.Id == userId).HouseholdId;
            var dashboardVM = new DashboardViewModel();
            dashboardVM.Household = db.Households.Find(houseId);
            return View(dashboardVM);
        }

        // GET: Households
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View(db.Households.ToList());
        }

        // GET: Households/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Household household = db.Households.Find(id);
        //    if (household == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(household);
        //}

        // GET: Households/Create
        //[Authorize(Roles = "Lobbyist")]
        //public ActionResult Create()
        //{
        //    return View();
        //}

        // POST: Households/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync([Bind(Include = "Name,Greeting")] Household household)
        {
            if (ModelState.IsValid)
            {
                household.Created = DateTimeOffset.UtcNow.ToLocalTime();
                db.Households.Add(household);
                db.SaveChanges();
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                user.HouseholdId = household.Id;
                db.SaveChanges();
                roleHelper.RemoveUserFromRole(userId, PortalRole.Lobbyist);
                roleHelper.AddUserToRole(userId, PortalRole.HeadOfHouse);

                await AdminHelper.ReathorizeUserAsync(userId);
                return RedirectToAction("Dashboard");
            }
            return View(household);
        }

        //POST: Households/WizardSubmit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WizardSubmit(BankAccount bankAccount, BudgetCategory budgetCategory, BudgetCategoryItem budgetCategoryItem, int houseId)
        {
            if (ModelState.IsValid)
            {
                bankAccount.HouseholdId = houseId;
                db.BankAccounts.Add(bankAccount);
                budgetCategory.HouseholdId = houseId;
                db.BudgetCategories.Add(budgetCategory);
                db.SaveChanges();

                budgetCategoryItem.BudgetCategoryId = budgetCategory.Id;
                db.BudgetCategoryItems.Add(budgetCategoryItem);

                var household = db.Households.Find(houseId);
                household.IsConfigured = true;
                db.SaveChanges();
            }
            return RedirectToAction("Dashboard");
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Configure(ConfigurationViewModel config)
        //{
        //    if(ModelState.IsValid)
        //    {
        //        db.BankAccounts.Add(config.BankAccount);
        //        db.BudgetCategories.Add(config.BudgetCategory);
        //        db.SaveChanges();

        //        config.BudgetCategoryItem.BudgetCategoryId = config.BudgetCategory.Id;
        //        db.BudgetCategoryItems.Add(config.BudgetCategoryItem);
        //        db.SaveChanges();

        //        var household = db.Households.Find(User.Identity.GetHouseholdId());
        //    }
        //    return RedirectToAction("Dashboard");
        //}

        //POST: Households/Invite
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> InviteMember([Bind(Include = "RecipientEmail")] Invitation invitation)
        {
            invitation.HouseholdId = User.Identity.GetHouseholdId();
            invitation.Code = Guid.NewGuid();
            invitation.SentBy = db.Users.Find(User.Identity.GetUserId()).FullName;

            db.Invitations.Add(invitation);
            db.SaveChanges();

            //now send the inv
            await invitation.SendAsync();
            return RedirectToAction("Dashboard");
        }

        //POST: Hoouseholds/Join
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Join(Guid code)
        {
            if (code == null)
            {
                RedirectToAction("Index", "Home");
            }
            var checkCode = db.Invitations.Where(i => i.Code == code).FirstOrDefault();
            if (checkCode == null || checkCode.Used)
            {
                RedirectToAction("Index", "Home");
            }
            else
            {
                checkCode.Used = true;
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                user.HouseholdId = checkCode.HouseholdId;
                roleHelper.RemoveUserFromRole(userId, PortalRole.Lobbyist);
                roleHelper.AddUserToRole(userId, PortalRole.Resident);
                db.SaveChanges();

                await AdminHelper.ReathorizeUserAsync(userId);
                return RedirectToAction("Dashboard", "Households");
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Households/Edit/5
        [Authorize(Roles = "HeadOfHouse")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            var HOHId = db.Roles.FirstOrDefault(r => r.Name == "HeadOfHouse").Id;
            ViewBag.Users = new SelectList(db.Users.Where(u => u.HouseholdId == id), "Id", "FullName", db.Users.Where(u => u.HouseholdId == id && u.Roles.FirstOrDefault().RoleId == HOHId));
            return View(household);
        }

        // POST: Households/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Greeting,Created")] Household household)
        {
            if (ModelState.IsValid)
            {
                db.Entry(household).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(household);
        }

        // GET: Households/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Household household = db.Households.Find(id);
        //    if (household == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(household);
        //}

        //POST: Households/Promote/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PromoteAsync(string targetUserId)
        {
            var userId = User.Identity.GetUserId();
            householdHelper.PromoteToHOH(userId, targetUserId);
            await AdminHelper.ReathorizeUserAsync(userId);
            return RedirectToAction("Dashboard");
        }

        //POST: Households/Leave/5
        [HttpPost, ActionName("Leave")]
        [ValidateAntiForgeryToken]
        public ActionResult LeaveHousehold()
        {
            var userId = User.Identity.GetUserId();
            householdHelper.RemoveResidentFromHousehold(userId);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        // POST: Households/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedAsync()
        {
            var userId = User.Identity.GetUserId();
            var householdId = User.Identity.GetHouseholdId();
            roleHelper.RemoveUserFromRole(userId, PortalRole.HeadOfHouse);
            roleHelper.AddUserToRole(userId, PortalRole.Lobbyist);

            db.Users.Find(userId).HouseholdId = db.Households.FirstOrDefault(h => h.Name == "The Lobby").Id;
            Household household = db.Households.Find(householdId);
            db.Households.Remove(household);
            db.SaveChanges();
            await AdminHelper.ReathorizeUserAsync(userId);
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
