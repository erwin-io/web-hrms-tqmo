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
    public class SystemWebAdminRoleDAC : RepositoryBase<SystemWebAdminRoleModel>, ISystemWebAdminRoleRepositoryDAC
    {
        private readonly IDbConnection _dBConnection;

        #region CONSTRUCTORS
        public SystemWebAdminRoleDAC(IDbConnection dbConnection)
        {
            _dBConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        }
        #endregion

        public override string Add(SystemWebAdminRoleModel model)
        {
            try
            {
                var id = Convert.ToString(_dBConnection.ExecuteScalar("usp_systemwebadminrole_add", new
                {
                    model.RoleName,
                    model.SystemRecordManager.CreatedBy,
                }, commandType: CommandType.StoredProcedure));

                if (id.Contains("Error"))
                    throw new Exception(id);

                return id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override SystemWebAdminRoleModel Find(string id)
        {
            try
            {
                using (var result = _dBConnection.QueryMultiple("usp_systemwebadminrole_getByID", new
                {
                    SystemWebAdminRoleId = id,
                }, commandType: CommandType.StoredProcedure))
                {
                    var model = result.Read<SystemWebAdminRoleModel>().FirstOrDefault();
                    if(model != null)
                    {
                        model.SystemRecordManager = result.Read<SystemRecordManagerModel>().FirstOrDefault();
                        model.EntityStatus = result.Read<EntityStatusModel>().FirstOrDefault();
                    }

                    return model;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override List<SystemWebAdminRoleModel> GetAll() => throw new NotImplementedException();

        public List<SystemWebAdminRoleModel> GetPage(string Search, int PageNo, int PageSize, string OrderColumn, string OrderDir)
        {
            var results = new List<SystemWebAdminRoleModel>();
            try
            {
                var lookup = new Dictionary<string, SystemWebAdminRoleModel>();

                _dBConnection.Query("usp_systemwebadminrole_getPaged",
                new[]
                {
                    typeof(SystemWebAdminRoleModel),
                    typeof(PageResultsModel),
                }, obj =>
                {
                    SystemWebAdminRoleModel sr = obj[0] as SystemWebAdminRoleModel;
                    PageResultsModel pr = obj[1] as PageResultsModel;
                    SystemWebAdminRoleModel model;
                    if (!lookup.TryGetValue(sr.SystemWebAdminRoleId, out model))
                        lookup.Add(sr.SystemWebAdminRoleId, model = sr);
                    sr.PageResult = pr;
                    return model;
                },
                new
                {
                    Search = Search,
                    PageNo = PageNo,
                    PageSize = PageSize,
                    OrderColumn = OrderColumn,
                    OrderDir = OrderDir
                }, splitOn: "SystemWebAdminRoleId,TotalRows", commandType: CommandType.StoredProcedure).ToList();
                if (lookup.Values.Any())
                {
                    results.AddRange(lookup.Values);
                }
                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override bool Remove(string id) => throw new NotImplementedException();

        public bool Remove(string id, string LastUpdatedBy)
        {
            bool success = false;
            try
            {
                int affectedRows = 0;
                var result = Convert.ToString(_dBConnection.ExecuteScalar("usp_systemwebadminrole_delete", new
                {
                    SystemWebAdminRoleId = id,
                    LastUpdatedBy = LastUpdatedBy
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

        public override bool Update(SystemWebAdminRoleModel model)
        {
            bool success = false;
            try
            {
                int affectedRows = 0;
                var result = Convert.ToString(_dBConnection.ExecuteScalar("usp_systemwebadminrole_update", new
                {
                    model.SystemWebAdminRoleId,
                    model.RoleName,
                    model.SystemRecordManager.LastUpdatedBy
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
