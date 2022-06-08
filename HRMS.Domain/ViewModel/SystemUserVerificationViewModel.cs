using System;
using System.Collections.Generic;

namespace HRMS.Domain.ViewModel
{
    public class SystemUserVerificationViewModel
    {
        public long Id { get; set; }
        public string VerificationSender { get; set; }
        public string VerificationTypeId { get; set; }
        public string VerificationCode { get; set; }
        public bool IsVerified { get; set; }
        public EntityStatusViewModel EntityStatus { get; set; }
    }
}
