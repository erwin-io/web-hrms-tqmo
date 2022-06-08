using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Facade.Interface
{
    public interface ILegalEntityAddressFacade
    {
        string Add(CreateLegalEntityAddressBindingModel model);
        List<LegalEntityAddressViewModel> FindBySystemUserId(string SystemUserId);
        List<LegalEntityAddressViewModel> FindByLegalEntityId(string LegalEntityId);
        LegalEntityAddressViewModel Find(string LegalEntityAddressId);
        bool Remove(string id);
        bool Update(UpdateLegalEntityAddressBindingModel model);
    }
}
