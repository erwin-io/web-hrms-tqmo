using HRMS.Data.Core;
using HRMS.Data.Entity;
using System.Collections.Generic;

namespace HRMS.Data.Interface
{
    public interface ISystemWebAdminMenuModuleRepositoryDAC : IRepository<SystemWebAdminModuleModel>
    {
        SystemWebAdminModuleModel FindByMenuId(long menuId);
    }
}
