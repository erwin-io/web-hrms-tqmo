using HRMS.Mapping;
using HRMS.Data.Entity;
using HRMS.Data.Interface;
using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using HRMS.Facade.Interface;
using System;
using System.Collections.Generic;
using System.Transactions;
using System.Linq;
using System.IO;

namespace HRMS.Facade
{
    public class ItemTypeFacade : IItemTypeFacade
    {
        private readonly IItemTypeRepositoryDAC _itemTypeRepositoryDAC;
        private readonly IFileRepositoryRepositoryDAC _fileRepositoryDAC;

        #region CONSTRUCTORS
        public ItemTypeFacade(IItemTypeRepositoryDAC itemTypeRepositoryDAC, IFileRepositoryRepositoryDAC fileRepositoryDAC)
        {
            _itemTypeRepositoryDAC = itemTypeRepositoryDAC ?? throw new ArgumentNullException(nameof(itemTypeRepositoryDAC));
            _fileRepositoryDAC = fileRepositoryDAC ?? throw new ArgumentNullException(nameof(fileRepositoryDAC));
        }
        #endregion
        public string Add(CreateItemTypeBindingModel model, string CreatedBy)
        {
            try
            {
                var id = string.Empty;
                using (var scope = new TransactionScope())
                {
                    var addModel = AutoMapperHelper<CreateItemTypeBindingModel, ItemTypeModel>.Map(model);

                    //Start Saving file
                    addModel.IconFile.FileContent = System.Convert.FromBase64String(model.IconFile.FileFromBase64String);
                    addModel.IconFile.SystemRecordManager.CreatedBy = CreatedBy;
                    var fileId = _fileRepositoryDAC.Add(addModel.IconFile);
                    if (string.IsNullOrEmpty(fileId))
                        throw new Exception("Error Saving File");
                    addModel.IconFile.FileId = fileId;
                    //End Saving file

                    //start store file directory
                    if (File.Exists(addModel.IconFile.FileName))
                    {
                        File.Delete(addModel.IconFile.FileName);
                    }
                    using (var fs = new FileStream(addModel.IconFile.FileName, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(addModel.IconFile.FileContent, 0, addModel.IconFile.FileContent.Length);
                    }
                    //end store file directory

                    addModel.SystemRecordManager.CreatedBy = CreatedBy;
                    id = _itemTypeRepositoryDAC.Add(addModel);
                    if (!string.IsNullOrEmpty(id))
                        scope.Complete();
                }
                return id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ItemTypeViewModel Find(string id)
        {
            var result = AutoMapperHelper<ItemTypeModel, ItemTypeViewModel>.Map(_itemTypeRepositoryDAC.Find(id));

            if (result.IconFile != null && File.Exists(result.IconFile.FileName))
                result.IconFile.FileContent = System.IO.File.ReadAllBytes(result.IconFile.FileName);
            return result;
        }

        public PageResultsViewModel<ItemTypeViewModel> GetPage(string Search, int PageNo, int PageSize, string OrderColumn, string OrderDir)
        {
            var result = new PageResultsViewModel<ItemTypeViewModel>();
            var data = _itemTypeRepositoryDAC.GetPage(Search, PageNo, PageSize, OrderColumn, OrderDir);
            result.Items = AutoMapperHelper<ItemTypeModel, ItemTypeViewModel>.MapList(data);
            foreach (var item in result.Items)
            {
                if (item.IconFile != null && File.Exists(item.IconFile.FileName))
                    item.IconFile.FileContent = System.IO.File.ReadAllBytes(item.IconFile.FileName);
            }
            result.TotalRows = data.Count > 0 ? data.FirstOrDefault().PageResult.TotalRows : 0;
            return result;
        }

        public bool Remove(string id, string LastUpdatedBy)
        {
            var success = false;
            using (var scope = new TransactionScope())
            {
                success = _itemTypeRepositoryDAC.Remove(id, LastUpdatedBy);
                if (success)
                    scope.Complete();
            }
            return success;
        }

        public bool Update(UpdateItemTypeBindingModel model, string LastUpdatedBy)
        {
            var success = false;
            using (var scope = new TransactionScope())
            {
                var updateModel = AutoMapperHelper<UpdateItemTypeBindingModel, ItemTypeModel>.Map(model);
                updateModel.SystemRecordManager.LastUpdatedBy = LastUpdatedBy;
                success = _itemTypeRepositoryDAC.Update(updateModel);
                if (success)
                {

                    //Start Saving file
                    if (model.IconFile == null || string.IsNullOrEmpty(model.IconFile.FileId))
                    {
                        updateModel.IconFile.FileContent = System.Convert.FromBase64String(model.IconFile.FileFromBase64String);
                        updateModel.IconFile.SystemRecordManager.CreatedBy = LastUpdatedBy;
                        var fileId = _fileRepositoryDAC.Add(updateModel.IconFile);
                        if (string.IsNullOrEmpty(fileId))
                            throw new Exception("Error Saving File");
                        updateModel.IconFile.FileId = fileId;
                    }
                    else
                    {
                        updateModel.IconFile.FileContent = System.Convert.FromBase64String(model.IconFile.FileFromBase64String);
                        updateModel.IconFile.SystemRecordManager.LastUpdatedBy = LastUpdatedBy;
                        success = _fileRepositoryDAC.Update(updateModel.IconFile);
                    }
                    //End Saving file

                    //start store file directory
                    if (File.Exists(updateModel.IconFile.FileName))
                    {
                        File.Delete(updateModel.IconFile.FileName);
                    }
                    using (var fs = new FileStream(updateModel.IconFile.FileName, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(updateModel.IconFile.FileContent, 0, updateModel.IconFile.FileContent.Length);
                    }
                    //end store file directory
                    if (success)
                        scope.Complete();
                }
            }
            return success;
        }
    }
}
