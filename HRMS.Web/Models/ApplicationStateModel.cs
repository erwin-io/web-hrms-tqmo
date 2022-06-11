using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Web.Models
{
    public class ApplicationStateModel
    {
        public ApplicationUserModel User { get; set; }
        public ApplicationTokenModel ApplicationToken { get; set; }
        public List<UserViewAccess> UserViewAccess { get; set; }
        public List<UserPrivilegeModel> Privileges { get; set; }
    }

    public class ApplicationUserModel
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string LegalEntityId { get; set; }
        public string Firstname { get; set; }
        public string Middlename { get; set; }
        public string Lastname { get; set; }
        public string FullName { get; set; }
        public string ProfilePictureSource { get; set; }
        public bool IsWebAdminGuestUser { get; set; }
        public ApplicationUserConfigModel SystemUserConfig { get; set; }
    }

    public class ApplicationUserConfigModel
    {
        public string SystemUserConfigId { get; set; }
        public bool IsUserEnable { get; set; }
        public bool IsUserAllowToPostNextReport { get; set; }
        public bool IsNextReportPublic { get; set; }
        public bool IsAnonymousNextReport { get; set; }
        public bool AllowReviewActionNextPost { get; set; }
        public bool AllowReviewCommentNextPost { get; set; }
        public bool IsAllReportPublic { get; set; }
        public bool IsAnonymousAllReport { get; set; }
        public bool AllowReviewActionAllReport { get; set; }
        public bool AllowReviewCommentAllReport { get; set; }
    }

    public class ApplicationTokenModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public class ApplicationErrorModel
    {
        public ExceptionModel Exception { get; set; }
    }

    public class ApplicationSateUserViewModel
    {
        public string MenuId { get; set; }
        public string ModuleName { get; set; }
        public string PageName { get; set; }
    }

    public class ApplicationSateUserViewAccess
    {
        public List<UserViewAccess> MenuRoles { get; set; }
    }

    public class UserViewAccess
    {
        public string MenuRoleId { get; set; }
        public string MenuId { get; set; }
        public string RoleId { get; set; }
        public string ModuleName { get; set; }
        public string PageName { get; set; }
    }

    public class UserPrivilegeModel
    {
        public long? SystemWebAdminPrivilegeId { get; set; }
        public string SystemWebAdminPrivilegeName { get; set; }
    }

    public class ApplicationActionExcecutingContextModel
    {
        public string Action { get; set; }
    }
}