using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Facade.Interface
{
    public interface ISystemWebAdminMenuRolesFacade
    {
        bool Set(SystemWebAdminMenuRolesBindingModel model, string CreatedBy);
        List<SystemWebAdminMenuRolesViewModel> FindBySystemWebAdminRoleIdandSystemWebAdminModuleId(string SystemWebAdminRoleId, long SystemWebAdminModuleId);
        List<SystemWebAdminMenuRolesViewModel> FindBySystemWebAdminRoleId(string SystemWebAdminRoleId);
    }
}
