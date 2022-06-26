using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRMS.Web.Models;

namespace HRMS.Web.Controllers
{
    [RoutePrefix("Admin/Appointments")]
    public class AppointmentController : Controller
    {

        public AppointmentController()
        {
        }

        //
        // GET: /Home/
        [HttpGet]
        [Route]
        [AuthorizationPrivilegeFilter(Pagename = "Appointments", DisplayName = "Appointments", EnablePrivilegeFilter = true)]
        public ActionResult Index()
        {
            var page = new PageModel();
            page.MenuName = "Appointments";
            page.Module = "Health Records";
            page.Title = "Appointments";
            ViewBag.Page = page;
            return View();
        }
        [HttpGet]
        [Route("{id}")]
        [AuthorizationPrivilegeFilter(Pagename = "Appointments", DisplayName = "Appointments", EnablePrivilegeFilter = true)]
        public ActionResult Details(string id)
        {
            var page = new PageModel();
            page.MenuName = "Appointments";
            page.Module = "Health Records";
            page.ParentName = "Appointments";
            page.ParentTitle = "Appointments";
            page.Title = id;
            ViewBag.Page = page;

            Dictionary<string, object> appSettings = new Dictionary<string, object>();
            appSettings.Add("AppointmentId", id);
            ViewBag.AppSettings = appSettings;
            return View();
        }
    }
}