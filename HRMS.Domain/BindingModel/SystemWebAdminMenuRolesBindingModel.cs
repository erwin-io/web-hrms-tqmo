using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.BindingModel
{
    public class SystemWebAdminMenuRolesBindingModel
    {
        public string SystemWebAdminRoleId { get; set; }
        public List<SystemWebAdminMenuBindingModel> SystemWebAdminMenu { get; set; }
    }
    public class SystemWebAdminMenuBindingModel
    {
        public long? SystemWebAdminMenuId { get; set; }
        public bool IsAllowed { get; set; }
    }
}
