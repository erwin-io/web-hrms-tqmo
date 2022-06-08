using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRMS.Web.Models;

namespace HRMS.Web.Controllers
{
    public class AccountController : Controller
    {
        public AccountController()
        {
        }
        public ActionResult Index()
        {
            var page = new PageModel();
            page.MenuName = "Account";
            page.Module = "Account";
            page.Title = "Account";
            ViewBag.Page = page;
            return View();
        }
        public ActionResult Login()
        {
            var page = new PageModel();
            page.MenuName = "Login";
            page.Module = "Account";
            page.Title = "Login";
            ViewBag.Page = page;
            return View();
        }
        public ActionResult Register()
        {
            var page = new PageModel();
            page.MenuName = "Register";
            page.Module = "Account";
            page.Title = "Register";
            ViewBag.Page = page;
            return View();
        }
        public ActionResult ResetPassword()
        {
            var page = new PageModel();
            page.MenuName = "Reset Password";
            page.Module = "Account";
            page.Title = "Reset Password";
            ViewBag.Page = page;
            return View();
        }
        [HttpGet]
        public JsonResult GetApplicationToken()
        {
            var jsonData = new JsonData<ApplicationTokenModel>();
            try
            {
                jsonData.Data = GlobalFunctions.GetApplicationToken();
                jsonData.Success = true;
            }
            catch (Exception ex)
            {
                jsonData.Success = false;
                jsonData.Message = ex.Message;
            }
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SetApplicationToken(ApplicationTokenModel ApplicationToken)
        {
            var jsonData = new JsonData<ApplicationTokenModel>();
            try
            {
                GlobalFunctions.SetApplicationToken(ApplicationToken);
                jsonData.Data = ApplicationToken;
                jsonData.Success = true;
            }
            catch (Exception ex)
            {
                jsonData.Success = false;
                jsonData.Message = ex.Message;
            }
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetApplicationState()
        {
            var jsonData = new JsonData<ApplicationStateModel>();
            try
            {
                var appState = GlobalFunctions.GetAppState();
                if (appState == null)
                {
                    jsonData.Success = false;
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }

                appState.ApplicationToken = GlobalFunctions.GetApplicationToken();

                if (appState.ApplicationToken == null || string.IsNullOrEmpty(appState.ApplicationToken.RefreshToken))
                {
                    jsonData.Success = false;
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }

                jsonData.Data = appState;
                jsonData.Success = true;
            }
            catch (Exception ex)
            {
                jsonData.Success = false;
                jsonData.Message = ex.Message;
            }
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SetApplicationState(ApplicationStateModel ApplicationState)
        {
            var jsonData = new JsonData<ApplicationStateModel>();
            try
            {
                GlobalFunctions.SetApplicationToken(ApplicationState?.ApplicationToken);
                GlobalFunctions.SetAppStateUserViewAccess(ApplicationState?.UserViewAccess);
                GlobalFunctions.SetAppState(ApplicationState);
                jsonData.Data = ApplicationState;
                jsonData.Success = true;
            }
            catch (Exception ex)
            {
                jsonData.Success = false;
                jsonData.Message = ex.Message;
            }
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}