using HRMS.Data.Core;
using HRMS.Data.Entity;
using System.Collections.Generic;

namespace HRMS.Data.Interface
{
    public interface ISystemUserVerificationRepositoryDAC : IRepository<SystemUserVerificationModel>
    {
        SystemUserVerificationModel FindBySender(string sender, string code);
        bool VerifyUser(long id);
    }
}
