using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.ViewModel
{
    public class ItemViewModel
    {
        public string ItemBrandId { get; set; }
        public string ItemBrandName { get; set; }
        public string ItemBrandDescription { get; set; }
        public ItemTypeViewModel ItemType { get; set; }
        public ItemBrandViewModel ItemBrand { get; set; }
        public FileViewModel IconFile { get; set; }
        public SystemRecordManagerViewModel SystemRecordManager { get; set; }
        public EntityStatusViewModel EntityStatus { get; set; }
    }
}
