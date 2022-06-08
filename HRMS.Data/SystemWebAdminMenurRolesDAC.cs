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
    public class SystemWebAdminMenurRolesDAC : RepositoryBase<SystemWebAdminMenuRolesModel>, ISystemWebAdminMenuRolesRepositoryDAC
    {
        private readonly IDbConnection _dBConnection;

        #region CONSTRUCTORS
        public SystemWebAdminMenurRolesDAC(IDbConnection dbConnection)
        {
            _dBConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        }
        #endregion

        public override string Add(SystemWebAdminMenuRolesModel model)
        {
            try
            {
                var id = Convert.ToString(_dBConnection.ExecuteScalar("usp_systemwebadminmenuroles_add", new
                {
                    model.SystemWebAdminMenu.SystemWebAdminMenuId,
                    model.IsAllowed,
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

        public override SystemWebAdminMenuRolesModel Find(string id) => throw new NotImplementedException();
        public SystemWebAdminMenuRolesModel FindBySystemWebAdminMenuIdAndSystemWebAdminRoleId(long SystemWebAdminMenuId, string SystemWebAdminRoleId)
        {
            try
            {
                using (var result = _dBConnection.QueryMultiple("usp_systemwebadminmenuroles_getBySystemWebAdminMenuIdAndSystemWebAdminRoleId", new
                {
                    SystemWebAdminMenuId = SystemWebAdminMenuId,
                    SystemWebAdminRoleId = SystemWebAdminRoleId,
                }, commandType: CommandType.StoredProcedure))
                {
                    var model = result.Read<SystemWebAdminMenuRolesModel>().FirstOrDefault();
                    if (model != null)
                    {
                        model.SystemWebAdminRole = result.Read<SystemWebAdminRoleModel>().FirstOrDefault();
                        model.SystemWebAdminMenu = result.Read<SystemWebAdminMenuModel>().FirstOrDefault();
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

        public override List<SystemWebAdminMenuRolesModel> GetAll() => throw new NotImplementedException();

        public List<SystemWebAdminMenuRolesModel> FindBySystemWebAdminRoleId(string SystemWebAdminRoleId)
        {
            var results = new List<SystemWebAdminMenuRolesModel>();
            try
            {
                var lookup = new Dictionary<string, SystemWebAdminMenuRolesModel>();

                _dBConnection.Query("usp_systemwebadminmenuroles_getBySystemWebAdminRoleId",
                new[]
                {
                    typeof(SystemWebAdminMenuRolesModel),
                    typeof(SystemWebAdminMenuModel),
                    typeof(SystemWebAdminRoleModel),
                }, obj =>
                {
                    SystemWebAdminMenuRolesModel swamr = obj[0] as SystemWebAdminMenuRolesModel;
                    SystemWebAdminMenuModel swam = obj[1] as SystemWebAdminMenuModel;
                    SystemWebAdminRoleModel swar = obj[2] as SystemWebAdminRoleModel;
                    SystemWebAdminMenuRolesModel model;
                    if (!lookup.TryGetValue(swamr.SystemWebAdminMenuRoleId, out model))
                        lookup.Add(swamr.SystemWebAdminMenuRoleId, model = swamr);
                    if (model.SystemWebAdminMenu == null)
                        model.SystemWebAdminMenu = new SystemWebAdminMenuModel();
                    if (model.SystemWebAdminRole == null)
                        model.SystemWebAdminRole = new SystemWebAdminRoleModel();
                    model.SystemWebAdminMenu = swam;
                    model.SystemWebAdminRole = swar;
                    return model;
                },
                new
                {
                    SystemWebAdminRoleId = SystemWebAdminRoleId,
                }, splitOn: "SystemWebAdminMenuRoleId,SystemWebAdminMenuId,SystemWebAdminRoleId", commandType: CommandType.StoredProcedure).ToList();
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

        public List<SystemWebAdminMenuRolesModel> FindBySystemWebAdminRoleIdandSystemWebAdminModuleId(string SystemWebAdminRoleId, long SystemWebAdminModuleId)
        {
            var results = new List<SystemWebAdminMenuRolesModel>();
            try
            {
                var lookup = new Dictionary<string, SystemWebAdminMenuRolesModel>();

                _dBConnection.Query("usp_systemwebadminmenuroles_getBySystemWebAdminRoleIdandSystemWebAdminModuleId",
                new[]
                {
                    typeof(SystemWebAdminMenuRolesModel),
                    typeof(SystemWebAdminMenuModel),
                    typeof(SystemWebAdminRoleModel),
                }, obj =>
                {
                    SystemWebAdminMenuRolesModel swamr = obj[0] as SystemWebAdminMenuRolesModel;
                    SystemWebAdminMenuModel swam = obj[1] as SystemWebAdminMenuModel;
                    SystemWebAdminRoleModel swar = obj[2] as SystemWebAdminRoleModel;
                    SystemWebAdminMenuRolesModel model;
                    if (!lookup.TryGetValue(swamr.SystemWebAdminMenuRoleId, out model))
                        lookup.Add(swamr.SystemWebAdminMenuRoleId, model = swamr);
                    if (model.SystemWebAdminMenu == null)
                        model.SystemWebAdminMenu = new SystemWebAdminMenuModel();
                    if (model.SystemWebAdminRole == null)
                        model.SystemWebAdminRole = new SystemWebAdminRoleModel();
                    model.SystemWebAdminMenu = swam;
                    model.SystemWebAdminRole = swar;
                    return model;
                },
                new
                {
                    SystemWebAdminRoleId = SystemWebAdminRoleId,
                    SystemWebAdminModuleId = SystemWebAdminModuleId,
                }, splitOn: "SystemWebAdminMenuRoleId,SystemWebAdminMenuId,SystemWebAdminRoleId", commandType: CommandType.StoredProcedure).ToList();
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
                var result = Convert.ToString(_dBConnection.ExecuteScalar("usp_systemwebadminmenuroles_delete", new
                {
                    SystemWebAdminMenuRoleId = id,
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

        public override bool Update(SystemWebAdminMenuRolesModel model)
        {
            bool success = false;
            try
            {
                int affectedRows = 0;
                var result = Convert.ToString(_dBConnection.ExecuteScalar("usp_systemwebadminmenuroles_update", new
                {
                    model.SystemWebAdminMenuRoleId,
                    model.IsAllowed,
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
