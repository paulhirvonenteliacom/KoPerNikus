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
    public class WatchLogsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: WatchLogs
        public ActionResult Index()
        {
            return View(db.WatchLogs.ToList());
        }

        public ActionResult Status(string Robot)
        {
            if (String.IsNullOrEmpty(Robot))
            {
                Robot = "A001325";
            }
            var robot = db.WatchLogs.Where(r => r.Robot ==  Robot).FirstOrDefault();
            return Json(robot, JsonRequestBehavior.AllowGet);
        }

        // GET: WatchLogs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WatchLog watchLog = db.WatchLogs.Find(id);
            if (watchLog == null)
            {
                return HttpNotFound();
            }
            return View(watchLog);
        }

        // GET: WatchLogs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WatchLogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,LogCode,LogDate,Robot,LogMsg")] WatchLog watchLog)
        {
            if (ModelState.IsValid)
            {
                db.WatchLogs.Add(watchLog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(watchLog);
        }

        // GET: WatchLogs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WatchLog watchLog = db.WatchLogs.Find(id);
            if (watchLog == null)
            {
                return HttpNotFound();
            }
            return View(watchLog);
        }

        // POST: WatchLogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,LogCode,LogDate,Robot,LogMsg")] WatchLog watchLog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(watchLog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(watchLog);
        }

        // GET: WatchLogs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WatchLog watchLog = db.WatchLogs.Find(id);
            if (watchLog == null)
            {
                return HttpNotFound();
            }
            return View(watchLog);
        }

        // POST: WatchLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WatchLog watchLog = db.WatchLogs.Find(id);
            db.WatchLogs.Remove(watchLog);
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
