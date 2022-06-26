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
    [RoutePrefix("api/v1/SystemUser")]
    public class SystemUserController : ApiController
    {
        private readonly ISystemUserFacade _systemUserFacade;
        private readonly ISystemUserVerificationFacade _systemUserVerificationFacade;
        private readonly ILegalEntityAddressFacade _legalEntityAddressFacade;
        private string RecordedBy { get; set; }
        #region CONSTRUCTORS
        public SystemUserController(ISystemUserFacade systemUserFacade, ISystemUserVerificationFacade systemUserVerificationFacade, ILegalEntityAddressFacade legalEntityAddressFacade)
        {
            _systemUserFacade = systemUserFacade ?? throw new ArgumentNullException(nameof(systemUserFacade));
            _systemUserVerificationFacade = systemUserVerificationFacade ?? throw new ArgumentNullException(nameof(systemUserVerificationFacade));
            _legalEntityAddressFacade = legalEntityAddressFacade ?? throw new ArgumentNullException(nameof(legalEntityAddressFacade));
        }
        #endregion


        [Authorize]
        [Route("getPage")]
        [HttpGet]
        [SwaggerOperation("getPage")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public IHttpActionResult GetPage(int Draw, long SystemUserType, long ApprovalStatus, string Search, int PageNo, int PageSize, string OrderColumn, string OrderDir)
        {
            DataTableResponseModel<IList<SystemUserViewModel>> response = new DataTableResponseModel<IList<SystemUserViewModel>>();

            try
            {
                long recordsFiltered = 0;
                long recordsTotal = 0;
                var pageResults = _systemUserFacade.GetPage(
                    (Search = string.IsNullOrEmpty(Search) ? string.Empty : Search),
                    SystemUserType,
                    ApprovalStatus,
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

                return new HRMSAPIHttpActionResult<DataTableResponseModel<IList<SystemUserViewModel>>>(Request, HttpStatusCode.OK, response);
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
            AppResponseModel<SystemUserViewModel> response = new AppResponseModel<SystemUserViewModel>();

            if (string.IsNullOrEmpty(id))
            {
                response.Message = string.Format(Messages.InvalidId, "System User");
                return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                SystemUserViewModel result = _systemUserFacade.Find(id);

                if (result != null)
                {
                    response.IsSuccess = true;
                    response.Data = result;
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.OK, response);
                }
                else
                {
                    response.Message = Messages.NoRecord;
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.NotFound, response);
                }

            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }

        [Authorize]
        [Route("{id}/TrackerStatus")]
        [HttpGet]
        [SwaggerOperation("get")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IHttpActionResult GetTrackerStatus(string id)
        {
            AppResponseModel<SystemUserViewModel> response = new AppResponseModel<SystemUserViewModel>();

            if (string.IsNullOrEmpty(id))
            {
                response.Message = string.Format(Messages.InvalidId, "System User");
                return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                SystemUserViewModel result = _systemUserFacade.GetTrackerStatus(id);

                if (result != null)
                {
                    response.IsSuccess = true;
                    response.Data = result;
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.OK, response);
                }
                else
                {
                    response.Message = Messages.NoRecord;
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.NotFound, response);
                }

            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }

        [HRMSAuthorizationFilter]
        [Route("GetByCredentials")]
        [HttpGet]
        [SwaggerOperation("GetByCredentials")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IHttpActionResult> GetByCredentials(string username, string password, long systemUserTypeId)
        {
            AppResponseModel<SystemUserViewModel> response = new AppResponseModel<SystemUserViewModel>();

            if (string.IsNullOrEmpty(username))
            {
                response.Message = string.Format(Messages.CustomError, "Invalid Username!");
                return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }

            if (string.IsNullOrEmpty(password))
            {
                response.Message = string.Format(Messages.CustomError, "Invalid Password!");
                return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
            if (systemUserTypeId == (int)SYSTEM_USER_TYPE_ENUMS.USER_PORTAL || systemUserTypeId == (int)SYSTEM_USER_TYPE_ENUMS.USER_WEBADMIN)
            {
                try
                {
                    SystemUserViewModel result = null;
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(GlobalVariables.goOAuthURI);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage responseMessage = new HttpResponseMessage();
                        List<KeyValuePair<string, string>> allIputParams = new List<KeyValuePair<string, string>>();
                        allIputParams.Add(new KeyValuePair<string, string>("grant_type", "password"));
                        allIputParams.Add(new KeyValuePair<string, string>("username", username));
                        allIputParams.Add(new KeyValuePair<string, string>("password", password));
                        allIputParams.Add(new KeyValuePair<string, string>("client_id", GlobalVariables.goClientId));
                        HttpContent requestParams = new FormUrlEncodedContent(allIputParams);
                        responseMessage = await client.PostAsync("oauth2/token", requestParams);

                        string _json = await responseMessage.Content.ReadAsStringAsync();

                        JObject obj = JsonConvert.DeserializeObject<JObject>(_json);
                        var token = obj.ToObject<SystemTokenViewModel>();

                        if (responseMessage.IsSuccessStatusCode)
                        {
                            result = _systemUserFacade.Find(username, password);

                            if (result != null && result.SystemUserType.SystemUserTypeId == systemUserTypeId)
                            {
                                result.Token = token;
                                response.IsSuccess = true;
                                response.Data = result;
                                return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.OK, response);
                            }
                            else
                            {
                                response.Message = string.Format(Messages.CustomError, "Username or Password is incorrect!");
                                return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.NotFound, response);
                            }
                        }
                        else
                        {
                            response.Message = string.IsNullOrEmpty(token.ErrorDescription) ? token.Error : token.ErrorDescription;
                            response.DeveloperMessage = string.IsNullOrEmpty(token.ErrorDescription) ? token.Error : token.ErrorDescription;
                            return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.NotFound, response);
                        }
                    }
                }
                catch (Exception ex)
                {
                    response.DeveloperMessage = ex.Message;
                    response.Message = Messages.ServerError;
                    //TODO Logging of exceptions
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
                }
            }
            else
            {
                response.Message = string.Format(Messages.CustomError, "Invalid systemUserTypeId!");
                return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }

        [Authorize]
        [Route("")]
        [HttpPost]
        [ValidateModel]
        [SwaggerOperation("create")]
        [SwaggerResponse(HttpStatusCode.Created)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public IHttpActionResult Create([FromBody] CreateSystemUserBindingModel model)
        {
            AppResponseModel<SystemUserViewModel> response = new AppResponseModel<SystemUserViewModel>();

            try
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    RecordedBy = identity.FindFirst("SystemUserId").Value;
                }
                var root = HttpContext.Current.Server.MapPath(GlobalVariables.goDefaultSystemUploadRootDirectory);

                var storageDirectory = Path.Combine(root, @"Storage\", string.Format(@"{0}\", RecordedBy));
                var newFileName = string.Format("{0}{1}-{2}-{3}{4}", storageDirectory, RecordedBy, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH-mm"), GlobalFunctions.GetFileExtensionByFileRawFormat(model.ProfilePicture.MimeType));
                model.ProfilePicture.FileName = newFileName;
                Directory.CreateDirectory(storageDirectory);
                string id = _systemUserFacade.Add(model, RecordedBy);

                if (!string.IsNullOrEmpty(id))
                {
                    var result = _systemUserFacade.Find(id);

                    response.IsSuccess = true;
                    response.Message = Messages.Created;
                    response.Data = result;
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.Created, response);
                }
                else
                {
                    response.Message = Messages.Failed;
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);

                }
            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }

        [HRMSAuthorizationFilter]
        [Route("CreateAccount")]
        [HttpPost]
        [ValidateModel]
        [SwaggerOperation("create")]
        [SwaggerResponse(HttpStatusCode.Created)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public IHttpActionResult CreateAccount([FromBody] CreateAccountSystemUserBindingModel model)
        {
            AppResponseModel<SystemUserViewModel> response = new AppResponseModel<SystemUserViewModel>();

            try
            {
                if (string.IsNullOrEmpty(model.VerificationCode))
                {
                    response.Message = string.Format(Messages.InvalidId, "Verification Code");
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
                }

                var verification = _systemUserVerificationFacade.FindBySender(model.VerificationSender, model.VerificationCode);

                if (verification == null)
                {
                    response.Message = "User is not verified";
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
                }
                else
                {
                    if (verification.IsVerified)
                    {
                        response.Message = "Username or Email already in used";
                        return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
                    }
                }
                FileBindingModel profilePic = null;
                string fileName = HttpContext.Current.Server.MapPath(GlobalVariables.goDefaultSystemUserProfilePicPath);
                var fileSize = new FileInfo(fileName).Length;
                using (Image image = Image.FromFile(fileName))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();
                        profilePic = new FileBindingModel()
                        {
                            FileName = fileName,
                            MimeType = image.RawFormat.ToString(),
                            FileContent = imageBytes,
                            IsDefault = true,
                            IsFromStorage = false
                        };

                    }
                }

                if(profilePic == null)
                {
                    response.Message = string.Format(Messages.CustomError, "There is a problem ecountered while creating System User, Default ProfilePic not found");
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
                }

                string id = _systemUserFacade.CreateAccount(model, profilePic);

                if (!string.IsNullOrEmpty(id))
                {
                    var result = _systemUserFacade.Find(id);

                    response.IsSuccess = true;
                    response.Message = Messages.Created;
                    response.Data = result;
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.Created, response);
                }
                else
                {
                    response.Message = Messages.Failed;
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);

                }
            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }

        [HRMSAuthorizationFilter]
        [Route("CreateWebAdminAccount")]
        [HttpPost]
        [ValidateModel]
        [SwaggerOperation("create")]
        [SwaggerResponse(HttpStatusCode.Created)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public IHttpActionResult CreateWebAdminAccount([FromBody] CreateWebAccountSystemUserBindingModel model)
        {
            AppResponseModel<SystemUserViewModel> response = new AppResponseModel<SystemUserViewModel>();

            try
            {
                if (string.IsNullOrEmpty(model.VerificationCode))
                {
                    response.Message = string.Format(Messages.InvalidId, "Verification Code");
                    response.DeveloperMessage = response.Message;
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
                }

                var verification = _systemUserVerificationFacade.FindBySender(model.VerificationSender, model.VerificationCode);

                if (verification == null)
                {
                    response.Message = "User is not verified";
                    response.DeveloperMessage = response.Message;
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
                }
                else
                {
                    if (verification.IsVerified)
                    {
                        response.Message = "Username or Email already in used";
                        response.DeveloperMessage = response.Message;
                        return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
                    }
                }

                FileBindingModel profilePic = null;
                string fileName = HttpContext.Current.Server.MapPath(GlobalVariables.goDefaultSystemUserProfilePicPath);
                var fileSize = new FileInfo(fileName).Length;
                using (Image image = Image.FromFile(fileName))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();
                        profilePic = new FileBindingModel()
                        {
                            FileName = fileName,
                            MimeType = image.RawFormat.ToString(),
                            FileContent = imageBytes,
                            IsDefault = true,
                            IsFromStorage = false
                        };

                    }
                }

                if (profilePic == null)
                {
                    response.Message = string.Format(Messages.CustomError, "There is a problem ecountered while creating System User, Default ProfilePic not found");
                    response.DeveloperMessage = response.Message;
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
                }

                string id = _systemUserFacade.CreateWebAdminAccount(model, profilePic);

                if (!string.IsNullOrEmpty(id))
                {
                    var result = _systemUserFacade.Find(id);

                    response.IsSuccess = true;
                    response.Message = Messages.Created;
                    response.Data = result;
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.Created, response);
                }
                else
                {
                    response.Message = Messages.Failed;
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);

                }
            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
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
        public IHttpActionResult Update([FromBody] UpdateSystemUserBindingModel model)
        {
            AppResponseModel<SystemUserViewModel> response = new AppResponseModel<SystemUserViewModel>();

            if (model != null && string.IsNullOrEmpty(model.SystemUserId))
            {
                response.Message = string.Format(Messages.InvalidId, "System User");
                return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    RecordedBy = identity.FindFirst("SystemUserId").Value;
                }
                var result = _systemUserFacade.Find(model.SystemUserId);
                if (result == null)
                {
                    response.Message = string.Format(Messages.InvalidId, "System User");
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
                }
                if (model.ProfilePicture != null)
                {
                    model.ProfilePicture.IsFromStorage = false;
                }


                if (model.ProfilePicture == null || string.IsNullOrEmpty(model.ProfilePicture.FileId) && model.ProfilePicture.FileContent == null)
                {

                    UpdateFileBindingModel profilePic = null;
                    string fileName = HttpContext.Current.Server.MapPath(GlobalVariables.goDefaultSystemUserProfilePicPath);
                    var fileSize = new FileInfo(fileName).Length;
                    using (Image image = Image.FromFile(fileName))
                    {
                        using (MemoryStream m = new MemoryStream())
                        {
                            image.Save(m, image.RawFormat);
                            byte[] imageBytes = m.ToArray();
                            profilePic = new UpdateFileBindingModel()
                            {
                                FileName = fileName,
                                MimeType = image.RawFormat.ToString(),
                                FileContent = imageBytes,
                                IsDefault = true,
                                IsFromStorage = false
                            };

                        }
                    }

                    if (profilePic == null)
                    {
                        response.Message = string.Format(Messages.CustomError, "There is a problem ecountered while creating System User, Default ProfilePic not found");
                        return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
                    }
                    model.ProfilePicture = profilePic;
                }

                bool success = _systemUserFacade.Update(model, RecordedBy);
                response.IsSuccess = success;

                if (success)
                {
                    result = _systemUserFacade.Find(model.SystemUserId);
                    response.Message = Messages.Updated;
                    response.Data = result;
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.OK, response);
                }
                else
                {
                    response.Message = Messages.Failed;
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadGateway, response);
                }
            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }

        [Authorize]
        [Route("UpdatePersonalDetails")]
        [HttpPut]
        [ValidateModel]
        [SwaggerOperation("update")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IHttpActionResult UpdatePersonalDetails([FromBody] UpdateSystemUserBindingModel model)
        {
            AppResponseModel<SystemUserViewModel> response = new AppResponseModel<SystemUserViewModel>();

            if (model != null && string.IsNullOrEmpty(model.SystemUserId))
            {
                response.Message = string.Format(Messages.InvalidId, "System User");
                return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    RecordedBy = identity.FindFirst("SystemUserId").Value;
                }
                var result = _systemUserFacade.Find(model.SystemUserId);
                if (result == null)
                {
                    response.Message = string.Format(Messages.InvalidId, "System User");
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
                }
                var root = HttpContext.Current.Server.MapPath(GlobalVariables.goDefaultSystemUploadRootDirectory);

                var storageDirectory = Path.Combine(root, @"Storage\", string.Format(@"{0}\", model.SystemUserId));
                var newFileName = string.Format("{0}{1}-{2}-{3}{4}", storageDirectory, model.SystemUserId, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH-mm"), GlobalFunctions.GetFileExtensionByFileRawFormat(model.ProfilePicture.MimeType));
                model.ProfilePicture.FileName = newFileName;
                Directory.CreateDirectory(storageDirectory);
                bool success = _systemUserFacade.UpdatePersonalDetails(model, RecordedBy);
                response.IsSuccess = success;

                if (success)
                {
                    result = _systemUserFacade.Find(model.SystemUserId);
                    response.Message = Messages.Updated;
                    response.Data = result;
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.OK, response);
                }
                else
                {
                    response.Message = Messages.Failed;
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadGateway, response);
                }
            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }

        [Authorize]
        [Route("UpdateUsername")]
        [HttpPut]
        [ValidateModel]
        [SwaggerOperation("update")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IHttpActionResult UpdateUsername([FromBody] UpdateSystemUserNameBindingModel model)
        {
            AppResponseModel<SystemUserViewModel> response = new AppResponseModel<SystemUserViewModel>();

            if (model != null && string.IsNullOrEmpty(model.SystemUserId))
            {
                response.Message = string.Format(Messages.InvalidId, "SystemUserId");
                return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    RecordedBy = identity.FindFirst("SystemUserId").Value;
                }
                var result = _systemUserFacade.Find(model.SystemUserId);
                if (result == null)
                {
                    response.Message = Messages.NoRecord;
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
                }

                bool success = _systemUserFacade.UpdateUsername(model, RecordedBy);
                response.IsSuccess = success;

                if (success)
                {
                    result = _systemUserFacade.Find(model.SystemUserId);
                    response.Message = Messages.Updated;
                    response.Data = result;
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.OK, response);
                }
                else
                {
                    response.Message = Messages.Failed;
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadGateway, response);
                }
            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }

        [HRMSAuthorizationFilter]
        [Route("ResetPassword")]
        [HttpPut]
        [ValidateModel]
        [SwaggerOperation("update")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IHttpActionResult ResetPassword([FromBody] UpdateSystemResetPasswordBindingModel model)
        {
            AppResponseModel<SystemUserViewModel> response = new AppResponseModel<SystemUserViewModel>();

            if (model != null && string.IsNullOrEmpty(model.SystemUserId))
            {
                response.Message = string.Format(Messages.InvalidId, "System User");
                return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                var result = _systemUserFacade.Find(model.SystemUserId);
                if (result == null)
                {
                    response.Message = string.Format(Messages.InvalidId, "SystemUserId");
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
                }

                bool success = _systemUserFacade.ResetPassword(model, model.SystemUserId);
                response.IsSuccess = success;

                if (success)
                {
                    result = _systemUserFacade.Find(model.SystemUserId);
                    response.Message = Messages.Updated;
                    response.Data = result;
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.OK, response);
                }
                else
                {
                    response.Message = Messages.Failed;
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadGateway, response);
                }
            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }

        [Authorize]
        [Route("UpdatePassword")]
        [HttpPut]
        [ValidateModel]
        [SwaggerOperation("update")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IHttpActionResult UpdatePassword([FromBody] UpdateSystemPasswordBindingModel model)
        {
            AppResponseModel<SystemUserViewModel> response = new AppResponseModel<SystemUserViewModel>();

            if (model != null && string.IsNullOrEmpty(model.SystemUserId))
            {
                response.Message = string.Format(Messages.InvalidId, "System User");
                return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    RecordedBy = identity.FindFirst("SystemUserId").Value;
                }
                var result = _systemUserFacade.Find(model.SystemUserId);
                if (result == null)
                {
                    response.Message = string.Format(Messages.InvalidId, "SystemUserId");
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
                }
                var userLogin = _systemUserFacade.Find(result.UserName, model.OldPassword);
                if (userLogin == null)
                {
                    response.Message = Messages.NoRecord;
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
                }

                bool success = _systemUserFacade.UpdatePassword(model, RecordedBy);
                response.IsSuccess = success;

                if (success)
                {
                    result = _systemUserFacade.Find(model.SystemUserId);
                    response.Message = Messages.Updated;
                    response.Data = result;
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.OK, response);
                }
                else
                {
                    response.Message = Messages.Failed;
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadGateway, response);
                }
            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
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
                response.Message = string.Format(Messages.InvalidId, "System User");
                return new HRMSAPIHttpActionResult<AppResponseModel<object>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    RecordedBy = identity.FindFirst("SystemUserId").Value;
                }

                var result = _systemUserFacade.Find(id);
                if (result == null)
                {
                    response.Message = string.Format(Messages.InvalidId, "System User");
                    return new HRMSAPIHttpActionResult<AppResponseModel<object>>(Request, HttpStatusCode.BadRequest, response);
                }

                bool success = _systemUserFacade.Remove(id, RecordedBy);
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
        [Route("GetAddressByLegalEntityId")]
        [HttpGet]
        [SwaggerOperation("get")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IHttpActionResult GetAddressByLegalEntityId(string legalEntityId)
        {
            AppResponseModel<List<LegalEntityAddressViewModel>> response = new AppResponseModel<List<LegalEntityAddressViewModel>>();

            if (string.IsNullOrEmpty(legalEntityId))
            {
                response.Message = string.Format(Messages.InvalidId, "System User LegalEntity Address");
                return new HRMSAPIHttpActionResult<AppResponseModel<List<LegalEntityAddressViewModel>>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                List<LegalEntityAddressViewModel> result = _legalEntityAddressFacade.FindByLegalEntityId(legalEntityId);

                if (result != null)
                {
                    response.IsSuccess = true;
                    response.Data = result;
                    return new HRMSAPIHttpActionResult<AppResponseModel<List<LegalEntityAddressViewModel>>>(Request, HttpStatusCode.OK, response);
                }
                else
                {
                    response.Message = Messages.NoRecord;
                    return new HRMSAPIHttpActionResult<AppResponseModel<List<LegalEntityAddressViewModel>>>(Request, HttpStatusCode.NotFound, response);
                }

            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<List<LegalEntityAddressViewModel>>>(Request, HttpStatusCode.BadRequest, response);
            }
        }

        [Authorize]
        [Route("createSystemUserAddress")]
        [HttpPost]
        [ValidateModel]
        [SwaggerOperation("create")]
        [SwaggerResponse(HttpStatusCode.Created)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public IHttpActionResult CreateSystemUserAddress([FromBody] CreateLegalEntityAddressBindingModel model)
        {
            AppResponseModel<LegalEntityAddressViewModel> response = new AppResponseModel<LegalEntityAddressViewModel>();

            try
            {
                string id = _legalEntityAddressFacade.Add(model);

                if (!string.IsNullOrEmpty(id))
                {
                    var result = _legalEntityAddressFacade.Find(id);

                    response.IsSuccess = true;
                    response.Message = Messages.Created;
                    response.Data = result;
                    return new HRMSAPIHttpActionResult<AppResponseModel<LegalEntityAddressViewModel>>(Request, HttpStatusCode.Created, response);
                }
                else
                {
                    response.Message = Messages.Failed;
                    return new HRMSAPIHttpActionResult<AppResponseModel<LegalEntityAddressViewModel>>(Request, HttpStatusCode.BadRequest, response);

                }
            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<LegalEntityAddressViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }

        [Authorize]
        [Route("UpdateSystemUserAddress")]
        [HttpPut]
        [ValidateModel]
        [SwaggerOperation("update")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IHttpActionResult UpdateSystemUserAddress([FromBody] UpdateLegalEntityAddressBindingModel model)
        {
            AppResponseModel<LegalEntityAddressViewModel> response = new AppResponseModel<LegalEntityAddressViewModel>();

            if (model != null && string.IsNullOrEmpty(model.LegalEntityAddressId))
            {
                response.Message = string.Format(Messages.InvalidId, "System User LegalEntity Address");
                return new HRMSAPIHttpActionResult<AppResponseModel<LegalEntityAddressViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                var result = _legalEntityAddressFacade.Find(model.LegalEntityAddressId);
                if (result == null)
                {
                    response.Message = string.Format(Messages.InvalidId, "System User LegalEntity Address");
                    return new HRMSAPIHttpActionResult<AppResponseModel<LegalEntityAddressViewModel>>(Request, HttpStatusCode.BadRequest, response);
                }
                bool success = _legalEntityAddressFacade.Update(model);
                response.IsSuccess = success;

                if (success)
                {
                    result = _legalEntityAddressFacade.Find(model.LegalEntityAddressId);
                    response.Message = Messages.Updated;
                    response.Data = result;
                    return new HRMSAPIHttpActionResult<AppResponseModel<LegalEntityAddressViewModel>>(Request, HttpStatusCode.OK, response);
                }
                else
                {
                    response.Message = Messages.Failed;
                    return new HRMSAPIHttpActionResult<AppResponseModel<LegalEntityAddressViewModel>>(Request, HttpStatusCode.BadGateway, response);
                }
            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<LegalEntityAddressViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }

        [Authorize]
        [Route("RemoveSystemUserAddress/{id}")]
        [HttpDelete]
        [SwaggerOperation("remove")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IHttpActionResult RemoveSystemUserAddress(string id)
        {
            AppResponseModel<object> response = new AppResponseModel<object>();

            if (string.IsNullOrEmpty(id))
            {
                response.Message = string.Format(Messages.InvalidId, "System User LegalEntity Address");
                return new HRMSAPIHttpActionResult<AppResponseModel<object>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {

                var result = _legalEntityAddressFacade.Find(id);
                if (result == null)
                {
                    response.Message = string.Format(Messages.InvalidId, "System User LegalEntity Address");
                    return new HRMSAPIHttpActionResult<AppResponseModel<object>>(Request, HttpStatusCode.BadRequest, response);
                }

                bool success = _legalEntityAddressFacade.Remove(id);
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


        [HRMSAuthorizationFilter]
        [Route("GetRefreshToken")]
        [HttpGet]
        [SwaggerOperation("GetRefreshToken")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IHttpActionResult> GetRefreshToken(string RefreshToken)
        {
            AppResponseModel<SystemTokenViewModel> response = new AppResponseModel<SystemTokenViewModel>();

            if (string.IsNullOrEmpty(RefreshToken))
            {
                response.Message = string.Format(Messages.CustomError, "Invalid Refresh Token!");
                return new HRMSAPIHttpActionResult<AppResponseModel<SystemTokenViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                SystemUserViewModel result = null;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(GlobalVariables.goOAuthURI);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage responseMessage = new HttpResponseMessage();
                    List<KeyValuePair<string, string>> allIputParams = new List<KeyValuePair<string, string>>();
                    allIputParams.Add(new KeyValuePair<string, string>("grant_type", "refresh_token"));
                    allIputParams.Add(new KeyValuePair<string, string>("refresh_token", RefreshToken));
                    HttpContent requestParams = new FormUrlEncodedContent(allIputParams);
                    responseMessage = await client.PostAsync("oauth2/token", requestParams);

                    string _json = await responseMessage.Content.ReadAsStringAsync();

                    JObject obj = JsonConvert.DeserializeObject<JObject>(_json);
                    var token = obj.ToObject<SystemTokenViewModel>();

                    if (!string.IsNullOrEmpty(token.AccessToken) && string.IsNullOrEmpty(token.Error))
                    {
                        if (responseMessage.IsSuccessStatusCode)
                        {
                            response.IsSuccess = true;
                            response.Data = token;
                            return new HRMSAPIHttpActionResult<AppResponseModel<SystemTokenViewModel>>(Request, HttpStatusCode.OK, response);
                        }
                        else
                        {
                            response.Message = Messages.ServerError;
                            response.DeveloperMessage = responseMessage.Content.ReadAsAsync<HttpError>().Result.ExceptionMessage;
                            return new HRMSAPIHttpActionResult<AppResponseModel<SystemTokenViewModel>>(Request, HttpStatusCode.NotFound, response);
                        }
                    }
                    else
                    {
                        response.Message = token.Error;
                        response.DeveloperMessage = responseMessage.Content.ReadAsAsync<HttpError>().Result.ExceptionMessage;
                        return new HRMSAPIHttpActionResult<AppResponseModel<SystemTokenViewModel>>(Request, HttpStatusCode.NotFound, response);
                    }
                }
            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<SystemTokenViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }


        [Authorize]
        [Route("UpdateSystemUserConfig")]
        [HttpPut]
        [ValidateModel]
        [SwaggerOperation("update")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IHttpActionResult UpdateSystemUserConfig([FromBody] UpdateSystemUserConfigBindingModel model)
        {
            AppResponseModel<SystemUserViewModel> response = new AppResponseModel<SystemUserViewModel>();

            if (model != null && string.IsNullOrEmpty(model.SystemUserConfigId))
            {
                response.Message = string.Format(Messages.InvalidId, "System User Config");
                return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    RecordedBy = identity.FindFirst("SystemUserId").Value;
                }
                bool success = _systemUserFacade.UpdateSystemUserConfig(model);
                response.IsSuccess = success;

                if (success)
                {
                    response.Message = Messages.Updated;
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.OK, response);
                }
                else
                {
                    response.Message = Messages.Failed;
                    return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadGateway, response);
                }
            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<SystemUserViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }
    }
}
