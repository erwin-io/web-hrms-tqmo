using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.ViewModel
{
    public class SystemWebAdminRolePrivilegesViewModel
    {
        public string SystemWebAdminRolePrivilegeId { get; set; }
        public SystemWebAdminRoleViewModel SystemWebAdminRole { get; set; }
        public SystemWebAdminPrivilegesViewModel SystemWebAdminPrivilege { get; set; }
        public bool IsAllowed { get; set; }
        public SystemRecordManagerViewModel SystemRecordManager { get; set; }
        public EntityStatusViewModel EntityStatus { get; set; }
    }
}
