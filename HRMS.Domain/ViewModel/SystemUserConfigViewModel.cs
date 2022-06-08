using System;
using System.Collections.Generic;

namespace HRMS.Domain.ViewModel
{
    public class SystemUserConfigViewModel
    {
        public string SystemUserConfigId { get; set; }
        public SystemUserViewModel SystemUser { get; set; }
        public bool IsUserEnable { get; set; }
    }
}
