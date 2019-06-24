using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Automata_DTaylor_FinancialPortal.Models;
using Microsoft.AspNet.Identity;

namespace Automata_DTaylor_FinancialPortal.Controllers
{
    [RequireHttps]
    public class BudgetCategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BudgetCategories
        [Authorize(Roles = "HeadOfHouse")]
        public ActionResult Index()
        {
            var budgetCategories = db.BudgetCategories.Include(b => b.Household);
            return View(budgetCategories.ToList());
        }

        // GET: BudgetCategories/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    BudgetCategory budgetCategory = db.BudgetCategories.Find(id);
        //    if (budgetCategory == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(budgetCategory);
        //}

        // GET: BudgetCategories/Create
        [Authorize(Roles = "HeadOfHouse")]
        public ActionResult Create()
        {
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name");
            return View();
        }

        // POST: BudgetCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BudgetCategoryName,TargetAmount")] BudgetCategory budgetCategory)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                var houseId = user.HouseholdId;
                budgetCategory.HouseholdId = houseId;

                db.BudgetCategories.Add(budgetCategory);
                db.SaveChanges();
                return RedirectToAction("Dashboard", "Households");
            }

            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", budgetCategory.HouseholdId);
            return View(budgetCategory);
        }

        // GET: BudgetCategories/Edit/5
        [Authorize(Roles = "HeadOfHouse")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetCategory budgetCategory = db.BudgetCategories.Find(id);
            if (budgetCategory == null)
            {
                return HttpNotFound();
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", budgetCategory.HouseholdId);
            return View(budgetCategory);
        }

        // POST: BudgetCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,HouseholdId,Name")] BudgetCategory budgetCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(budgetCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", budgetCategory.HouseholdId);
            return View(budgetCategory);
        }

        // GET: BudgetCategories/Delete/5
        //[Authorize(Roles = "HeadOfHouse")]
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    BudgetCategory budgetCategory = db.BudgetCategories.Find(id);
        //    if (budgetCategory == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(budgetCategory);
        //}

        // POST: BudgetCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BudgetCategory budgetCategory = db.BudgetCategories.Find(id);
            db.BudgetCategories.Remove(budgetCategory);
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
