using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Facade.Interface
{
    public interface IDiagnosisFacade
    {
        string Add(CreateDiagnosisBindingModel model, string CreatedBy);
        List<DiagnosisViewModel> GetByAppointmentId(string AppointmentId);
        List<DiagnosisViewModel> GetByPatientId(string PatientId);
        DiagnosisViewModel Find(string DiagnosisId);
        bool Remove(string id, string LastUpdatedBy);
        bool Update(UpdateDiagnosisBindingModel model, string LastUpdatedBy);
        bool UpdateStatus(UpdateDiagnosisStatusBindingModel model, string LastUpdatedBy);
    }
}
