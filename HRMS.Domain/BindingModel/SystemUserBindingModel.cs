using System;
using System.Collections.Generic;

namespace HRMS.Domain.BindingModel
{
    public class SystemUserBindingModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public long? GenderId { get; set; }
        public DateTime? BirthDate { get; set; }
        public string EmailAddress { get; set; }
        public string MobileNumber { get; set; }
        public List<SystemWebAdminUserRolesBindingModel> SystemWebAdminUserRoles { get; set; }
    }
    public class CreateSystemUserBindingModel : SystemUserBindingModel
    {
        public long SystemUserTypeId { get; set; }
        public FileBindingModel ProfilePicture { get; set; }
        public List<LegalEntityAddressBindingModel> LegalEntityAddress { get; set; }
    }

    public class CreateAccountSystemUserBindingModel
    {
        public string VerificationCode { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public DateTime? BirthDate { get; set; }
        public long? GenderId { get; set; }
    }

    public class CreateWebAccountSystemUserBindingModel : CreateAccountSystemUserBindingModel
    {
        public string EnforcementStationGuestCode { get; set; }
    }
    public class UpdateSystemUserBindingModel : SystemUserBindingModel
    {
        public string SystemUserId { get; set; }
        public UpdateFileBindingModel ProfilePicture { get; set; }
    }
    public class UpdateSystemUserNameBindingModel
    {
        public string SystemUserId { get; set; }
        public string UserName { get; set; }
    }
    public class UpdateSystemResetPasswordBindingModel
    {
        public string SystemUserId { get; set; }
        public string NewPassword { get; set; }
    }
    public class UpdateSystemPasswordBindingModel : UpdateSystemResetPasswordBindingModel
    {
        public string OldPassword { get; set; }
    }

    public class SystemUserStatusTrackerBindingModel
    {
        public string SystemUserId { get; set; }
        public DateTime LasteDateTimeActive { get; set; }
        public DateTime LasteDateTimeLogin { get; set; }
    }
}
