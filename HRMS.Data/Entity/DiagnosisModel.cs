using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Data.Entity
{
    public class DiagnosisModel
    {
        public string DiagnosisId { get; set; }
        public DateTime DiagnosisDate { get; set; }
        public string DescOfDiagnosis { get; set; }
        public string DescOfTreatment { get; set; }
        public bool IsActive { get; set; }
        public AppointmentModel Appointment { get; set; }
        public SystemRecordManagerModel SystemRecordManager { get; set; }
        public EntityStatusModel EntityStatus { get; set; }
        public PageResultsModel PageResult { get; set; }
    }
}
