using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Data.Entity
{
    public class SystemRecordManagerModel
    {
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedByFullName { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public string LastUpdatedByFullName { get; set; }
        
    }
}
