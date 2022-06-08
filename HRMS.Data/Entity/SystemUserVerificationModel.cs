using System;
using System.Collections.Generic;

namespace HRMS.Data.Entity
{
    public class SystemUserVerificationModel
    {
        public long Id { get; set; }
        public string VerificationSender { get; set; }
        public string VerificationTypeId { get; set; }
        public string VerificationCode { get; set; }
        public bool IsVerified { get; set; }
        public EntityStatusModel EntityStatus { get; set; }
    }
}
