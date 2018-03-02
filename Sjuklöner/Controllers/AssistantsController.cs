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
    [Authorize]
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
            assistantIndexVM.CareCompanyName = db.CareCompanies.Where(c => c.Id == companyId).FirstOrDefault().CompanyName;
            //assistantIndexVM.CareCompanyName = "Smart Assistans"; //Hardcoded for now
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
            Assistant assistant = db.Assistants.Include(a => a.CareCompany).FirstOrDefault(a => a.Id == id);
            if (assistant == null)
            {
                return HttpNotFound();
            }
            return View(assistant);
        }

        // GET: Assistants/Create
        public ActionResult Create()
        {
            AssistantCreateVM assistantCreateVM = new AssistantCreateVM();
            return View(assistantCreateVM);
        }

        // POST: Assistants/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FirstName,LastName,AssistantSSN,Email,PhoneNumber,HourlySalary,HolidayPayRate,PayrollTaxRate,PensionAndInsuranceRate")] AssistantCreateVM assistantCreateVM)
        {
            var currentId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.Where(u => u.Id == currentId).FirstOrDefault();

            //Check that the assistant SSN is 12 or 13 characters. If it is 13 then the 9th shall be a "-". t will always be saved as 13 characters where the 9th is a "-".
            bool errorFound = false;
            if (!string.IsNullOrWhiteSpace(assistantCreateVM.AssistantSSN) && (assistantCreateVM.AssistantSSN.Length == 12 || assistantCreateVM.AssistantSSN.Length == 13))
            {
                if (assistantCreateVM.AssistantSSN.Length == 12 && assistantCreateVM.AssistantSSN.Contains("-"))
                {
                    errorFound = true;
                }
                if (assistantCreateVM.AssistantSSN.Length == 12 && !errorFound)
                {
                    assistantCreateVM.AssistantSSN = assistantCreateVM.AssistantSSN.Insert(8, "-");
                }
                if (assistantCreateVM.AssistantSSN.Length == 13 && assistantCreateVM.AssistantSSN.Substring(8, 1) != "-")
                {
                    errorFound = true;
                }
            }
            else
            {
                errorFound = true;
            }
            if (errorFound)
            {
                ModelState.AddModelError("AssistantSSN", "Ej giltigt personnummer. Formaten YYYYMMDD-NNNN och YYYYMMDDNNNN är giltiga.");
            }

            //Check if there is an assistant with the same SSN already in the company. The same assistant is allowed in another company.
            if (!errorFound)
            {
                var twinAssistant = db.Assistants.Where(a => a.AssistantSSN == assistantCreateVM.AssistantSSN).FirstOrDefault();
                if (twinAssistant != null && twinAssistant.CareCompanyId == currentUser.CareCompanyId)
                {
                    ModelState.AddModelError("AssistantSSN", "Det finns redan en assistent med detta personnummer");
                }
            }

            if (ModelState.IsValid)
            {
                Assistant assistant = new Assistant();
                assistant.FirstName = assistantCreateVM.FirstName;
                assistant.LastName = assistantCreateVM.LastName;
                assistant.AssistantSSN = assistantCreateVM.AssistantSSN;
                assistant.PhoneNumber = assistantCreateVM.PhoneNumber;
                assistant.Email = assistantCreateVM.Email;
                assistant.HourlySalary = assistantCreateVM.HourlySalary;
                assistant.HolidayPayRate = assistantCreateVM.HolidayPayRate;
                assistant.PayrollTaxRate = assistantCreateVM.PayrollTaxRate;
                assistant.PensionAndInsuranceRate = assistantCreateVM.PensionAndInsuranceRate;
                assistant.CareCompanyId = (int)currentUser.CareCompanyId;

                db.Assistants.Add(assistant);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(assistantCreateVM);
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
            AssistantEditVM assistantEditVM = new AssistantEditVM();
            assistantEditVM.FirstName = assistant.FirstName;
            assistantEditVM.LastName = assistant.LastName;
            assistantEditVM.AssistantSSN = assistant.AssistantSSN;
            assistantEditVM.PhoneNumber = assistant.PhoneNumber;
            assistantEditVM.Email = assistant.Email;
            assistantEditVM.HourlySalary = assistant.HourlySalary;
            assistantEditVM.HolidayPayRate = assistant.HolidayPayRate;
            assistantEditVM.PayrollTaxRate = assistant.PayrollTaxRate;
            assistantEditVM.PensionAndInsuranceRate = assistant.PensionAndInsuranceRate;
            assistantEditVM.Id = (int)id;

            return View(assistantEditVM);
        }

        // POST: Assistants/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,AssistantSSN,Email,PhoneNumber,HourlySalary,HolidayPayRate,PayrollTaxRate,PensionAndInsuranceRate")] AssistantEditVM assistantEditVM)
        {
            var currentId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.Where(u => u.Id == currentId).FirstOrDefault();

            //Check that the assistant SSN is 12 or 13 characters. If it is 13 then the 9th shall be a "-". t will always be saved as 13 characters where the 9th is a "-".
            bool errorFound = false;
            if (!string.IsNullOrWhiteSpace(assistantEditVM.AssistantSSN) && (assistantEditVM.AssistantSSN.Length == 12 || assistantEditVM.AssistantSSN.Length == 13))
            {
                if (assistantEditVM.AssistantSSN.Length == 12 && assistantEditVM.AssistantSSN.Contains("-"))
                {
                    errorFound = true;
                }
                if (assistantEditVM.AssistantSSN.Length == 12 && !errorFound)
                {
                    assistantEditVM.AssistantSSN = assistantEditVM.AssistantSSN.Insert(8, "-");
                }
                if (assistantEditVM.AssistantSSN.Length == 13 && assistantEditVM.AssistantSSN.Substring(8, 1) != "-")
                {
                    errorFound = true;
                }
            }
            else
            {
                errorFound = true;
            }
            if (errorFound)
            {
                ModelState.AddModelError("AssistantSSN", "Ej giltigt personnummer. Formaten YYYYMMDD-NNNN och YYYYMMDDNNNN är giltiga.");
            }

            //Check if there is an assistant with the same SSN already in the company. The same assistant is allowed in another company.
            if (!errorFound)
            {
                var twinAssistant = db.Assistants.Where(a => a.AssistantSSN == assistantEditVM.AssistantSSN).Where(a => a.Id != assistantEditVM.Id).FirstOrDefault();
                if (twinAssistant != null && twinAssistant.CareCompanyId == currentUser.CareCompanyId)
                {
                    ModelState.AddModelError("AssistantSSN", "Det finns redan en assistent med detta personnummer");
                }
            }

            if (ModelState.IsValid)
            {
                var assistant = db.Assistants.Where(a => a.Id == assistantEditVM.Id).FirstOrDefault();
                if (assistant != null)
                {
                    assistant.FirstName = assistantEditVM.FirstName;
                    assistant.LastName = assistantEditVM.LastName;
                    assistant.AssistantSSN = assistantEditVM.AssistantSSN;
                    assistant.PhoneNumber = assistantEditVM.PhoneNumber;
                    assistant.Email = assistantEditVM.Email;
                    assistant.HourlySalary = assistantEditVM.HourlySalary;
                    assistant.HolidayPayRate = assistantEditVM.HolidayPayRate;
                    assistant.PayrollTaxRate = assistantEditVM.PayrollTaxRate;
                    assistant.PensionAndInsuranceRate = assistantEditVM.PensionAndInsuranceRate;
                    assistant.CareCompanyId = (int)currentUser.CareCompanyId;
                    db.Entry(assistant).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(assistantEditVM);
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
        public ActionResult DeleteConfirmed(int id, string submitButton)
        {
            if (submitButton == "Bekräfta")
            {
                Assistant assistant = db.Assistants.Find(id);
                db.Assistants.Remove(assistant);
                db.SaveChanges();
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
