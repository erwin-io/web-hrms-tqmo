using HRMS.Data.Core;
using HRMS.Data.Entity;
using System.Collections.Generic;

namespace HRMS.Data.Interface
{
    public interface ISystemWebAdminRolePrivilegesRepositoryDAC : IRepository<SystemWebAdminRolePrivilegesModel>
    {
        SystemWebAdminRolePrivilegesModel FindBySystemWebAdminPrivilegeIdAndSystemWebAdminRoleId(long SystemWebAdminPrivilegeId, string SystemWebAdminRoleId);
        List<SystemWebAdminRolePrivilegesModel> FindBySystemWebAdminRoleId(string SystemWebAdminRoleId);
        bool Remove(string id, string LastUpdatedBy);
    }
}
