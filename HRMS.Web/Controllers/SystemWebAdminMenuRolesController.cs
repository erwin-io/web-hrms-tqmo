using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRMS.Web.Models;

namespace HRMS.Web.Controllers
{
    public class SystemWebAdminMenuRolesController : Controller
    {

        public SystemWebAdminMenuRolesController()
        {
        }

        //
        // GET: /Home/
        [AuthorizationPrivilegeFilter(Pagename = "System Menu Roles", DisplayName = "System Menu Roles", EnablePrivilegeFilter = true)]
        public ActionResult Index()
        {
            var page = new PageModel();
            page.MenuName = "System Menu Roles";
            page.Module = "System Admin Security";
            page.Title = "System Menu Roles";
            ViewBag.Page = page;
            return View();
        }
	}
}