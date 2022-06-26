using System;
using System.Collections.Generic;

namespace HRMS.Domain.ViewModel
{
    public class PatientViewModel
    {
        public string PatientId { get; set; }
        public string IsNew { get; set; }
        public string Occupation { get; set; }
        public CivilStatusViewModel CivilStatus { get; set; }
        public LegalEntityViewModel LegalEntity { get; set; }
        public SystemRecordManagerViewModel SystemRecordManager { get; set; }
        public EntityStatusViewModel EntityStatus { get; set; }
    }
}
