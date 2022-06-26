using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Data.Entity
{
    public class PatientModel
    {
        public string PatientId { get; set; }
        public string IsNew { get; set; }
        public bool IsUser { get; set; }
        public string SystemUserId { get; set; }
        public string Occupation { get; set; }
        public CivilStatusModel CivilStatus { get; set; }
        public LegalEntityModel LegalEntity { get; set; }
        public SystemRecordManagerModel SystemRecordManager { get; set; }
        public EntityStatusModel EntityStatus { get; set; }
        public PageResultsModel PageResult { get; set; }
    }
}
