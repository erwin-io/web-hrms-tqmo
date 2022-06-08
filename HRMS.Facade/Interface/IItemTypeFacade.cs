using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Facade.Interface
{
    public interface IItemTypeFacade
    {
        string Add(CreateItemTypeBindingModel model, string CreatedBy);
        ItemTypeViewModel Find(string id);
        bool Remove(string id, string LastUpdatedBy);
        bool Update(UpdateItemTypeBindingModel model, string LastUpdatedBy);
        PageResultsViewModel<ItemTypeViewModel> GetPage(string Search, int PageNo, int PageSize, string OrderColumn, string OrderDir);
    }
}
