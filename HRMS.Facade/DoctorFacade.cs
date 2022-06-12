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
    public class DoctorFacade : IDoctorFacade
    {
        private readonly IDoctorRepositoryDAC _doctorRepository;
        private readonly ILegalEntityRepository _legalEntityRepository;

        #region CONSTRUCTORS
        public DoctorFacade(IDoctorRepositoryDAC doctorRepository, ILegalEntityRepository legalEntityRepository)
        {
            _doctorRepository = doctorRepository ?? throw new ArgumentNullException(nameof(doctorRepository));
            _legalEntityRepository = legalEntityRepository ?? throw new ArgumentNullException(nameof(legalEntityRepository));
        }
        #endregion

        public string Add(CreateDoctorBindingModel model, string CreatedBy)
        {
            try
            {
                var id = string.Empty;
                using (var scope = new TransactionScope())
                {
                    var addModel = AutoMapperHelper<CreateDoctorBindingModel, DoctorModel>.Map(model);

                    //Start Saving LegalEntity
                    addModel.SystemRecordManager.CreatedBy = CreatedBy;
                    var legalEntityId = _legalEntityRepository.Add(addModel.LegalEntity);
                    addModel.LegalEntity.LegalEntityId = legalEntityId;
                    //End Saving LegalEntity

                    addModel.SystemRecordManager.CreatedBy = CreatedBy;
                    addModel.LegalEntity.LegalEntityId = legalEntityId;
                    id = _doctorRepository.Add(addModel);
                    if (string.IsNullOrEmpty(id))
                    {
                        throw new Exception("Error Creating Doctor");
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
        public PageResultsViewModel<DoctorViewModel> GetPage(string Search, long PageNo, long PageSize, string OrderColumn, string OrderDir)
        {
            var result = new PageResultsViewModel<DoctorViewModel>();
            var data = _doctorRepository.GetPage(Search, PageNo, PageSize, OrderColumn, OrderDir);
            result.Items = AutoMapperHelper<DoctorModel, DoctorViewModel>.MapList(data);
            result.TotalRows = data.Count > 0 ? data.FirstOrDefault().PageResult.TotalRows : 0;
            return result;
        }
        public DoctorViewModel Find(string id)
        {
            var result = AutoMapperHelper<DoctorModel, DoctorViewModel>.Map(_doctorRepository.Find(id));
            return result;
        }
        public bool Remove(string id, string LastUpdatedBy)
        {
            var success = false;
            using (var scope = new TransactionScope())
            {
                success = _doctorRepository.Remove(id, LastUpdatedBy);
                if(success)
                    scope.Complete();
            }
            return success;
        }

        public bool Update(UpdateDoctorBindingModel model, string LastUpdatedBy)
        {
            var success = false;
            using (var scope = new TransactionScope())
            {
                var updateModel = AutoMapperHelper<UpdateDoctorBindingModel, DoctorModel>.Map(model);
                var doctor = AutoMapperHelper<DoctorModel, DoctorViewModel>.Map(_doctorRepository.Find(updateModel.DoctorId));
                //Start Saving LegalEntity
                updateModel.SystemRecordManager.LastUpdatedBy = LastUpdatedBy;
                updateModel.LegalEntity.LegalEntityId = doctor.LegalEntity.LegalEntityId;
                success = _legalEntityRepository.Update(updateModel.LegalEntity);
                if (!success)
                {
                    throw new Exception("Error Updating Doctor");
                }
                //End Saving LegalEntity
                updateModel.SystemRecordManager.LastUpdatedBy = LastUpdatedBy;
                success = _doctorRepository.Update(updateModel);
                if (!success)
                {
                    throw new Exception("Error Updating Doctor");
                }

                scope.Complete();
            }
            return success;
        }
    }
}
