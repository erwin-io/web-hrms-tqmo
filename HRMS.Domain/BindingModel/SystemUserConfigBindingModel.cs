using System;
using System.Collections.Generic;

namespace HRMS.Domain.BindingModel
{
    public class UpdateSystemUserConfigBindingModel
    {
        public string SystemUserConfigId { get; set; }
        public bool IsUserEnable { get; set; }
    }
}
