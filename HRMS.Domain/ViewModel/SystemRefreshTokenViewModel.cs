using Newtonsoft.Json;
using System;

namespace HRMS.Domain.ViewModel
{
    public class SystemRefreshTokenViewModel
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
