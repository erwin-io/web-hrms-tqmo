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
    public class SystemUserVerificationDAC : RepositoryBase<SystemUserVerificationModel>, ISystemUserVerificationRepositoryDAC
    {
        private readonly IDbConnection _dBConnection;

        #region CONSTRUCTORS
        public SystemUserVerificationDAC(IDbConnection dbConnection)
        {
            _dBConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        }
        #endregion

        public override string Add(SystemUserVerificationModel model)
        {
            try
            {
                var id = Convert.ToString(_dBConnection.ExecuteScalar("usp_systemuserverification_add", new
                {
                    model.VerificationSender,
                    model.VerificationTypeId,
                    model.VerificationCode
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

        public override SystemUserVerificationModel Find(string id)
        {
            try
            {
                return _dBConnection.Query<SystemUserVerificationModel>("usp_systemuserverification_getByID", new
                {
                    Id = id,
                }, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public SystemUserVerificationModel FindBySender(string sender, string code)
        {
            try
            {
                return _dBConnection.Query<SystemUserVerificationModel>("usp_systemuserverification_getBySender", new
                {
                    VerificationSender = sender,
                    VerificationCode = code,
                }, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override List<SystemUserVerificationModel> GetAll() => throw new NotImplementedException();
        public override bool Remove(string id) => throw new NotImplementedException();

        public override bool Update(SystemUserVerificationModel model) => throw new NotImplementedException();

        public bool VerifyUser(long id)
        {
            bool success = false;
            try
            {
                int affectedRows = 0;
                var result = Convert.ToString(_dBConnection.ExecuteScalar("usp_systemuserverification_verifyUser", new
                {
                    Id = id
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
