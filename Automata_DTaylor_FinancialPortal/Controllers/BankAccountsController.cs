using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Automata_DTaylor_FinancialPortal.Models;
using Automata_DTaylor_FinancialPortal.ExtensionMethods;
using Automata_DTaylor_FinancialPortal.Enumerations;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;

namespace Automata_DTaylor_FinancialPortal.Controllers
{
    [RequireHttps]
    public class BankAccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BankAccounts
        [Authorize(Roles ="HeadOfHouse")]
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var houseId = db.Users.AsNoTracking().FirstOrDefault(u => u.Id == userId).HouseholdId;
            var bankAccounts = db.BankAccounts.Where(b => b.HouseholdId == houseId);
            return View(bankAccounts.ToList());
        }

        // GET: BankAccounts/Details/5
        //[Authorize(Roles = "HeadOfHouse")]
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    BankAccount bankAccount = db.BankAccounts.Find(id);
        //    if (bankAccount == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(bankAccount);
        //}

        // GET: BankAccounts/Create
        [Authorize(Roles = "HeadOfHouse")]
        public ActionResult Create()
        {
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name");
            return View();
        }

        // POST: BankAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AccountName,AccountType,StartingBalance,LowBalanceLevel")] BankAccount bankAccount)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                var houseId = user.HouseholdId;
                bankAccount.HouseholdId = houseId;

                bankAccount.CurrentBalance = bankAccount.StartingBalance;
                db.BankAccounts.Add(bankAccount);
                db.SaveChanges();
                return RedirectToAction("Dashboard", "Households");
            }

            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", bankAccount.HouseholdId);
            return View(bankAccount);
        }

        // GET: BankAccounts/Edit/5
        [Authorize(Roles = "HeadOfHouse")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.BankAccounts.Find(id);
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", bankAccount.HouseholdId);
            return View(bankAccount);
        }

        // POST: BankAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,HouseholdId,AccountName,AccountType,StartingBalance,CurrentBalance,LowBalanceLevel")] BankAccount bankAccount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bankAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", bankAccount.HouseholdId);
            return View(bankAccount);
        }

        // GET: BankAccounts/Delete/5
        //[Authorize(Roles = "HeadOfHouse")]
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    BankAccount bankAccount = db.BankAccounts.Find(id);
        //    if (bankAccount == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(bankAccount);
        //}

        // POST: BankAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BankAccount bankAccount = db.BankAccounts.Find(id);
            db.BankAccounts.Remove(bankAccount);
            db.SaveChanges();
            return RedirectToAction("Index");
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
