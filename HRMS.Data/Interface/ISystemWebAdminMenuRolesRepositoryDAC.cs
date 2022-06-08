using HRMS.Data.Core;
using HRMS.Data.Entity;
using System.Collections.Generic;

namespace HRMS.Data.Interface
{
    public interface ISystemWebAdminMenuRolesRepositoryDAC : IRepository<SystemWebAdminMenuRolesModel>
    {
        SystemWebAdminMenuRolesModel FindBySystemWebAdminMenuIdAndSystemWebAdminRoleId(long SystemWebAdminMenuId, string SystemWebAdminRoleId);
        List<SystemWebAdminMenuRolesModel> FindBySystemWebAdminRoleId(string SystemWebAdminRoleId);
        List<SystemWebAdminMenuRolesModel> FindBySystemWebAdminRoleIdandSystemWebAdminModuleId(string SystemWebAdminRoleId, long SystemWebAdminModuleId);
        bool Remove(string id, string LastUpdatedBy);
    }
}
