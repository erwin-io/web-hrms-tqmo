using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Data.Entity
{
    public class LookupTableModel
    {
        public string LookupName { get; set; }
        public List<LookupModel> LookupData { get; set; }
    }
}
