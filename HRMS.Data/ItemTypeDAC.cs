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
    public class ItemTypeDAC : RepositoryBase<ItemTypeModel>, IItemTypeRepositoryDAC
    {
        private readonly IDbConnection _dBConnection;

        #region CONSTRUCTORS
        public ItemTypeDAC(IDbConnection dbConnection)
        {
            _dBConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        }
        #endregion

        public override string Add(ItemTypeModel model)
        {
            try
            {
                var id = Convert.ToString(_dBConnection.ExecuteScalar("usp_itemtype_add", new
                {
                    model.ItemTypeName,
                    model.ItemTypeDescription,
                    IconFileId = model.IconFile.FileId,
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

        public override ItemTypeModel Find(string id)
        {
            try
            {
                using (var result = _dBConnection.QueryMultiple("usp_itemtype_getByID", new
                {
                    ItemTypeId = id,
                }, commandType: CommandType.StoredProcedure))
                {
                    var model = result.Read<ItemTypeModel>().FirstOrDefault();
                    if(model != null)
                    {
                        model.IconFile = result.Read<FileModel>().FirstOrDefault();
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

        public override List<ItemTypeModel> GetAll() => throw new NotImplementedException();

        public List<ItemTypeModel> GetPage(string Search, int PageNo, int PageSize, string OrderColumn, string OrderDir)
        {
            var results = new List<ItemTypeModel>();
            try
            {
                var lookup = new Dictionary<string, ItemTypeModel>();

                _dBConnection.Query("usp_itemtype_getPaged",
                new[]
                {
                    typeof(ItemTypeModel),
                    typeof(FileModel),
                    typeof(PageResultsModel),
                }, obj =>
                {
                    ItemTypeModel cit = obj[0] as ItemTypeModel;
                    FileModel f = obj[1] as FileModel;
                    PageResultsModel pr = obj[2] as PageResultsModel;
                    ItemTypeModel model;
                    if (!lookup.TryGetValue(cit.ItemTypeId, out model))
                        lookup.Add(cit.ItemTypeId, model = cit);
                    cit.IconFile = f;
                    cit.PageResult = pr;
                    return model;
                },
                new
                {
                    Search = Search,
                    PageNo = PageNo,
                    PageSize = PageSize,
                    OrderColumn = OrderColumn,
                    OrderDir = OrderDir
                }, splitOn: "ItemTypeId,FileId,TotalRows", commandType: CommandType.StoredProcedure).ToList();
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
                var result = Convert.ToString(_dBConnection.ExecuteScalar("usp_itemtype_delete", new
                {
                    ItemTypeId = id,
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

        public override bool Update(ItemTypeModel model)
        {
            bool success = false;
            try
            {
                int affectedRows = 0;
                var result = Convert.ToString(_dBConnection.ExecuteScalar("usp_itemtype_update", new
                {
                    model.ItemTypeId,
                    model.ItemTypeName,
                    model.ItemTypeDescription,
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
