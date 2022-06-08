using HRMS.Data.Core;
using HRMS.Data.Entity;
using System.Collections.Generic;

namespace HRMS.Data.Interface
{
    public interface ISystemWebAdminRoleRepositoryDAC : IRepository<SystemWebAdminRoleModel>
    {
        List<SystemWebAdminRoleModel> GetPage(string Search, int PageNo, int PageSize, string OrderColumn, string OrderDir);
        bool Remove(string id, string LastUpdatedBy);
    }
}
