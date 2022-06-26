using System;
using System.Collections.Generic;

namespace HRMS.Domain.BindingModel
{
    public class AppointmentBindingModel
    {
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
    }
    public class CreateAppointmentBindingModel : AppointmentBindingModel
    {
        public string PatientId { get; set; }
        public bool IsPatientInRecord { get; set; }
        public bool IsUser { get; set; }
        public string SystemUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public long? GenderId { get; set; }
        public DateTime? BirthDate { get; set; }
        public string EmailAddress { get; set; }
        public string MobileNumber { get; set; }
        public long? CivilStatusId { get; set; }
        public string Occupation { get; set; }
        public string CompleteAddress { get; set; }
    }
    public class UpdateAppointmentBindingModel : AppointmentBindingModel
    {
        public string AppointmentId { get; set; }
    }
    public class UpdateAppointmentStatusBindingModel
    {
        public string AppointmentId { get; set; }
        public long? AppointmentStatusId { get; set; }
    }
}
