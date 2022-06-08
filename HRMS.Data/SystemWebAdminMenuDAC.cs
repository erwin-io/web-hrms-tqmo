using HRMS.Data.Core;
using HRMS.Data.Interface;
using HRMS.Data.Entity;
using System.Collections.Generic;
using System.Data;
using System;
using Dapper;
using System.Linq;

namespace HRMS.Data
{
    public class SystemWebAdminMenuDAC : RepositoryBase<SystemWebAdminMenuModel>, ISystemWebAdminMenuRepositoryDAC
    {
        private readonly IDbConnection _dBConnection;

        #region CONSTRUCTORS
        public SystemWebAdminMenuDAC(IDbConnection dbConnection)
        {
            _dBConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        }
        #endregion

        public override string Add(SystemWebAdminMenuModel model) => throw new NotImplementedException();

        public override SystemWebAdminMenuModel Find(string id) => throw new NotImplementedException();
        public override List<SystemWebAdminMenuModel> GetAll() => throw new NotImplementedException();

        public override bool Remove(string id) => throw new NotImplementedException();
        public override bool Update(SystemWebAdminMenuModel model) => throw new NotImplementedException();
    }
}
