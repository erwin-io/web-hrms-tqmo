using HRMS.Data.Core;
using HRMS.Data.Entity;
using System.Collections.Generic;

namespace HRMS.Data.Interface
{
    public interface ISystemConfigRepositoryDAC : IRepository<SystemConfigModel>
    {
        SystemConfigModel Find(long id);
    }
}
