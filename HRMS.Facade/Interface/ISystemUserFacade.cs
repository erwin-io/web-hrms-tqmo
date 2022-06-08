using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Facade.Interface
{
    public interface ISystemUserFacade
    {
        string Add(CreateSystemUserBindingModel model, string CreatedBy);
        string CreateAccount(CreateAccountSystemUserBindingModel model, FileBindingModel profilePic);
        string CreateWebAdminAccount(CreateAccountSystemUserBindingModel model, FileBindingModel profilePic);
        PageResultsViewModel<SystemUserViewModel> GetPage(string Search, long SystemUserType, long ApprovalStatus, long PageNo, long PageSize, string OrderColumn, string OrderDir);
        SystemUserViewModel Find(string id);
        SystemUserViewModel GetTrackerStatus(string id);
        SystemUserViewModel FindByUsername(string Username);
        SystemUserViewModel Find(string Username, string Password);
        bool Remove(string id, string LastUpdatedBy);
        bool Update(UpdateSystemUserBindingModel model, string LastUpdatedBy);
        bool UpdatePersonalDetails(UpdateSystemUserBindingModel model, string LastUpdatedBy);
        bool UpdateUsername(UpdateSystemUserNameBindingModel model, string LastUpdatedBy);
        bool UpdatePassword(UpdateSystemPasswordBindingModel model, string LastUpdatedBy);
        bool ResetPassword(UpdateSystemResetPasswordBindingModel model, string LastUpdatedBy);
        bool UpdateSystemUserConfig(UpdateSystemUserConfigBindingModel model);
    }
}
