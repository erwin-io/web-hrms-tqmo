using HRMS.Mapping;
using HRMS.Data.Entity;
using HRMS.Data.Interface;
using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using HRMS.Facade.Interface;
using System;
using System.Collections.Generic;
using System.Transactions;
using System.Linq;

namespace HRMS.Facade
{
    public class LookupFacade : ILookupFacade
    {
        private readonly ILookupTableRepositoryDAC _lookupTableRepositoryDAC;

        #region CONSTRUCTORS
        public LookupFacade(ILookupTableRepositoryDAC lookupTableRepositoryDAC)
        {
            _lookupTableRepositoryDAC = lookupTableRepositoryDAC ?? throw new ArgumentNullException(nameof(lookupTableRepositoryDAC));
        }
        #endregion

        public List<LookupTableViewModel> FindLookupByTableNames(string TableNames) => AutoMapperHelper<LookupTableModel, LookupTableViewModel>.MapList(_lookupTableRepositoryDAC.FindLookupByTableNames(TableNames)).ToList();
        public List<LookupTableViewModel> FindEnforcementUnitByEnforcementStationId(string EnforcementStationId) => AutoMapperHelper<LookupTableModel, LookupTableViewModel>.MapList(_lookupTableRepositoryDAC.FindEnforcementUnitByEnforcementStationId(EnforcementStationId)).ToList();
    }
}
