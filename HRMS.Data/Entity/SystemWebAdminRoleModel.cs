using System;
using System.Collections.Generic;

namespace HRMS.Data.Entity
{
    public class SystemWebAdminRoleModel
    {
        public string SystemWebAdminRoleId { get; set; }
        public string RoleName { get; set; }
        public SystemRecordManagerModel SystemRecordManager { get; set; }
        public EntityStatusModel EntityStatus { get; set; }
        public PageResultsModel PageResult { get; set; }
    }
}
