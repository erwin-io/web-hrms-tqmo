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
    public class AppointmentFacade : IAppointmentFacade
    {
        private readonly IAppointmentRepositoryDAC _appointmentRepository;
        private readonly IPatientRepositoryDAC _patientRepository;
        private readonly ILegalEntityRepository _legalEntityRepository;
        private readonly ISystemUserRepositoryDAC _systemUserRepositoryDAC;

        #region CONSTRUCTORS
        public AppointmentFacade(IAppointmentRepositoryDAC appointmentRepository, 
            IPatientRepositoryDAC patientRepository, 
            ILegalEntityRepository legalEntityRepository,
            ISystemUserRepositoryDAC systemUserRepositoryDAC)
        {
            _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _legalEntityRepository = legalEntityRepository ?? throw new ArgumentNullException(nameof(legalEntityRepository));
            _systemUserRepositoryDAC = systemUserRepositoryDAC ?? throw new ArgumentNullException(nameof(systemUserRepositoryDAC));
        }
        #endregion

        public string Add(CreateAppointmentBindingModel model, string CreatedBy)
        {
            try
            {
                var id = string.Empty;
                using (var scope = new TransactionScope())
                {
                    var addModel = AutoMapperHelper<CreateAppointmentBindingModel, AppointmentModel>.Map(model);

                    addModel.SystemRecordManager.CreatedBy = CreatedBy;
                    SystemUserModel getUser = null;
                    addModel.Patient.SystemUserId = string.IsNullOrEmpty(model.SystemUserId)? null : model.SystemUserId;
                    //verify if user exist
                    if (model.IsUser && !string.IsNullOrEmpty(model.SystemUserId))
                    {
                        getUser = _systemUserRepositoryDAC.Find(model.SystemUserId);
                        if(getUser == null)
                            throw new Exception("User not found");
                    }

                    PatientModel getPatient = null;
                    //recheck if patient exist using user id
                    if (!model.IsPatientInRecord && getUser != null && model.IsUser)
                    {
                        getPatient = _patientRepository.GetBySystemUserId(model.SystemUserId);
                        model.IsPatientInRecord = getPatient != null;
                        addModel.Patient.PatientId = model.IsPatientInRecord ? getPatient.PatientId : null;
                    }
                    else if(model.IsPatientInRecord)
                    {
                        //else use patient id
                        if(string.IsNullOrEmpty(model.PatientId))
                            throw new Exception("Invalid Patient Id");
                        getPatient = _patientRepository.Find(model.PatientId);
                        model.IsPatientInRecord = getPatient != null;
                    }

                    if (!model.IsPatientInRecord)
                    {
                        addModel.Patient.CivilStatus = new CivilStatusModel() { CivilStatusId = 1 };
                        addModel.Patient.Occupation = string.Empty;
                        addModel.Patient.LegalEntity = new LegalEntityModel()
                        {
                            FirstName = getUser.LegalEntity.FirstName,
                            MiddleName = getUser.LegalEntity.MiddleName,
                            LastName = getUser.LegalEntity.LastName,
                            Gender = getUser.LegalEntity.Gender,
                            BirthDate = getUser.LegalEntity.BirthDate,
                            EmailAddress = getUser.LegalEntity.EmailAddress,
                            MobileNumber = getUser.LegalEntity.MobileNumber,
                            CompleteAddress = getUser.LegalEntity.CompleteAddress,
                        };
                        //Start Saving LegalEntity
                        var legalEntityId = _legalEntityRepository.Add(addModel.Patient.LegalEntity);
                        if (string.IsNullOrEmpty(legalEntityId))
                            throw new Exception("Error Creating Patient");
                        addModel.Patient.LegalEntity.LegalEntityId = legalEntityId;
                        //End Saving LegalEntity
                        addModel.Patient.SystemRecordManager.CreatedBy = CreatedBy;
                        var patientId = _patientRepository.Add(addModel.Patient);
                        if (string.IsNullOrEmpty(patientId))
                            throw new Exception("Error Creating Patient");

                        addModel.Patient.PatientId = patientId;
                    }
                    else
                    {
                        addModel.Patient.PatientId = addModel.Patient.PatientId;
                    }

                    addModel.SystemRecordManager.CreatedBy = CreatedBy;
                    id = _appointmentRepository.Add(addModel);
                    if (string.IsNullOrEmpty(id))
                        throw new Exception("Error Creating Appointment");
                    scope.Complete();
                }
                return id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public PageResultsViewModel<AppointmentViewModel> GetPage(string Search,
                                                                   bool IsAdvanceSearchMode,
                                                                   string AppointmentId,
                                                                   string Patient,
                                                                   DateTime AppointmentDateFrom,
                                                                   DateTime AppointmentDateTo,
                                                                   string AppointmentStatusId,
                                                                   string ProcessedBy,
                                                                   int PageNo,
                                                                   int PageSize,
                                                                   string OrderColumn,
                                                                   string OrderDir)
        {
            var result = new PageResultsViewModel<AppointmentViewModel>();
            var data = _appointmentRepository.GetPage(Search,
                                                     IsAdvanceSearchMode,
                                                     AppointmentId,
                                                     Patient,
                                                     AppointmentDateFrom,
                                                     AppointmentDateTo,
                                                     AppointmentStatusId,
                                                     ProcessedBy,
                                                     PageNo,
                                                     PageSize,
                                                     OrderColumn,
                                                     OrderDir);
            result.Items = AutoMapperHelper<AppointmentModel, AppointmentViewModel>.MapList(data);
            result.TotalRows = data.Count > 0 ? data.FirstOrDefault().PageResult.TotalRows : 0;
            return result;
        }

        public PageResultsViewModel<AppointmentViewModel> GetPageBySystemUserId(string Search,
                                                                                bool IsAdvanceSearchMode,
                                                                                string CreatedBy,
                                                                                string AppointmentId,
                                                                                DateTime AppointmentDateFrom,
                                                                                DateTime AppointmentDateTo,
                                                                                string AppointmentStatusId,
                                                                                int PageNo,
                                                                                int PageSize,
                                                                                string OrderColumn,
                                                                                string OrderDir)
        {
            var result = new PageResultsViewModel<AppointmentViewModel>();
            var data = _appointmentRepository.GetPageBySystemUserId(Search,
                                                                    IsAdvanceSearchMode,
                                                                    CreatedBy,
                                                                    AppointmentId,
                                                                    AppointmentDateFrom,
                                                                    AppointmentDateTo,
                                                                    AppointmentStatusId,
                                                                    PageNo,
                                                                    PageSize,
                                                                    OrderColumn,
                                                                    OrderDir);
            result.Items = AutoMapperHelper<AppointmentModel, AppointmentViewModel>.MapList(data);
            result.TotalRows = data.Count > 0 ? data.FirstOrDefault().PageResult.TotalRows : 0;
            return result;
        }
        public AppointmentViewModel Find(string id)
        {
            var result = AutoMapperHelper<AppointmentModel, AppointmentViewModel>.Map(_appointmentRepository.Find(id));
            return result;
        }

        public bool Update(UpdateAppointmentBindingModel model, string LastUpdatedBy)
        {
            var success = false;
            using (var scope = new TransactionScope())
            {
                var updateModel = AutoMapperHelper<UpdateAppointmentBindingModel, AppointmentModel>.Map(model);
                updateModel.SystemRecordManager.LastUpdatedBy = LastUpdatedBy;
                success = _appointmentRepository.Update(updateModel, LastUpdatedBy);
                if (!success)
                {
                    throw new Exception("Error Updating Appointment");
                }

                scope.Complete();
            }
            return success;
        }
        public bool UpdateStatus(UpdateAppointmentStatusBindingModel model, string LastUpdatedBy)
        {
            var success = false;
            using (var scope = new TransactionScope())
            {
                var findAppointment = AutoMapperHelper<AppointmentModel, AppointmentViewModel>.Map(_appointmentRepository.Find(model.AppointmentId));
                var updateModel = AutoMapperHelper<UpdateAppointmentStatusBindingModel, AppointmentModel>.Map(model);

                if (updateModel.AppointmentStatus.AppointmentStatusId == (int)APPOINTMENT_STATUS.Pending)
                    updateModel.ProcessedBy = new SystemUserModel() { SystemUserId = findAppointment.SystemRecordManager.CreatedBy };
                else if (updateModel.AppointmentStatus.AppointmentStatusId == (int)APPOINTMENT_STATUS.Processed)
                    updateModel.ProcessedBy = new SystemUserModel() { SystemUserId = LastUpdatedBy };
                else if (updateModel.AppointmentStatus.AppointmentStatusId == (int)APPOINTMENT_STATUS.Approved)
                    updateModel.ProcessedBy = new SystemUserModel() { SystemUserId = LastUpdatedBy };
                else if (updateModel.AppointmentStatus.AppointmentStatusId == (int)APPOINTMENT_STATUS.Completed)
                    updateModel.ProcessedBy = new SystemUserModel() { SystemUserId = LastUpdatedBy };
                else if (updateModel.AppointmentStatus.AppointmentStatusId == (int)APPOINTMENT_STATUS.Canceled)
                    updateModel.ProcessedBy = new SystemUserModel() { SystemUserId = findAppointment.SystemRecordManager.CreatedBy };
                else if (updateModel.AppointmentStatus.AppointmentStatusId == (int)APPOINTMENT_STATUS.Declined)
                    updateModel.ProcessedBy = new SystemUserModel() { SystemUserId = LastUpdatedBy };
                success = _appointmentRepository.UpdateStatus(updateModel, LastUpdatedBy);
                if (success)
                    scope.Complete();
            }
            return success;
        }
    }
}
