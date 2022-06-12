using System;
using System.Collections.Generic;

namespace HRMS.Domain.BindingModel
{
    public class DoctorBindingModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public long? GenderId { get; set; }
        public DateTime? BirthDate { get; set; }
        public string EmailAddress { get; set; }
        public string MobileNumber { get; set; }
        public string CompleteAddress { get; set; }
    }
    public class CreateDoctorBindingModel : DoctorBindingModel
    {
    }
    public class UpdateDoctorBindingModel : DoctorBindingModel
    {
        public string DoctorId { get; set; }
        public string LegalEntityId { get; set; }
    }
}
