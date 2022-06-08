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
    public class SystemWebAdminUserRolesDAC : RepositoryBase<SystemWebAdminUserRolesModel>, ISystemWebAdminUserRolesRepositoryDAC
    {
        private readonly IDbConnection _dBConnection;

        #region CONSTRUCTORS
        public SystemWebAdminUserRolesDAC(IDbConnection dbConnection)
        {
            _dBConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        }
        #endregion

        public override string Add(SystemWebAdminUserRolesModel model)
        {
            try
            {
                var id = Convert.ToString(_dBConnection.ExecuteScalar("usp_systemwebadminuserroles_add", new
                {
                    model.SystemUser.SystemUserId,
                    model.SystemWebAdminRole.SystemWebAdminRoleId,
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

        public override SystemWebAdminUserRolesModel Find(string id)
        {
            try
            {
                using (var result = _dBConnection.QueryMultiple("usp_systemwebadminuserroles_getBySystemWebAdminUserRoleId", new
                {
                    SystemWebAdminUserRoleId = id,
                }, commandType: CommandType.StoredProcedure))
                {
                    var model = result.Read<SystemWebAdminUserRolesModel>().FirstOrDefault();
                    if(model != null)
                    {
                        model.SystemWebAdminRole = result.Read<SystemWebAdminRoleModel>().FirstOrDefault();
                        model.SystemUser = result.Read<SystemUserModel>().FirstOrDefault();
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
        public SystemWebAdminUserRolesModel FindBySystemWebAdminRoleIdAndSystemUserId(string SystemWebAdminRoleId, string SystemUserId)
        {
            try
            {
                using (var result = _dBConnection.QueryMultiple("usp_systemwebadminuserroles_getBySystemWebAdminRoleIdAndSystemUserId", new
                {
                    SystemWebAdminRoleId = SystemWebAdminRoleId,
                    SystemUserId = SystemUserId,
                }, commandType: CommandType.StoredProcedure))
                {
                    var model = result.Read<SystemWebAdminUserRolesModel>().FirstOrDefault();
                    if (model != null)
                    {
                        model.SystemWebAdminRole = result.Read<SystemWebAdminRoleModel>().FirstOrDefault();
                        model.SystemUser = result.Read<SystemUserModel>().FirstOrDefault();
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

        public override List<SystemWebAdminUserRolesModel> GetAll() => throw new NotImplementedException();

        public List<SystemWebAdminUserRolesModel> FindBySystemUserId(string SystemUserId)
        {
            var results = new List<SystemWebAdminUserRolesModel>();
            try
            {
                var lookup = new Dictionary<string, SystemWebAdminUserRolesModel>();

                _dBConnection.Query("usp_systemwebadminuserroles_getBySystemUserId",
                new[]
                {
                    typeof(SystemWebAdminUserRolesModel),
                    typeof(SystemWebAdminRoleModel),
                }, obj =>
                {
                    SystemWebAdminUserRolesModel swaur = obj[0] as SystemWebAdminUserRolesModel;
                    SystemWebAdminRoleModel swar = obj[1] as SystemWebAdminRoleModel;
                    SystemWebAdminUserRolesModel model;
                    if (!lookup.TryGetValue(swaur.SystemWebAdminUserRoleId, out model))
                        lookup.Add(swaur.SystemWebAdminUserRoleId, model = swaur);
                    if (model.SystemWebAdminRole == null)
                        model.SystemWebAdminRole = new SystemWebAdminRoleModel();
                    model.SystemWebAdminRole = swar;
                    return model;
                },
                new
                {
                    SystemUserId = SystemUserId,
                }, splitOn: "SystemWebAdminUserRoleId,SystemWebAdminRoleId", commandType: CommandType.StoredProcedure).ToList();
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

        public List<SystemWebAdminUserRolesModel> GetPage(string Search, string SystemUserId, int PageNo, int PageSize, string OrderColumn, string OrderDir)
        {
            var results = new List<SystemWebAdminUserRolesModel>();
            try
            {
                var lookup = new Dictionary<string, SystemWebAdminUserRolesModel>();

                _dBConnection.Query("usp_systemwebadminuserroles_getPaged",
                new[]
                {
                    typeof(SystemWebAdminUserRolesModel),
                    typeof(SystemWebAdminRoleModel),
                    typeof(SystemUserModel),
                    typeof(PageResultsModel),
                }, obj =>
                {
                    SystemWebAdminUserRolesModel swaur = obj[0] as SystemWebAdminUserRolesModel;
                    SystemWebAdminRoleModel swar = obj[1] as SystemWebAdminRoleModel;
                    SystemUserModel su = obj[2] as SystemUserModel;
                    PageResultsModel pr = obj[3] as PageResultsModel;
                    SystemWebAdminUserRolesModel model;
                    if (!lookup.TryGetValue(swaur.SystemWebAdminUserRoleId, out model))
                        lookup.Add(swaur.SystemWebAdminUserRoleId, model = swaur);
                    if (model.SystemWebAdminRole == null)
                        model.SystemWebAdminRole = new SystemWebAdminRoleModel();
                    if (model.SystemUser == null)
                        model.SystemUser = new SystemUserModel();
                    if (model.PageResult == null)
                        model.PageResult = new PageResultsModel();
                    model.SystemWebAdminRole = swar;
                    model.SystemUser = su;
                    model.PageResult = pr;
                    return model;
                },
                new
                {
                    SystemUserId = SystemUserId,
                    Search = Search,
                    PageNo = PageNo,
                    PageSize = PageSize,
                    OrderColumn = OrderColumn,
                    OrderDir = OrderDir
                }, splitOn: "SystemWebAdminUserRoleId,SystemWebAdminRoleId,TotalRows", commandType: CommandType.StoredProcedure).ToList();
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
                var result = Convert.ToString(_dBConnection.ExecuteScalar("usp_systemwebadminuserroles_delete", new
                {
                    SystemWebAdminUserRoleId = id,
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

        public override bool Update(SystemWebAdminUserRolesModel model) => throw new NotImplementedException();
    }
}
