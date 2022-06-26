using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Facade.Interface
{
    public interface IAppointmentFacade
    {
        string Add(CreateAppointmentBindingModel model, string CreatedBy);
        PageResultsViewModel<AppointmentViewModel> GetPage(string Search,
                                                           bool IsAdvanceSearchMode,
                                                           string AppointmentId,
                                                           string Patient,
                                                           DateTime AppointmentDateFrom,
                                                           DateTime AppointmentDateTo,
                                                           string AppointmentStatusId,
                                                           string ProcessedBy,
                                                           int PageNo,
                                                           int PageSize,
                                                           string OrderColumn,
                                                           string OrderDir);
        PageResultsViewModel<AppointmentViewModel> GetPageBySystemUserId(string Search,
                                                                         bool IsAdvanceSearchMode,
                                                                         string CreatedBy,
                                                                         string AppointmentId,
                                                                         DateTime AppointmentDateFrom,
                                                                         DateTime AppointmentDateTo,
                                                                         string AppointmentStatusId,
                                                                         int PageNo,
                                                                         int PageSize,
                                                                         string OrderColumn,
                                                                         string OrderDir);
        AppointmentViewModel Find(string id);
        bool Update(UpdateAppointmentBindingModel model, string LastUpdatedBy);
        bool UpdateStatus(UpdateAppointmentStatusBindingModel model, string LastUpdatedBy);
    }
}
