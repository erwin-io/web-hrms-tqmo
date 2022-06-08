using System.Collections.Generic;

namespace HRMS.Domain.BindingModel
{
    public class ItemBrandBindingModel
    {
        public string ItemBrandName { get; set; }
        public string ItemBrandDescription { get; set; }
    }
    public class CreateItemBrandBindingModel : ItemBrandBindingModel
    {
        public FileBindingModel IconFile { get; set; }
    }
    public class UpdateItemBrandBindingModel : ItemBrandBindingModel
    {
        public string ItemBrandId { get; set; }
        public UpdateFileBindingModel IconFile { get; set; }
    }
}
