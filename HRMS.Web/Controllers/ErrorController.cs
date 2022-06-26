using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRMS.Web.Models;

namespace HRMS.Web.Controllers
{
    [RoutePrefix("Admin/Error")]
    public class ErrorController : Controller
    {

        public ErrorController()
        {
        }

        [AuthorizationPrivilegeFilter(EnablePrivilegeFilter = false)]
        public ActionResult Index(string ErrorCode)
        {
            var page = new PageModel();
            page.Module = "Error";
            page.MenuName = "";
            page.Title = string.Format("Error Page {0}", ErrorCode);
            page.ParentName = null;
            page.ParentTitle = null;
            ViewBag.Page = page;
            var pageError = GlobalFunctions.GetApplicationError();
            if (pageError != null)
            {
                pageError.Exception.Title = pageError.Exception.Title != null ? pageError.Exception.Title : ErrorCode;
            }
            else
            {
                pageError = new ApplicationErrorModel
                {
                    Exception = new ExceptionModel { Title = ErrorCode, Message = string.Empty, StackTrace = string.Empty }
                };
            }
            ViewBag.Error = pageError.Exception;
            GlobalFunctions.SetApplicationError(null);
            return View();
        }
    }
}