using HRMS.Data.Core;
using HRMS.Data.Entity;
using System.Collections.Generic;

namespace HRMS.Data.Interface
{
    public interface IDoctorRepositoryDAC : IRepository<DoctorModel>
    {
        List<DoctorModel> GetPage(string Search, long PageNo, long PageSize, string OrderColumn, string OrderDir);
        bool Remove(string id, string LastUpdatedBy);
    }
}
