using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.ViewModel
{
    public class SystemWebAdminMenuViewModel
    {
        public long? SystemWebAdminMenuId { get; set; }
        public SystemWebAdminModuleViewModel SystemWebAdminModule { get; set; }
        public string SystemWebAdminMenuName { get; set; }
    }
}
