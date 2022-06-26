using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.Enumerations
{
    public enum SYSTEM_USER_TYPE_ENUMS
    {
        USER_WEBADMIN = 1,
        USER_PORTAL = 2
    }
    public enum ICON_PROFILE_FORMAT_ENUMS
    {
        NOTSUPPORTED = 0,
        PNG = 1,
        JPEG = 2,
        JPG = 3
    }
    public enum MOBILE_REPORTMEDIA_FORMAT_ENUMS
    {
        NOTSUPPORTED = 0,
        PNG = 1,
        JPEG = 2,
        JPG = 3,
        MP4 = 4
    }
    public enum DOC_REPORT_MEDIA_TYPE_ENUMS
    {
        IMAGE = 1,
        VIDEO = 2,
        AUDIO = 3
    }
    public enum HRMSServerStatusEnums
    {
        ACTIVE = 1,
        DISABLED = 2,
    }
    public enum APPOINTMENT_STATUS
    {
        Pending = 1,
        Processed = 2,
        Approved = 3,
        Completed = 4,
        Canceled = 5,
        Declined = 6
    }
}
