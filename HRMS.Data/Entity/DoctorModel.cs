using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Data.Entity
{
    public class DoctorModel
    {
        public string DoctorId { get; set; }
        public string CompleteAddress { get; set; }
        public LegalEntityModel LegalEntity { get; set; }
        public SystemRecordManagerModel SystemRecordManager { get; set; }
        public EntityStatusModel EntityStatus { get; set; }
        public PageResultsModel PageResult { get; set; }
    }
}
