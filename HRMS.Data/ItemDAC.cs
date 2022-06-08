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
    public class ItemDAC : RepositoryBase<ItemModel>, IItemRepositoryDAC
    {
        private readonly IDbConnection _dBConnection;

        #region CONSTRUCTORS
        public ItemDAC(IDbConnection dbConnection)
        {
            _dBConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        }
        #endregion

        public override string Add(ItemModel model)
        {
            try
            {
                var id = Convert.ToString(_dBConnection.ExecuteScalar("usp_item_add", new
                {
                    model.ItemName,
                    model.ItemDescription,
                    model.ItemType.ItemTypeId,
                    model.ItemBrand.ItemBrandId,
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

        public override ItemModel Find(string id)
        {
            try
            {
                using (var result = _dBConnection.QueryMultiple("usp_item_getByID", new
                {
                    ItemId = id,
                }, commandType: CommandType.StoredProcedure))
                {
                    var model = result.Read<ItemModel>().FirstOrDefault();
                    if(model != null)
                    {
                        model.ItemType = result.Read<ItemTypeModel>().FirstOrDefault();
                        model.ItemBrand = result.Read<ItemBrandModel>().FirstOrDefault();
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

        public override List<ItemModel> GetAll() => throw new NotImplementedException();

        public List<ItemModel> GetPage(string Search, int PageNo, int PageSize, string OrderColumn, string OrderDir)
        {
            var results = new List<ItemModel>();
            try
            {
                var lookup = new Dictionary<string, ItemModel>();

                _dBConnection.Query("usp_Item_getPaged",
                new[]
                {
                    typeof(ItemModel),
                    typeof(FileModel),
                    typeof(ItemTypeModel),
                    typeof(ItemBrandModel),
                    typeof(PageResultsModel),
                }, obj =>
                {
                    ItemModel cit = obj[0] as ItemModel;
                    FileModel f = obj[1] as FileModel;
                    ItemTypeModel it = obj[2] as ItemTypeModel;
                    ItemBrandModel ib = obj[3] as ItemBrandModel;
                    PageResultsModel pr = obj[4] as PageResultsModel;
                    ItemModel model;
                    if (!lookup.TryGetValue(cit.ItemId, out model))
                        lookup.Add(cit.ItemId, model = cit);
                    cit.IconFile = f;
                    cit.ItemType = it;
                    cit.ItemBrand = ib;
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
                }, splitOn: "ItemId,FileId,ItemTypeId,ItemBrandId,TotalRows", commandType: CommandType.StoredProcedure).ToList();
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
                var result = Convert.ToString(_dBConnection.ExecuteScalar("usp_item_delete", new
                {
                    ItemId = id,
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

        public override bool Update(ItemModel model)
        {
            bool success = false;
            try
            {
                int affectedRows = 0;
                var result = Convert.ToString(_dBConnection.ExecuteScalar("usp_item_update", new
                {
                    model.ItemId,
                    model.ItemName,
                    model.ItemDescription,
                    model.ItemType.ItemTypeId,
                    model.ItemBrand.ItemBrandId,
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
