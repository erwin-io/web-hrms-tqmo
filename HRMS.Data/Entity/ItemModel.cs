using System;
using System.Collections.Generic;

namespace HRMS.Data.Entity
{
    public class ItemModel
    {
        public string ItemId { get; set; }
        public string ItemName   { get; set; }
        public string ItemDescription { get; set; }
        public ItemTypeModel ItemType { get; set; }
        public ItemBrandModel ItemBrand { get; set; }
        public int Stock { get; set; }
        public FileModel IconFile { get; set; }
        public SystemRecordManagerModel SystemRecordManager { get; set; }
        public EntityStatusModel EntityStatus { get; set; }
        public PageResultsModel PageResult { get; set; }
    }
}
