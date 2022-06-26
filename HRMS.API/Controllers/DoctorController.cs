using Newtonsoft.Json;
using HRMS.API.Filters;
using HRMS.API.Helpers;
using HRMS.API.Models;
using HRMS.Domain.ViewModel;
using HRMS.Domain.BindingModel;
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
    [RoutePrefix("api/v1/Doctor")]
    public class DoctorController : ApiController
    {
        private readonly IDoctorFacade _doctor;
        private string RecordedBy { get; set; }
        #region CONSTRUCTORS
        public DoctorController(IDoctorFacade doctor)
        {
            _doctor = doctor ?? throw new ArgumentNullException(nameof(doctor));
        }
        #endregion


        [Authorize]
        [Route("getPage")]
        [HttpGet]
        [SwaggerOperation("getPage")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public IHttpActionResult GetPage(int Draw, string Search, int PageNo, int PageSize, string OrderColumn, string OrderDir)
        {
            DataTableResponseModel<IList<DoctorViewModel>> response = new DataTableResponseModel<IList<DoctorViewModel>>();

            try
            {
                long recordsFiltered = 0;
                long recordsTotal = 0;
                var pageResults = _doctor.GetPage(
                    (Search = string.IsNullOrEmpty(Search) ? string.Empty : Search),
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

                return new HRMSAPIHttpActionResult<DataTableResponseModel<IList<DoctorViewModel>>>(Request, HttpStatusCode.OK, response);
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
            AppResponseModel<DoctorViewModel> response = new AppResponseModel<DoctorViewModel>();

            if (string.IsNullOrEmpty(id))
            {
                response.Message = string.Format(Messages.InvalidId, "Doctor");
                return new HRMSAPIHttpActionResult<AppResponseModel<DoctorViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                DoctorViewModel result = _doctor.Find(id);

                if (result != null)
                {
                    response.IsSuccess = true;
                    response.Data = result;
                    return new HRMSAPIHttpActionResult<AppResponseModel<DoctorViewModel>>(Request, HttpStatusCode.OK, response);
                }
                else
                {
                    response.Message = Messages.NoRecord;
                    return new HRMSAPIHttpActionResult<AppResponseModel<DoctorViewModel>>(Request, HttpStatusCode.NotFound, response);
                }

            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<DoctorViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }

        [Authorize]
        [Route("")]
        [HttpPost]
        [ValidateModel]
        [SwaggerOperation("create")]
        [SwaggerResponse(HttpStatusCode.Created)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public IHttpActionResult Create([FromBody] CreateDoctorBindingModel model)
        {
            AppResponseModel<DoctorViewModel> response = new AppResponseModel<DoctorViewModel>();

            try
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    RecordedBy = identity.FindFirst("SystemUserId").Value;
                }

                string id = _doctor.Add(model, RecordedBy);

                if (!string.IsNullOrEmpty(id))
                {
                    var result = _doctor.Find(id);

                    response.IsSuccess = true;
                    response.Message = Messages.Created;
                    response.Data = result;
                    return new HRMSAPIHttpActionResult<AppResponseModel<DoctorViewModel>>(Request, HttpStatusCode.Created, response);
                }
                else
                {
                    response.Message = Messages.Failed;
                    return new HRMSAPIHttpActionResult<AppResponseModel<DoctorViewModel>>(Request, HttpStatusCode.BadRequest, response);

                }
            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<DoctorViewModel>>(Request, HttpStatusCode.BadRequest, response);
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
        public IHttpActionResult Update([FromBody] UpdateDoctorBindingModel model)
        {
            AppResponseModel<DoctorViewModel> response = new AppResponseModel<DoctorViewModel>();

            if (model != null && string.IsNullOrEmpty(model.DoctorId))
            {
                response.Message = string.Format(Messages.InvalidId, "Doctor");
                return new HRMSAPIHttpActionResult<AppResponseModel<DoctorViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    RecordedBy = identity.FindFirst("SystemUserId").Value;
                }
                var result = _doctor.Find(model.DoctorId);
                if (result == null)
                {
                    response.Message = string.Format(Messages.InvalidId, "Doctor");
                    return new HRMSAPIHttpActionResult<AppResponseModel<DoctorViewModel>>(Request, HttpStatusCode.BadRequest, response);
                }

                bool success = _doctor.Update(model, RecordedBy);
                response.IsSuccess = success;

                if (success)
                {
                    result = _doctor.Find(model.DoctorId);
                    response.Message = Messages.Updated;
                    response.Data = result;
                    return new HRMSAPIHttpActionResult<AppResponseModel<DoctorViewModel>>(Request, HttpStatusCode.OK, response);
                }
                else
                {
                    response.Message = Messages.Failed;
                    return new HRMSAPIHttpActionResult<AppResponseModel<DoctorViewModel>>(Request, HttpStatusCode.BadGateway, response);
                }
            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<DoctorViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }

        [Authorize]
        [Route("{id}")]
        [HttpDelete]
        [SwaggerOperation("remove")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IHttpActionResult Remove(string id)
        {
            AppResponseModel<object> response = new AppResponseModel<object>();

            if (string.IsNullOrEmpty(id))
            {
                response.Message = string.Format(Messages.InvalidId, "Doctor");
                return new HRMSAPIHttpActionResult<AppResponseModel<object>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    RecordedBy = identity.FindFirst("SystemUserId").Value;
                }

                var result = _doctor.Find(id);
                if (result == null)
                {
                    response.Message = string.Format(Messages.InvalidId, "Doctor");
                    return new HRMSAPIHttpActionResult<AppResponseModel<object>>(Request, HttpStatusCode.BadRequest, response);
                }

                bool success = _doctor.Remove(id, RecordedBy);
                response.IsSuccess = success;

                if (success)
                {
                    response.Message = Messages.Removed;
                    return new HRMSAPIHttpActionResult<AppResponseModel<object>>(Request, HttpStatusCode.OK, response);
                }
                else
                {
                    response.Message = Messages.NoRecord;
                    return new HRMSAPIHttpActionResult<AppResponseModel<object>>(Request, HttpStatusCode.NotFound, response);
                }

            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<object>>(Request, HttpStatusCode.BadRequest, response);
            }
        }
    }
}
