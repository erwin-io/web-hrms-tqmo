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
    [RoutePrefix("api/v1/Diagnosis")]
    public class DiagnosisController : ApiController
    {
        private readonly IDiagnosisFacade _diagnosisFacade;
        private string RecordedBy { get; set; }
        #region CONSTRUCTORS
        public DiagnosisController(IDiagnosisFacade diagnosisFacade)
        {
            _diagnosisFacade = diagnosisFacade ?? throw new ArgumentNullException(nameof(diagnosisFacade));
        }
        #endregion


        [Authorize]
        [Route("getByAppointmentId")]
        [HttpGet]
        [SwaggerOperation("get")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public IHttpActionResult GetByAppointmentId(string AppointmentId)
        {
            AppResponseModel<List<DiagnosisViewModel>> response = new AppResponseModel<List<DiagnosisViewModel>>();

            if (string.IsNullOrEmpty(AppointmentId))
            {
                response.Message = string.Format(Messages.InvalidId, "Appointment");
                return new HRMSAPIHttpActionResult<AppResponseModel<List<DiagnosisViewModel>>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                List<DiagnosisViewModel> result = _diagnosisFacade.GetByAppointmentId(AppointmentId);

                if (result != null)
                {
                    response.IsSuccess = true;
                    response.Data = result;
                    return new HRMSAPIHttpActionResult<AppResponseModel<List<DiagnosisViewModel>>>(Request, HttpStatusCode.OK, response);
                }
                else
                {
                    response.Message = Messages.NoRecord;
                    return new HRMSAPIHttpActionResult<AppResponseModel<List<DiagnosisViewModel>>>(Request, HttpStatusCode.NotFound, response);
                }

            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<List<DiagnosisViewModel>>>(Request, HttpStatusCode.BadRequest, response);
            }
        }



        [Authorize]
        [Route("getByPatientId")]
        [HttpGet]
        [SwaggerOperation("get")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public IHttpActionResult GetByPatientId(string PatientId)
        {
            AppResponseModel<List<DiagnosisViewModel>> response = new AppResponseModel<List<DiagnosisViewModel>>();

            if (string.IsNullOrEmpty(PatientId))
            {
                response.Message = string.Format(Messages.InvalidId, "Patient");
                return new HRMSAPIHttpActionResult<AppResponseModel<List<DiagnosisViewModel>>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                List<DiagnosisViewModel> result = _diagnosisFacade.GetByPatientId(PatientId);

                if (result != null)
                {
                    response.IsSuccess = true;
                    response.Data = result;
                    return new HRMSAPIHttpActionResult<AppResponseModel<List<DiagnosisViewModel>>>(Request, HttpStatusCode.OK, response);
                }
                else
                {
                    response.Message = Messages.NoRecord;
                    return new HRMSAPIHttpActionResult<AppResponseModel<List<DiagnosisViewModel>>>(Request, HttpStatusCode.NotFound, response);
                }

            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<List<DiagnosisViewModel>>>(Request, HttpStatusCode.BadRequest, response);
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
            AppResponseModel<DiagnosisViewModel> response = new AppResponseModel<DiagnosisViewModel>();

            if (string.IsNullOrEmpty(id))
            {
                response.Message = string.Format(Messages.InvalidId, "Diagnosis");
                return new HRMSAPIHttpActionResult<AppResponseModel<DiagnosisViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                DiagnosisViewModel result = _diagnosisFacade.Find(id);

                if (result != null)
                {
                    response.IsSuccess = true;
                    response.Data = result;
                    return new HRMSAPIHttpActionResult<AppResponseModel<DiagnosisViewModel>>(Request, HttpStatusCode.OK, response);
                }
                else
                {
                    response.Message = Messages.NoRecord;
                    return new HRMSAPIHttpActionResult<AppResponseModel<DiagnosisViewModel>>(Request, HttpStatusCode.NotFound, response);
                }

            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<DiagnosisViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }

        [Authorize]
        [Route("")]
        [HttpPost]
        [ValidateModel]
        [SwaggerOperation("create")]
        [SwaggerResponse(HttpStatusCode.Created)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public IHttpActionResult Create([FromBody] CreateDiagnosisBindingModel model)
        {
            AppResponseModel<DiagnosisViewModel> response = new AppResponseModel<DiagnosisViewModel>();

            try
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    RecordedBy = identity.FindFirst("SystemUserId").Value;
                }

                string id = _diagnosisFacade.Add(model, RecordedBy);

                if (!string.IsNullOrEmpty(id))
                {
                    var result = _diagnosisFacade.Find(id);

                    response.IsSuccess = true;
                    response.Message = Messages.Created;
                    response.Data = result;
                    return new HRMSAPIHttpActionResult<AppResponseModel<DiagnosisViewModel>>(Request, HttpStatusCode.Created, response);
                }
                else
                {
                    response.Message = Messages.Failed;
                    return new HRMSAPIHttpActionResult<AppResponseModel<DiagnosisViewModel>>(Request, HttpStatusCode.BadRequest, response);

                }
            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<DiagnosisViewModel>>(Request, HttpStatusCode.BadRequest, response);
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
        public IHttpActionResult Update([FromBody] UpdateDiagnosisBindingModel model)
        {
            AppResponseModel<DiagnosisViewModel> response = new AppResponseModel<DiagnosisViewModel>();

            if (model != null && string.IsNullOrEmpty(model.DiagnosisId))
            {
                response.Message = string.Format(Messages.InvalidId, "Diagnosis");
                return new HRMSAPIHttpActionResult<AppResponseModel<DiagnosisViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    RecordedBy = identity.FindFirst("SystemUserId").Value;
                }
                var result = _diagnosisFacade.Find(model.DiagnosisId);
                if (result == null)
                {
                    response.Message = string.Format(Messages.InvalidId, "Diagnosis");
                    return new HRMSAPIHttpActionResult<AppResponseModel<DiagnosisViewModel>>(Request, HttpStatusCode.BadRequest, response);
                }

                bool success = _diagnosisFacade.Update(model, RecordedBy);
                response.IsSuccess = success;

                if (success)
                {
                    result = _diagnosisFacade.Find(model.DiagnosisId);
                    response.Message = Messages.Updated;
                    response.Data = result;
                    return new HRMSAPIHttpActionResult<AppResponseModel<DiagnosisViewModel>>(Request, HttpStatusCode.OK, response);
                }
                else
                {
                    response.Message = Messages.Failed;
                    return new HRMSAPIHttpActionResult<AppResponseModel<DiagnosisViewModel>>(Request, HttpStatusCode.BadGateway, response);
                }
            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<DiagnosisViewModel>>(Request, HttpStatusCode.BadRequest, response);
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
                response.Message = string.Format(Messages.InvalidId, "Diagnosis");
                return new HRMSAPIHttpActionResult<AppResponseModel<object>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    RecordedBy = identity.FindFirst("SystemUserId").Value;
                }

                var result = _diagnosisFacade.Find(id);
                if (result == null)
                {
                    response.Message = string.Format(Messages.InvalidId, "Diagnosis");
                    return new HRMSAPIHttpActionResult<AppResponseModel<object>>(Request, HttpStatusCode.BadRequest, response);
                }

                bool success = _diagnosisFacade.Remove(id, RecordedBy);
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

        [Authorize]
        [Route("updateStatus")]
        [HttpPut]
        [ValidateModel]
        [SwaggerOperation("update")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IHttpActionResult UpdateStatus([FromBody] UpdateDiagnosisStatusBindingModel model)
        {
            AppResponseModel<DiagnosisViewModel> response = new AppResponseModel<DiagnosisViewModel>();

            if (model != null && string.IsNullOrEmpty(model.DiagnosisId))
            {
                response.Message = string.Format(Messages.InvalidId, "Diagnosis");
                return new HRMSAPIHttpActionResult<AppResponseModel<DiagnosisViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    RecordedBy = identity.FindFirst("SystemUserId").Value;
                }
                var result = _diagnosisFacade.Find(model.DiagnosisId);
                if (result == null)
                {
                    response.Message = string.Format(Messages.InvalidId, "Diagnosis");
                    return new HRMSAPIHttpActionResult<AppResponseModel<DiagnosisViewModel>>(Request, HttpStatusCode.BadRequest, response);
                }

                bool success = _diagnosisFacade.UpdateStatus(model, RecordedBy);
                response.IsSuccess = success;

                if (success)
                {
                    result = _diagnosisFacade.Find(model.DiagnosisId);
                    response.Message = Messages.Updated;
                    response.Data = result;
                    return new HRMSAPIHttpActionResult<AppResponseModel<DiagnosisViewModel>>(Request, HttpStatusCode.OK, response);
                }
                else
                {
                    response.Message = Messages.Failed;
                    return new HRMSAPIHttpActionResult<AppResponseModel<DiagnosisViewModel>>(Request, HttpStatusCode.BadGateway, response);
                }
            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<DiagnosisViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }
    }
}
