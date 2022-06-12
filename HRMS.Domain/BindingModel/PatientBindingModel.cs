using System;
using System.Collections.Generic;

namespace HRMS.Domain.BindingModel
{
    public class PatientBindingModel
    {
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
    public class CreatePatientBindingModel : PatientBindingModel
    {
    }
    public class UpdatePatientBindingModel : PatientBindingModel
    {
        public string PatientId { get; set; }
        public string LegalEntityId { get; set; }
    }
}
