using System;
using System.Collections.Generic;

namespace HRMS.Data.Entity
{
    public class SystemUserModel
    {
        public string SystemUserId { get; set; }
        public SystemUserTypeModel SystemUserType { get; set; }
        public FileModel ProfilePicture { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool HasFirstLogin { get; set; }
        public DateTime LasteDateTimeActive { get; set; }
        public DateTime LasteDateTimeLogin { get; set; }
        public bool IsWebAdminGuestUser { get; set; }
        public LegalEntityModel LegalEntity { get; set; }
        public SystemUserConfigModel SystemUserConfig { get; set; }
        public List<SystemWebAdminUserRolesModel> SystemWebAdminUserRoles { get; set; }
        public List<SystemWebAdminMenuModel> SystemWebAdminMenus { get; set; }
        public List<SystemWebAdminPrivilegesModel> SystemWebAdminPrivileges { get; set; }
        public SystemRecordManagerModel SystemRecordManager { get; set; }
        public EntityStatusModel EntityStatus { get; set; }
        public PageResultsModel PageResult { get; set; }
    }
}
