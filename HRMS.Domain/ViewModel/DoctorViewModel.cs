using System;
using System.Collections.Generic;

namespace HRMS.Domain.ViewModel
{
    public class DoctorViewModel
    {
        public string DoctorId { get; set; }
        public LegalEntityViewModel LegalEntity { get; set; }
        public SystemRecordManagerViewModel SystemRecordManager { get; set; }
        public EntityStatusViewModel EntityStatus { get; set; }
    }
}
