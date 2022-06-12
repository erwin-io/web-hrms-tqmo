using HRMS.Mapping;
using HRMS.Data.Entity;
using HRMS.Data.Interface;
using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using HRMS.Domain.Enumerations;
using HRMS.Facade.Interface;
using System;
using System.Collections.Generic;
using System.Transactions;
using System.Linq;
using System.IO;

namespace HRMS.Facade
{
    public class PatientFacade : IPatientFacade
    {
        private readonly IPatientRepositoryDAC _patientRepository;
        private readonly ILegalEntityRepository _legalEntityRepository;

        #region CONSTRUCTORS
        public PatientFacade(IPatientRepositoryDAC patientRepository, ILegalEntityRepository legalEntityRepository)
        {
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _legalEntityRepository = legalEntityRepository ?? throw new ArgumentNullException(nameof(legalEntityRepository));
        }
        #endregion

        public string Add(CreatePatientBindingModel model, string CreatedBy)
        {
            try
            {
                var id = string.Empty;
                using (var scope = new TransactionScope())
                {
                    var addModel = AutoMapperHelper<CreatePatientBindingModel, PatientModel>.Map(model);

                    //Start Saving LegalEntity
                    addModel.SystemRecordManager.CreatedBy = CreatedBy;
                    var legalEntityId = _legalEntityRepository.Add(addModel.LegalEntity);
                    addModel.LegalEntity.LegalEntityId = legalEntityId;
                    //End Saving LegalEntity

                    addModel.SystemRecordManager.CreatedBy = CreatedBy;
                    addModel.LegalEntity.LegalEntityId = legalEntityId;
                    id = _patientRepository.Add(addModel);
                    if (string.IsNullOrEmpty(id))
                    {
                        throw new Exception("Error Creating Patient");
                    }
                    scope.Complete();
                }
                return id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public PageResultsViewModel<PatientViewModel> GetPage(string Search, long PageNo, long PageSize, string OrderColumn, string OrderDir)
        {
            var result = new PageResultsViewModel<PatientViewModel>();
            var data = _patientRepository.GetPage(Search, PageNo, PageSize, OrderColumn, OrderDir);
            result.Items = AutoMapperHelper<PatientModel, PatientViewModel>.MapList(data);
            result.TotalRows = data.Count > 0 ? data.FirstOrDefault().PageResult.TotalRows : 0;
            return result;
        }
        public PatientViewModel Find(string id)
        {
            var result = AutoMapperHelper<PatientModel, PatientViewModel>.Map(_patientRepository.Find(id));
            return result;
        }
        public bool Remove(string id, string LastUpdatedBy)
        {
            var success = false;
            using (var scope = new TransactionScope())
            {
                success = _patientRepository.Remove(id, LastUpdatedBy);
                if(success)
                    scope.Complete();
            }
            return success;
        }

        public bool Update(UpdatePatientBindingModel model, string LastUpdatedBy)
        {
            var success = false;
            using (var scope = new TransactionScope())
            {
                var updateModel = AutoMapperHelper<UpdatePatientBindingModel, PatientModel>.Map(model);
                var patient = AutoMapperHelper<PatientModel, PatientViewModel>.Map(_patientRepository.Find(updateModel.PatientId));
                //Start Saving LegalEntity
                updateModel.SystemRecordManager.LastUpdatedBy = LastUpdatedBy;
                updateModel.LegalEntity.LegalEntityId = patient.LegalEntity.LegalEntityId;
                success = _legalEntityRepository.Update(updateModel.LegalEntity);
                if (!success)
                {
                    throw new Exception("Error Updating Patient");
                }
                //End Saving LegalEntity
                updateModel.SystemRecordManager.LastUpdatedBy = LastUpdatedBy;
                success = _patientRepository.Update(updateModel);
                if (!success)
                {
                    throw new Exception("Error Updating Patient");
                }

                scope.Complete();
            }
            return success;
        }
    }
}
