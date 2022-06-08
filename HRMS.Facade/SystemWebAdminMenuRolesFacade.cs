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
    public class SystemWebAdminMenuRolesFacade : ISystemWebAdminMenuRolesFacade
    {
        private readonly ISystemWebAdminMenuRolesRepositoryDAC _systemWebAdminMenuRolesRepositoryDAC;
        private readonly ISystemWebAdminMenuModuleRepositoryDAC _systemWebAdminMenuModuleRepositoryDAC;

        #region CONSTRUCTORS
        public SystemWebAdminMenuRolesFacade(ISystemWebAdminMenuRolesRepositoryDAC systemWebAdminMenuRolesRepositoryDAC, ISystemWebAdminMenuModuleRepositoryDAC systemWebAdminMenuModuleRepositoryDAC)
        {
            _systemWebAdminMenuRolesRepositoryDAC = systemWebAdminMenuRolesRepositoryDAC ?? throw new ArgumentNullException(nameof(systemWebAdminMenuRolesRepositoryDAC));
            _systemWebAdminMenuModuleRepositoryDAC = systemWebAdminMenuModuleRepositoryDAC ?? throw new ArgumentNullException(nameof(systemWebAdminMenuModuleRepositoryDAC));
        }
        #endregion

        public bool Set(SystemWebAdminMenuRolesBindingModel model, string CreatedBy)
        {
            try
            {
                var success = false;
                using (var scope = new TransactionScope())
                {
                    var menuId = int.Parse(model.SystemWebAdminMenu.FirstOrDefault().SystemWebAdminMenuId.ToString());
                    var menuModuleId = int.Parse(AutoMapperHelper<SystemWebAdminModuleModel, SystemWebAdminModuleViewModel>.Map(_systemWebAdminMenuModuleRepositoryDAC.FindByMenuId(menuId)).SystemWebAdminModuleId.ToString());
                    var currentSystemWebAdminMenuRoles = AutoMapperHelper<SystemWebAdminMenuRolesModel, SystemWebAdminMenuRolesViewModel>.MapList(_systemWebAdminMenuRolesRepositoryDAC.FindBySystemWebAdminRoleIdandSystemWebAdminModuleId(model.SystemWebAdminRoleId, menuModuleId));
                    var newSystemWebAdminMenuRoles = new List<SystemWebAdminMenuRolesViewModel>();
                    foreach (var menu in currentSystemWebAdminMenuRoles)
                    {
                        if (menu != null && menu.SystemWebAdminMenu.SystemWebAdminMenuId != null && menu.IsAllowed && !model.SystemWebAdminMenu.Any(swamr => swamr.SystemWebAdminMenuId == menu.SystemWebAdminMenu.SystemWebAdminMenuId))
                        {
                            var systemUserRole = _systemWebAdminMenuRolesRepositoryDAC.FindBySystemWebAdminMenuIdAndSystemWebAdminRoleId(menu.SystemWebAdminMenu.SystemWebAdminMenuId.Value, model.SystemWebAdminRoleId);
                            if (systemUserRole != null)
                            {
                                if (!_systemWebAdminMenuRolesRepositoryDAC.Remove(systemUserRole.SystemWebAdminMenuRoleId, CreatedBy))
                                    throw new Exception("Error Updating System Menu Roles");
                            }
                        }
                    }
                    foreach (var menu in model.SystemWebAdminMenu)
                    {
                        if (!currentSystemWebAdminMenuRoles.Where(x=>x.IsAllowed).ToList().Any(swamr => swamr.SystemWebAdminMenu.SystemWebAdminMenuId == menu.SystemWebAdminMenuId))
                        {
                            var systemWebAdminMenuRoleId = _systemWebAdminMenuRolesRepositoryDAC.Add(new SystemWebAdminMenuRolesModel()
                            {
                                SystemWebAdminMenu = new SystemWebAdminMenuModel() { SystemWebAdminMenuId = menu.SystemWebAdminMenuId },
                                SystemWebAdminRole = new SystemWebAdminRoleModel() { SystemWebAdminRoleId = model.SystemWebAdminRoleId },
                                IsAllowed = menu.IsAllowed,
                                SystemRecordManager = new SystemRecordManagerModel() { CreatedBy = CreatedBy }
                            });
                            if (string.IsNullOrEmpty(systemWebAdminMenuRoleId))
                            {
                                throw new Exception("Error Creating System Menu Roles");
                            }
                        }
                        else
                        {
                            var systemWebAdminMenuRole = _systemWebAdminMenuRolesRepositoryDAC.FindBySystemWebAdminMenuIdAndSystemWebAdminRoleId(menu.SystemWebAdminMenuId.Value, model.SystemWebAdminRoleId);
                            if (systemWebAdminMenuRole != null)
                            {
                                if (!_systemWebAdminMenuRolesRepositoryDAC.Update(new SystemWebAdminMenuRolesModel()
                                {
                                    SystemWebAdminMenuRoleId = systemWebAdminMenuRole.SystemWebAdminMenuRoleId,
                                    SystemWebAdminMenu = new SystemWebAdminMenuModel() { SystemWebAdminMenuId = menu.SystemWebAdminMenuId },
                                    SystemWebAdminRole = new SystemWebAdminRoleModel() { SystemWebAdminRoleId = model.SystemWebAdminRoleId },
                                    IsAllowed = menu.IsAllowed,
                                    SystemRecordManager = new SystemRecordManagerModel() { LastUpdatedBy = CreatedBy }
                                }))
                                    throw new Exception("Error Updating System User Role");
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
        public List<SystemWebAdminMenuRolesViewModel> FindBySystemWebAdminRoleIdandSystemWebAdminModuleId(string SystemWebAdminRoleId, long SystemWebAdminModuleId) => AutoMapperHelper<SystemWebAdminMenuRolesModel, SystemWebAdminMenuRolesViewModel>.MapList(_systemWebAdminMenuRolesRepositoryDAC.FindBySystemWebAdminRoleIdandSystemWebAdminModuleId(SystemWebAdminRoleId, SystemWebAdminModuleId)).ToList();
        public List<SystemWebAdminMenuRolesViewModel> FindBySystemWebAdminRoleId(string SystemWebAdminRoleId) => AutoMapperHelper<SystemWebAdminMenuRolesModel, SystemWebAdminMenuRolesViewModel>.MapList(_systemWebAdminMenuRolesRepositoryDAC.FindBySystemWebAdminRoleId(SystemWebAdminRoleId)).ToList();
    }
}
