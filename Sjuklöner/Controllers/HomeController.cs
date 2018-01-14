using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sjuklöner.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            if (User.IsInRole("Assistant"))
            {
                return RedirectToAction("Create", "Claims");
            }
            else if (User.IsInRole("Ombud"))
            {
                return RedirectToAction("Index", "Claims");
            }
            else if (User.IsInRole("AdministrativeOfficial"))
            {
                return RedirectToAction("Index", "Claims");
            }
            else if (User.IsInRole("Admin"))
            {
                return View();
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}