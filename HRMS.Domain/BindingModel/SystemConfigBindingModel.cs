using System.Collections.Generic;

namespace HRMS.Domain.BindingModel
{
    public class SystemConfigBindingModel
    {
        public string ConfigName { get; set; }
        public string ConfigGroup { get; set; }
        public string ConfigKey { get; set; }
        public string ConfigValue { get; set; }
        public long SystemConfigTypeId { get; set; }
        public bool IsUserConfigurable { get; set; }
    }
    public class UpdateSystemConfigBindingModel : SystemConfigBindingModel
    {
        public long SystemConfigId { get; set; }
    }
}
