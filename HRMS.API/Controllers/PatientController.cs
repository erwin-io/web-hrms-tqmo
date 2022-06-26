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
    [RoutePrefix("api/v1/Patient")]
    public class PatientController : ApiController
    {
        private readonly IPatientFacade _patient;
        private string RecordedBy { get; set; }
        #region CONSTRUCTORS
        public PatientController(IPatientFacade patient)
        {
            _patient = patient ?? throw new ArgumentNullException(nameof(patient));
        }
        #endregion


        [Authorize]
        [Route("getPage")]
        [HttpGet]
        [SwaggerOperation("getPage")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public IHttpActionResult GetPage(int Draw, string Search, int PageNo, int PageSize, string OrderColumn, string OrderDir)
        {
            DataTableResponseModel<IList<PatientViewModel>> response = new DataTableResponseModel<IList<PatientViewModel>>();

            try
            {
                long recordsFiltered = 0;
                long recordsTotal = 0;
                var pageResults = _patient.GetPage(
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

                return new HRMSAPIHttpActionResult<DataTableResponseModel<IList<PatientViewModel>>>(Request, HttpStatusCode.OK, response);
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
            AppResponseModel<PatientViewModel> response = new AppResponseModel<PatientViewModel>();

            if (string.IsNullOrEmpty(id))
            {
                response.Message = string.Format(Messages.InvalidId, "Patient");
                return new HRMSAPIHttpActionResult<AppResponseModel<PatientViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                PatientViewModel result = _patient.Find(id);

                if (result != null)
                {
                    response.IsSuccess = true;
                    response.Data = result;
                    return new HRMSAPIHttpActionResult<AppResponseModel<PatientViewModel>>(Request, HttpStatusCode.OK, response);
                }
                else
                {
                    response.Message = Messages.NoRecord;
                    return new HRMSAPIHttpActionResult<AppResponseModel<PatientViewModel>>(Request, HttpStatusCode.NotFound, response);
                }

            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<PatientViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }

        [Authorize]
        [Route("")]
        [HttpPost]
        [ValidateModel]
        [SwaggerOperation("create")]
        [SwaggerResponse(HttpStatusCode.Created)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public IHttpActionResult Create([FromBody] CreatePatientBindingModel model)
        {
            AppResponseModel<PatientViewModel> response = new AppResponseModel<PatientViewModel>();

            try
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    RecordedBy = identity.FindFirst("SystemUserId").Value;
                }

                string id = _patient.Add(model, RecordedBy);

                if (!string.IsNullOrEmpty(id))
                {
                    var result = _patient.Find(id);

                    response.IsSuccess = true;
                    response.Message = Messages.Created;
                    response.Data = result;
                    return new HRMSAPIHttpActionResult<AppResponseModel<PatientViewModel>>(Request, HttpStatusCode.Created, response);
                }
                else
                {
                    response.Message = Messages.Failed;
                    return new HRMSAPIHttpActionResult<AppResponseModel<PatientViewModel>>(Request, HttpStatusCode.BadRequest, response);

                }
            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<PatientViewModel>>(Request, HttpStatusCode.BadRequest, response);
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
        public IHttpActionResult Update([FromBody] UpdatePatientBindingModel model)
        {
            AppResponseModel<PatientViewModel> response = new AppResponseModel<PatientViewModel>();

            if (model != null && string.IsNullOrEmpty(model.PatientId))
            {
                response.Message = string.Format(Messages.InvalidId, "Patient");
                return new HRMSAPIHttpActionResult<AppResponseModel<PatientViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    RecordedBy = identity.FindFirst("SystemUserId").Value;
                }
                var result = _patient.Find(model.PatientId);
                if (result == null)
                {
                    response.Message = string.Format(Messages.InvalidId, "Patient");
                    return new HRMSAPIHttpActionResult<AppResponseModel<PatientViewModel>>(Request, HttpStatusCode.BadRequest, response);
                }

                bool success = _patient.Update(model, RecordedBy);
                response.IsSuccess = success;

                if (success)
                {
                    result = _patient.Find(model.PatientId);
                    response.Message = Messages.Updated;
                    response.Data = result;
                    return new HRMSAPIHttpActionResult<AppResponseModel<PatientViewModel>>(Request, HttpStatusCode.OK, response);
                }
                else
                {
                    response.Message = Messages.Failed;
                    return new HRMSAPIHttpActionResult<AppResponseModel<PatientViewModel>>(Request, HttpStatusCode.BadGateway, response);
                }
            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<PatientViewModel>>(Request, HttpStatusCode.BadRequest, response);
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
                response.Message = string.Format(Messages.InvalidId, "Patient");
                return new HRMSAPIHttpActionResult<AppResponseModel<object>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    RecordedBy = identity.FindFirst("SystemUserId").Value;
                }

                var result = _patient.Find(id);
                if (result == null)
                {
                    response.Message = string.Format(Messages.InvalidId, "Patient");
                    return new HRMSAPIHttpActionResult<AppResponseModel<object>>(Request, HttpStatusCode.BadRequest, response);
                }

                bool success = _patient.Remove(id, RecordedBy);
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
