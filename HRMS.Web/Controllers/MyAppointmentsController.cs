using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRMS.Web.Models;

namespace HRMS.Web.Controllers
{
    [RoutePrefix("MyAppointments")]
    public class MyAppointmentsController : Controller
    {

        public MyAppointmentsController()
        {
        }

        [HttpGet]
        [Route]
        [AuthorizationPrivilegeFilter(Pagename = "MyAppointments", DisplayName = "My Appointments")]
        public ActionResult Index()
        {
            var page = new PageModel();
            page.MenuName = "My Appointments";
            page.Title = "My Appointments";
            ViewBag.Page = page;
            return View();
        }

        [HttpGet]
        [Route("{id}")]
        [AuthorizationPrivilegeFilter(Pagename = "MyAppointments", DisplayName = "My Appointments")]
        public ActionResult Details(string id)
        {
            var page = new PageModel();
            page.MenuName = "My Appointments";
            page.ParentName = "MyAppointments";
            page.ParentTitle = "My Appointments";
            page.Title = "Details";
            ViewBag.Page = page;

            Dictionary<string, object> appSettings = new Dictionary<string, object>();
            appSettings.Add("AppointmentId", id);
            ViewBag.AppSettings = appSettings;
            return View();
        }
    }
}