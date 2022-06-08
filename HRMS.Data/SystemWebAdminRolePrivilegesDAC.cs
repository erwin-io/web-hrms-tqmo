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
    public class SystemWebAdminRolePrivilegesDAC : RepositoryBase<SystemWebAdminRolePrivilegesModel>, ISystemWebAdminRolePrivilegesRepositoryDAC
    {
        private readonly IDbConnection _dBConnection;

        #region CONSTRUCTORS
        public SystemWebAdminRolePrivilegesDAC(IDbConnection dbConnection)
        {
            _dBConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        }
        #endregion

        public override string Add(SystemWebAdminRolePrivilegesModel model)
        {
            try
            {
                var id = Convert.ToString(_dBConnection.ExecuteScalar("usp_systemwebadminroleprivileges_add", new
                {
                    model.SystemWebAdminPrivilege.SystemWebAdminPrivilegeId,
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

        public override SystemWebAdminRolePrivilegesModel Find(string id) => throw new NotImplementedException();
        public SystemWebAdminRolePrivilegesModel FindBySystemWebAdminPrivilegeIdAndSystemWebAdminRoleId(long SystemWebAdminPrivilegeId, string SystemWebAdminRoleId)
        {
            try
            {
                using (var result = _dBConnection.QueryMultiple("usp_systemwebadminroleprivileges_getBySystemWebAdminPrivilegeIdAndSystemWebAdminRoleId", new
                {
                    SystemWebAdminPrivilegeId = SystemWebAdminPrivilegeId,
                    SystemWebAdminRoleId = SystemWebAdminRoleId,
                }, commandType: CommandType.StoredProcedure))
                {
                    var model = result.Read<SystemWebAdminRolePrivilegesModel>().FirstOrDefault();
                    if (model != null)
                    {
                        model.SystemWebAdminRole = result.Read<SystemWebAdminRoleModel>().FirstOrDefault();
                        model.SystemWebAdminPrivilege = result.Read<SystemWebAdminPrivilegesModel>().FirstOrDefault();
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

        public override List<SystemWebAdminRolePrivilegesModel> GetAll() => throw new NotImplementedException();

        public List<SystemWebAdminRolePrivilegesModel> FindBySystemWebAdminRoleId(string SystemWebAdminRoleId)
        {
            var results = new List<SystemWebAdminRolePrivilegesModel>();
            try
            {
                var lookup = new Dictionary<string, SystemWebAdminRolePrivilegesModel>();

                _dBConnection.Query("usp_systemwebadminroleprivileges_getBySystemWebAdminRoleId",
                new[]
                {
                    typeof(SystemWebAdminRolePrivilegesModel),
                    typeof(SystemWebAdminPrivilegesModel),
                    typeof(SystemWebAdminRoleModel),
                }, obj =>
                {
                    SystemWebAdminRolePrivilegesModel swamr = obj[0] as SystemWebAdminRolePrivilegesModel;
                    SystemWebAdminPrivilegesModel swap = obj[1] as SystemWebAdminPrivilegesModel;
                    SystemWebAdminRoleModel swar = obj[2] as SystemWebAdminRoleModel;
                    SystemWebAdminRolePrivilegesModel model;
                    if (!lookup.TryGetValue(swamr.SystemWebAdminRolePrivilegeId, out model))
                        lookup.Add(swamr.SystemWebAdminRolePrivilegeId, model = swamr);
                    if (model.SystemWebAdminPrivilege == null)
                        model.SystemWebAdminPrivilege = new SystemWebAdminPrivilegesModel();
                    if (model.SystemWebAdminRole == null)
                        model.SystemWebAdminRole = new SystemWebAdminRoleModel();
                    model.SystemWebAdminPrivilege = swap;
                    model.SystemWebAdminRole = swar;
                    return model;
                },
                new
                {
                    SystemWebAdminRoleId = SystemWebAdminRoleId,
                }, splitOn: "SystemWebAdminMenuRoleId,SystemWebAdminPrivilegeId,SystemWebAdminRoleId", commandType: CommandType.StoredProcedure).ToList();
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
                var result = Convert.ToString(_dBConnection.ExecuteScalar("usp_systemWebAdminroleprivileges_delete", new
                {
                    SystemWebAdminRolePrivilegeId = id,
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

        public override bool Update(SystemWebAdminRolePrivilegesModel model)
        {
            bool success = false;
            try
            {
                int affectedRows = 0;
                var result = Convert.ToString(_dBConnection.ExecuteScalar("usp_systemwebadminroleprivileges_update", new
                {
                    model.SystemWebAdminRolePrivilegeId,
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
