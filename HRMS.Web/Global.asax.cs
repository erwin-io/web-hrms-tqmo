using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using HRMS.Web.Models;
using HRMS.Web.App_Start;
using System.Configuration;

namespace HRMS.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            GlobalFunctions.ResetVisitors_Online();

            ApplicationSettings.MD5ServicePrividerKey = ConfigurationManager.AppSettings["MD5ServicePrividerKey"];
        }

        protected void Application_End(object sender, EventArgs e)
        {
            //GlobalFunctions.SetCloseLog();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            GlobalFunctions.SetVisitors_Online();
        }

        protected void Session_End(object sender, EventArgs e)
        {
            GlobalFunctions.SetVisitors_Offline();
        }

        protected void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var error = Server.GetLastError();
            var appError = new ApplicationErrorModel();
            appError.Exception = new ExceptionModel() { Message = error.Message, StackTrace = error.StackTrace };
            if (HttpContext.Current.Session["ApplicationError"] != null)
            {
                HttpContext.Current.Session["ApplicationError"] = appError;
            }
            else
            {
                HttpContext.Current.Session.Add("ApplicationError", appError);
            }
        }
    }
}
