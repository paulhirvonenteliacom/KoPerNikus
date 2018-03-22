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
using static Sjuklöner.Viewmodels.CollectiveAgreementEditVM;

namespace Sjuklöner.Controllers
{
    [Authorize]
    public class CollectiveAgreementsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CollectiveAgreements
        [Authorize(Roles = "Admin, AdministrativeOfficial")]
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
            //Check if the time period overlaps with an existing time period for the same collective agreement
            if (!collectiveAgreement.NewAgreement)
            {
                var collectiveAgreementInfos = db.CollectiveAgreementInfos.Where(c => c.CollectiveAgreementHeaderId == collectiveAgreement.HeaderId).ToList();
                bool noOverlapFound = true;
                int idx = 0;
                do
                {
                    if (collectiveAgreement.StartDate >= collectiveAgreementInfos[idx].StartDate && collectiveAgreement.StartDate <= collectiveAgreementInfos[idx].EndDate)
                    {
                        ModelState.AddModelError("StartDate", "Datumet överlappar med en existerande period.");
                        noOverlapFound = false;
                    }
                    idx++;

                } while (noOverlapFound && idx < collectiveAgreementInfos.Count());

                noOverlapFound = true;
                idx = 0;
                do
                {
                    if (collectiveAgreement.EndDate >= collectiveAgreementInfos[idx].StartDate && collectiveAgreement.EndDate <= collectiveAgreementInfos[idx].EndDate)
                    {
                        ModelState.AddModelError("EndDate", "Datumet överlappar med en existerande period.");
                        noOverlapFound = false;
                    }
                    idx++;

                } while (noOverlapFound && idx < collectiveAgreementInfos.Count());
            }
            //Check if a collective agreement with the same name already exists
            if (collectiveAgreement.NewAgreement)
            {
                if (db.CollectiveAgreementHeaders.Where(c => c.Name == collectiveAgreement.Name).FirstOrDefault() != null)
                {
                    ModelState.AddModelError("Name", "Det finns redan ett avtal med samma namn.");
                }
            }
            //Check that the last date of the agreement is equal to or greater than the first date
            if (collectiveAgreement.StartDate.Date > collectiveAgreement.EndDate.Date)
            {
                ModelState.AddModelError("EndDate", "Kollektivavtalets slutdatum får inte vara tidigare än avtalets startdatum.");
            }
            if (ModelState.IsValid)
            {
                if (submitButton == "Spara")
                {
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
                }
            }
            else
            {
                return View(collectiveAgreement);
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

                CollAgreementHeaderForVM collAgreementHeader = new CollAgreementHeaderForVM();
                collectiveAgreementEditVM.CollAgreementHeader = collAgreementHeader;
                var collectiveAgreementHeader = db.CollectiveAgreementHeaders.Where(c => c.Id == headerId).FirstOrDefault();
                collectiveAgreementEditVM.CollAgreementHeader.Id = collectiveAgreementHeader.Id;
                collectiveAgreementEditVM.CollAgreementHeader.Name = collectiveAgreementHeader.Name;
                collectiveAgreementEditVM.CollAgreementHeader.Counter = collectiveAgreementHeader.Counter;

                if (type == "header")
                {
                    List<CollAgreementInfoForVM> collAgreementInfos = new List<CollAgreementInfoForVM>();
                    collectiveAgreementEditVM.CollAgreementInfo = collAgreementInfos;
                    var collectiveAgreementInfos = db.CollectiveAgreementInfos.Where(c => c.CollectiveAgreementHeaderId == collectiveAgreementHeader.Id).ToList();
                    foreach (var collectiveAgreementInfo in collectiveAgreementInfos)
                    {
                        CollAgreementInfoForVM collAgreementInfo = new CollAgreementInfoForVM();
                        collAgreementInfo.Id = collectiveAgreementInfo.Id;
                        collAgreementInfo.StartDate = collectiveAgreementInfo.StartDate;
                        collAgreementInfo.EndDate = collectiveAgreementInfo.EndDate;
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
                    List<CollAgreementInfoForVM> collAgreementInfos = new List<CollAgreementInfoForVM>();
                    collectiveAgreementEditVM.CollAgreementInfo = collAgreementInfos;
                    var collectiveAgreementInfos = db.CollectiveAgreementInfos.Where(c => c.Id == infoId).FirstOrDefault();
                    CollAgreementInfoForVM collAgreementInfo = new CollAgreementInfoForVM();
                    collAgreementInfo.Id = collectiveAgreementInfos.Id;
                    collAgreementInfo.StartDate = collectiveAgreementInfos.StartDate;
                    collAgreementInfo.EndDate = collectiveAgreementInfos.EndDate;
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
            //Check if a collective agreement with the same name already exists
            if (db.CollectiveAgreementHeaders.Where(c => c.Name == collectiveAgreementEditVM.CollAgreementHeader.Name).Where(c => c.Id != collectiveAgreementEditVM.CollAgreementHeader.Id).FirstOrDefault() != null)
            {
                ModelState.AddModelError("CollagreementHeader.Name", "Det finns redan ett avtal med samma namn.");
            }
            //Check that the last date of the agreement is equal to or greater than the first date
            for (int i = 0; i < collectiveAgreementEditVM.CollAgreementInfo.Count(); i++)
            {
                if (collectiveAgreementEditVM.CollAgreementInfo[i].StartDate.Date > collectiveAgreementEditVM.CollAgreementInfo[i].EndDate.Date)
                {
                    ModelState.AddModelError("CollAgreementInfo[" + i.ToString() + "].EndDate", "Kollektivavtalets slutdatum får inte vara tidigare än avtalets startdatum.");
                }
            }
            //Check if there is an overlap between time periods for the same collective agreement for the case where several time periods may have been edited
            if (collectiveAgreementEditVM.CollAgreementInfo.Count() > 1)
            {
                //Check start date for overlaps
                bool noOverlapFound = true;
                int idx = 1;
                int startIdx = 0;
                int stopIdx = collectiveAgreementEditVM.CollAgreementInfo.Count();
                do
                {
                    do
                    {
                        if (collectiveAgreementEditVM.CollAgreementInfo[startIdx].StartDate >= collectiveAgreementEditVM.CollAgreementInfo[idx].StartDate && collectiveAgreementEditVM.CollAgreementInfo[startIdx].StartDate <= collectiveAgreementEditVM.CollAgreementInfo[idx].EndDate)
                        {
                            ModelState.AddModelError("CollAgreementInfo[" + startIdx.ToString() + "].StartDate", "Datumet överlappar med en existerande period.");
                            noOverlapFound = false;
                        }
                        idx++;
                    } while (noOverlapFound && idx < stopIdx);
                    startIdx++;
                    idx = startIdx + 1;
                } while (noOverlapFound && idx < stopIdx);

                //Check end date for overlaps
                noOverlapFound = true;
                idx = 1;
                startIdx = 0;
                do
                {
                    do
                    {
                        if (collectiveAgreementEditVM.CollAgreementInfo[startIdx].EndDate >= collectiveAgreementEditVM.CollAgreementInfo[idx].StartDate && collectiveAgreementEditVM.CollAgreementInfo[startIdx].EndDate <= collectiveAgreementEditVM.CollAgreementInfo[idx].EndDate)
                        {
                            ModelState.AddModelError("CollAgreementInfo[" + startIdx.ToString() + "].EndDate", "Datumet överlappar med en existerande period.");
                            noOverlapFound = false;
                        }
                        idx++;
                    } while (noOverlapFound && idx < stopIdx);
                    startIdx++;
                    idx = startIdx + 1;
                } while (noOverlapFound && idx < stopIdx);
            }
            else if (collectiveAgreementEditVM.CollAgreementInfo.Count() == 1 && collectiveAgreementEditVM.CollAgreementHeader.Counter > 1)
            {
                int collAgreementInfoId = collectiveAgreementEditVM.CollAgreementInfo[0].Id;
                var collectiveAgreementInfos = db.CollectiveAgreementInfos.Where(c => c.CollectiveAgreementHeaderId == collectiveAgreementEditVM.CollAgreementHeader.Id).Where(c => c.Id != collAgreementInfoId).ToList();
                //Check start date for overlaps for the case where only one time period has been edited
                bool noOverlapFound = true;
                int idx = 0;
                int stopIdx = collectiveAgreementInfos.Count();
                do
                {
                    if (collectiveAgreementEditVM.CollAgreementInfo[0].StartDate >= collectiveAgreementInfos[idx].StartDate && collectiveAgreementEditVM.CollAgreementInfo[0].StartDate <= collectiveAgreementInfos[idx].EndDate)
                    {
                        ModelState.AddModelError("CollAgreementInfo[0].StartDate", "Datumet överlappar med en existerande period.");
                        noOverlapFound = false;
                    }
                    idx++;
                } while (noOverlapFound && idx < stopIdx);

                noOverlapFound = true;
                idx = 0;
                do
                {
                    if (collectiveAgreementEditVM.CollAgreementInfo[0].EndDate >= collectiveAgreementInfos[idx].StartDate && collectiveAgreementEditVM.CollAgreementInfo[0].EndDate <= collectiveAgreementInfos[idx].EndDate)
                    {
                        ModelState.AddModelError("CollAgreementInfo[0].EndDate", "Datumet överlappar med en existerande period.");
                        noOverlapFound = false;
                    }
                    idx++;
                } while (noOverlapFound && idx < stopIdx);
            }

            if (ModelState.IsValid)
            {
                if (submitButton == "Spara")
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
                            collectiveAgreementInfo.StartDate = Convert.ToDateTime(collectiveAgreementEditVM.CollAgreementInfo[i].StartDate).Date;
                            collectiveAgreementInfo.EndDate = Convert.ToDateTime(collectiveAgreementEditVM.CollAgreementInfo[i].EndDate).Date;
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
                        var headerId = collectiveAgreementEditVM.CollAgreementHeader.Id;
                        var header = db.CollectiveAgreementHeaders.Where(c => c.Id == headerId).FirstOrDefault();

                        header.Name = collectiveAgreementEditVM.CollAgreementHeader.Name;
                        db.Entry(header).State = EntityState.Modified;

                        int infoId = collectiveAgreementEditVM.CollAgreementInfo[0].Id;
                        var collAgreementInfo = db.CollectiveAgreementInfos.Where(c => c.Id == infoId).FirstOrDefault();
                        db.Entry(collAgreementInfo).State = EntityState.Modified;
                        collAgreementInfo.StartDate = Convert.ToDateTime(collectiveAgreementEditVM.CollAgreementInfo[0].StartDate).Date;
                        collAgreementInfo.EndDate = Convert.ToDateTime(collectiveAgreementEditVM.CollAgreementInfo[0].EndDate).Date;
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
            else
            {
                return View(collectiveAgreementEditVM);
            }
            return RedirectToAction("Index");
        }

        // GET: CollectiveAgreements/Delete/5
        public ActionResult Delete(string type, int? headerId, int? infoId)
        {
            // It should not be possible to delete the last "Kollektivavtal"  
            if (db.CollectiveAgreementHeaders.Count() < 2  && type == "header") {
                return RedirectToAction("Index");
            }

            if (headerId != null)
            {
                CollectiveAgreementDeleteVM collectiveAgreementDeleteVM = new CollectiveAgreementDeleteVM();
                collectiveAgreementDeleteVM.Type = type;

                CollAgreementHeader collAgreementHeader = new CollAgreementHeader();
                collectiveAgreementDeleteVM.CollAgreementHeader = collAgreementHeader;
                var collectiveAgreementHeader = db.CollectiveAgreementHeaders.Where(c => c.Id == headerId).FirstOrDefault();

                // It should not be possible to delete the last Time Period in a "Kollektivavtal"  
                if (collectiveAgreementHeader.Counter < 2 && type == "info")
                {
                    return RedirectToAction("Index");
                }

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
