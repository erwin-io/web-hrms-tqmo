using System;
using System.Collections.Generic;

namespace HRMS.Domain.ViewModel
{
    public class LegalEntityViewModel
    {
        public string LegalEntityId { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public EntityGenderViewModel Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public long? Age { get; set; }
        public string EmailAddress { get; set; }
        public long? MobileNumber { get; set; }
        public List<LegalEntityAddressViewModel> LegalEntityAddress { get; set; }
        public EntityStatusViewModel EntityStatus { get; set; }
    }
}
