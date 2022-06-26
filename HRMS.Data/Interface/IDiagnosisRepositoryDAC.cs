using HRMS.Data.Core;
using HRMS.Data.Entity;
using System.Collections.Generic;

namespace HRMS.Data.Interface
{
    public interface IDiagnosisRepositoryDAC : IRepository<DiagnosisModel>
    {
        List<DiagnosisModel> GetByAppointmentId(string AppointmentId);
        List<DiagnosisModel> GetByPatientId(string PatientId);
        bool Remove(string id, string LastUpdatedBy);
        bool UpdateStatus(DiagnosisModel model);
    }
}
