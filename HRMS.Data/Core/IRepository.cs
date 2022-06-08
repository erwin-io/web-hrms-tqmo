using System.Collections.Generic;

namespace HRMS.Data.Core
{
    public interface IRepository<T>
    {
        List<T> GetAll();
        T Find(string id);
        string Add(T model);
        bool Remove(string id);
        bool Update(T model);

    }
}
