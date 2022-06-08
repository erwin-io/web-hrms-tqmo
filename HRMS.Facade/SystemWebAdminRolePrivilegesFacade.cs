using HRMS.Mapping;
using HRMS.Data.Entity;
using HRMS.Data.Interface;
using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using HRMS.Facade.Interface;
using System;
using System.Collections.Generic;
using System.Transactions;
using System.Linq;

namespace HRMS.Facade
{
    public class SystemWebAdminRolePrivilegesFacade : ISystemWebAdminRolePrivilegesFacade
    {
        private readonly ISystemWebAdminRolePrivilegesRepositoryDAC _systemWebAdminRolePrivilegesRepositoryDAC;

        #region CONSTRUCTORS
        public SystemWebAdminRolePrivilegesFacade(ISystemWebAdminRolePrivilegesRepositoryDAC systemWebAdminRolePrivilegesRepositoryDAC)
        {
            _systemWebAdminRolePrivilegesRepositoryDAC = systemWebAdminRolePrivilegesRepositoryDAC ?? throw new ArgumentNullException(nameof(systemWebAdminRolePrivilegesRepositoryDAC));
        }
        #endregion

        public bool Set(SystemWebAdminRolePrivilegesBindingModel model, string CreatedBy)
        {
            try
            {
                var success = false;
                using (var scope = new TransactionScope())
                {
                    var currentSystemWebAdminMenuRoles = AutoMapperHelper<SystemWebAdminRolePrivilegesModel, SystemWebAdminRolePrivilegesViewModel>.MapList(_systemWebAdminRolePrivilegesRepositoryDAC.FindBySystemWebAdminRoleId(model.SystemWebAdminRoleId));
                    var newSystemWebAdminMenuRoles = new List<SystemWebAdminMenuRolesViewModel>();
                    foreach (var privilege in currentSystemWebAdminMenuRoles)
                    {
                        if (privilege != null && privilege.SystemWebAdminPrivilege.SystemWebAdminPrivilegeId != null && privilege.IsAllowed && !model.SystemWebAdminPrivilege.Any(swamr => swamr.SystemWebAdminPrivilegeId == privilege.SystemWebAdminPrivilege.SystemWebAdminPrivilegeId && swamr.IsAllowed))
                        {
                            var systemUserRole = _systemWebAdminRolePrivilegesRepositoryDAC.FindBySystemWebAdminPrivilegeIdAndSystemWebAdminRoleId(privilege.SystemWebAdminPrivilege.SystemWebAdminPrivilegeId.Value, model.SystemWebAdminRoleId);
                            if (systemUserRole != null)
                            {
                                if (!_systemWebAdminRolePrivilegesRepositoryDAC.Remove(systemUserRole.SystemWebAdminRolePrivilegeId, CreatedBy))
                                    throw new Exception("Error Updating System Role Privileges");
                            }
                        }
                    }
                    foreach (var privilege in model.SystemWebAdminPrivilege)
                    {
                        if (privilege.IsAllowed == true && !currentSystemWebAdminMenuRoles.Where(x=>x.IsAllowed).ToList().Any(swamr => swamr.SystemWebAdminPrivilege.SystemWebAdminPrivilegeId == privilege.SystemWebAdminPrivilegeId))
                        {
                            var systemWebAdminMenuRoleId = _systemWebAdminRolePrivilegesRepositoryDAC.Add(new SystemWebAdminRolePrivilegesModel()
                            {
                                SystemWebAdminPrivilege = new SystemWebAdminPrivilegesModel() { SystemWebAdminPrivilegeId = privilege.SystemWebAdminPrivilegeId },
                                SystemWebAdminRole = new SystemWebAdminRoleModel() { SystemWebAdminRoleId = model.SystemWebAdminRoleId },
                                IsAllowed = privilege.IsAllowed,
                                SystemRecordManager = new SystemRecordManagerModel() { CreatedBy = CreatedBy }
                            });
                            if (string.IsNullOrEmpty(systemWebAdminMenuRoleId))
                            {
                                    throw new Exception("Error Updating System Role Privileges");
                            }
                        }
                    }
                    success = true;
                    scope.Complete();
                }
                return success;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public List<SystemWebAdminRolePrivilegesViewModel> FindBySystemWebAdminRoleId(string SystemWebAdminRoleId) => AutoMapperHelper<SystemWebAdminRolePrivilegesModel, SystemWebAdminRolePrivilegesViewModel>.MapList(_systemWebAdminRolePrivilegesRepositoryDAC.FindBySystemWebAdminRoleId(SystemWebAdminRoleId)).ToList();
    }
}
