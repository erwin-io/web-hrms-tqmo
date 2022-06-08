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
    public class SystemWebAdminMenuModuleDAC : RepositoryBase<SystemWebAdminModuleModel>, ISystemWebAdminMenuModuleRepositoryDAC
    {
        private readonly IDbConnection _dBConnection;

        #region CONSTRUCTORS
        public SystemWebAdminMenuModuleDAC(IDbConnection dbConnection)
        {
            _dBConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        }
        #endregion

        public override string Add(SystemWebAdminModuleModel model) => throw new NotImplementedException();

        public override SystemWebAdminModuleModel Find(string id) => throw new NotImplementedException();

        public override List<SystemWebAdminModuleModel> GetAll() => throw new NotImplementedException();

        public override bool Remove(string id) => throw new NotImplementedException();
        public override bool Update(SystemWebAdminModuleModel model) => throw new NotImplementedException();
        public SystemWebAdminModuleModel FindByMenuId(long menuId)
        {
            try
            {
                using (var result = _dBConnection.QueryMultiple("usp_systemwebadminmenumodule_getBySystemWebAdminMenuId", new
                {
                    SystemWebAdminMenuId = menuId,
                }, commandType: CommandType.StoredProcedure))
                {
                    var model = result.Read<SystemWebAdminModuleModel>().FirstOrDefault();

                    return model;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
