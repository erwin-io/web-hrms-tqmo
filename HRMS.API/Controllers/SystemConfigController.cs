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

namespace HRMS.API.Controllers
{
    [Authorize]
    [RoutePrefix("api/v1/SystemConfig")]
    public class SystemConfigController : ApiController
    {
        private readonly ISystemConfigFacade _systemConfigFacade;
        private string RecordedBy { get; set; }
        #region CONSTRUCTORS
        public SystemConfigController(ISystemConfigFacade systemConfigFacade)
        {
            _systemConfigFacade = systemConfigFacade ?? throw new ArgumentNullException(nameof(systemConfigFacade));
        }
        #endregion

        [SilupostAuthorizationFilter]
        [Route("")]
        [HttpGet]
        [SwaggerOperation("getAll")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public IHttpActionResult GetAll()
        {
            AppResponseModel<IList<SystemConfigViewModel>> response = new AppResponseModel<IList<SystemConfigViewModel>>();

            try
            {
                IList<SystemConfigViewModel> result = _systemConfigFacade.GetAll();

                if (result != null)
                {
                    response.IsSuccess = true;
                    response.Data = result;
                    return new SilupostAPIHttpActionResult<AppResponseModel<IList<SystemConfigViewModel>>>(Request, HttpStatusCode.OK, response);
                }
                else
                {
                    response.Message = Messages.NoRecord;
                    return new SilupostAPIHttpActionResult<AppResponseModel<IList<SystemConfigViewModel>>>(Request, HttpStatusCode.NotFound, response);
                }
            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new SilupostAPIHttpActionResult<AppResponseModel<IList<SystemConfigViewModel>>>(Request, HttpStatusCode.BadRequest, response);
            }
        }

        [SilupostAuthorizationFilter]
        [Route("{id}/detail")]
        [HttpGet]
        [SwaggerOperation("get")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IHttpActionResult Get(string id)
        {
            AppResponseModel<SystemConfigViewModel> response = new AppResponseModel<SystemConfigViewModel>();
            if (string.IsNullOrEmpty(id))
            {
                response.Message = string.Format(Messages.InvalidId, "System Config");
                return new SilupostAPIHttpActionResult<AppResponseModel<SystemConfigViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                SystemConfigViewModel result = _systemConfigFacade.Find(int.Parse(id));

                if (result != null)
                {
                    response.IsSuccess = true;
                    response.Data = result;
                    return new SilupostAPIHttpActionResult<AppResponseModel<SystemConfigViewModel>>(Request, HttpStatusCode.OK, response);
                }
                else
                {
                    response.Message = Messages.NoRecord;
                    return new SilupostAPIHttpActionResult<AppResponseModel<SystemConfigViewModel>>(Request, HttpStatusCode.NotFound, response);
                }

            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new SilupostAPIHttpActionResult<AppResponseModel<SystemConfigViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }

        [AllowAnonymous]
        [Route("GetServerStatus")]
        [HttpGet]
        [SwaggerOperation("get")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IHttpActionResult GetServerStatus()
        {
            AppResponseModel<int> response = new AppResponseModel<int>();

            try
            {
                response.IsSuccess = true;
                response.Data = GlobalVariables.goEnableAPI ? (int)SilupostServerStatusEnums.ACTIVE : (int)SilupostServerStatusEnums.DISABLED;
                return new SilupostAPIHttpActionResult<AppResponseModel<int>>(Request, HttpStatusCode.OK, response);

            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new SilupostAPIHttpActionResult<AppResponseModel<int>>(Request, HttpStatusCode.BadRequest, response);
            }
        }

        [SilupostAuthorizationFilter]
        [Route("")]
        [HttpPut]
        [ValidateModel]
        [SwaggerOperation("update")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IHttpActionResult Update([FromBody] UpdateSystemConfigBindingModel model)
        {
            AppResponseModel<SystemConfigViewModel> response = new AppResponseModel<SystemConfigViewModel>();

            if (model != null && model.SystemConfigId == 0)
            {
                response.Message = string.Format(Messages.InvalidId, "System Config Id");
                return new SilupostAPIHttpActionResult<AppResponseModel<SystemConfigViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    RecordedBy = identity.FindFirst("SystemUserId").Value;
                }
                var result = _systemConfigFacade.Find(model.SystemConfigId);
                if (result == null)
                {
                    response.Message = string.Format(Messages.InvalidId, "System Config");
                    return new SilupostAPIHttpActionResult<AppResponseModel<SystemConfigViewModel>>(Request, HttpStatusCode.BadRequest, response);
                }
                bool success = _systemConfigFacade.Update(model);
                response.IsSuccess = success;

                if (success)
                {
                    result = _systemConfigFacade.Find(model.SystemConfigId);
                    response.Message = Messages.Updated;
                    response.Data = result;
                    return new SilupostAPIHttpActionResult<AppResponseModel<SystemConfigViewModel>>(Request, HttpStatusCode.OK, response);
                }
                else
                {
                    response.Message = Messages.Failed;
                    return new SilupostAPIHttpActionResult<AppResponseModel<SystemConfigViewModel>>(Request, HttpStatusCode.BadGateway, response);
                }
            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new SilupostAPIHttpActionResult<AppResponseModel<SystemConfigViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }
    }
}
