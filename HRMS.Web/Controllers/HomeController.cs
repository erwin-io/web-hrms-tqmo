using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRMS.Web.Models;

namespace HRMS.Web.Controllers
{
    public class HomeController : Controller
    {

        public HomeController()
        {
        }

        [HttpGet]
        [Route]
        //
        // GET: /Home/
        [AuthorizationPrivilegeFilter(Pagename = "Dashboard", DisplayName = "Dashboard")]
        public ActionResult Index()
        {
            var page = new PageModel();
            page.MenuName = "Dashboard";
            page.Module = "Dashboard";
            page.Title = "Dashboard";
            ViewBag.Page = page;
            return View();
        }

        [HttpGet]
        [Route("MyDiagnosis")]
        [AuthorizationPrivilegeFilter(Pagename = "My Diagnosis and Treatment", DisplayName = "My Diagnosis and Treatment")]
        public ActionResult MyDiagnosis()
        {
            var page = new PageModel();
            page.MenuName = "My Diagnosis and Treatment";
            page.Title = "My Diagnosis and Treatment";
            ViewBag.Page = page;
            return View("~/Views/Home/MyDiagnosis.cshtml");
        }
    }
}