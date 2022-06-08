using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Data.Entity
{
    public class SystemConfigModel
    {
        public long SystemConfigId { get; set; }
        public string ConfigName { get; set; }
        public string ConfigGroup { get; set; }
        public string ConfigKey { get; set; }
        public string ConfigValue { get; set; }
        public SystemConfigTypeModel SystemConfigType { get; set; }
        public bool IsUserConfigurable { get; set; }
    }
}
