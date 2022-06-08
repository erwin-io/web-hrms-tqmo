using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Facade.Interface
{
    public interface ILookupFacade
    {
        List<LookupTableViewModel> FindLookupByTableNames(string TableNames);
        List<LookupTableViewModel> FindEnforcementUnitByEnforcementStationId(string EnforcementStationId);
    }
}
