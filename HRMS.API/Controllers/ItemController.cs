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
    [RoutePrefix("api/v1/Item")]
    public class ItemController : ApiController
    {
        private readonly IItemFacade _itemFacade;
        private string RecordedBy { get; set; }
        #region CONSTRUCTORS
        public ItemController(IItemFacade itemFacade)
        {
            _itemFacade = itemFacade ?? throw new ArgumentNullException(nameof(itemFacade));
        }

        #endregion


        [Route("getPage")]
        [HttpGet]
        [SwaggerOperation("getPage")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public IHttpActionResult GetPage(int Draw, string Search, int PageNo, int PageSize, string OrderColumn, string OrderDir)
        {
            DataTableResponseModel<IList<ItemViewModel>> response = new DataTableResponseModel<IList<ItemViewModel>>();

            try
            {
                long recordsFiltered = 0;
                long recordsTotal = 0;
                var pageResults = _itemFacade.GetPage(
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

                return new SilupostAPIHttpActionResult<DataTableResponseModel<IList<ItemViewModel>>>(Request, HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                var exception = new AppResponseModel<object>();
                exception.DeveloperMessage = ex.Message;
                exception.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new SilupostAPIHttpActionResult<AppResponseModel<object>>(Request, HttpStatusCode.OK, exception);
            }
        }

        [Route("{id}/detail")]
        [HttpGet]
        [SwaggerOperation("get")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IHttpActionResult Get(string id)
        {
            AppResponseModel<ItemViewModel> response = new AppResponseModel<ItemViewModel>();

            if (string.IsNullOrEmpty(id))
            {
                response.Message = string.Format(Messages.InvalidId, "Item");
                return new SilupostAPIHttpActionResult<AppResponseModel<ItemViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                ItemViewModel result = _itemFacade.Find(id);

                if (result != null)
                {
                    response.IsSuccess = true;
                    response.Data = result;
                    return new SilupostAPIHttpActionResult<AppResponseModel<ItemViewModel>>(Request, HttpStatusCode.OK, response);
                }
                else
                {
                    response.Message = Messages.NoRecord;
                    return new SilupostAPIHttpActionResult<AppResponseModel<ItemViewModel>>(Request, HttpStatusCode.NotFound, response);
                }

            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new SilupostAPIHttpActionResult<AppResponseModel<ItemViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }

        [Route("")]
        [HttpPost]
        [ValidateModel]
        [SwaggerOperation("create")]
        [SwaggerResponse(HttpStatusCode.Created)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public IHttpActionResult Create([FromBody] CreateItemBindingModel model)
        {
            AppResponseModel<ItemViewModel> response = new AppResponseModel<ItemViewModel>();

            if (model != null && model.IconFile == null)
            {
                response.Message = string.Format(Messages.CustomError, "Icon file");
                return new SilupostAPIHttpActionResult<AppResponseModel<ItemViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
            if (string.IsNullOrEmpty(model.IconFile.FileFromBase64String) || string.IsNullOrEmpty(model.IconFile.FileName) || model.IconFile.FileSize <= 0)
            {
                response.Message = string.Format(Messages.CustomError, "Icon file");
                return new SilupostAPIHttpActionResult<AppResponseModel<ItemViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    RecordedBy = identity.FindFirst("SystemUserId").Value;
                }
                var root = HttpContext.Current.Server.MapPath(GlobalVariables.goDefaultSystemUploadRootDirectory);

                var storageDirectory = Path.Combine(root, @"Storage\", string.Format(@"{0}\", RecordedBy));
                var newFileName = string.Format("{0}{1}-{2}-{3}{4}", storageDirectory, RecordedBy, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH-mm"), GlobalFunctions.GetFileExtensionByFileRawFormat(model.IconFile.MimeType));
                model.IconFile.FileName = newFileName;
                Directory.CreateDirectory(storageDirectory);

                string id = _itemFacade.Add(model, RecordedBy);

                if (!string.IsNullOrEmpty(id))
                {
                    var result = _itemFacade.Find(id);

                    response.IsSuccess = true;
                    response.Message = Messages.Created;
                    response.Data = result;
                    return new SilupostAPIHttpActionResult<AppResponseModel<ItemViewModel>>(Request, HttpStatusCode.Created, response);
                }
                else
                {
                    response.Message = Messages.Failed;
                    return new SilupostAPIHttpActionResult<AppResponseModel<ItemViewModel>>(Request, HttpStatusCode.BadRequest, response);

                }
            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new SilupostAPIHttpActionResult<AppResponseModel<ItemViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }
        [Route("")]
        [HttpPut]
        [ValidateModel]
        [SwaggerOperation("update")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IHttpActionResult Update([FromBody] UpdateItemBindingModel model)
        {
            AppResponseModel<ItemViewModel> response = new AppResponseModel<ItemViewModel>();

            if (model != null && string.IsNullOrEmpty(model.ItemId))
            {
                response.Message = string.Format(Messages.InvalidId, "Item");
                return new SilupostAPIHttpActionResult<AppResponseModel<ItemViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
            if (model != null && model.IconFile == null)
            {
                response.Message = string.Format(Messages.CustomError, "Icon file");
                return new SilupostAPIHttpActionResult<AppResponseModel<ItemViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    RecordedBy = identity.FindFirst("SystemUserId").Value;
                }

                var result = _itemFacade.Find(model.ItemId);
                if (result == null)
                {
                    response.Message = string.Format(Messages.InvalidId, "Item");
                    return new SilupostAPIHttpActionResult<AppResponseModel<ItemViewModel>>(Request, HttpStatusCode.BadRequest, response);
                }

                var root = HttpContext.Current.Server.MapPath(GlobalVariables.goDefaultSystemUploadRootDirectory);

                var storageDirectory = Path.Combine(root, @"Storage\", string.Format(@"{0}\", RecordedBy));
                var newFileName = string.Format("{0}{1}-{2}-{3}{4}", storageDirectory, RecordedBy, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH-mm"), GlobalFunctions.GetFileExtensionByFileRawFormat(model.IconFile.MimeType));
                model.IconFile.FileName = newFileName;
                Directory.CreateDirectory(storageDirectory);

                bool success = _itemFacade.Update(model, RecordedBy);
                response.IsSuccess = success;

                if (success)
                {
                    result = _itemFacade.Find(model.ItemId);
                    response.Message = Messages.Updated;
                    response.Data = result;
                    return new SilupostAPIHttpActionResult<AppResponseModel<ItemViewModel>>(Request, HttpStatusCode.OK, response);
                }
                else
                {
                    response.Message = Messages.Failed;
                    return new SilupostAPIHttpActionResult<AppResponseModel<ItemViewModel>>(Request, HttpStatusCode.BadGateway, response);
                }
            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new SilupostAPIHttpActionResult<AppResponseModel<ItemViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }

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
                response.Message = string.Format(Messages.InvalidId, "Item");
                return new SilupostAPIHttpActionResult<AppResponseModel<object>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    RecordedBy = identity.FindFirst("SystemUserId").Value;
                }

                var result = _itemFacade.Find(id);
                if (result == null)
                {
                    response.Message = string.Format(Messages.InvalidId, "Item");
                    return new SilupostAPIHttpActionResult<AppResponseModel<object>>(Request, HttpStatusCode.BadRequest, response);
                }

                bool success = _itemFacade.Remove(id, RecordedBy);
                response.IsSuccess = success;

                if (success)
                {
                    response.Message = Messages.Removed;
                    return new SilupostAPIHttpActionResult<AppResponseModel<object>>(Request, HttpStatusCode.OK, response);
                }
                else
                {
                    response.Message = Messages.NoRecord;
                    return new SilupostAPIHttpActionResult<AppResponseModel<object>>(Request, HttpStatusCode.NotFound, response);
                }

            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new SilupostAPIHttpActionResult<AppResponseModel<object>>(Request, HttpStatusCode.BadRequest, response);
            }
        }
    }
}
