using HRMS.Data.Core;
using HRMS.Data.Interface;
using HRMS.Data.Entity;
using System.Collections.Generic;
using System.Data;
using System;
using Dapper;
using System.Linq;

namespace HRMS.Data
{
    public class DiagnosisDAC : RepositoryBase<DiagnosisModel>, IDiagnosisRepositoryDAC
    {
        private readonly IDbConnection _dBConnection;

        #region CONSTRUCTORS
        public DiagnosisDAC(IDbConnection dbConnection)
        {
            _dBConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        }
        #endregion

        public override string Add(DiagnosisModel model)
        {
            try
            {
                var id = Convert.ToString(_dBConnection.ExecuteScalar("usp_diagnosis_add", new
                {
                    model.Appointment.AppointmentId,
                    model.DiagnosisDate,
                    model.DescOfDiagnosis,
                    model.DescOfTreatment,
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

        public override DiagnosisModel Find(string id)
        {
            try
            {
                using (var result = _dBConnection.QueryMultiple("usp_diagnosis_getByID", new
                {
                    DiagnosisId = id
                }, commandType: CommandType.StoredProcedure))
                {
                    var model = result.Read<DiagnosisModel>().FirstOrDefault();
                    if (model != null)
                    {
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

        public override List<DiagnosisModel> GetAll() => throw new NotImplementedException();

        public List<DiagnosisModel> GetByAppointmentId(string AppointmentId)
        {
            var results = new List<DiagnosisModel>();
            try
            {
                var lookup = new Dictionary<string, DiagnosisModel>();

                _dBConnection.Query("usp_diagnosis_getByAppointmentId",
                new[]
                {
                    typeof(DiagnosisModel),
                }, obj =>
                {
                    DiagnosisModel d = obj[0] as DiagnosisModel;
                    DiagnosisModel model;
                    if (!lookup.TryGetValue(d.DiagnosisId, out model))
                        lookup.Add(d.DiagnosisId, model = d);
                    return model;
                },
                new
                {
                    AppointmentId = AppointmentId,
                }, splitOn: "DiagnosisId", commandType: CommandType.StoredProcedure).ToList();
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

        public List<DiagnosisModel> GetByPatientId(string PatientId)
        {
            var results = new List<DiagnosisModel>();
            try
            {
                var lookup = new Dictionary<string, DiagnosisModel>();

                _dBConnection.Query("usp_diagnosis_getByPatientId",
                new[]
                {
                    typeof(DiagnosisModel),
                    typeof(AppointmentModel),
                }, obj =>
                {
                    DiagnosisModel d = obj[0] as DiagnosisModel;
                    AppointmentModel a = obj[1] as AppointmentModel;
                    DiagnosisModel model;
                    if (!lookup.TryGetValue(d.DiagnosisId, out model))
                        lookup.Add(d.DiagnosisId, model = d);
                    if (model.Appointment == null)
                        model.Appointment = new AppointmentModel();
                    model.Appointment = a;
                    return model;
                },
                new
                {
                    PatientId = PatientId,
                }, splitOn: "DiagnosisId,AppointmentId", commandType: CommandType.StoredProcedure).ToList();
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
        public bool Remove(string id, string LastUpdatedBy)
        {
            bool success = false;
            try
            {
                int affectedRows = 0;
                var result = Convert.ToString(_dBConnection.ExecuteScalar("usp_diagnosis_delete", new
                {
                    DiagnosisId = id,
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

        public override bool Update(DiagnosisModel model)
        {
            bool success = false;
            try
            {
                int affectedRows = 0;
                var result = Convert.ToString(_dBConnection.ExecuteScalar("usp_diagnosis_update", new
                {
                    model.DiagnosisId,
                    model.DiagnosisDate,
                    model.DescOfDiagnosis,
                    model.DescOfTreatment,
                    model.SystemRecordManager.LastUpdatedBy
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

        public bool UpdateStatus(DiagnosisModel model)
        {
            bool success = false;
            try
            {
                int affectedRows = 0;
                var result = Convert.ToString(_dBConnection.ExecuteScalar("usp_diagnosis_updateStatus", new
                {
                    model.DiagnosisId,
                    model.IsActive,
                    model.SystemRecordManager.LastUpdatedBy
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
