using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Data.Entity
{
    public class SystemTokenModel
    {
        public string TokenId { get; set; }
        public SystemUserModel SystemUser { get; set; }
        public string ClientId { get; set; }
        public string Subject { get; set; }
        public DateTime IssuedUtc { get; set; }
        public DateTime ExpiresUtc { get; set; }
        public string ProtectedTicket { get; set; }
        public string TokenType { get; set; }
    }
}
