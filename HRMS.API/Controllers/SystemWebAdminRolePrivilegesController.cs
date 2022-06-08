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
using System.Security.Claims;

namespace HRMS.API.Controllers
{
    [Authorize]
    [SilupostAuthorizationFilter]
    [RoutePrefix("api/v1/SystemWebAdminRolePrivileges")]
    public class SystemWebAdminRolePrivilegesController : ApiController
    {
        private readonly ISystemWebAdminRolePrivilegesFacade _systemWebAdminRolePrivilegesFacade;
        private string RecordedBy { get; set; }
        #region CONSTRUCTORS
        public SystemWebAdminRolePrivilegesController(ISystemWebAdminRolePrivilegesFacade systemWebAdminRolePrivilegesFacade)
        {
            _systemWebAdminRolePrivilegesFacade = systemWebAdminRolePrivilegesFacade ?? throw new ArgumentNullException(nameof(systemWebAdminRolePrivilegesFacade));
        }
        #endregion


        [Route("getBySystemWebAdminRoleId")]
        [HttpGet]
        [SwaggerOperation("")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public IHttpActionResult GetBySystemWebAdminRoleId(string SystemWebAdminRoleId)
        {
            AppResponseModel<List<SystemWebAdminRolePrivilegesViewModel>> response = new AppResponseModel<List<SystemWebAdminRolePrivilegesViewModel>>();

            try
            {
                var data = _systemWebAdminRolePrivilegesFacade.FindBySystemWebAdminRoleId(SystemWebAdminRoleId);
                response.Data = data;
                response.IsSuccess = true;
                return new SilupostAPIHttpActionResult<AppResponseModel<List<SystemWebAdminRolePrivilegesViewModel>>>(Request, HttpStatusCode.OK, response);

            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new SilupostAPIHttpActionResult<AppResponseModel<List<SystemWebAdminRolePrivilegesViewModel>>>(Request, HttpStatusCode.OK, response);
            }
        }

        [Route("SetSystemWebAdminRolePrivileges")]
        [HttpPut]
        [ValidateModel]
        [SwaggerOperation("SetSystemWebAdminRolePrivileges")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IHttpActionResult SetSystemWebAdminRolePrivileges([FromBody] SystemWebAdminRolePrivilegesBindingModel model)
        {
            AppResponseModel<List<SystemWebAdminRolePrivilegesViewModel>> response = new AppResponseModel<List<SystemWebAdminRolePrivilegesViewModel>>();

            if (model != null && string.IsNullOrEmpty(model.SystemWebAdminRoleId))
            {
                response.Message = string.Format(Messages.InvalidId, "System Web Admin Role Privilege");
                return new SilupostAPIHttpActionResult<AppResponseModel<List<SystemWebAdminRolePrivilegesViewModel>>>(Request, HttpStatusCode.BadRequest, response);
            }

            if (!model.SystemWebAdminPrivilege.Any())
            {
                response.Message = string.Format(Messages.InvalidId, "System Web Admin Role Privilege");
                return new SilupostAPIHttpActionResult<AppResponseModel<List<SystemWebAdminRolePrivilegesViewModel>>>(Request, HttpStatusCode.BadRequest, response);
            }

            if (model.SystemWebAdminPrivilege.Any(m=>m.SystemWebAdminPrivilegeId == null || m.SystemWebAdminPrivilegeId <= 0))
            {
                response.Message = string.Format(Messages.InvalidId, "System Web Admin Role Privilege");
                return new SilupostAPIHttpActionResult<AppResponseModel<List<SystemWebAdminRolePrivilegesViewModel>>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    RecordedBy = identity.FindFirst("SystemUserId").Value;
                }
                bool success = _systemWebAdminRolePrivilegesFacade.Set(model, RecordedBy);
                response.IsSuccess = success;

                if (success)
                {
                    var data = _systemWebAdminRolePrivilegesFacade.FindBySystemWebAdminRoleId(model.SystemWebAdminRoleId);
                    response.Message = Messages.Updated;
                    response.Data = data;
                    return new SilupostAPIHttpActionResult<AppResponseModel<List<SystemWebAdminRolePrivilegesViewModel>>>(Request, HttpStatusCode.OK, response);
                }
                else
                {
                    response.Message = Messages.Failed;
                    return new SilupostAPIHttpActionResult<AppResponseModel<List<SystemWebAdminRolePrivilegesViewModel>>>(Request, HttpStatusCode.BadGateway, response);
                }
            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new SilupostAPIHttpActionResult<AppResponseModel<List<SystemWebAdminRolePrivilegesViewModel>>>(Request, HttpStatusCode.BadRequest, response);
            }
        }
    }
}
