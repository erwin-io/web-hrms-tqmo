using Newtonsoft.Json;
using HRMS.API.Filters;
using HRMS.API.Helpers;
using HRMS.API.Models;
using HRMS.Domain.ViewModel;
using HRMS.Domain.BindingModel;
using HRMS.Domain.Enumerations;
using HRMS.Facade.Interface;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Net.Http.Formatting;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.Net.Http.Headers;

namespace HRMS.API.Controllers
{
    [HRMSAuthorizationFilter]
    [RoutePrefix("api/v1/Appointment")]
    public class AppointmentController : ApiController
    {
        private readonly IAppointmentFacade _appointmentFacade;
        private readonly IDiagnosisFacade _diagnosisFacade;
        private string RecordedBy { get; set; }
        #region CONSTRUCTORS
        public AppointmentController(IAppointmentFacade appointmentFacade, IDiagnosisFacade diagnosisFacade)
        {
            _appointmentFacade = appointmentFacade ?? throw new ArgumentNullException(nameof(appointmentFacade));
            _diagnosisFacade = diagnosisFacade ?? throw new ArgumentNullException(nameof(diagnosisFacade));
        }
        #endregion


        [Authorize]
        [Route("getPage")]
        [HttpGet]
        [SwaggerOperation("getPage")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public IHttpActionResult GetPage(int Draw,
                                         string Search,
                                         int PageNo,
                                         int PageSize,
                                         string OrderColumn,
                                         string OrderDir,
                                         bool IsAdvanceSearchMode,
                                         string AppointmentId,
                                         string AppointmentStatusId,
                                         string Patient,
                                         DateTime AppointmentDateFrom,
                                         DateTime AppointmentDateTo,
                                         string ProcessedBy)
        {
            DataTableResponseModel<IList<AppointmentViewModel>> response = new DataTableResponseModel<IList<AppointmentViewModel>>();

            try
            {
                long recordsFiltered = 0;
                long recordsTotal = 0;
                var pageResults = _appointmentFacade.GetPage(
                    (Search = string.IsNullOrEmpty(Search) ? string.Empty : Search),
                    IsAdvanceSearchMode,
                    AppointmentId,
                    Patient,
                    AppointmentDateFrom,
                    AppointmentDateTo,
                    AppointmentStatusId,
                    ProcessedBy,
                    PageNo,
                    PageSize,
                    OrderColumn,
                    OrderDir);
                var records = pageResults.Items.ToList();
                recordsTotal = pageResults.TotalRows;
                recordsFiltered = pageResults.TotalRows;

                response.draw = Draw;
                response.recordsFiltered = recordsFiltered;
                response.recordsTotal = recordsTotal;
                response.data = pageResults.Items.ToList();

                return new HRMSAPIHttpActionResult<DataTableResponseModel<IList<AppointmentViewModel>>>(Request, HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                var exception = new AppResponseModel<object>();
                exception.DeveloperMessage = ex.Message;
                exception.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<object>>(Request, HttpStatusCode.OK, exception);
            }
        }


        [Authorize]
        [Route("GetPageBySystemUserId")]
        [HttpGet]
        [SwaggerOperation("getPage")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public IHttpActionResult GetPageBySystemUserId(int Draw,
                                                       string Search,
                                                       int PageNo,
                                                       int PageSize,
                                                       string OrderColumn,
                                                       string OrderDir,
                                                       bool IsAdvanceSearchMode,
                                                       string CreatedBy,
                                                       string AppointmentId,
                                                       DateTime AppointmentDateFrom,
                                                       DateTime AppointmentDateTo,
                                                       string AppointmentStatusId)
        {
            DataTableResponseModel<IList<AppointmentViewModel>> response = new DataTableResponseModel<IList<AppointmentViewModel>>();

            try
            {
                long recordsFiltered = 0;
                long recordsTotal = 0;
                var pageResults = _appointmentFacade.GetPageBySystemUserId(
                    (Search = string.IsNullOrEmpty(Search) ? string.Empty : Search),
                    IsAdvanceSearchMode,
                    CreatedBy,
                    AppointmentId,
                    AppointmentDateFrom,
                    AppointmentDateTo,
                    AppointmentStatusId,
                    PageNo,
                    PageSize,
                    OrderColumn,
                    OrderDir);
                var records = pageResults.Items.ToList();
                recordsTotal = pageResults.TotalRows;
                recordsFiltered = pageResults.TotalRows;

                response.draw = Draw;
                response.recordsFiltered = recordsFiltered;
                response.recordsTotal = recordsTotal;
                response.data = pageResults.Items.ToList();

                return new HRMSAPIHttpActionResult<DataTableResponseModel<IList<AppointmentViewModel>>>(Request, HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                var exception = new AppResponseModel<object>();
                exception.DeveloperMessage = ex.Message;
                exception.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<object>>(Request, HttpStatusCode.OK, exception);
            }
        }

        [Authorize]
        [Route("{id}/detail")]
        [HttpGet]
        [SwaggerOperation("get")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IHttpActionResult Get(string id)
        {
            AppResponseModel<AppointmentViewModel> response = new AppResponseModel<AppointmentViewModel>();

            if (string.IsNullOrEmpty(id))
            {
                response.Message = string.Format(Messages.InvalidId, "Appointment");
                return new HRMSAPIHttpActionResult<AppResponseModel<AppointmentViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                AppointmentViewModel result = _appointmentFacade.Find(id);

                if (result != null)
                {
                    response.IsSuccess = true;
                    response.Data = result;
                    return new HRMSAPIHttpActionResult<AppResponseModel<AppointmentViewModel>>(Request, HttpStatusCode.OK, response);
                }
                else
                {
                    response.Message = Messages.NoRecord;
                    return new HRMSAPIHttpActionResult<AppResponseModel<AppointmentViewModel>>(Request, HttpStatusCode.NotFound, response);
                }

            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<AppointmentViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }

        [Authorize]
        [Route("")]
        [HttpPost]
        [ValidateModel]
        [SwaggerOperation("create")]
        [SwaggerResponse(HttpStatusCode.Created)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public IHttpActionResult Create([FromBody] CreateAppointmentBindingModel model)
        {
            AppResponseModel<AppointmentViewModel> response = new AppResponseModel<AppointmentViewModel>();

            try
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    RecordedBy = identity.FindFirst("SystemUserId").Value;
                }

                string id = _appointmentFacade.Add(model, RecordedBy);

                if (!string.IsNullOrEmpty(id))
                {
                    var result = _appointmentFacade.Find(id);

                    response.IsSuccess = true;
                    response.Message = Messages.Created;
                    response.Data = result;
                    return new HRMSAPIHttpActionResult<AppResponseModel<AppointmentViewModel>>(Request, HttpStatusCode.Created, response);
                }
                else
                {
                    response.Message = Messages.Failed;
                    return new HRMSAPIHttpActionResult<AppResponseModel<AppointmentViewModel>>(Request, HttpStatusCode.BadRequest, response);

                }
            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<AppointmentViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }

        [Authorize]
        [Route("")]
        [HttpPut]
        [ValidateModel]
        [SwaggerOperation("update")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IHttpActionResult Update([FromBody] UpdateAppointmentBindingModel model)
        {
            AppResponseModel<AppointmentViewModel> response = new AppResponseModel<AppointmentViewModel>();

            if (model != null && string.IsNullOrEmpty(model.AppointmentId))
            {
                response.Message = string.Format(Messages.InvalidId, "Appointment");
                return new HRMSAPIHttpActionResult<AppResponseModel<AppointmentViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    RecordedBy = identity.FindFirst("SystemUserId").Value;
                }
                var result = _appointmentFacade.Find(model.AppointmentId);
                if (result == null)
                {
                    response.Message = string.Format(Messages.InvalidId, "Appointment");
                    return new HRMSAPIHttpActionResult<AppResponseModel<AppointmentViewModel>>(Request, HttpStatusCode.BadRequest, response);
                }

                bool success = _appointmentFacade.Update(model, RecordedBy);
                response.IsSuccess = success;

                if (success)
                {
                    result = _appointmentFacade.Find(model.AppointmentId);
                    response.Message = Messages.Updated;
                    response.Data = result;
                    return new HRMSAPIHttpActionResult<AppResponseModel<AppointmentViewModel>>(Request, HttpStatusCode.OK, response);
                }
                else
                {
                    response.Message = Messages.Failed;
                    return new HRMSAPIHttpActionResult<AppResponseModel<AppointmentViewModel>>(Request, HttpStatusCode.BadGateway, response);
                }
            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<AppointmentViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }

        //[Authorize]
        //[Route("{id}")]
        //[HttpDelete]
        //[SwaggerOperation("remove")]
        //[SwaggerResponse(HttpStatusCode.OK)]
        //[SwaggerResponse(HttpStatusCode.NotFound)]
        //public IHttpActionResult Remove(string id)
        //{
        //    AppResponseModel<object> response = new AppResponseModel<object>();

        //    if (string.IsNullOrEmpty(id))
        //    {
        //        response.Message = string.Format(Messages.InvalidId, "Appointment");
        //        return new HRMSAPIHttpActionResult<AppResponseModel<object>>(Request, HttpStatusCode.BadRequest, response);
        //    }

        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        if (identity != null)
        //        {
        //            RecordedBy = identity.FindFirst("SystemUserId").Value;
        //        }

        //        var result = _appointmentFacade.Find(id);
        //        if (result == null)
        //        {
        //            response.Message = string.Format(Messages.InvalidId, "Appointment");
        //            return new HRMSAPIHttpActionResult<AppResponseModel<object>>(Request, HttpStatusCode.BadRequest, response);
        //        }

        //        bool success = _appointmentFacade.Remove(id, RecordedBy);
        //        response.IsSuccess = success;

        //        if (success)
        //        {
        //            response.Message = Messages.Removed;
        //            return new HRMSAPIHttpActionResult<AppResponseModel<object>>(Request, HttpStatusCode.OK, response);
        //        }
        //        else
        //        {
        //            response.Message = Messages.NoRecord;
        //            return new HRMSAPIHttpActionResult<AppResponseModel<object>>(Request, HttpStatusCode.NotFound, response);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        response.DeveloperMessage = ex.Message;
        //        response.Message = Messages.ServerError;
        //        //TODO Logging of exceptions
        //        return new HRMSAPIHttpActionResult<AppResponseModel<object>>(Request, HttpStatusCode.BadRequest, response);
        //    }
        //}

        [Authorize]
        [Route("UpdateStatus")]
        [HttpPut]
        [ValidateModel]
        [SwaggerOperation("update")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IHttpActionResult UpdateStatus([FromBody] UpdateAppointmentStatusBindingModel model)
        {
            AppResponseModel<AppointmentViewModel> response = new AppResponseModel<AppointmentViewModel>();

            if (model != null && string.IsNullOrEmpty(model.AppointmentId))
            {
                response.Message = string.Format(Messages.InvalidId, "Appointment");
                return new HRMSAPIHttpActionResult<AppResponseModel<AppointmentViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    RecordedBy = identity.FindFirst("SystemUserId").Value;
                }
                var result = _appointmentFacade.Find(model.AppointmentId);
                if (result == null)
                {
                    response.Message = string.Format(Messages.InvalidId, "Appointment");
                    return new HRMSAPIHttpActionResult<AppResponseModel<AppointmentViewModel>>(Request, HttpStatusCode.BadRequest, response);
                }

                if(model.AppointmentStatusId == (int)APPOINTMENT_STATUS.Completed)
                {
                    var getDiagnosis = _diagnosisFacade.GetByAppointmentId(result.AppointmentId);
                    if (getDiagnosis.Count <= 0)
                    {
                        response.Message = string.Format(Messages.Failed, "Diagnosis required!");
                        response.DeveloperMessage = string.Format(Messages.Failed, "Diagnosis required!");
                        return new HRMSAPIHttpActionResult<AppResponseModel<AppointmentViewModel>>(Request, HttpStatusCode.BadRequest, response);
                    }
                }
                bool success = _appointmentFacade.UpdateStatus(model, RecordedBy);
                response.IsSuccess = success;

                if (success)
                {
                    result = _appointmentFacade.Find(model.AppointmentId);
                    response.Message = Messages.Updated;
                    response.Data = result;
                    return new HRMSAPIHttpActionResult<AppResponseModel<AppointmentViewModel>>(Request, HttpStatusCode.OK, response);
                }
                else
                {
                    response.Message = Messages.Failed;
                    return new HRMSAPIHttpActionResult<AppResponseModel<AppointmentViewModel>>(Request, HttpStatusCode.BadGateway, response);
                }
            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<AppointmentViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }
    }
}
