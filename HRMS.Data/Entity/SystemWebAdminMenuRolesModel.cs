using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Data.Entity
{
    public class SystemWebAdminMenuRolesModel
    {
        public string SystemWebAdminMenuRoleId { get; set; }
        public SystemWebAdminRoleModel SystemWebAdminRole { get; set; }
        public SystemWebAdminMenuModel SystemWebAdminMenu { get; set; }
        public bool IsAllowed { get; set; }
        public SystemRecordManagerModel SystemRecordManager { get; set; }
        public EntityStatusModel EntityStatus { get; set; }
    }
}
