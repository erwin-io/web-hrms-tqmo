using HRMS.Data.Core;
using HRMS.Data.Interface;
using HRMS.Data.Entity;
using HRMS.Domain.Enumerations;
using System.Collections.Generic;
using System.Data;
using System;
using Dapper;
using System.Linq;

namespace HRMS.Data
{
    public class AppointmentDAC : RepositoryBase<AppointmentModel>, IAppointmentRepositoryDAC
    {
        private readonly IDbConnection _dBConnection;

        #region CONSTRUCTORS
        public AppointmentDAC(IDbConnection dbConnection)
        {
            _dBConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        }
        #endregion

        public override string Add(AppointmentModel model)
        {
            try
            {
                var id = Convert.ToString(_dBConnection.ExecuteScalar("usp_appointment_add", new
                {
                    model.Patient.PatientId,
                    model.AppointmentDate,
                    model.AppointmentTime,
                    model.PrimaryReason,
                    model.DateSymtomsFirstNoted,
                    model.DescOfCharOfSymtoms,
                    model.HasPrevMedTreatment,
                    model.IsTakingBloodThinningDrugs,
                    model.PatientGuardian,
                    model.PatientGuardianMobileNumber,
                    model.PatientRelative,
                    model.PatientRelativeMobileNumber,
                    model.SystemRecordManager.CreatedBy,
                }, commandType: CommandType.StoredProcedure));

                if (id.Contains("Error"))
                    throw new Exception(id);

                return id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override AppointmentModel Find(string id)
        {
            try
            {
                using (var result = _dBConnection.QueryMultiple("usp_appointment_getByID", new
                {
                    AppointmentId = id,
                }, commandType: CommandType.StoredProcedure))
                {
                    var model = result.Read<AppointmentModel>().FirstOrDefault();
                    if (model != null)
                    {
                        model.AppointmentStatus = result.Read<AppointmentStatusModel>().FirstOrDefault();
                        model.Patient = result.Read<PatientModel>().FirstOrDefault();
                        if (model.Patient.CivilStatus == null)
                            model.Patient.CivilStatus = new CivilStatusModel();
                        model.Patient.CivilStatus = result.Read<CivilStatusModel>().FirstOrDefault();
                        if (model.Patient.LegalEntity == null)
                            model.Patient.LegalEntity = new LegalEntityModel();
                        model.Patient.LegalEntity = result.Read<LegalEntityModel>().FirstOrDefault();
                        if (model.Patient.LegalEntity.Gender == null)
                            model.Patient.LegalEntity.Gender = new EntityGenderModel();
                        model.Patient.LegalEntity.Gender = result.Read<EntityGenderModel>().FirstOrDefault();
                        model.ProcessedBy = result.Read<SystemUserModel>().FirstOrDefault();
                        if (model.ProcessedBy.LegalEntity == null)
                            model.ProcessedBy.LegalEntity = new LegalEntityModel();
                        model.ProcessedBy.LegalEntity = result.Read<LegalEntityModel>().FirstOrDefault();
                        model.SystemRecordManager = result.Read<SystemRecordManagerModel>().FirstOrDefault();
                        model.EntityStatus = result.Read<EntityStatusModel>().FirstOrDefault();
                    }

                    return model;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override List<AppointmentModel> GetAll() => throw new NotImplementedException();

        public List<AppointmentModel> GetPage(string Search,
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
            var results = new List<AppointmentModel>();
            try
            {
                var lookup = new Dictionary<string, AppointmentModel>();

                _dBConnection.Query("usp_appointment_getPaged",
                new[]
                {
                    typeof(AppointmentModel),
                    typeof(PatientModel),
                    typeof(LegalEntityModel),
                    typeof(AppointmentStatusModel),
                    typeof(PageResultsModel),
                }, obj =>
                {
                    AppointmentModel a = obj[0] as AppointmentModel;
                    PatientModel p = obj[1] as PatientModel;
                    LegalEntityModel le = obj[2] as LegalEntityModel;
                    AppointmentStatusModel astat = obj[3] as AppointmentStatusModel;
                    PageResultsModel pr = obj[4] as PageResultsModel;
                    AppointmentModel model;
                    if (!lookup.TryGetValue(a.AppointmentId, out model))
                        lookup.Add(a.AppointmentId, model = a);
                    if (model.Patient == null)
                        model.Patient = new PatientModel();
                    if (model.Patient.LegalEntity == null)
                        model.Patient.LegalEntity = new LegalEntityModel();
                    if (model.AppointmentStatus == null)
                        model.AppointmentStatus = new AppointmentStatusModel();
                    if (model.PageResult == null)
                        model.PageResult = new PageResultsModel();
                    model.Patient = p;
                    model.Patient.LegalEntity = le;
                    model.AppointmentStatus = astat;
                    model.PageResult = pr;
                    return model;
                },
                new
                {
                    Search = Search,
                    IsAdvanceSearchMode = IsAdvanceSearchMode,
                    AppointmentId = AppointmentId,
                    Patient = Patient,
                    AppointmentDateFrom = AppointmentDateFrom,
                    AppointmentDateTo = AppointmentDateTo,
                    AppointmentStatusId = AppointmentStatusId,
                    ProcessedBy = ProcessedBy,
                    PageNo = PageNo,
                    PageSize = PageSize,
                    OrderColumn = OrderColumn,
                    OrderDir = OrderDir
                }, splitOn: "AppointmentId,PatientId,LegalEntityId,AppointmentStatusId,TotalRows", commandType: CommandType.StoredProcedure).ToList();
                if (lookup.Values.Any())
                {
                    results.AddRange(lookup.Values);
                }
                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public List<AppointmentModel> GetPageBySystemUserId(string Search,
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
            var results = new List<AppointmentModel>();
            try
            {
                var lookup = new Dictionary<string, AppointmentModel>();

                _dBConnection.Query("usp_appointment_getPagedBySystemUserId",
                new[]
                {
                    typeof(AppointmentModel),
                    typeof(AppointmentStatusModel),
                    typeof(PageResultsModel),
                }, obj =>
                {
                    AppointmentModel a = obj[0] as AppointmentModel;
                    AppointmentStatusModel astat = obj[1] as AppointmentStatusModel;
                    PageResultsModel pr = obj[2] as PageResultsModel;
                    AppointmentModel model;
                    if (!lookup.TryGetValue(a.AppointmentId, out model))
                        lookup.Add(a.AppointmentId, model = a);
                    if (model.AppointmentStatus == null)
                        model.AppointmentStatus = new AppointmentStatusModel();
                    if (model.PageResult == null)
                        model.PageResult = new PageResultsModel();
                    model.AppointmentStatus = astat;
                    model.PageResult = pr;
                    return model;
                },
                new
                {
                    Search = Search,
                    IsAdvanceSearchMode = IsAdvanceSearchMode,
                    CreatedBy = CreatedBy,
                    AppointmentId = AppointmentId,
                    AppointmentDateFrom = AppointmentDateFrom,
                    AppointmentDateTo = AppointmentDateTo,
                    AppointmentStatusId = AppointmentStatusId,
                    PageNo = PageNo,
                    PageSize = PageSize,
                    OrderColumn = OrderColumn,
                    OrderDir = OrderDir
                }, splitOn: "AppointmentId,AppointmentStatusId,TotalRows", commandType: CommandType.StoredProcedure).ToList();
                if (lookup.Values.Any())
                {
                    results.AddRange(lookup.Values);
                }
                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override bool Remove(string id) => throw new NotImplementedException();
        public override bool Update(AppointmentModel model) => throw new NotImplementedException();

        public bool Update(AppointmentModel model, string LastUpdatedBy)
        {
            bool success = false;
            try
            {
                int affectedRows = 0;
                var result = Convert.ToString(_dBConnection.ExecuteScalar("usp_appointment_update", new
                {
                    model.AppointmentId,
                    model.AppointmentDate,
                    model.AppointmentTime,
                    model.PrimaryReason,
                    model.DateSymtomsFirstNoted,
                    model.DescOfCharOfSymtoms,
                    model.HasPrevMedTreatment,
                    model.IsTakingBloodThinningDrugs,
                    model.PatientGuardian,
                    model.PatientGuardianMobileNumber,
                    model.PatientRelative,
                    model.PatientRelativeMobileNumber,
                    LastUpdatedBy = LastUpdatedBy
                }, commandType: CommandType.StoredProcedure));

                if (result.Contains("Error"))
                    throw new Exception(result);

                affectedRows = Convert.ToInt32(result);
                success = affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return success;
        }

        public bool UpdateStatus(AppointmentModel model, string LastUpdatedBy)
        {
            bool success = false;
            try
            {
                int affectedRows = 0;
                var result = Convert.ToString(_dBConnection.ExecuteScalar("usp_appointment_updateStatus", new
                {
                    model.AppointmentId,
                    ProcessedBy = model.ProcessedBy.SystemUserId,
                    model.AppointmentStatus.AppointmentStatusId,
                    LastUpdatedBy = LastUpdatedBy
                }, commandType: CommandType.StoredProcedure));

                if (result.Contains("Error"))
                    throw new Exception(result);

                affectedRows = Convert.ToInt32(result);
                success = affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return success;
        }
    }
}
