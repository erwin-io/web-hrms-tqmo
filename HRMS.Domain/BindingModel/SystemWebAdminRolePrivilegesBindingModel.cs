using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.BindingModel
{
    public class SystemWebAdminRolePrivilegesBindingModel
    {
        public string SystemWebAdminRoleId { get; set; }
        public List<SystemWebAdminPrivilegeBindingModel> SystemWebAdminPrivilege { get; set; }
    }
    public class SystemWebAdminPrivilegeBindingModel
    {
        public long? SystemWebAdminPrivilegeId { get; set; }
        public bool IsAllowed { get; set; }
    }
}
