using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Facade.Interface
{
    public interface ISystemWebAdminRoleFacade
    {
        string Add(SystemWebAdminRoleBindingModel model, string CreatedBy);
        SystemWebAdminRoleViewModel Find(string id);
        bool Remove(string id, string LastUpdatedBy);
        bool Update(UpdateSystemWebAdminRoleBindingModel model, string LastUpdatedBy);
        PageResultsViewModel<SystemWebAdminRoleViewModel> GetPage(string Search, int PageNo, int PageSize, string OrderColumn, string OrderDir);
    }
}
