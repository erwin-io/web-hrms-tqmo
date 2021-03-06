using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Facade.Interface
{
    public interface ISystemUserVerificationFacade
    {
        string Add(SystemUserVerificationBindingModel model, string code);
        SystemUserVerificationViewModel FindById(string id);
        SystemUserVerificationViewModel FindBySender(string sender, string code);
    }
}
