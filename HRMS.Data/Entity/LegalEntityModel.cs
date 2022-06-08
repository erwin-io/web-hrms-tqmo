using System;
using System.Collections.Generic;

namespace HRMS.Data.Entity
{
    public class LegalEntityModel
    {
        public string LegalEntityId { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public EntityGenderModel Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public long? Age { get; set; }
        public string EmailAddress { get; set; }
        public string MobileNumber { get; set; }
        public List<LegalEntityAddressModel> LegalEntityAddress { get; set; }
        public EntityStatusModel EntityStatus { get; set; }
    }
}
