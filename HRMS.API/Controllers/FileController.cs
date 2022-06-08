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
using System.Threading;

namespace HRMS.API.Controllers
{
    [SilupostAuthorizationFilter]
    [RoutePrefix("api/v1/File")]
    public class FileController : ApiController
    {
        private readonly IFileFacade _fileFacade;
        private string RecordedBy { get; set; }
        #region CONSTRUCTORS
        public FileController(IFileFacade fileFacade)
        {
            _fileFacade = fileFacade ?? throw new ArgumentNullException(nameof(fileFacade));
        }

        #endregion
        [Route("getFile")]
        [HttpGet]
        [SwaggerOperation("get")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public IHttpActionResult GetFile(string FileId)
        {
            IHttpActionResult response;

            try
            {
                var result = _fileFacade.Find(FileId);
                if (result == null)
                {
                    string filePath = HttpContext.Current.Server.MapPath(GlobalVariables.goEmailTempProfilePath);
                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    var fileSize = new FileInfo(filePath).Length;
                    using (Image image = Image.FromFile(filePath))
                    {
                        using (MemoryStream m = new MemoryStream())
                        {
                            image.Save(m, image.RawFormat);
                            byte[] imageBytes = m.ToArray();
                            result = new FileViewModel()
                            {
                                FileName = fileName,
                                FileSize = int.Parse(fileSize.ToString()),
                                MimeType = image.RawFormat.ToString(),
                                FileContent = imageBytes
                            };
                        }
                    }
                }
                string mimeType = MimeMapping.GetMimeMapping(result.FileName);
                var contentType = new MediaTypeHeaderValue(mimeType);
                HttpResponseMessage responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                Stream fileStream;
                if (result.IsFromStorage)
                {
                    fileStream = new MemoryStream(System.IO.File.ReadAllBytes(result.FileName));
                }
                else
                {
                    fileStream = new MemoryStream(result.FileContent);
                }
                responseMessage.Content = new StreamContent(fileStream);
                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = result.FileName,
                    Inline = false,
                };
                responseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue(cd.DispositionType.ToString());
                responseMessage.Content.Headers.ContentDisposition.FileName = result.FileName;
                //responseMessage.Content.Headers.ContentType = contentType;
                responseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = ResponseMessage(responseMessage);
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [Route("getDefaultSystemUserProfilePic")]
        [HttpGet]
        [SwaggerOperation("get")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public IHttpActionResult GetDefaultSystemUserProfilePic()
        {
            AppResponseModel<FileViewModel> response = new AppResponseModel<FileViewModel>();

            try
            {
                string filePath = HttpContext.Current.Server.MapPath(GlobalVariables.goDefaultSystemUserProfilePicPath);
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                var fileSize = new FileInfo(filePath).Length;
                using (Image image = Image.FromFile(filePath))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();
                        var file = new FileViewModel()
                        {
                            FileName = fileName,
                            FileSize = int.Parse(fileSize.ToString()),
                            MimeType = image.RawFormat.ToString(),
                            FileContent = imageBytes
                        };
                        response.Data = file;
                        response.IsSuccess = true;
                        return new SilupostAPIHttpActionResult<AppResponseModel<FileViewModel>>(Request, HttpStatusCode.OK, response);
                    }
                }

            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new SilupostAPIHttpActionResult<AppResponseModel<FileViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }

        [Route("getDefaultItemBrandProfilePic")]
        [HttpGet]
        [SwaggerOperation("get")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public IHttpActionResult GetDefaultItemBrandProfilePic()
        {
            AppResponseModel<FileViewModel> response = new AppResponseModel<FileViewModel>();

            try
            {
                string filePath = HttpContext.Current.Server.MapPath(GlobalVariables.goDefaultItemBrandIconFilePath);
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                var fileSize = new FileInfo(filePath).Length;
                using (Image image = Image.FromFile(filePath))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();
                        var file = new FileViewModel()
                        {
                            FileName = fileName,
                            FileSize = int.Parse(fileSize.ToString()),
                            MimeType = image.RawFormat.ToString(),
                            FileContent = imageBytes
                        };
                        response.Data = file;
                        response.IsSuccess = true;
                        return new SilupostAPIHttpActionResult<AppResponseModel<FileViewModel>>(Request, HttpStatusCode.OK, response);
                    }
                }

            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new SilupostAPIHttpActionResult<AppResponseModel<FileViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }


        [Route("getDefaultItemProfilePic")]
        [HttpGet]
        [SwaggerOperation("get")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public IHttpActionResult GetDefaultItemProfilePic()
        {
            AppResponseModel<FileViewModel> response = new AppResponseModel<FileViewModel>();

            try
            {
                string filePath = HttpContext.Current.Server.MapPath(GlobalVariables.goDefaultItemIconFilePath);
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                var fileSize = new FileInfo(filePath).Length;
                using (Image image = Image.FromFile(filePath))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();
                        var file = new FileViewModel()
                        {
                            FileName = fileName,
                            FileSize = int.Parse(fileSize.ToString()),
                            MimeType = image.RawFormat.ToString(),
                            FileContent = imageBytes
                        };
                        response.Data = file;
                        response.IsSuccess = true;
                        return new SilupostAPIHttpActionResult<AppResponseModel<FileViewModel>>(Request, HttpStatusCode.OK, response);
                    }
                }

            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new SilupostAPIHttpActionResult<AppResponseModel<FileViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }


    }
}
