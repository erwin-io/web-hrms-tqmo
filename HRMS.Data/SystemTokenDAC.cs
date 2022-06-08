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
    public class SystemTokenDAC : RepositoryBase<SystemTokenModel>, ISystemTokenRepositoryDAC
    {
        private readonly IDbConnection _dBConnection;

        #region CONSTRUCTORS
        public SystemTokenDAC(IDbConnection dbConnection)
        {
            _dBConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        }
        #endregion

        public override string Add(SystemTokenModel model)
        {
            try
            {
                var id = Convert.ToString(_dBConnection.ExecuteScalar("usp_systemtoken_add", new
                {
                    model.TokenId,
                    model.SystemUser.SystemUserId,
                    model.ClientId,
                    model.Subject,
                    model.IssuedUtc,
                    model.ExpiresUtc,
                    model.ProtectedTicket,
                    model.TokenType,
                }, commandType: CommandType.StoredProcedure));

                if(id.Contains("Error"))
                    throw new Exception(id);

                return id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override SystemTokenModel Find(string hashedTokenId)
        {
            try
            {
                using (var result = _dBConnection.QueryMultiple("usp_systemtoken_getById", new
                {
                    HashedTokenId = hashedTokenId,
                }, commandType: CommandType.StoredProcedure))
                {
                    var model = result.Read<SystemTokenModel>().FirstOrDefault(); if (model != null)
                    {
                        model.SystemUser = result.Read<SystemUserModel>().FirstOrDefault();
                    }

                    return model;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override List<SystemTokenModel> GetAll() => throw new NotImplementedException();

        public override bool Remove(string id) => throw new NotImplementedException();
        public override bool Update(SystemTokenModel model) => throw new NotImplementedException();
    }
}
