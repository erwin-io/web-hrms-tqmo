using System.Collections.Generic;

namespace HRMS.Domain.BindingModel
{
    public class ItemTypeBindingModel
    {
        public string ItemTypeName { get; set; }
        public string ItemTypeDescription { get; set; }
    }
    public class CreateItemTypeBindingModel : ItemTypeBindingModel
    {
        public FileBindingModel IconFile { get; set; }
    }
    public class UpdateItemTypeBindingModel : ItemTypeBindingModel
    {
        public string ItemTypeId { get; set; }
        public UpdateFileBindingModel IconFile { get; set; }
    }
}
