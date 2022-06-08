using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRMS.Web.Models;

namespace HRMS.Web.Controllers
{
    public class ItemTypeController : Controller
    {

        public ItemTypeController()
        {
        }

        //
        // GET: /Home/
        [AuthorizationPrivilegeFilter(Pagename = "Item Type", DisplayName = "Item Type", EnablePrivilegeFilter = true)]
        public ActionResult Index()
        {
            var page = new PageModel();
            page.MenuName = "Item Type";
            page.Module = "System Setup";
            page.Title = "Item Type";
            ViewBag.Page = page;
            return View();
        }
	}
}