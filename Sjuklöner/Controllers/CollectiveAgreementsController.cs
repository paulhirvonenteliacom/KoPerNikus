using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Sjuklöner.Models;
using Sjuklöner.Viewmodels;

namespace Sjuklöner.Controllers
{
    public class CollectiveAgreementsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CollectiveAgreements
        public ActionResult Index()
        {
            CollectiveAgreementIndexVM collectiveAgreementIndexVM = new CollectiveAgreementIndexVM();
            collectiveAgreementIndexVM.NumberOfCollectiveAgreements = 0;

            var collectiveAgreementHeaders = db.CollectiveAgreementHeaders.OrderBy(c => c.Id).ToList();
            collectiveAgreementIndexVM.NumberOfCollectiveAgreements = collectiveAgreementHeaders.Count();
            var collectiveAgreementInfos = db.CollectiveAgreementInfos.OrderBy(c => c.CollectiveAgreementHeaderId).ThenBy(c => c.StartDate).ToList();

            List<CollAgreementHeader> collAgreementHeaders = new List<CollAgreementHeader>();
            collectiveAgreementIndexVM.CollectiveAgreementHeader = collAgreementHeaders;

            foreach (var collectiveAgreementHeader in collectiveAgreementHeaders)
            {
                CollAgreementHeader collAgreementHeader = new CollAgreementHeader();
                collAgreementHeader.Name = collectiveAgreementHeader.Name;
                collAgreementHeader.Counter = collectiveAgreementHeader.Counter;
                collAgreementHeader.Id = collectiveAgreementHeader.Id;
                collectiveAgreementIndexVM.CollectiveAgreementHeader.Add(collAgreementHeader);
            }

            List<CollAgreementInfo> collAgreementInfos = new List<CollAgreementInfo>();
            collectiveAgreementIndexVM.CollectiveAgreementInfo = collAgreementInfos;

            foreach (var collectiveAgreementInfo in collectiveAgreementInfos)
            {
                CollAgreementInfo collAgreementInfo = new CollAgreementInfo();
                collAgreementInfo.Id = collectiveAgreementInfo.Id;
                collAgreementInfo.StartDate = collectiveAgreementInfo.StartDate.ToShortDateString();
                collAgreementInfo.EndDate = collectiveAgreementInfo.EndDate.ToShortDateString();
                collAgreementInfo.PerHourUnsocialEvening = collectiveAgreementInfo.PerHourUnsocialEvening;
                collAgreementInfo.PerHourUnsocialNight = collectiveAgreementInfo.PerHourUnsocialNight;
                collAgreementInfo.PerHourUnsocialWeekend = collectiveAgreementInfo.PerHourUnsocialWeekend;
                collAgreementInfo.PerHourUnsocialHoliday = collectiveAgreementInfo.PerHourUnsocialHoliday;
                collAgreementInfo.PerHourOnCallWeekday = collectiveAgreementInfo.PerHourOnCallWeekday;
                collAgreementInfo.PerHourOnCallWeekend = collectiveAgreementInfo.PerHourOnCallWeekend;
                collectiveAgreementIndexVM.CollectiveAgreementInfo.Add(collAgreementInfo);
            }
            return View(collectiveAgreementIndexVM);
        }

