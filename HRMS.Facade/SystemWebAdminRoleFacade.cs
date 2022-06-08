
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
    public class SystemWebAdminRoleFacade : ISystemWebAdminRoleFacade
    {
        private readonly ISystemWebAdminRoleRepositoryDAC _systemWebAdminRoleRepositoryDAC;

        #region CONSTRUCTORS
        public SystemWebAdminRoleFacade(ISystemWebAdminRoleRepositoryDAC systemWebAdminRoleRepositoryDAC)
        {
            _systemWebAdminRoleRepositoryDAC = systemWebAdminRoleRepositoryDAC ?? throw new ArgumentNullException(nameof(systemWebAdminRoleRepositoryDAC));
        }
        #endregion
        public string Add(SystemWebAdminRoleBindingModel model, string CreatedBy)
        {
            try
            {
                var id = string.Empty;
                using (var scope = new TransactionScope())
                {
                    var addModel = AutoMapperHelper<SystemWebAdminRoleBindingModel, SystemWebAdminRoleModel>.Map(model);
                    addModel.SystemRecordManager.CreatedBy = CreatedBy;
                    id = _systemWebAdminRoleRepositoryDAC.Add(addModel);
                    if (!string.IsNullOrEmpty(id))
                        scope.Complete();
                }
                return id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SystemWebAdminRoleViewModel Find(string id) => AutoMapperHelper<SystemWebAdminRoleModel, SystemWebAdminRoleViewModel>.Map(_systemWebAdminRoleRepositoryDAC.Find(id));

        public PageResultsViewModel<SystemWebAdminRoleViewModel> GetPage(string Search, int PageNo, int PageSize, string OrderColumn, string OrderDir) 
        {
            var result = new PageResultsViewModel<SystemWebAdminRoleViewModel>();
            var data = _systemWebAdminRoleRepositoryDAC.GetPage(Search, PageNo, PageSize, OrderColumn, OrderDir);
            result.Items = AutoMapperHelper<SystemWebAdminRoleModel, SystemWebAdminRoleViewModel>.MapList(data);
            result.TotalRows = data.Count > 0 ? data.FirstOrDefault().PageResult.TotalRows: 0;
            return result;
        } 

        public bool Remove(string id, string LastUpdatedBy)
        {
            var success = false;
            using (var scope = new TransactionScope())
            {
                success = _systemWebAdminRoleRepositoryDAC.Remove(id, LastUpdatedBy);
                if (success)
                    scope.Complete();
            }
            return success;
        }

        public bool Update(UpdateSystemWebAdminRoleBindingModel model, string LastUpdatedBy)
        {
            var success = false;
            using (var scope = new TransactionScope())
            {
                var updateModel = AutoMapperHelper<UpdateSystemWebAdminRoleBindingModel, SystemWebAdminRoleModel>.Map(model);
                updateModel.SystemRecordManager.LastUpdatedBy = LastUpdatedBy;
                success = _systemWebAdminRoleRepositoryDAC.Update(updateModel);
                if (success)
                    scope.Complete();
            }
            return success;
        }
    }
}
