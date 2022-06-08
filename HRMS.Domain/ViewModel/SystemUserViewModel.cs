using System;
using System.Collections.Generic;

namespace HRMS.Domain.ViewModel
{
    public class SystemUserViewModel
    {
        public string SystemUserId { get; set; }
        public SystemUserTypeViewModel SystemUserType { get; set; }
        public FileViewModel ProfilePicture { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool HasFirstLogin { get; set; }
        public DateTime LasteDateTimeActive { get; set; }
        public DateTime LasteDateTimeLogin { get; set; }
        public bool IsWebAdminGuestUser { get; set; }
        public LegalEntityViewModel LegalEntity { get; set; }
        public SystemUserConfigViewModel SystemUserConfig { get; set; }
        public List<SystemWebAdminUserRolesViewModel> SystemWebAdminUserRoles { get; set; }
        public List<SystemWebAdminMenuViewModel> SystemWebAdminMenus { get; set; }
        public List<SystemWebAdminPrivilegesViewModel> SystemWebAdminPrivileges { get; set; }
        public SystemRecordManagerViewModel SystemRecordManager { get; set; }
        public EntityStatusViewModel EntityStatus { get; set; }
        public SystemTokenViewModel Token { get; set; }
    }
}
