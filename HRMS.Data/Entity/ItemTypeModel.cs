using System;
using System.Collections.Generic;

namespace HRMS.Data.Entity
{
    public class ItemTypeModel
    {
        public string ItemTypeId { get; set; }
        public string ItemTypeName { get; set; }
        public string ItemTypeDescription { get; set; }
        public FileModel IconFile { get; set; }
        public SystemRecordManagerModel SystemRecordManager { get; set; }
        public EntityStatusModel EntityStatus { get; set; }
        public PageResultsModel PageResult { get; set; }
    }
}
