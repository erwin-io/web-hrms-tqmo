using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRMS.Web.Models;

namespace HRMS.Web.Controllers
{
    public class PatientController : Controller
    {

        public PatientController()
        {
        }

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
	}
}