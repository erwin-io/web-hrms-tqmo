using HRMS.Data.Core;
using HRMS.Data.Entity;
using System.Collections.Generic;

namespace HRMS.Data.Interface
{
    public interface ILookupTableRepositoryDAC : IRepository<LookupTableModel>
    {
        List<LookupTableModel> FindLookupByTableNames(string TableNames);
    }
}
