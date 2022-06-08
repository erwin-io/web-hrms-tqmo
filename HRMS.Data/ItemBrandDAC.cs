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
    public class ItemBrandDAC : RepositoryBase<ItemBrandModel>, IItemBrandRepositoryDAC
    {
        private readonly IDbConnection _dBConnection;

        #region CONSTRUCTORS
        public ItemBrandDAC(IDbConnection dbConnection)
        {
            _dBConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        }
        #endregion

        public override string Add(ItemBrandModel model)
        {
            try
            {
                var id = Convert.ToString(_dBConnection.ExecuteScalar("usp_itembrand_add", new
                {
                    model.ItemBrandName,
                    model.ItemBrandDescription,
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

        public override ItemBrandModel Find(string id)
        {
            try
            {
                using (var result = _dBConnection.QueryMultiple("usp_itembrand_getByID", new
                {
                    ItemBrandId = id,
                }, commandType: CommandType.StoredProcedure))
                {
                    var model = result.Read<ItemBrandModel>().FirstOrDefault();
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

        public override List<ItemBrandModel> GetAll() => throw new NotImplementedException();

        public List<ItemBrandModel> GetPage(string Search, int PageNo, int PageSize, string OrderColumn, string OrderDir)
        {
            var results = new List<ItemBrandModel>();
            try
            {
                var lookup = new Dictionary<string, ItemBrandModel>();

                _dBConnection.Query("usp_itembrand_getPaged",
                new[]
                {
                    typeof(ItemBrandModel),
                    typeof(FileModel),
                    typeof(PageResultsModel),
                }, obj =>
                {
                    ItemBrandModel cit = obj[0] as ItemBrandModel;
                    FileModel f = obj[1] as FileModel;
                    PageResultsModel pr = obj[2] as PageResultsModel;
                    ItemBrandModel model;
                    if (!lookup.TryGetValue(cit.ItemBrandId, out model))
                        lookup.Add(cit.ItemBrandId, model = cit);
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
                }, splitOn: "ItemBrandId,FileId,TotalRows", commandType: CommandType.StoredProcedure).ToList();
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
                var result = Convert.ToString(_dBConnection.ExecuteScalar("usp_itembrand_delete", new
                {
                    ItemBrandId = id,
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

        public override bool Update(ItemBrandModel model)
        {
            bool success = false;
            try
            {
                int affectedRows = 0;
                var result = Convert.ToString(_dBConnection.ExecuteScalar("usp_itembrand_update", new
                {
                    model.ItemBrandId,
                    model.ItemBrandName,
                    model.ItemBrandDescription,
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
