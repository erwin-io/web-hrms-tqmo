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
    public class SystemTokenFacade : ISystemTokenFacade
    {
        private readonly ISystemTokenRepositoryDAC _systemTokenRepositoryDAC;

        #region CONSTRUCTORS
        public SystemTokenFacade(ISystemTokenRepositoryDAC systemTokenRepositoryDAC)
        {
            _systemTokenRepositoryDAC = systemTokenRepositoryDAC ?? throw new ArgumentNullException(nameof(systemTokenRepositoryDAC));
        }
        #endregion

        public string Add(SystemRefreshTokenBindingModel model)
        {
            try
            {
                var id = _systemTokenRepositoryDAC.Add(AutoMapperHelper<SystemRefreshTokenBindingModel, SystemTokenModel>.Map(model));
                return id;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public SystemRefreshTokenViewModel Find(string hashedTokenId) => AutoMapperHelper<SystemTokenModel, SystemRefreshTokenViewModel>.Map(_systemTokenRepositoryDAC.Find(hashedTokenId));
    }
}