        // GET: CollectiveAgreements/Details/5
        public ActionResult Details(int? id)
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //CollectiveAgreementCreateVM collectiveAgreement = db.CollectiveAgreements.Find(id);
            //if (collectiveAgreement == null)
            //{
            //    return HttpNotFound();
            //}
            return View();
        }

        // GET: CollectiveAgreements/Create
        public ActionResult Create(int? id)
        {
            CollectiveAgreementCreateVM collectiveAgreementCreateVM = new CollectiveAgreementCreateVM();

            if (id == null) //This is a new collective agreement
            {
                collectiveAgreementCreateVM.NewAgreement = true;
            }
            else //This is an existing collective agreement
            {
                collectiveAgreementCreateVM.NewAgreement = false;
                var collectiveAgreementHeader = db.CollectiveAgreementHeaders.Where(c => c.Id == id).FirstOrDefault();
                collectiveAgreementCreateVM.HeaderId = collectiveAgreementHeader.Id;
                collectiveAgreementCreateVM.Name = collectiveAgreementHeader.Name;
            }
            collectiveAgreementCreateVM.StartDate = DateTime.Now.AddDays(1);
            collectiveAgreementCreateVM.EndDate = DateTime.Now.AddDays(1);
            return View(collectiveAgreementCreateVM);
        }

        // POST: CollectiveAgreements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,Name,StartDate,EndDate,PerHourUnsocialEvening,PerHourUnsocialNight,PerHourUnsocialWeekend,PerHourUnsocialHoliday,PerHourOnCallWeekday,PerHourOnCallWeekend,NewAgreement")] CollectiveAgreementCreateVM collectiveAgreement, string submitButton)
        public ActionResult Create(CollectiveAgreementCreateVM collectiveAgreement, string submitButton)
        {
            if (submitButton == "Spara")
            {
                //if (ModelState.IsValid)
                //{
                int latestHeaderId = 1;

                if (collectiveAgreement.NewAgreement == true)
                {
                    CollectiveAgreementHeader collectiveAgreementHeader = new CollectiveAgreementHeader();
                    collectiveAgreementHeader.Name = collectiveAgreement.Name;
                    collectiveAgreementHeader.Counter = 1;
                    db.CollectiveAgreementHeaders.Add(collectiveAgreementHeader);
                    db.SaveChanges();
                    latestHeaderId = db.CollectiveAgreementHeaders.ToList().Last().Id;
                }
                else
                {
                    var header = db.CollectiveAgreementHeaders.Where(c => c.Id == collectiveAgreement.HeaderId).FirstOrDefault();
                    header.Counter++;
                    db.Entry(header).State = EntityState.Modified;
                }
                CollectiveAgreementInfo collectiveAgreementInfo = new CollectiveAgreementInfo();
                collectiveAgreementInfo.StartDate = collectiveAgreement.StartDate;
                collectiveAgreementInfo.EndDate = collectiveAgreement.EndDate;
                collectiveAgreementInfo.PerHourUnsocialEvening = collectiveAgreement.PerHourUnsocialEvening;
                collectiveAgreementInfo.PerHourUnsocialNight = collectiveAgreement.PerHourUnsocialNight;
                collectiveAgreementInfo.PerHourUnsocialWeekend = collectiveAgreement.PerHourUnsocialWeekend;
                collectiveAgreementInfo.PerHourUnsocialHoliday = collectiveAgreement.PerHourUnsocialHoliday;
                collectiveAgreementInfo.PerHourOnCallWeekday = collectiveAgreement.PerHourOnCallWeekday;
                collectiveAgreementInfo.PerHourOnCallWeekend = collectiveAgreement.PerHourOnCallWeekend;

                if (collectiveAgreement.NewAgreement == true)
                {
                    collectiveAgreementInfo.CollectiveAgreementHeaderId = latestHeaderId;
                }
                else
                {
                    collectiveAgreementInfo.CollectiveAgreementHeaderId = collectiveAgreement.HeaderId;
                }
                db.CollectiveAgreementInfos.Add(collectiveAgreementInfo);
                db.SaveChanges();
                //}
            }
            return RedirectToAction("Index");
        }

        // GET: CollectiveAgreements/Edit/5
        public ActionResult Edit(string type, int? headerId, int? infoId)
        {
            if (headerId != null)
            {
                CollectiveAgreementEditVM collectiveAgreementEditVM = new CollectiveAgreementEditVM();
                collectiveAgreementEditVM.Type = type;

                CollAgreementHeader collAgreementHeader = new CollAgreementHeader();
                collectiveAgreementEditVM.CollAgreementHeader = collAgreementHeader;
                var collectiveAgreementHeader = db.CollectiveAgreementHeaders.Where(c => c.Id == headerId).FirstOrDefault();
                collectiveAgreementEditVM.CollAgreementHeader.Id = collectiveAgreementHeader.Id;
                collectiveAgreementEditVM.CollAgreementHeader.Name = collectiveAgreementHeader.Name;
                collectiveAgreementEditVM.CollAgreementHeader.Counter = collectiveAgreementHeader.Counter;

                if (type == "header")
                {
                    List<CollAgreementInfo> collAgreementInfos = new List<CollAgreementInfo>();
                    collectiveAgreementEditVM.CollAgreementInfo = collAgreementInfos;
                    var collectiveAgreementInfos = db.CollectiveAgreementInfos.Where(c => c.CollectiveAgreementHeaderId == collectiveAgreementHeader.Id).ToList();
                    foreach (var collectiveAgreementInfo in collectiveAgreementInfos)
                    {
                        CollAgreementInfo collAgreementInfo = new CollAgreementInfo();
                        collAgreementInfo.Id = collectiveAgreementInfo.Id;
                        collAgreementInfo.StartDate = collectiveAgreementInfo.StartDate.ToShortDateString();
                        collAgreementInfo.EndDate = collectiveAgreementInfo.EndDate.ToShortDateString();
                        collAgreementInfo.PerHourUnsocialEvening = collectiveAgreementInfo.PerHourUnsocialEvening;
                        collAgreementInfo.PerHourUnsocialNight = collectiveAgreementInfo.PerHourUnsocialNight;
                        collAgreementInfo.PerHourUnsocialWeekend = collectiveAgreementInfo.PerHourUnsocialWeekend;
                        collAgreementInfo.PerHourUnsocialHoliday = collectiveAgreementInfo.PerHourUnsocialHoliday;
                        collAgreementInfo.PerHourOnCallWeekday = collectiveAgreementInfo.PerHourOnCallWeekday;
                        collAgreementInfo.PerHourOnCallWeekend = collectiveAgreementInfo.PerHourOnCallWeekend;
                        collectiveAgreementEditVM.CollAgreementInfo.Add(collAgreementInfo);
                    }
                }
                else if (type == "info" && infoId != null)
                {
                    List<CollAgreementInfo> collAgreementInfos = new List<CollAgreementInfo>();
                    collectiveAgreementEditVM.CollAgreementInfo = collAgreementInfos;
                    var collectiveAgreementInfos = db.CollectiveAgreementInfos.Where(c => c.Id == infoId).FirstOrDefault();
                    CollAgreementInfo collAgreementInfo = new CollAgreementInfo();
                    collAgreementInfo.Id = collectiveAgreementInfos.Id;
                    collAgreementInfo.StartDate = collectiveAgreementInfos.StartDate.ToShortDateString();
                    collAgreementInfo.EndDate = collectiveAgreementInfos.EndDate.ToShortDateString();
                    collAgreementInfo.PerHourUnsocialEvening = collectiveAgreementInfos.PerHourUnsocialEvening;
                    collAgreementInfo.PerHourUnsocialNight = collectiveAgreementInfos.PerHourUnsocialNight;
                    collAgreementInfo.PerHourUnsocialWeekend = collectiveAgreementInfos.PerHourUnsocialWeekend;
                    collAgreementInfo.PerHourUnsocialHoliday = collectiveAgreementInfos.PerHourUnsocialHoliday;
                    collAgreementInfo.PerHourOnCallWeekday = collectiveAgreementInfos.PerHourOnCallWeekday;
                    collAgreementInfo.PerHourOnCallWeekend = collectiveAgreementInfos.PerHourOnCallWeekend;
                    collectiveAgreementEditVM.CollAgreementInfo.Add(collAgreementInfo);
                }
                return View(collectiveAgreementEditVM);
            }
            else
            {
                return View();
            }
        }

        // POST: CollectiveAgreements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CollectiveAgreementEditVM collectiveAgreementEditVM, string submitButton)
        {
            if (submitButton == "Spara")
            {
                if (ModelState.IsValid)
                {
                    if (collectiveAgreementEditVM.Type == "header")
                    {
                        var headerId = collectiveAgreementEditVM.CollAgreementHeader.Id;
                        var header = db.CollectiveAgreementHeaders.Where(c => c.Id == headerId).FirstOrDefault();

                        header.Name = collectiveAgreementEditVM.CollAgreementHeader.Name;
                        db.Entry(header).State = EntityState.Modified;

                        //Remove the existing collective agreement info records
                        var collAgreementInfos = db.CollectiveAgreementInfos.Where(c => c.CollectiveAgreementHeaderId == headerId).ToList();
                        db.CollectiveAgreementInfos.RemoveRange(collAgreementInfos);

                        List<CollectiveAgreementInfo> updatedCollAgreementInfos = new List<CollectiveAgreementInfo>();

                        //Add updated collective agreement info records
                        for (int i = 0; i < collectiveAgreementEditVM.CollAgreementInfo.Count(); i++)
                        {
                            CollectiveAgreementInfo collectiveAgreementInfo = new CollectiveAgreementInfo();
                            collectiveAgreementInfo.CollectiveAgreementHeaderId = headerId;
                            collectiveAgreementInfo.StartDate = Convert.ToDateTime(collectiveAgreementEditVM.CollAgreementInfo[i].StartDate);
                            collectiveAgreementInfo.EndDate = Convert.ToDateTime(collectiveAgreementEditVM.CollAgreementInfo[i].EndDate);
                            collectiveAgreementInfo.PerHourUnsocialEvening = collectiveAgreementEditVM.CollAgreementInfo[i].PerHourUnsocialEvening;
                            collectiveAgreementInfo.PerHourUnsocialNight = collectiveAgreementEditVM.CollAgreementInfo[i].PerHourUnsocialNight;
                            collectiveAgreementInfo.PerHourUnsocialWeekend = collectiveAgreementEditVM.CollAgreementInfo[i].PerHourUnsocialWeekend;
                            collectiveAgreementInfo.PerHourUnsocialHoliday = collectiveAgreementEditVM.CollAgreementInfo[i].PerHourUnsocialHoliday;
                            collectiveAgreementInfo.PerHourOnCallWeekday = collectiveAgreementEditVM.CollAgreementInfo[i].PerHourOnCallWeekday;
                            collectiveAgreementInfo.PerHourOnCallWeekend = collectiveAgreementEditVM.CollAgreementInfo[i].PerHourOnCallWeekend;
                            updatedCollAgreementInfos.Add(collectiveAgreementInfo);
                        }
                        db.CollectiveAgreementInfos.AddRange(updatedCollAgreementInfos);
                    }
                    else if (collectiveAgreementEditVM.Type == "info")
                    {
                        int infoId = collectiveAgreementEditVM.CollAgreementInfo[0].Id;
                        var collAgreementInfo = db.CollectiveAgreementInfos.Where(c => c.Id == infoId).FirstOrDefault();
                        db.Entry(collAgreementInfo).State = EntityState.Modified;
                        collAgreementInfo.StartDate = Convert.ToDateTime(collectiveAgreementEditVM.CollAgreementInfo[0].StartDate);
                        collAgreementInfo.EndDate = Convert.ToDateTime(collectiveAgreementEditVM.CollAgreementInfo[0].EndDate);
                        collAgreementInfo.PerHourUnsocialEvening = collectiveAgreementEditVM.CollAgreementInfo[0].PerHourUnsocialEvening;
                        collAgreementInfo.PerHourUnsocialNight = collectiveAgreementEditVM.CollAgreementInfo[0].PerHourUnsocialNight;
                        collAgreementInfo.PerHourUnsocialWeekend = collectiveAgreementEditVM.CollAgreementInfo[0].PerHourUnsocialWeekend;
                        collAgreementInfo.PerHourUnsocialHoliday = collectiveAgreementEditVM.CollAgreementInfo[0].PerHourUnsocialHoliday;
                        collAgreementInfo.PerHourOnCallWeekday = collectiveAgreementEditVM.CollAgreementInfo[0].PerHourOnCallWeekday;
                        collAgreementInfo.PerHourOnCallWeekend = collectiveAgreementEditVM.CollAgreementInfo[0].PerHourOnCallWeekend;
                    }
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        // GET: CollectiveAgreements/Delete/5
        public ActionResult Delete(string type, int? headerId, int? infoId)
        {
            if (headerId != null)
            {
                CollectiveAgreementDeleteVM collectiveAgreementDeleteVM = new CollectiveAgreementDeleteVM();
                collectiveAgreementDeleteVM.Type = type;

                CollAgreementHeader collAgreementHeader = new CollAgreementHeader();
                collectiveAgreementDeleteVM.CollAgreementHeader = collAgreementHeader;
                var collectiveAgreementHeader = db.CollectiveAgreementHeaders.Where(c => c.Id == headerId).FirstOrDefault();
                collectiveAgreementDeleteVM.CollAgreementHeader.Id = collectiveAgreementHeader.Id;
                collectiveAgreementDeleteVM.CollAgreementHeader.Name = collectiveAgreementHeader.Name;
                collectiveAgreementDeleteVM.CollAgreementHeader.Counter = collectiveAgreementHeader.Counter;

                if (type == "header")
                {
                    List<CollAgreementInfo> collAgreementInfos = new List<CollAgreementInfo>();
                    collectiveAgreementDeleteVM.CollAgreementInfo = collAgreementInfos;
                    var collectiveAgreementInfos = db.CollectiveAgreementInfos.Where(c => c.CollectiveAgreementHeaderId == collectiveAgreementHeader.Id).ToList();
                    foreach (var collectiveAgreementInfo in collectiveAgreementInfos)
                    {
                        CollAgreementInfo collAgreementInfo = new CollAgreementInfo();
                        collAgreementInfo.Id = collectiveAgreementInfo.Id;
                        collAgreementInfo.StartDate = collectiveAgreementInfo.StartDate.ToShortDateString();
                        collAgreementInfo.EndDate = collectiveAgreementInfo.EndDate.ToShortDateString();
                        collAgreementInfo.PerHourUnsocialEvening = collectiveAgreementInfo.PerHourUnsocialEvening;
                        collAgreementInfo.PerHourUnsocialNight = collectiveAgreementInfo.PerHourUnsocialNight;
                        collAgreementInfo.PerHourUnsocialWeekend = collectiveAgreementInfo.PerHourUnsocialWeekend;
                        collAgreementInfo.PerHourUnsocialHoliday = collectiveAgreementInfo.PerHourUnsocialHoliday;
                        collAgreementInfo.PerHourOnCallWeekday = collectiveAgreementInfo.PerHourOnCallWeekday;
                        collAgreementInfo.PerHourOnCallWeekend = collectiveAgreementInfo.PerHourOnCallWeekend;
                        collectiveAgreementDeleteVM.CollAgreementInfo.Add(collAgreementInfo);
                    }
                }
                else if (type == "info" && infoId != null)
                {
                    List<CollAgreementInfo> collAgreementInfos = new List<CollAgreementInfo>();
                    collectiveAgreementDeleteVM.CollAgreementInfo = collAgreementInfos;
                    var collectiveAgreementInfos = db.CollectiveAgreementInfos.Where(c => c.Id == infoId).FirstOrDefault();
                    CollAgreementInfo collAgreementInfo = new CollAgreementInfo();
                    collAgreementInfo.Id = collectiveAgreementInfos.Id;
                    collAgreementInfo.StartDate = collectiveAgreementInfos.StartDate.ToShortDateString();
                    collAgreementInfo.EndDate = collectiveAgreementInfos.EndDate.ToShortDateString();
                    collAgreementInfo.PerHourUnsocialEvening = collectiveAgreementInfos.PerHourUnsocialEvening;
                    collAgreementInfo.PerHourUnsocialNight = collectiveAgreementInfos.PerHourUnsocialNight;
                    collAgreementInfo.PerHourUnsocialWeekend = collectiveAgreementInfos.PerHourUnsocialWeekend;
                    collAgreementInfo.PerHourUnsocialHoliday = collectiveAgreementInfos.PerHourUnsocialHoliday;
                    collAgreementInfo.PerHourOnCallWeekday = collectiveAgreementInfos.PerHourOnCallWeekday;
                    collAgreementInfo.PerHourOnCallWeekend = collectiveAgreementInfos.PerHourOnCallWeekend;
                    collectiveAgreementDeleteVM.CollAgreementInfo.Add(collAgreementInfo);
                }
                return View(collectiveAgreementDeleteVM);
            }
            else
            {
                return View();
            }
        }

        // POST: CollectiveAgreements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(CollectiveAgreementDeleteVM collectiveAgreementDeleteVM, string submitButton)
        {
            if (submitButton == "Bekräfta")
            {
                if (ModelState.IsValid)
                {
                    var header = db.CollectiveAgreementHeaders.Where(c => c.Id == collectiveAgreementDeleteVM.CollAgreementHeader.Id).FirstOrDefault();
                    if (collectiveAgreementDeleteVM.Type == "header")
                    {
                        //Remove the collective agreement header
                        db.CollectiveAgreementHeaders.Remove(header);

                        //Remove the collective agreement info records
                        var collAgreementInfos = db.CollectiveAgreementInfos.Where(c => c.CollectiveAgreementHeaderId == collectiveAgreementDeleteVM.CollAgreementHeader.Id).ToList();
                        db.CollectiveAgreementInfos.RemoveRange(collAgreementInfos);
                    }
                    else if (collectiveAgreementDeleteVM.Type == "info")
                    {
                        int infoId = collectiveAgreementDeleteVM.CollAgreementInfo[0].Id;
                        var collAgreementInfo = db.CollectiveAgreementInfos.Where(c => c.Id == infoId).FirstOrDefault();
                        db.CollectiveAgreementInfos.Remove(collAgreementInfo);
                        header.Counter = header.Counter - 1;
                        db.Entry(header).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }
            }
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
