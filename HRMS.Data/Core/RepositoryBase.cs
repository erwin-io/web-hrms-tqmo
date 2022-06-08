using System.Collections.Generic;

namespace HRMS.Data.Core
{
    public abstract class RepositoryBase<T> : IRepository<T>
    {
        public abstract string Add(T model);
        public abstract T Find(string id);
        public abstract List<T> GetAll();
        public abstract bool Remove(string id);
        public abstract bool Update(T model);        
    }
}
