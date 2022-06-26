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
    public class LegalEntityDAC : RepositoryBase<LegalEntityModel>, ILegalEntityRepository
    {
        private readonly IDbConnection _dBConnection;

        #region CONSTRUCTORS
        public LegalEntityDAC(IDbConnection dbConnection)
        {
            _dBConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        }
        #endregion

        public override string Add(LegalEntityModel model)
        {
            try
            {
                var id = Convert.ToString(_dBConnection.ExecuteScalar("usp_legalentity_add", new
                {
                    model.FirstName,
                    model.LastName,
                    model.MiddleName,
                    model.Gender.GenderId,
                    model.BirthDate,
                    model.EmailAddress,
                    model.MobileNumber,
                    model.CompleteAddress,
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

        public override LegalEntityModel Find(string id) => throw new NotImplementedException();
        public override List<LegalEntityModel> GetAll() => throw new NotImplementedException();

        public override bool Remove(string id) => throw new NotImplementedException();

        public override bool Update(LegalEntityModel model)
        {
            bool success = false;
            try
            {
                int affectedRows = 0;
                var result = Convert.ToString(_dBConnection.ExecuteScalar("usp_legalentity_update", new
                {
                    model.LegalEntityId,
                    model.FirstName,
                    model.LastName,
                    model.MiddleName,
                    model.Gender.GenderId,
                    model.BirthDate,
                    model.EmailAddress,
                    model.MobileNumber,
                    model.CompleteAddress,
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
