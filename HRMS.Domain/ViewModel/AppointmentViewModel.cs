﻿using System;
using System.Collections.Generic;

namespace HRMS.Domain.ViewModel
{
    public class AppointmentViewModel
    {
        public string AppointmentId { get; set; }
        public PatientViewModel Patient { get; set; }
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
        public AppointmentStatusViewModel AppointmentStatus { get; set; }
        public SystemUserViewModel ProcessedBy { get; set; }
        public SystemRecordManagerViewModel SystemRecordManager { get; set; }
        public EntityStatusViewModel EntityStatus { get; set; }
    }
}
