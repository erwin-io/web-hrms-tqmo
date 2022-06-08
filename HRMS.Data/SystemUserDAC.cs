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
    public class SystemUserDAC : RepositoryBase<SystemUserModel>, ISystemUserRepositoryDAC
    {
        private readonly IDbConnection _dBConnection;

        #region CONSTRUCTORS
        public SystemUserDAC(IDbConnection dbConnection)
        {
            _dBConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        }
        #endregion

        public override string Add(SystemUserModel model)
        {
            try
            {
                var id = Convert.ToString(_dBConnection.ExecuteScalar("usp_systemuser_add", new
                {
                    model.SystemUserType.SystemUserTypeId,
                    model.LegalEntity.LegalEntityId,
                    ProfilePictureFile = model.ProfilePicture.FileId,
                    model.UserName,
                    model.Password,
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

        public override SystemUserModel Find(string id)
        {
            try
            {
                var lookupSystemWebAdminUserRoles = new Dictionary<string, SystemWebAdminUserRolesModel>();
                var lookupSystemWebAdminMenus = new Dictionary<long?, SystemWebAdminMenuModel>();
                using (var result = _dBConnection.QueryMultiple("usp_systemuser_getByID", new
                {
                    SystemUserId = id
                }, commandType: CommandType.StoredProcedure))
                {
                    var model = result.Read<SystemUserModel>().FirstOrDefault();
                    if (model != null)
                    {
                        model.SystemUserType = result.Read<SystemUserTypeModel>().FirstOrDefault();
                        model.ProfilePicture = result.Read<FileModel>().FirstOrDefault();
                        model.LegalEntity = result.Read<LegalEntityModel>().FirstOrDefault();
                        model.LegalEntity.Gender = result.Read<EntityGenderModel>().FirstOrDefault();
                        model.SystemUserConfig = result.Read<SystemUserConfigModel>().FirstOrDefault();

                        result.Read<SystemWebAdminUserRolesModel, SystemWebAdminRoleModel, SystemWebAdminUserRolesModel>((swaur, swar) =>
                        {
                            SystemWebAdminUserRolesModel systemWebAdminUserRolesModel;
                            if (!lookupSystemWebAdminUserRoles.TryGetValue(swaur.SystemWebAdminUserRoleId, out systemWebAdminUserRolesModel))
                                lookupSystemWebAdminUserRoles.Add(swaur.SystemWebAdminUserRoleId, systemWebAdminUserRolesModel = swaur);
                            systemWebAdminUserRolesModel.SystemWebAdminRole = swar;
                            return systemWebAdminUserRolesModel;
                        }, splitOn: "SystemWebAdminUserRoleId,SystemWebAdminRoleId").ToList();
                        if (model.SystemWebAdminUserRoles == null)
                            model.SystemWebAdminUserRoles = new List<SystemWebAdminUserRolesModel>();
                        model.SystemWebAdminUserRoles.AddRange(lookupSystemWebAdminUserRoles.Values);

                        result.Read<SystemWebAdminMenuModel, SystemWebAdminModuleModel, SystemWebAdminMenuModel>((swam, swamd) =>
                        {
                            SystemWebAdminMenuModel systemWebAdminMenuModel;
                            if (!lookupSystemWebAdminMenus.TryGetValue(swam.SystemWebAdminMenuId, out systemWebAdminMenuModel))
                                lookupSystemWebAdminMenus.Add(swam.SystemWebAdminMenuId, systemWebAdminMenuModel = swam);
                            systemWebAdminMenuModel.SystemWebAdminModule = swamd;
                            return systemWebAdminMenuModel;
                        }, splitOn: "SystemWebAdminMenuId,SystemWebAdminModuleId").ToList();
                        if (model.SystemWebAdminMenus == null)
                            model.SystemWebAdminMenus = new List<SystemWebAdminMenuModel>();
                        model.SystemWebAdminMenus.AddRange(lookupSystemWebAdminMenus.Values);

                        model.SystemWebAdminPrivileges = result.Read<SystemWebAdminPrivilegesModel>().ToList();
                        if (model.SystemWebAdminPrivileges == null)
                            model.SystemWebAdminPrivileges = new List<SystemWebAdminPrivilegesModel>();

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


        public SystemUserModel FindByUsername(string Username)
        {
            try
            {
                var lookupSystemWebAdminUserRoles = new Dictionary<string, SystemWebAdminUserRolesModel>();
                var lookupSystemWebAdminMenus = new Dictionary<long?, SystemWebAdminMenuModel>();
                using (var result = _dBConnection.QueryMultiple("usp_systemuser_getByUsername", new
                {
                    Username = Username,
                }, commandType: CommandType.StoredProcedure))
                {
                    var model = result.Read<SystemUserModel>().FirstOrDefault();
                    if (model != null)
                    {
                        model.SystemUserType = result.Read<SystemUserTypeModel>().FirstOrDefault();
                        model.ProfilePicture = result.Read<FileModel>().FirstOrDefault();
                        model.LegalEntity = result.Read<LegalEntityModel>().FirstOrDefault();
                        model.LegalEntity.Gender = result.Read<EntityGenderModel>().FirstOrDefault();
                        model.SystemUserConfig = result.Read<SystemUserConfigModel>().FirstOrDefault();

                        result.Read<SystemWebAdminUserRolesModel, SystemWebAdminRoleModel, SystemWebAdminUserRolesModel>((swaur, swar) =>
                        {
                            SystemWebAdminUserRolesModel systemWebAdminUserRolesModel;
                            if (!lookupSystemWebAdminUserRoles.TryGetValue(swaur.SystemWebAdminUserRoleId, out systemWebAdminUserRolesModel))
                                lookupSystemWebAdminUserRoles.Add(swaur.SystemWebAdminUserRoleId, systemWebAdminUserRolesModel = swaur);
                            systemWebAdminUserRolesModel.SystemWebAdminRole = swar;
                            return systemWebAdminUserRolesModel;
                        }, splitOn: "SystemWebAdminUserRoleId,SystemWebAdminRoleId").ToList();
                        if (model.SystemWebAdminUserRoles == null)
                            model.SystemWebAdminUserRoles = new List<SystemWebAdminUserRolesModel>();
                        model.SystemWebAdminUserRoles.AddRange(lookupSystemWebAdminUserRoles.Values);

                        result.Read<SystemWebAdminMenuModel, SystemWebAdminModuleModel, SystemWebAdminMenuModel>((swam, swamd) =>
                        {
                            SystemWebAdminMenuModel systemWebAdminMenuModel;
                            if (!lookupSystemWebAdminMenus.TryGetValue(swam.SystemWebAdminMenuId, out systemWebAdminMenuModel))
                                lookupSystemWebAdminMenus.Add(swam.SystemWebAdminMenuId, systemWebAdminMenuModel = swam);
                            systemWebAdminMenuModel.SystemWebAdminModule = swamd;
                            return systemWebAdminMenuModel;
                        }, splitOn: "SystemWebAdminMenuId,SystemWebAdminModuleId").ToList();
                        if (model.SystemWebAdminMenus == null)
                            model.SystemWebAdminMenus = new List<SystemWebAdminMenuModel>();
                        model.SystemWebAdminMenus.AddRange(lookupSystemWebAdminMenus.Values);

                        model.SystemWebAdminPrivileges = result.Read<SystemWebAdminPrivilegesModel>().ToList();
                        if (model.SystemWebAdminPrivileges == null)
                            model.SystemWebAdminPrivileges = new List<SystemWebAdminPrivilegesModel>();

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
        public SystemUserModel Find(string Username, string Password)
        {
            try
            {
                var lookupSystemWebAdminUserRoles = new Dictionary<string, SystemWebAdminUserRolesModel>();
                var lookupSystemWebAdminMenus = new Dictionary<long?, SystemWebAdminMenuModel>();
                using (var result = _dBConnection.QueryMultiple("usp_systemuser_getByCredentials", new
                {
                    Username = Username,
                    Password = Password,
                }, commandType: CommandType.StoredProcedure))
                {
                    var model = result.Read<SystemUserModel>().FirstOrDefault();
                    if(model != null)
                    {
                        model.SystemUserType = result.Read<SystemUserTypeModel>().FirstOrDefault();
                        model.ProfilePicture = result.Read<FileModel>().FirstOrDefault();
                        model.LegalEntity = result.Read<LegalEntityModel>().FirstOrDefault();
                        model.LegalEntity.Gender = result.Read<EntityGenderModel>().FirstOrDefault();
                        model.SystemUserConfig = result.Read<SystemUserConfigModel>().FirstOrDefault();

                        result.Read<SystemWebAdminUserRolesModel, SystemWebAdminRoleModel, SystemWebAdminUserRolesModel>((swaur, swar) =>
                        {
                            SystemWebAdminUserRolesModel systemWebAdminUserRolesModel;
                            if (!lookupSystemWebAdminUserRoles.TryGetValue(swaur.SystemWebAdminUserRoleId, out systemWebAdminUserRolesModel))
                                lookupSystemWebAdminUserRoles.Add(swaur.SystemWebAdminUserRoleId, systemWebAdminUserRolesModel = swaur);
                            systemWebAdminUserRolesModel.SystemWebAdminRole = swar;
                            return systemWebAdminUserRolesModel;
                        }, splitOn: "SystemWebAdminUserRoleId,SystemWebAdminRoleId").ToList();
                        if (model.SystemWebAdminUserRoles == null)
                            model.SystemWebAdminUserRoles = new List<SystemWebAdminUserRolesModel>();
                        model.SystemWebAdminUserRoles.AddRange(lookupSystemWebAdminUserRoles.Values);

                        result.Read<SystemWebAdminMenuModel, SystemWebAdminModuleModel, SystemWebAdminMenuModel>((swam, swamd) =>
                        {
                            SystemWebAdminMenuModel systemWebAdminMenuModel;
                            if (!lookupSystemWebAdminMenus.TryGetValue(swam.SystemWebAdminMenuId, out systemWebAdminMenuModel))
                                lookupSystemWebAdminMenus.Add(swam.SystemWebAdminMenuId, systemWebAdminMenuModel = swam);
                            systemWebAdminMenuModel.SystemWebAdminModule = swamd;
                            return systemWebAdminMenuModel;
                        }, splitOn: "SystemWebAdminMenuId,SystemWebAdminModuleId").ToList();
                        if (model.SystemWebAdminMenus == null)
                            model.SystemWebAdminMenus = new List<SystemWebAdminMenuModel>();
                        model.SystemWebAdminMenus.AddRange(lookupSystemWebAdminMenus.Values);

                        model.SystemWebAdminPrivileges = result.Read<SystemWebAdminPrivilegesModel>().ToList();
                        if (model.SystemWebAdminPrivileges == null)
                            model.SystemWebAdminPrivileges = new List<SystemWebAdminPrivilegesModel>();

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
        public SystemUserModel GetTrackerStatus(string id)
        {
            try
            {
                var lookupSystemWebAdminUserRoles = new Dictionary<string, SystemWebAdminUserRolesModel>();
                var lookupSystemWebAdminMenus = new Dictionary<long?, SystemWebAdminMenuModel>();
                return _dBConnection.Query<SystemUserModel>("usp_systemuser_getTrackerStatus", new
                {
                    SystemUserId = id,
                }, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override List<SystemUserModel> GetAll() => throw new NotImplementedException();

        public List<SystemUserModel> GetPage(string Search, long SystemUserType, long ApprovalStatus, long PageNo, long PageSize, string OrderColumn, string OrderDir)
        {
            var results = new List<SystemUserModel>();
            try
            {
                var lookup = new Dictionary<string, SystemUserModel>();

                _dBConnection.Query("usp_systemuser_getPaged",
                new[]
                {
                    typeof(SystemUserModel),
                    typeof(FileModel),
                    typeof(SystemUserTypeModel),
                    typeof(LegalEntityModel),
                    typeof(EntityGenderModel),
                    typeof(SystemWebAdminUserRolesModel),
                    typeof(SystemWebAdminRoleModel),
                    typeof(PageResultsModel),
                }, obj =>
                {
                    SystemUserModel su = obj[0] as SystemUserModel;
                    FileModel pf = obj[1] as FileModel;
                    SystemUserTypeModel sut = obj[2] as SystemUserTypeModel;
                    LegalEntityModel le = obj[3] as LegalEntityModel;
                    EntityGenderModel eg = obj[4] as EntityGenderModel;
                    SystemWebAdminUserRolesModel swaur = obj[5] as SystemWebAdminUserRolesModel;
                    SystemWebAdminRoleModel swar = obj[6] as SystemWebAdminRoleModel;
                    PageResultsModel pr = obj[7] as PageResultsModel;

                    SystemUserModel model;
                    if (!lookup.TryGetValue(su.SystemUserId, out model))
                        lookup.Add(su.SystemUserId, model = su);
                    if (model.ProfilePicture == null)
                        model.ProfilePicture = new FileModel();
                    if (model.SystemUserType == null)
                        model.SystemUserType = new SystemUserTypeModel();
                    if (model.LegalEntity == null)
                        model.LegalEntity = new LegalEntityModel();
                    if (model.LegalEntity.Gender == null)
                        model.LegalEntity.Gender = new EntityGenderModel();
                    if (model.SystemWebAdminUserRoles == null)
                        model.SystemWebAdminUserRoles = new List<SystemWebAdminUserRolesModel>();
                    if (model.PageResult == null)
                        model.PageResult = new PageResultsModel();
                    model.ProfilePicture = pf;
                    model.SystemUserType = sut;
                    model.LegalEntity = le;
                    model.LegalEntity.Gender = eg;
                    if(swaur != null)
                    {
                        if (swaur.SystemWebAdminRole == null)
                            swaur.SystemWebAdminRole = new SystemWebAdminRoleModel();
                        swaur.SystemWebAdminRole = swar;
                        model.SystemWebAdminUserRoles.Add(swaur);
                    }
                    model.PageResult = pr;
                    return model;
                },
                new
                {
                    Search = Search,
                    SystemUserType = SystemUserType,
                    ApprovalStatus = ApprovalStatus,
                    PageNo = PageNo,
                    PageSize = PageSize,
                    OrderColumn = OrderColumn,
                    OrderDir = OrderDir
                }, splitOn: "SystemUserId,FileId,SystemUserTypeId,LegalEntityId,GenderId,SystemWebAdminUserRoleId,SystemWebAdminRoleId,TotalRows", commandType: CommandType.StoredProcedure).ToList();
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
                var result = Convert.ToString(_dBConnection.ExecuteScalar("usp_systemuser_delete", new
                {
                    SystemUserId = id,
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

        public override bool Update(SystemUserModel model)
        {
            bool success = false;
            try
            {
                int affectedRows = 0;
                var result = Convert.ToString(_dBConnection.ExecuteScalar("usp_systemuser_update", new
                {
                    model.SystemUserId,
                    model.IsWebAdminGuestUser,
                    ProfilePictureFile = model?.ProfilePicture?.FileId,
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

        public string CreateAccount(SystemUserModel model)
        {
            try
            {
                var id = Convert.ToString(_dBConnection.ExecuteScalar("usp_systemuser_createAccount", new
                {
                    model.SystemUserType.SystemUserTypeId,
                    ProfilePictureFile = model.ProfilePicture.FileId,
                    model.LegalEntity.LegalEntityId,
                    model.UserName,
                    model.Password,
                    model.IsWebAdminGuestUser,
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

        public bool UpdateUsername(SystemUserModel model)
        {
            bool success = false;
            try
            {
                int affectedRows = 0;
                var result = Convert.ToString(_dBConnection.ExecuteScalar("usp_systemuser_changeUsername", new
                {
                    model.SystemUserId,
                    model.UserName,
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
        public bool UpdatePassword(SystemUserModel model)
        {
            bool success = false;
            try
            {
                int affectedRows = 0;
                var result = Convert.ToString(_dBConnection.ExecuteScalar("usp_systemuser_changePassword", new
                {
                    model.SystemUserId,
                    model.Password,
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
