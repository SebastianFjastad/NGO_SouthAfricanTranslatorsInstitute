using System;
using System.Web.Mvc;

namespace SATI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (User.IsInRole("Admin"))
                return RedirectToAction("Index", "Members", new { area = "Admin" });

            if (User.IsInRole("Member"))
                return RedirectToAction("Index", "Details", new { area = "Members" });

            throw new InvalidOperationException("Member not in any roles.");
        }
    }
}