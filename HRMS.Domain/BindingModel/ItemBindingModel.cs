using System.Collections.Generic;

namespace HRMS.Domain.BindingModel
{
    public class ItemBindingModel
    {
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public string ItemTypeId { get; set; }
        public string ItemBrandId { get; set; }
    }
    public class CreateItemBindingModel : ItemBindingModel
    {
        public FileBindingModel IconFile { get; set; }
    }
    public class UpdateItemBindingModel : ItemBindingModel
    {
        public string ItemId { get; set; }
        public UpdateFileBindingModel IconFile { get; set; }
    }
}
