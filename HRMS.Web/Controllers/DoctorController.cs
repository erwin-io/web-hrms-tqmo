using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRMS.Web.Models;

namespace HRMS.Web.Controllers
{
    [RoutePrefix("Admin/Doctors")]
    public class DoctorController : Controller
    {

        public DoctorController()
        {
        }

        [HttpGet]
        [Route]
        //
        // GET: /Home/
        [AuthorizationPrivilegeFilter(Pagename = "Doctors", DisplayName = "Patients", EnablePrivilegeFilter = true)]
        public ActionResult Index()
        {
            var page = new PageModel();
            page.MenuName = "Doctors";
            page.Module = "System Setup";
            page.Title = "Doctors";
            ViewBag.Page = page;
            return View();
        }
	}
}