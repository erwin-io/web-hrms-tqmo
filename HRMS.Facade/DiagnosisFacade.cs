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
    public class DiagnosisFacade : IDiagnosisFacade
    {
        private readonly IDiagnosisRepositoryDAC _diagnosisRepositoryDAC;

        #region CONSTRUCTORS
        public DiagnosisFacade(IDiagnosisRepositoryDAC diagnosisRepositoryDAC)
        {
            _diagnosisRepositoryDAC = diagnosisRepositoryDAC ?? throw new ArgumentNullException(nameof(diagnosisRepositoryDAC));
        }
        #endregion

        public string Add(CreateDiagnosisBindingModel model, string CreatedBy)
        {
            try
            {
                var id = string.Empty;
                using (var scope = new TransactionScope())
                {
                    var addModel = AutoMapperHelper<CreateDiagnosisBindingModel, DiagnosisModel>.Map(model);
                    addModel.SystemRecordManager.CreatedBy = CreatedBy;
                    id = _diagnosisRepositoryDAC.Add(addModel);
                    if (!string.IsNullOrEmpty(id))
                        scope.Complete();
                }
                return id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DiagnosisViewModel> GetByAppointmentId(string AppointmentId) => AutoMapperHelper<DiagnosisModel, DiagnosisViewModel>.MapList(_diagnosisRepositoryDAC.GetByAppointmentId(AppointmentId)).ToList();
        public List<DiagnosisViewModel> GetByPatientId(string PatientId) => AutoMapperHelper<DiagnosisModel, DiagnosisViewModel>.MapList(_diagnosisRepositoryDAC.GetByPatientId(PatientId)).ToList();
        public DiagnosisViewModel Find(string id) => AutoMapperHelper<DiagnosisModel, DiagnosisViewModel>.Map(_diagnosisRepositoryDAC.Find(id));

        public bool Remove(string id, string LastUpdatedBy)
        {
            var success = false;
            using (var scope = new TransactionScope())
            {
                success = _diagnosisRepositoryDAC.Remove(id, LastUpdatedBy);
                if (success)
                    scope.Complete();
            }
            return success;
        }

        public bool Update(UpdateDiagnosisBindingModel model, string LastUpdatedBy)
        {
            var success = false;
            using (var scope = new TransactionScope())
            {
                var updateModel = AutoMapperHelper<UpdateDiagnosisBindingModel, DiagnosisModel>.Map(model);
                updateModel.SystemRecordManager.LastUpdatedBy = LastUpdatedBy;
                success = _diagnosisRepositoryDAC.Update(updateModel);
                if (success)
                    scope.Complete();
            }
            return success;
        }

        public bool UpdateStatus(UpdateDiagnosisStatusBindingModel model, string LastUpdatedBy)
        {
            var success = false;
            using (var scope = new TransactionScope())
            {
                var updateModel = AutoMapperHelper<UpdateDiagnosisStatusBindingModel, DiagnosisModel>.Map(model);
                updateModel.SystemRecordManager.LastUpdatedBy = LastUpdatedBy;
                success = _diagnosisRepositoryDAC.UpdateStatus(updateModel);
                if (success)
                    scope.Complete();
            }
            return success;
        }
    }
}
