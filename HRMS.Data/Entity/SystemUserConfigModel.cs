using System;
using System.Collections.Generic;

namespace HRMS.Data.Entity
{
    public class SystemUserConfigModel
    {
        public string SystemUserConfigId { get; set; }
        public SystemUserModel SystemUser { get; set; }
        public bool IsUserEnable { get; set; }
    }
}
