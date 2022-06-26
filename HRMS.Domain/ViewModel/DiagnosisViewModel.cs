using System;
using System.Collections.Generic;

namespace HRMS.Domain.ViewModel
{
    public class DiagnosisViewModel
    {
        public string DiagnosisId { get; set; }
        public DateTime DiagnosisDate { get; set; }
        public string DescOfDiagnosis { get; set; }
        public string DescOfTreatment { get; set; }
        public bool IsActive { get; set; }
        public AppointmentViewModel Appointment { get; set; }
        public SystemRecordManagerViewModel SystemRecordManager { get; set; }
        public EntityStatusViewModel EntityStatus { get; set; }
    }
}
