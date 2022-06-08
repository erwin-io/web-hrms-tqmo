using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Facade.Interface
{
    public interface ISystemTokenFacade
    {
        string Add(SystemRefreshTokenBindingModel model);
        SystemRefreshTokenViewModel Find(string hashedTokenId);
    }
}
