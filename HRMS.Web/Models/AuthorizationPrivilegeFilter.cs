using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRMS.Web.Providers;
using Newtonsoft.Json;

namespace HRMS.Web.Models
{
    public class AuthorizationPrivilegeFilter: ActionFilterAttribute
    {
        public string Pagename { get; set; }
        public string DisplayName { get; set; }
        public bool EnablePrivilegeFilter { get; set; }

        public AuthorizationPrivilegeFilter()
        {
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var appState = GlobalFunctions.GetAppState();
            var user = appState != null ? appState.User : null;
            var appStateUserView = GlobalFunctions.GetAppStateUserViewAccess();
            var menuroles = appStateUserView != null ? appStateUserView : new List<UserViewAccess>();
            //var menuroles = appState.UserViewAccess != null ? appState.UserViewAccess : new List<UserViewAccess>();
            var action = new ApplicationActionExcecutingContextModel();

            var isAllowed = false;

            var appError = new ApplicationErrorModel();

            if (EnablePrivilegeFilter && !(Pagename.Equals("Dashboard") && appState.User != null && appState.User.IsWebAdminGuestUser))
            {
                if (user != null)
                {
                    if (menuroles.Any())
                    {
                        action = GlobalFunctions.GetActionExecutingContext();
                        if (action != null)
                        {
                            isAllowed = menuroles.Any(x => x.PageName.Equals(Pagename));
                            //Redirect to warning no access page
                            if (!isAllowed)
                            {
                                appError = new ApplicationErrorModel();
                                appError.Exception = new ExceptionModel() 
                                { 
                                    Message = string.Format("Sorry your not allowed to access this {0}", DisplayName), 
                                    Title = "Unauthorized access", 
                                    StackTrace = string.Empty 
                                };
                                filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary { { "controller", "Error" }, { "action", "Index" }, { "ErrorCode", "NotAllowed" } });
                            }
                        }
                        else
                        {
                            filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary { { "controller", "Account" }, { "action", "Login" } });
                        }
                    }
                    else
                    {
                        appError.Exception = new ExceptionModel() 
                        {
                            Message = string.Format("Sorry your not allowed to access this site"), 
                            Title = "Unauthorized access",
                            StackTrace = "Your Current Group(s) are currently blocked" 
                        };
                        filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary { { "controller", "Error" }, { "action", "Index" }, { "ErrorCode", "NotAllowed" } });
                    }
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary { { "controller", "Account" }, { "action", "Login" } });
                }
                action = new ApplicationActionExcecutingContextModel 
                {
                    Action = MD5ServiceProvider.Encrypt(filterContext.ActionDescriptor.ActionName, ApplicationSettings.MD5ServicePrividerKey)
                };
                GlobalFunctions.SetActionExecutingContext(action);
            }

            if (appError.Exception != null)
            {
                GlobalFunctions.SetApplicationError(appError);
            }

            base.OnActionExecuting(filterContext);
        }
    }
}