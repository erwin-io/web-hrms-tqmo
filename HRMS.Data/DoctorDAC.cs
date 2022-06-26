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
    public class DoctorDAC : RepositoryBase<DoctorModel>, IDoctorRepositoryDAC
    {
        private readonly IDbConnection _dBConnection;

        #region CONSTRUCTORS
        public DoctorDAC(IDbConnection dbConnection)
        {
            _dBConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        }
        #endregion

        public override string Add(DoctorModel model)
        {
            try
            {
                var id = Convert.ToString(_dBConnection.ExecuteScalar("usp_doctor_add", new
                {
                    model.LegalEntity.LegalEntityId,
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

        public override DoctorModel Find(string id)
        {
            try
            {
                using (var result = _dBConnection.QueryMultiple("usp_doctor_getByID", new
                {
                    DoctorId = id
                }, commandType: CommandType.StoredProcedure))
                {
                    var model = result.Read<DoctorModel>().FirstOrDefault();
                    if (model != null)
                    {
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

        public override List<DoctorModel> GetAll() => throw new NotImplementedException();

        public List<DoctorModel> GetPage(string Search, long PageNo, long PageSize, string OrderColumn, string OrderDir)
        {
            var results = new List<DoctorModel>();
            try
            {
                var lookup = new Dictionary<string, DoctorModel>();

                _dBConnection.Query("usp_doctor_getPaged",
                new[]
                {
                    typeof(DoctorModel),
                    typeof(LegalEntityModel),
                    typeof(EntityGenderModel),
                    typeof(PageResultsModel),
                }, obj =>
                {
                    DoctorModel d = obj[0] as DoctorModel;
                    LegalEntityModel le = obj[1] as LegalEntityModel;
                    EntityGenderModel eg = obj[2] as EntityGenderModel;
                    PageResultsModel pr = obj[3] as PageResultsModel;

                    DoctorModel model;
                    if (!lookup.TryGetValue(d.DoctorId, out model))
                        lookup.Add(d.DoctorId, model = d);
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
                }, splitOn: "DoctorId,LegalEntityId,GenderId,TotalRows", commandType: CommandType.StoredProcedure).ToList();
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
                var result = Convert.ToString(_dBConnection.ExecuteScalar("usp_doctor_delete", new
                {
                    DoctorId = id,
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

        public override bool Update(DoctorModel model)
        {
            bool success = false;
            try
            {
                int affectedRows = 0;
                var result = Convert.ToString(_dBConnection.ExecuteScalar("usp_doctor_update", new
                {
                    model.DoctorId,
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
