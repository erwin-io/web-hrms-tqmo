using System.Web;
using System.Web.Mvc;
using HRMS.Web.Models;

namespace HRMS.Web.App_Start
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthorizationPrivilegeFilter());
        }
    }
}
