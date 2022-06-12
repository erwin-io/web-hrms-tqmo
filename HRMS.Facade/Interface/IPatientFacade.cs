using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Facade.Interface
{
    public interface IPatientFacade
    {
        string Add(CreatePatientBindingModel model, string CreatedBy);
        PageResultsViewModel<PatientViewModel> GetPage(string Search, long PageNo, long PageSize, string OrderColumn, string OrderDir);
        PatientViewModel Find(string id);
        bool Remove(string id, string LastUpdatedBy);
        bool Update(UpdatePatientBindingModel model, string LastUpdatedBy);
    }
}
