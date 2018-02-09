using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Sjuklöner.Models;
using Microsoft.AspNet.Identity;
using Sjuklöner.Viewmodels;

namespace Sjuklöner.Controllers
{
    public class AssistantsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Assistants
        [Authorize(Roles = "Ombud")]
        public ActionResult Index()
        {
            var currentId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.Where(u => u.Id == currentId).FirstOrDefault();

            var companyId = currentUser.CareCompanyId;

            var assistants = db.Assistants.Where(a => a.CareCompanyId == companyId).OrderBy(a => a.LastName).ToList();

            AssistantIndexVM assistantIndexVM = new AssistantIndexVM();
            assistantIndexVM.CareCompanyName = "Smart Assistans"; //Hardcoded for now
            assistantIndexVM.AssistantList = assistants;

            return View(assistantIndexVM);
        }

        // GET: Assistants/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assistant assistant = db.Assistants.Find(id);
            if (assistant == null)
            {
                return HttpNotFound();
            }
            return View(assistant);
        }

        // GET: Assistants/Create
        public ActionResult Create()
        {
            var currentId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.Where(u => u.Id == currentId).FirstOrDefault();

            Assistant assistant = new Assistant();
            assistant.CareCompanyId = (int)currentUser.CareCompanyId;
            return View(assistant);
        }

        // POST: Assistants/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CareCompanyId,FirstName,LastName,AssistantSSN,Email,PhoneNumber,HourlySalary,HolidayPayRate,PayrollTaxRate,InsuranceRate,PensionRate")] Assistant assistant)
        {
            ModelState.Remove(nameof(Assistant.Id));
            if (ModelState.IsValid)
            {
                db.Assistants.Add(assistant);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(assistant);
        }

        // GET: Assistants/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assistant assistant = db.Assistants.Find(id);
            if (assistant == null)
            {
                return HttpNotFound();
            }
            return View(assistant);
        }

        // POST: Assistants/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CareCompanyId,FirstName,LastName,AssistantSSN,Email,PhoneNumber,HourlySalary,HolidayPayRate,PayrollTaxRate,InsuranceRate,PensionRate")] Assistant assistant)
        {
            if (ModelState.IsValid)
            {
                db.Entry(assistant).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(assistant);
        }

        // GET: Assistants/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assistant assistant = db.Assistants.Find(id);
            if (assistant == null)
            {
                return HttpNotFound();
            }
            return View(assistant);
        }

        // POST: Assistants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Assistant assistant = db.Assistants.Find(id);
            db.Assistants.Remove(assistant);
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
