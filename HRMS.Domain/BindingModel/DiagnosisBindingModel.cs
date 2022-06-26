using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.BindingModel
{
    public class DiagnosisBindingModel
    {
        public DateTime DiagnosisDate { get; set; }
        public string DescOfDiagnosis { get; set; }
        public string DescOfTreatment { get; set; }
    }
    public class CreateDiagnosisBindingModel : DiagnosisBindingModel
    {
        public string AppointmentId { get; set; }
    }
    public class UpdateDiagnosisBindingModel : DiagnosisBindingModel
    {
        public string DiagnosisId { get; set; }
    }
    public class UpdateDiagnosisStatusBindingModel
    {
        public bool IsActive { get; set; }
        public string DiagnosisId { get; set; }
    }
}
