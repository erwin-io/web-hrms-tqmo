using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.ViewModel
{
    public class ItemBrandViewModel
    {
        public string ItemBrandId { get; set; }
        public string ItemBrandName { get; set; }
        public string ItemBrandDescription { get; set; }
        public FileViewModel IconFile { get; set; }
        public SystemRecordManagerViewModel SystemRecordManager { get; set; }
        public EntityStatusViewModel EntityStatus { get; set; }
    }
}
