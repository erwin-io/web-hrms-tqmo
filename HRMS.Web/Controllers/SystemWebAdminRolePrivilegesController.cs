using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRMS.Web.Models;

namespace HRMS.Web.Controllers
{
    [RoutePrefix("Admin/SystemWebAdminRolePrivileges")]
    public class SystemWebAdminRolePrivilegesController : Controller
    {

        public SystemWebAdminRolePrivilegesController()
        {
        }

        [HttpGet]
        [Route]
        //
        // GET: /Home/
        [AuthorizationPrivilegeFilter(Pagename = "System Role Privileges", DisplayName = "System Role Privileges", EnablePrivilegeFilter = true)]
        public ActionResult Index()
        {
            var page = new PageModel();
            page.MenuName = "System Role Privileges";
            page.Module = "System Admin Security";
            page.Title = "System Role Privileges";
            ViewBag.Page = page;
            return View();
        }
	}
}