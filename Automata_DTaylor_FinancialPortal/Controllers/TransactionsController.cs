using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Automata_DTaylor_FinancialPortal.Enumerations;
using Automata_DTaylor_FinancialPortal.ExtensionMethods;
using Automata_DTaylor_FinancialPortal.Models;
using Microsoft.AspNet.Identity;

namespace Automata_DTaylor_FinancialPortal.Controllers
{
    [RequireHttps]
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        [Authorize(Roles = "HeadOfHouse")]
        public ActionResult Index()
        {
            var transactions = db.Transactions.Include(t => t.BankAccount);
            return View(transactions.ToList());
        }

        // GET: Transactions/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Transaction transaction = db.Transactions.Find(id);
        //    if (transaction == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(transaction);
        //}

        // GET: Transactions/Create
        [Authorize(Roles = "HeadOfHouse")]
        public ActionResult Create()
        {
            var houseId = User.Identity.GetHouseholdId();
            var myAccounts = db.BankAccounts.Where(b => b.HouseholdId == houseId).ToList();
            var myItems = db.BudgetCategoryItems.Where(b => b.BudgetCategory.HouseholdId == houseId).ToList();
            ViewBag.BankAccountId = new SelectList(myAccounts, "Id", "AccountName");
            ViewBag.BudgetCategoryItemId = new SelectList(myItems, "Id", "ItemName");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BankAccountId,BudgetCategoryItemId,Amount,TransactionType,Payee,Memo,Reconciled")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                transaction.CreatedById = User.Identity.GetUserId();
                transaction.Created = DateTimeOffset.UtcNow.ToLocalTime();
                db.Transactions.Add(transaction);
                db.SaveChanges();

                transaction.UpdateAccountBalance();
                transaction.NotifyOnBalanceIssues();

                return RedirectToAction("Dashboard", "Households");
            }

            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "Name", transaction.BankAccountId);
            return View(transaction);
        }

        //POST: Transactions/CustomCreate
        [Authorize(Roles ="HeadOfHouse, Resident")]
        public ActionResult CustomCreate()
        {
            var userId = User.Identity.GetUserId();
            var houseId = db.Users.Find(userId).HouseholdId;
            var house = db.Households.Find(houseId);

            if(!house.HasAccounts())
            {
                //add a gritter error
                return View("Dashboard", "Households");
            }
            else
            {
                return View("Create");
            }
        }

        // GET: Transactions/Edit/5
        [Authorize(Roles = "HeadOfHouse")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            var userId = User.Identity.GetUserId();
            var houseId = db.Users.AsNoTracking().FirstOrDefault(u => u.Id == userId).HouseholdId;
            var myAccounts = db.BankAccounts.Where(a => a.HouseholdId == houseId);
            ViewBag.BankAccountId = new SelectList(myAccounts, "Id", "AccountName", transaction.BankAccountId);

            var myItems = db.BudgetCategories.Where(c => c.HouseholdId == houseId).SelectMany(c => c.BudgetCategoryItems);
            ViewBag.BudgetCategoryItemId = new SelectList(myItems, "Id", "ItemName", transaction.BankAccountId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BankAccountId,TransactionType,Payee,Memo,Created,CreatedById,Amount,BudgetCategoryItemId")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                var oldTransaction = db.Transactions.AsNoTracking().FirstOrDefault(t => t.Id == transaction.Id);
                oldTransaction.RevertAccountBalance();
                db.SaveChanges();

                transaction.Reconciled = true;
                transaction.ReconciledDate = DateTimeOffset.UtcNow.ToLocalTime();
                db.Entry(transaction).State = EntityState.Modified;
                transaction.UpdateAccountBalance();
                db.SaveChanges();

                transaction.NotifyOnBalanceIssues();
                return RedirectToAction("Index");
            }
            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "Name", transaction.BankAccountId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        [Authorize(Roles = "HeadOfHouse")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            transaction.RevertAccountBalance();
            db.Transactions.Remove(transaction);
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
