using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Sjuklöner.Models;

namespace Sjuklöner.Controllers
{
    public class CollectiveAgreementsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CollectiveAgreements
        public ActionResult Index()
        {
            return View(db.CollectiveAgreements.ToList());
        }

        // GET: CollectiveAgreements/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CollectiveAgreement collectiveAgreement = db.CollectiveAgreements.Find(id);
            if (collectiveAgreement == null)
            {
                return HttpNotFound();
            }
            return View(collectiveAgreement);
        }

        // GET: CollectiveAgreements/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CollectiveAgreements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,PerHourUnsocialEvening,PerHourUnsocialNight,PerHourUnsocialWeekend,PerHourUnsocialHoliday,PerHourOnCallWeekday,PerHourOnCallWeekend,PerHourPreparedWeekday,PerHourPreparedWeekend,OvertimeFactor,ExtraHOurFactor")] CollectiveAgreement collectiveAgreement)
        {
            if (ModelState.IsValid)
            {
                db.CollectiveAgreements.Add(collectiveAgreement);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(collectiveAgreement);
        }

        // GET: CollectiveAgreements/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CollectiveAgreement collectiveAgreement = db.CollectiveAgreements.Find(id);
            if (collectiveAgreement == null)
            {
                return HttpNotFound();
            }
            return View(collectiveAgreement);
        }

        // POST: CollectiveAgreements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,PerHourUnsocialEvening,PerHourUnsocialNight,PerHourUnsocialWeekend,PerHourUnsocialHoliday,PerHourOnCallWeekday,PerHourOnCallWeekend,PerHourPreparedWeekday,PerHourPreparedWeekend,OvertimeFactor,ExtraHOurFactor")] CollectiveAgreement collectiveAgreement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(collectiveAgreement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(collectiveAgreement);
        }

        // GET: CollectiveAgreements/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CollectiveAgreement collectiveAgreement = db.CollectiveAgreements.Find(id);
            if (collectiveAgreement == null)
            {
                return HttpNotFound();
            }
            return View(collectiveAgreement);
        }

        // POST: CollectiveAgreements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CollectiveAgreement collectiveAgreement = db.CollectiveAgreements.Find(id);
            db.CollectiveAgreements.Remove(collectiveAgreement);
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
