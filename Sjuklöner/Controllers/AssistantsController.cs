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
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace Sjuklöner.Controllers
{
    public class AssistantsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserManager<ApplicationUser> _userManager;
        private RoleStore<IdentityRole> _roleStore;
        private RoleManager<IdentityRole> _roleManager;

        public RoleStore<IdentityRole> RoleStore
        {
            get { return _roleStore; }
        }
        public RoleManager<IdentityRole> RoleManager => _roleManager;
        public UserManager<ApplicationUser> UserManager
        {
            get
            {

                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }
        public AssistantsController()
        {
            _roleStore = new RoleStore<IdentityRole>(db);
            _roleManager = new RoleManager<IdentityRole>(_roleStore);
        }

        // GET: Assistants
        public ActionResult Index()
        {
            var assistantRole = RoleManager.FindByName("Assistant");

            var assistants = db.Users
                .Where(u => u.Roles.FirstOrDefault().RoleId == assistantRole.Id)
                .ToList();

            return View(assistants);
        }

        // GET: Assistants/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assistant assistant = db.Users.Find(id) as Assistant;
            if (assistant == null)
            {
                return HttpNotFound();
            }
            return View(assistant);
        }

        // GET: Assistants/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Assistants/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,LastLogon,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,CompanyName,StreetAddress,Postcode,City,ClearingNumber,AccountNumber,StorageApproval")] Assistant assistant)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(assistant);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(assistant);
        }

        // GET: Assistants/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assistant assistant = db.Users.Find(id) as Assistant;
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
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,LastLogon,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,CompanyName,StreetAddress,Postcode,City,ClearingNumber,AccountNumber,StorageApproval")] Assistant assistant)
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
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assistant assistant = db.Users.Find(id) as Assistant;
            if (assistant == null)
            {
                return HttpNotFound();
            }
            return View(assistant);
        }

        // POST: Assistants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Assistant assistant = db.Users.Find(id) as Assistant;
            db.Users.Remove(assistant);
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
