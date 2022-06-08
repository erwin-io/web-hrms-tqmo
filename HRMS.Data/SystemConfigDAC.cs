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
    public class SystemConfigDAC : RepositoryBase<SystemConfigModel>, ISystemConfigRepositoryDAC
    {
        private readonly IDbConnection _dBConnection;

        #region CONSTRUCTORS
        public SystemConfigDAC(IDbConnection dbConnection)
        {
            _dBConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        }
        #endregion

        public override string Add(SystemConfigModel model) => throw new NotImplementedException();
        public override SystemConfigModel Find(string id) => throw new NotImplementedException();
        public SystemConfigModel Find(long id)
        {
            try
            {
                using (var result = _dBConnection.QueryMultiple("usp_systemconfig_getById", new
                {
                    SystemConfigId = id,
                }, commandType: CommandType.StoredProcedure))
                {
                    var model = result.Read<SystemConfigModel>().FirstOrDefault();

                    return model;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override List<SystemConfigModel> GetAll()
        {
            try
            {
                return _dBConnection.Query<SystemConfigModel>("usp_systemconfig_getAll", commandType: CommandType.StoredProcedure).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override bool Remove(string id) => throw new NotImplementedException();
        public override bool Update(SystemConfigModel model)
        {
            bool success = false;
            try
            {
                int affectedRows = 0;
                var result = Convert.ToString(_dBConnection.ExecuteScalar("usp_systemconfig_update", new
                {
                    model.SystemConfigId,
                    model.ConfigName,
                    model.ConfigGroup,
                    model.ConfigKey,
                    model.ConfigValue,
                    model.SystemConfigType.SystemConfigTypeId,
                    model.IsUserConfigurable,
                }, commandType: CommandType.StoredProcedure));

                if (result.Contains("Error"))
                    throw new Exception(result);

                affectedRows = Convert.ToInt32(result);
                success = affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return success;
        }
    }
}
