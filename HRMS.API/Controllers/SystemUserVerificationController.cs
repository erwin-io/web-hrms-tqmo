using Newtonsoft.Json;
using HRMS.API.Filters;
using HRMS.API.Utility;
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
    [RoutePrefix("api/v1/SystemUserVerification")]
    public class SystemUserVerificationController : ApiController
    {
        private readonly ISystemUserFacade _systemUserFacade;
        private readonly ISystemUserVerificationFacade _systemUserVerificationFacade;
        private string RecordedBy { get; set; }
        #region CONSTRUCTORS
        public SystemUserVerificationController(ISystemUserVerificationFacade systemUserVerificationFacade, ISystemUserFacade systemUserFacade)
        {
            _systemUserVerificationFacade = systemUserVerificationFacade ?? throw new ArgumentNullException(nameof(systemUserVerificationFacade));
            _systemUserFacade = systemUserFacade ?? throw new ArgumentNullException(nameof(systemUserFacade));
        }
        #endregion
        [AllowAnonymous]
        [Route("GetBySender")]
        [HttpGet]
        [SwaggerOperation("GetBySender")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IHttpActionResult> GetBySender(string sender, string code)
        {
            AppResponseModel<SystemUserVerificationViewModel> response = new AppResponseModel<SystemUserVerificationViewModel>();

            if (string.IsNullOrEmpty(sender))
            {
                response.Message = string.Format(Messages.CustomError, "Invalid Sender!");
                return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserVerificationViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }

            if (string.IsNullOrEmpty(code))
            {
                response.Message = string.Format(Messages.CustomError, "Invalid Code!");
                return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserVerificationViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                SystemUserVerificationViewModel result = _systemUserVerificationFacade.FindBySender(sender, code);

                if (result != null)
                {
                    response.IsSuccess = true;
                    response.Data = result;
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserVerificationViewModel>>(Request, HttpStatusCode.OK, response);
                }
                else
                {
                    response.Message = Messages.NoRecord;
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserVerificationViewModel>>(Request, HttpStatusCode.NotFound, response);
                }

            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserVerificationViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }

        [Route("")]
        [HttpPost]
        [ValidateModel]
        [SwaggerOperation("create")]
        [SwaggerResponse(HttpStatusCode.Created)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public IHttpActionResult Create([FromBody] SystemUserVerificationBindingModel model)
        {
            AppResponseModel<SystemUserVerificationViewModel> response = new AppResponseModel<SystemUserVerificationViewModel>();

            try
            {
                string code = EmailService.GenerateVerificationCode();
                string id = _systemUserVerificationFacade.Add(model, code);

                if (!string.IsNullOrEmpty(id))
                {

                    var result = _systemUserVerificationFacade.FindById(id);

                    //var success = EmailService.SendEmailVerification(model.VerificationSender, result.VerificationCode);

                    //if (success)
                    //{
                    //    response.IsSuccess = true;
                    //    response.Message = Messages.Created;
                    //    response.Data = new SystemUserVerificationViewModel() { VerificationSender = model.VerificationSender };
                    //    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserVerificationViewModel>>(Request, HttpStatusCode.Created, response);
                    //}
                    //else
                    //{
                    //    response.Message = Messages.Failed;
                    //    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserVerificationViewModel>>(Request, HttpStatusCode.BadRequest, response);
                    //}

                    var success = false;
                    if (string.IsNullOrEmpty(GlobalVariables.goUserVerificationEnable))
                    {
                        response.Message = string.Format(Messages.CustomError, "Invalid User Verification Enable value!");
                        response.DeveloperMessage = string.Format(Messages.CustomError, "Invalid User Verification Enable value!");
                        return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserVerificationViewModel>>(Request, HttpStatusCode.BadRequest, response);
                    }


                    string[] stringArray = { "1", "true", "Yes", "Enable" };
                    var userVerificationEnable = stringArray.ToList().Any(x => GlobalVariables.goUserVerificationEnable.ToLower().Contains(x.ToLower()));
                    if (userVerificationEnable)
                    {
                        success = SMSService.SendVerification(model.VerificationSender, result.VerificationCode);
                        if (success)
                        {
                            response.IsSuccess = true;
                            response.Message = Messages.Created;
                            response.Data = new SystemUserVerificationViewModel() { VerificationSender = model.VerificationSender, IsVerificationEnable = true, };
                            return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserVerificationViewModel>>(Request, HttpStatusCode.Created, response);
                        }
                        else
                        {
                            response.Message = Messages.Failed;
                            return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserVerificationViewModel>>(Request, HttpStatusCode.BadRequest, response);
                        }
                    }
                    else
                    {
                        response.IsSuccess = true;
                        response.Message = Messages.Created;
                        response.Data = new SystemUserVerificationViewModel() { VerificationSender = model.VerificationSender, IsVerificationEnable = false, VerificationCode = code };
                        return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserVerificationViewModel>>(Request, HttpStatusCode.Created, response);
                    }


                }
                else
                {
                    response.Message = Messages.Failed;
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserVerificationViewModel>>(Request, HttpStatusCode.BadRequest, response);

                }
            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserVerificationViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }
        
        [AllowAnonymous]
        [Route("SendVerificationMailKit")]
        [HttpPost]
        [ValidateModel]
        [SwaggerOperation("create")]
        [SwaggerResponse(HttpStatusCode.Created)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public IHttpActionResult SendVerificationMailKit([FromBody] SystemUserVerificationBindingModel model)
        {
            AppResponseModel<SystemUserVerificationViewModel> response = new AppResponseModel<SystemUserVerificationViewModel>();

            try
            {
                string code = EmailService.GenerateVerificationCode();
                string id = _systemUserVerificationFacade.Add(model, code);

                if (!string.IsNullOrEmpty(id))
                {

                    var result = _systemUserVerificationFacade.FindById(id);

                    var success = EmailService.SendEmailVerificationMailKit(model.VerificationSender, result.VerificationCode);

                    if (success)
                    {
                        response.IsSuccess = true;
                        response.Message = Messages.Created;
                        response.Data = new SystemUserVerificationViewModel() { VerificationSender = model.VerificationSender };
                        return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserVerificationViewModel>>(Request, HttpStatusCode.Created, response);
                    }
                    else
                    {
                        response.Message = Messages.Failed;
                        return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserVerificationViewModel>>(Request, HttpStatusCode.BadRequest, response);
                    }
                }
                else
                {
                    response.Message = Messages.Failed;
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserVerificationViewModel>>(Request, HttpStatusCode.BadRequest, response);

                }
            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserVerificationViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }

        [AllowAnonymous]
        [Route("sendEmailChangePasswordRequest")]
        [HttpPost]
        [ValidateModel]
        [SwaggerOperation("SendEmailChangePasswordRequest")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IHttpActionResult SendEmailChangePasswordRequest([FromBody] SystemUserVerificationBindingModel model)
        {
            AppResponseModel<SystemUserVerificationViewModel> response = new AppResponseModel<SystemUserVerificationViewModel>();

            try
            {
                string code = EmailService.GenerateVerificationCode();
                string id = _systemUserVerificationFacade.Add(model, code);

                if (!string.IsNullOrEmpty(id))
                {

                    var result = _systemUserFacade.FindByUsername(model.VerificationSender);
                    if(result == null)
                    {
                        response.Message = Messages.NoRecord;
                        return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserVerificationViewModel>>(Request, HttpStatusCode.NotFound, response);
                    }
                    var verification = _systemUserVerificationFacade.FindById(id);

                    var success = EmailService.SendEmailChangePassword(model.VerificationSender, result.SystemUserId, code);

                    if (success)
                    {
                        response.IsSuccess = true;
                        response.Message = Messages.Created;
                        response.Data = new SystemUserVerificationViewModel() { VerificationSender = model.VerificationSender };
                        return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserVerificationViewModel>>(Request, HttpStatusCode.Created, response);
                    }
                    else
                    {
                        response.Message = Messages.Failed;
                        return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserVerificationViewModel>>(Request, HttpStatusCode.BadRequest, response);
                    }
                }
                else
                {
                    response.Message = Messages.Failed;
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserVerificationViewModel>>(Request, HttpStatusCode.BadRequest, response);

                }
            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserVerificationViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }
    }
}
