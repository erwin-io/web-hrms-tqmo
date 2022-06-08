using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using HRMS.API.Helpers;

namespace HRMS.API.Filters
{
    public class SilupostAuthorizationFilter : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var isAuthorized = true;
            //IEnumerable<string> headerValues = actionContext.Request.Headers.GetValues("CustomKey").FirstOrDefault();
            // Do some work here to determine if the user has the
            // correct permissions to be authorized anywhere this
            // attribute is used. Assume the username is how
            // you'd link back to a custom user permission scheme.
            if (!GlobalVariables.goEnableAPI)
            {
                return false;
            }
            return isAuthorized;
        }
    }
}