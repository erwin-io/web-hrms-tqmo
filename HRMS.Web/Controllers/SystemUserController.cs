using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRMS.Web.Models;

namespace HRMS.Web.Controllers
{
    [RoutePrefix("Admin/SystemUsers")]
    public class SystemUserController : Controller
    {

        public SystemUserController()
        {
        }

        [HttpGet]
        [Route]
        //
        // GET: /Home/
        [AuthorizationPrivilegeFilter(Pagename = "System User", DisplayName = "System User", EnablePrivilegeFilter = true)]
        public ActionResult Index()
        {
            var page = new PageModel();
            page.MenuName = "System User";
            page.Module = "System Admin Security";
            page.Title = "System User";
            ViewBag.Page = page;
            return View();
        }
	}
}