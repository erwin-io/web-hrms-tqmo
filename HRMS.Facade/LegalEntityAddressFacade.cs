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
    public class LegalEntityAddressFacade : ILegalEntityAddressFacade
    {
        private readonly ILegalEntityAddressRepositoryDAC _legalEntityAddressRepositoryDAC;

        #region CONSTRUCTORS
        public LegalEntityAddressFacade(ILegalEntityAddressRepositoryDAC legalEntityAddressRepositoryDAC)
        {
            _legalEntityAddressRepositoryDAC = legalEntityAddressRepositoryDAC ?? throw new ArgumentNullException(nameof(legalEntityAddressRepositoryDAC));
        }
        #endregion

        public string Add(CreateLegalEntityAddressBindingModel model)
        {
            try
            {
                var id = string.Empty;
                using (var scope = new TransactionScope())
                {
                    id = _legalEntityAddressRepositoryDAC.Add(AutoMapperHelper<CreateLegalEntityAddressBindingModel, LegalEntityAddressModel>.Map(model));
                    if(!string.IsNullOrEmpty(id))
                        scope.Complete();
                }
                return id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<LegalEntityAddressViewModel> FindBySystemUserId(string SystemUserId) => AutoMapperHelper<LegalEntityAddressModel, LegalEntityAddressViewModel>.MapList(_legalEntityAddressRepositoryDAC.FindBySystemUserId(SystemUserId)).ToList();
        public List<LegalEntityAddressViewModel> FindByLegalEntityId(string LegalEntityId) => AutoMapperHelper<LegalEntityAddressModel, LegalEntityAddressViewModel>.MapList(_legalEntityAddressRepositoryDAC.FindByLegalEntityId(LegalEntityId)).ToList();
        public LegalEntityAddressViewModel Find(string id) => AutoMapperHelper<LegalEntityAddressModel, LegalEntityAddressViewModel>.Map(_legalEntityAddressRepositoryDAC.Find(id));

        public bool Remove(string id)
        {
            var success = false;
            using (var scope = new TransactionScope())
            {
                success = _legalEntityAddressRepositoryDAC.Remove(id);
                if(success)
                    scope.Complete();
            }
            return success;
        }

        public bool Update(UpdateLegalEntityAddressBindingModel model)
        {
            var success = false;
            using (var scope = new TransactionScope())
            {
                success = _legalEntityAddressRepositoryDAC.Update(AutoMapperHelper<UpdateLegalEntityAddressBindingModel, LegalEntityAddressModel>.Map(model));
                if(success)
                    scope.Complete();
            }
            return success;
        }
    }
}
