using System;
using System.Collections.Generic;

namespace HRMS.Data.Entity
{
    public class ItemBrandModel
    {
        public string ItemBrandId { get; set; }
        public string ItemBrandName { get; set; }
        public string ItemBrandDescription { get; set; }
        public FileModel IconFile { get; set; }
        public SystemRecordManagerModel SystemRecordManager { get; set; }
        public EntityStatusModel EntityStatus { get; set; }
        public PageResultsModel PageResult { get; set; }
    }
}
