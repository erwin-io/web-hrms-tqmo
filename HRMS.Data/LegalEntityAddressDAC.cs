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
    public class LegalEntityAddressDAC : RepositoryBase<LegalEntityAddressModel>, ILegalEntityAddressRepositoryDAC
    {
        private readonly IDbConnection _dBConnection;

        #region CONSTRUCTORS
        public LegalEntityAddressDAC(IDbConnection dbConnection)
        {
            _dBConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        }
        #endregion

        public override string Add(LegalEntityAddressModel model)
        {
            try
            {
                var id = Convert.ToString(_dBConnection.ExecuteScalar("usp_legalentityaddress_add", new
                {
                    model.LegalEntity.LegalEntityId,
                    model.Address,
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

        public override LegalEntityAddressModel Find(string id)
        {
            try
            {
                using (var result = _dBConnection.QueryMultiple("usp_legalentityaddress_getById", new
                {
                    LegalEntityAddressId = id,
                }, commandType: CommandType.StoredProcedure))
                {
                    var model = result.Read<LegalEntityAddressModel>().FirstOrDefault();
                    if(model != null)
                    {
                        model.LegalEntity = result.Read<LegalEntityModel>().FirstOrDefault();
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

        public override List<LegalEntityAddressModel> GetAll() => throw new NotImplementedException();

        public List<LegalEntityAddressModel> FindBySystemUserId(string SystemUserId)
        {
            var results = new List<LegalEntityAddressModel>();
            try
            {
                var lookup = new Dictionary<string, LegalEntityAddressModel>();

                _dBConnection.Query("usp_legalentityaddress_getBySystemUserId",
                new[]
                {
                    typeof(LegalEntityAddressModel),
                    typeof(LegalEntityModel),
                }, obj =>
                {
                    LegalEntityAddressModel lea = obj[0] as LegalEntityAddressModel;
                    LegalEntityModel le = obj[1] as LegalEntityModel;
                    LegalEntityAddressModel model;
                    if (!lookup.TryGetValue(lea.LegalEntityAddressId, out model))
                        lookup.Add(lea.LegalEntityAddressId, model = lea);
                    if (model.LegalEntity == null)
                        model.LegalEntity = new LegalEntityModel();
                    model.LegalEntity = le;
                    return model;
                },
                new
                {
                    SystemUserId = SystemUserId,
                }, splitOn: "LegalEntityAddressId,LegalEntityId", commandType: CommandType.StoredProcedure).ToList();
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


        public List<LegalEntityAddressModel> FindByLegalEntityId(string LegalEntityId)
        {
            var results = new List<LegalEntityAddressModel>();
            try
            {
                var lookup = new Dictionary<string, LegalEntityAddressModel>();

                _dBConnection.Query("usp_legalentityaddress_getByLegalEntityId",
                new[]
                {
                    typeof(LegalEntityAddressModel),
                    typeof(LegalEntityModel),
                }, obj =>
                {
                    LegalEntityAddressModel lea = obj[0] as LegalEntityAddressModel;
                    LegalEntityModel le = obj[1] as LegalEntityModel;
                    LegalEntityAddressModel model;
                    if (!lookup.TryGetValue(lea.LegalEntityAddressId, out model))
                        lookup.Add(lea.LegalEntityAddressId, model = lea);
                    if (model.LegalEntity == null)
                        model.LegalEntity = new LegalEntityModel();
                    model.LegalEntity = le;
                    return model;
                },
                new
                {
                    LegalEntityId = LegalEntityId,
                }, splitOn: "LegalEntityAddressId,LegalEntityId", commandType: CommandType.StoredProcedure).ToList();
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
        public override bool Remove(string id)
        {
            bool success = false;
            try
            {
                int affectedRows = 0;
                var result = Convert.ToString(_dBConnection.ExecuteScalar("usp_legalentityaddress_delete", new
                {
                    LegalEntityAddressId = id,
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

        public override bool Update(LegalEntityAddressModel model)
        {
            bool success = false;
            try
            {
                int affectedRows = 0;
                var result = Convert.ToString(_dBConnection.ExecuteScalar("usp_legalentityaddress_update", new
                {
                    model.LegalEntityAddressId,
                    model.Address,
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
