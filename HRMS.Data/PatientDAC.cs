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
    public class PatientDAC : RepositoryBase<PatientModel>, IPatientRepositoryDAC
    {
        private readonly IDbConnection _dBConnection;

        #region CONSTRUCTORS
        public PatientDAC(IDbConnection dbConnection)
        {
            _dBConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        }
        #endregion

        public override string Add(PatientModel model)
        {
            try
            {
                var id = Convert.ToString(_dBConnection.ExecuteScalar("usp_patient_add", new
                {
                    model.LegalEntity.LegalEntityId,
                    model.CivilStatus.CivilStatusId,
                    model.Occupation,
                    model.CompleteAddress,
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

        public override PatientModel Find(string id)
        {
            try
            {
                using (var result = _dBConnection.QueryMultiple("usp_patient_getByID", new
                {
                    PatientId = id
                }, commandType: CommandType.StoredProcedure))
                {
                    var model = result.Read<PatientModel>().FirstOrDefault();
                    if (model != null)
                    {
                        model.CivilStatus = result.Read<CivilStatusModel>().FirstOrDefault();
                        model.LegalEntity = result.Read<LegalEntityModel>().FirstOrDefault();
                        model.LegalEntity.Gender = result.Read<EntityGenderModel>().FirstOrDefault();
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

        public override List<PatientModel> GetAll() => throw new NotImplementedException();

        public List<PatientModel> GetPage(string Search, long PageNo, long PageSize, string OrderColumn, string OrderDir)
        {
            var results = new List<PatientModel>();
            try
            {
                var lookup = new Dictionary<string, PatientModel>();

                _dBConnection.Query("usp_patient_getPaged",
                new[]
                {
                    typeof(PatientModel),
                    typeof(LegalEntityModel),
                    typeof(EntityGenderModel),
                    typeof(PageResultsModel),
                }, obj =>
                {
                    PatientModel p = obj[0] as PatientModel;
                    LegalEntityModel le = obj[1] as LegalEntityModel;
                    EntityGenderModel eg = obj[2] as EntityGenderModel;
                    PageResultsModel pr = obj[3] as PageResultsModel;

                    PatientModel model;
                    if (!lookup.TryGetValue(p.PatientId, out model))
                        lookup.Add(p.PatientId, model = p);
                    if (model.LegalEntity == null)
                        model.LegalEntity = new LegalEntityModel();
                    if (model.LegalEntity.Gender == null)
                        model.LegalEntity.Gender = new EntityGenderModel();
                    if (model.PageResult == null)
                        model.PageResult = new PageResultsModel();
                    model.LegalEntity = le;
                    model.LegalEntity.Gender = eg;
                    model.PageResult = pr;
                    return model;
                },
                new
                {
                    Search = Search,
                    PageNo = PageNo,
                    PageSize = PageSize,
                    OrderColumn = OrderColumn,
                    OrderDir = OrderDir
                }, splitOn: "PatientId,LegalEntityId,GenderId,TotalRows", commandType: CommandType.StoredProcedure).ToList();
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
                var result = Convert.ToString(_dBConnection.ExecuteScalar("usp_patient_delete", new
                {
                    PatientId = id,
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

        public override bool Update(PatientModel model)
        {
            bool success = false;
            try
            {
                int affectedRows = 0;
                var result = Convert.ToString(_dBConnection.ExecuteScalar("usp_patient_update", new
                {
                    model.PatientId,
                    model.CivilStatus.CivilStatusId,
                    model.Occupation,
                    model.CompleteAddress,
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
