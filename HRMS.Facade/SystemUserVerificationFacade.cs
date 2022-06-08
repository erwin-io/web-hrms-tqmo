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
    public class SystemUserVerificationFacade : ISystemUserVerificationFacade
    {
        private readonly ISystemUserVerificationRepositoryDAC _systemUserVerificationRepositoryDAC;

        #region CONSTRUCTORS
        public SystemUserVerificationFacade(ISystemUserVerificationRepositoryDAC systemUserVerificationRepositoryDAC)
        {
            _systemUserVerificationRepositoryDAC = systemUserVerificationRepositoryDAC ?? throw new ArgumentNullException(nameof(systemUserVerificationRepositoryDAC));
        }
        #endregion

        public string Add(SystemUserVerificationBindingModel model, string code)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    var addModel = AutoMapperHelper<SystemUserVerificationBindingModel, SystemUserVerificationModel>.Map(model);
                    addModel.VerificationCode = code;
                    var id = _systemUserVerificationRepositoryDAC.Add(addModel);
                    if (string.IsNullOrEmpty(id))
                    {
                        throw new Exception("Error creating verification code");
                    }
                    scope.Complete();
                    return id;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public SystemUserVerificationViewModel FindById(string id) => AutoMapperHelper<SystemUserVerificationModel, SystemUserVerificationViewModel>.Map(_systemUserVerificationRepositoryDAC.Find(id));
        public SystemUserVerificationViewModel FindBySender(string sender, string code) => AutoMapperHelper<SystemUserVerificationModel, SystemUserVerificationViewModel>.Map(_systemUserVerificationRepositoryDAC.FindBySender(sender, code));
    }
}
