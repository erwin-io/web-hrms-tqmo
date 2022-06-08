using System.Collections.Generic;

namespace HRMS.Domain.BindingModel
{
    public class SystemWebAdminRoleBindingModel
    {
        public string RoleName { get; set; }
    }
    public class UpdateSystemWebAdminRoleBindingModel : SystemWebAdminRoleBindingModel
    {
        public string SystemWebAdminRoleId { get; set; }
    }
}
