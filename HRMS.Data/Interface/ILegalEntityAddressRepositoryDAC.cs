using HRMS.Data.Core;
using HRMS.Data.Entity;
using System.Collections.Generic;

namespace HRMS.Data.Interface
{
    public interface ILegalEntityAddressRepositoryDAC : IRepository<LegalEntityAddressModel>
    {
        List<LegalEntityAddressModel> FindBySystemUserId(string SystemUserId);
        List<LegalEntityAddressModel> FindByLegalEntityId(string LegalEntityId);
    }
}