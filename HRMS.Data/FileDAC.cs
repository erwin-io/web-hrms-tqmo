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
    public class FileDAC : RepositoryBase<FileModel>, IFileRepositoryRepositoryDAC
    {
        private readonly IDbConnection _dBConnection;

        #region CONSTRUCTORS
        public FileDAC(IDbConnection dbConnection)
        {
            _dBConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        }
        #endregion

        public override string Add(FileModel model)
        {
            try
            {
                var id = Convert.ToString(_dBConnection.ExecuteScalar("usp_file_add", new
                {
                    model.FileName,
                    model.MimeType,
                    model.FileContent,
                    model.IsFromStorage,
                    model.SystemRecordManager.CreatedBy,
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

        public override FileModel Find(string id)
        {
            try
            {
                using (var result = _dBConnection.QueryMultiple("usp_file_getById", new
                {
                    FileId = id,
                }, commandType: CommandType.StoredProcedure))
                {
                    var model = result.Read<FileModel>().FirstOrDefault();

                    return model;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override List<FileModel> GetAll() => throw new NotImplementedException();

        public override bool Remove(string id) => throw new NotImplementedException();
        public override bool Update(FileModel model)
        {
            bool success = false;
            try
            {
                int affectedRows = 0;
                var result = Convert.ToString(_dBConnection.ExecuteScalar("usp_file_update", new
                {
                    model.FileId,
                    model.FileName,
                    model.MimeType,
                    model.FileContent,
                    model.IsFromStorage,
                    model.SystemRecordManager.LastUpdatedBy,
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
