using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Data.Entity
{
    public class SystemWebAdminUserRolesModel
    {
        public string SystemWebAdminUserRoleId { get; set; }
        public SystemWebAdminRoleModel SystemWebAdminRole { get; set; }
        public SystemUserModel SystemUser { get; set; }
        public SystemRecordManagerModel SystemRecordManager { get; set; }
        public EntityStatusModel EntityStatus { get; set; }
        public PageResultsModel PageResult { get; set; }
    }
}
