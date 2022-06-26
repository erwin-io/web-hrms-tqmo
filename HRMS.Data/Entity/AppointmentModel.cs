using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Data.Entity
{
    public class AppointmentModel
    {
        public string AppointmentId { get; set; }
        public PatientModel Patient { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string AppointmentTime { get; set; }
        public string PrimaryReason { get; set; }
        public DateTime DateSymtomsFirstNoted { get; set; }
        public string DescOfCharOfSymtoms { get; set; }
        public bool HasPrevMedTreatment { get; set; }
        public bool IsTakingBloodThinningDrugs { get; set; }
        public string PatientGuardian { get; set; }
        public string PatientGuardianMobileNumber { get; set; }
        public string PatientRelative { get; set; }
        public string PatientRelativeMobileNumber { get; set; }
        public AppointmentStatusModel AppointmentStatus { get; set; }
        public SystemUserModel ProcessedBy { get; set; }
        public SystemRecordManagerModel SystemRecordManager { get; set; }
        public EntityStatusModel EntityStatus { get; set; }
        public PageResultsModel PageResult { get; set; }
    }
}
