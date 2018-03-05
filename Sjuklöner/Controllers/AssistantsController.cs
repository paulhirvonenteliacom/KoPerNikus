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
using System.Text.RegularExpressions;

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

            bool errorFound = false;
            //Check that the SSN has the correct format
            if (!string.IsNullOrWhiteSpace(assistantCreateVM.AssistantSSN))
            {
                assistantCreateVM.AssistantSSN = assistantCreateVM.AssistantSSN.Trim();
                Regex regex = new Regex(@"^([1-9][0-9]{3})(((0[13578]|1[02])(0[1-9]|[12][0-9]|3[01]))|((0[469]|11)(0[1-9]|[12][0-9]|30))|(02(0[1-9]|[12][0-9])))[-]?\d{4}$");
                Match match = regex.Match(assistantCreateVM.AssistantSSN);
                if (!match.Success)
                {
                    ModelState.AddModelError("AssistantSSN", "Ej giltigt personnummer. Formaten YYYYMMDD-NNNN och YYYYMMDDNNNN är giltiga.");
                    errorFound = true;
                }
            }
            else
            {
                errorFound = true;
            }

            //Check that the assistant is born in the 20th or 21st century
            if (!errorFound)
            {
                if (int.Parse(assistantCreateVM.AssistantSSN.Substring(0, 2)) != 19 && int.Parse(assistantCreateVM.AssistantSSN.Substring(0, 2)) != 20)
                {
                    ModelState.AddModelError("AssistantSSN", "Assistenten måste vara född på 1900- eller 2000-talet.");
                    errorFound = true;
                }
            }

            //Check that the assistant is at least 18 years old and was not born in the future:-)
            if (!errorFound)
            {
                DateTime assistantBirthday = new DateTime(int.Parse(assistantCreateVM.AssistantSSN.Substring(0, 4)), int.Parse(assistantCreateVM.AssistantSSN.Substring(4, 2)), int.Parse(assistantCreateVM.AssistantSSN.Substring(6, 2)));
                if (assistantBirthday.Date > DateTime.Now.Date)
                {
                    ModelState.AddModelError("AssistantSSN", "Födelsedatumet får inte vara senare än idag.");
                    errorFound = true;
                }
                else if (assistantBirthday > DateTime.Now.AddYears(-18))
                {
                    ModelState.AddModelError("AssistantSSN", "Assistenten måste vara minst 18 år.");
                    errorFound = true;
                }
            }

            //Check if there is an assistant with the same SSN already in the company. The same assistant is allowed in another company.
            if (!errorFound)
            {
                var twinAssistant = db.Assistants.Where(a => a.AssistantSSN == assistantCreateVM.AssistantSSN).FirstOrDefault();
                if (twinAssistant != null && twinAssistant.CareCompanyId == currentUser.CareCompanyId)
                {
                    ModelState.AddModelError("AssistantSSN", "Det finns redan en assistent med detta personnummer");
                    errorFound = true;
                }
            }

            if (!errorFound)
            {
                if (assistantCreateVM.AssistantSSN.Length == 12)
                {
                    assistantCreateVM.AssistantSSN = assistantCreateVM.AssistantSSN.Insert(8, "-");
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

            bool errorFound = false;
            //Check that the SSN has the correct format
            if (!string.IsNullOrWhiteSpace(assistantEditVM.AssistantSSN))
            {
                assistantEditVM.AssistantSSN = assistantEditVM.AssistantSSN.Trim();
                Regex regex = new Regex(@"^([1-9][0-9]{3})(((0[13578]|1[02])(0[1-9]|[12][0-9]|3[01]))|((0[469]|11)(0[1-9]|[12][0-9]|30))|(02(0[1-9]|[12][0-9])))[-]?\d{4}$");
                Match match = regex.Match(assistantEditVM.AssistantSSN);
                if (!match.Success)
                {
                    ModelState.AddModelError("AssistantSSN", "Ej giltigt personnummer. Formaten YYYYMMDD-NNNN och YYYYMMDDNNNN är giltiga.");
                    errorFound = true;
                }
            }
            else
            {
                errorFound = true;
            }

            //Check that the assistant is born in the 20th or 21st century
            if (!errorFound)
            {
                if (int.Parse(assistantEditVM.AssistantSSN.Substring(0, 2)) != 19 && int.Parse(assistantEditVM.AssistantSSN.Substring(0, 2)) != 20)
                {
                    ModelState.AddModelError("AssistantSSN", "Assistenten måste vara född på 1900- eller 2000-talet.");
                    errorFound = true;
                }
            }

            //Check that the assistant is at least 18 years old and was not born in the future:-)
            if (!errorFound)
            {
                DateTime assistantBirthday = new DateTime(int.Parse(assistantEditVM.AssistantSSN.Substring(0, 4)), int.Parse(assistantEditVM.AssistantSSN.Substring(4, 2)), int.Parse(assistantEditVM.AssistantSSN.Substring(6, 2)));
                if (assistantBirthday.Date > DateTime.Now.Date)
                {
                    ModelState.AddModelError("AssistantSSN", "Födelsedatumet får inte vara senare än idag.");
                    errorFound = true;
                }
                else if (assistantBirthday > DateTime.Now.AddYears(-18))
                {
                    ModelState.AddModelError("AssistantSSN", "Assistenten måste vara minst 18 år.");
                    errorFound = true;
                }
            }

            //Check if there is an assistant with the same SSN already in the company. The same assistant is allowed in another company.
            if (!errorFound)
            {
                var twinAssistant = db.Assistants.Where(a => a.AssistantSSN == assistantEditVM.AssistantSSN).Where(a => a.Id != assistantEditVM.Id).FirstOrDefault();
                if (twinAssistant != null && twinAssistant.CareCompanyId == currentUser.CareCompanyId)
                {
                    ModelState.AddModelError("AssistantSSN", "Det finns redan en assistent med detta personnummer");
                    errorFound = true;
                }
            }

            if (!errorFound)
            {
                if (assistantEditVM.AssistantSSN.Length == 12)
                {
                    assistantEditVM.AssistantSSN = assistantEditVM.AssistantSSN.Insert(8, "-");
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
