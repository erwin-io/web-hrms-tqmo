using System;
using System.Collections.Generic;

namespace HRMS.Data.Entity
{
    public class LegalEntityAddressModel
    {
        public string LegalEntityAddressId { get; set; }
        public LegalEntityModel LegalEntity { get; set; }
        public string Address { get; set; }
        public EntityStatusModel EntityStatus { get; set; }
    }
}
