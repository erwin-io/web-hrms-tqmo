using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRMS.Web.Models;

namespace HRMS.Web.Controllers
{
    [RoutePrefix("Admin/Patients")]
    public class PatientController : Controller
    {

        public PatientController()
        {
        }


        [HttpGet]
        [Route]
        //
        // GET: /Home/
        [AuthorizationPrivilegeFilter(Pagename = "Patients", DisplayName = "Patients", EnablePrivilegeFilter = true)]
        public ActionResult Index()
        {
            var page = new PageModel();
            page.MenuName = "Patients";
            page.Module = "System Setup";
            page.Title = "Patients";
            ViewBag.Page = page;
            return View();
        }

        [HttpGet]
        [Route("{id}")]
        [AuthorizationPrivilegeFilter(Pagename = "Patients", DisplayName = "Patients", EnablePrivilegeFilter = true)]
        public ActionResult Details(string id)
        {
            var page = new PageModel();
            page.MenuName = "Patients";
            page.Module = "Health Records";
            page.ParentName = "Patients";
            page.ParentTitle = "Patients";
            page.Title = id;
            ViewBag.Page = page;

            Dictionary<string, object> appSettings = new Dictionary<string, object>();
            appSettings.Add("PatientId", id);
            ViewBag.AppSettings = appSettings;
            return View();
        }
    }
}