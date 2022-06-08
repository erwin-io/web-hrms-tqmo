using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.ViewModel
{
    public class SystemWebAdminMenuRolesViewModel
    {
        public string SystemWebAdminMenuRoleId { get; set; }
        public SystemWebAdminRoleViewModel SystemWebAdminRole { get; set; }
        public SystemWebAdminMenuViewModel SystemWebAdminMenu { get; set; }
        public bool IsAllowed { get; set; }
        public SystemRecordManagerViewModel SystemRecordManager { get; set; }
        public EntityStatusViewModel EntityStatus { get; set; }
    }
}
