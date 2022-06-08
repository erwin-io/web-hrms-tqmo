using System;
using System.Collections.Generic;

namespace HRMS.Domain.BindingModel
{
    public class SystemRefreshTokenBindingModel
    {
        public string TokenId { get; set; }
        public string UserId { get; set; }
        public string ClientId { get; set; }
        public string Subject { get; set; }
        public DateTime IssuedUtc { get; set; }
        public DateTime ExpiresUtc { get; set; }
        public string ProtectedTicket { get; set; }
        public string TokenType { get; set; }
    }
}
