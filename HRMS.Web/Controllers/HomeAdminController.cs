using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRMS.Web.Models;

namespace HRMS.Web.Controllers
{
    [RoutePrefix("Admin")]
    public class HomeAdminController : Controller
    {

        public HomeAdminController()
        {
        }

        //
        // GET: /Home/
        [HttpGet]
        [Route]
        [AuthorizationPrivilegeFilter(Pagename = "Dashboard", DisplayName = "Dashboard", EnablePrivilegeFilter = true)]
        public ActionResult Index()
        {
            var page = new PageModel();
            page.MenuName = "Dashboard";
            page.Module = "Dashboard";
            page.Title = "Dashboard";
            ViewBag.Page = page;
            //return View();
            return View("~/Views/HomeAdmin/Index.cshtml");
        }
	}
}