using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Facade.Interface
{
    public interface IDoctorFacade
    {
        string Add(CreateDoctorBindingModel model, string CreatedBy);
        PageResultsViewModel<DoctorViewModel> GetPage(string Search, long PageNo, long PageSize, string OrderColumn, string OrderDir);
        DoctorViewModel Find(string id);
        bool Remove(string id, string LastUpdatedBy);
        bool Update(UpdateDoctorBindingModel model, string LastUpdatedBy);
    }
}
