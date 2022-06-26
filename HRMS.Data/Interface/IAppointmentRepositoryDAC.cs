using HRMS.Data.Core;
using HRMS.Data.Entity;
using System;
using System.Collections.Generic;

namespace HRMS.Data.Interface
{
    public interface IAppointmentRepositoryDAC : IRepository<AppointmentModel>
    {
        List<AppointmentModel> GetPage(string Search,
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

        List<AppointmentModel> GetPageBySystemUserId(string Search,
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
        bool Update(AppointmentModel model, string LastUpdatedBy);
        bool UpdateStatus(AppointmentModel model, string LastUpdatedBy);
    }
}
