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
    [RoutePrefix("api/v1/ItemBrand")]
    public class ItemBrandController : ApiController
    {
        private readonly IItemBrandFacade _itemBrandFacade;
        private string RecordedBy { get; set; }
        #region CONSTRUCTORS
        public ItemBrandController(IItemBrandFacade itemBrandFacade)
        {
            _itemBrandFacade = itemBrandFacade ?? throw new ArgumentNullException(nameof(itemBrandFacade));
        }

        #endregion


        [Route("getPage")]
        [HttpGet]
        [SwaggerOperation("getPage")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public IHttpActionResult GetPage(int Draw, string Search, int PageNo, int PageSize, string OrderColumn, string OrderDir)
        {
            DataTableResponseModel<IList<ItemBrandViewModel>> response = new DataTableResponseModel<IList<ItemBrandViewModel>>();

            try
            {
                long recordsFiltered = 0;
                long recordsTotal = 0;
                var pageResults = _itemBrandFacade.GetPage(
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

                return new SilupostAPIHttpActionResult<DataTableResponseModel<IList<ItemBrandViewModel>>>(Request, HttpStatusCode.OK, response);
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
            AppResponseModel<ItemBrandViewModel> response = new AppResponseModel<ItemBrandViewModel>();

            if (string.IsNullOrEmpty(id))
            {
                response.Message = string.Format(Messages.InvalidId, "Item Brand");
                return new SilupostAPIHttpActionResult<AppResponseModel<ItemBrandViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                ItemBrandViewModel result = _itemBrandFacade.Find(id);

                if (result != null)
                {
                    response.IsSuccess = true;
                    response.Data = result;
                    return new SilupostAPIHttpActionResult<AppResponseModel<ItemBrandViewModel>>(Request, HttpStatusCode.OK, response);
                }
                else
                {
                    response.Message = Messages.NoRecord;
                    return new SilupostAPIHttpActionResult<AppResponseModel<ItemBrandViewModel>>(Request, HttpStatusCode.NotFound, response);
                }

            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new SilupostAPIHttpActionResult<AppResponseModel<ItemBrandViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }

        [Route("")]
        [HttpPost]
        [ValidateModel]
        [SwaggerOperation("create")]
        [SwaggerResponse(HttpStatusCode.Created)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public IHttpActionResult Create([FromBody] CreateItemBrandBindingModel model)
        {
            AppResponseModel<ItemBrandViewModel> response = new AppResponseModel<ItemBrandViewModel>();

            if (model != null && model.IconFile == null)
            {
                response.Message = string.Format(Messages.CustomError, "Icon file");
                return new SilupostAPIHttpActionResult<AppResponseModel<ItemBrandViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
            if (string.IsNullOrEmpty(model.IconFile.FileFromBase64String) || string.IsNullOrEmpty(model.IconFile.FileName) || model.IconFile.FileSize <= 0)
            {
                response.Message = string.Format(Messages.CustomError, "Icon file");
                return new SilupostAPIHttpActionResult<AppResponseModel<ItemBrandViewModel>>(Request, HttpStatusCode.BadRequest, response);
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

                string id = _itemBrandFacade.Add(model, RecordedBy);

                if (!string.IsNullOrEmpty(id))
                {
                    var result = _itemBrandFacade.Find(id);

                    response.IsSuccess = true;
                    response.Message = Messages.Created;
                    response.Data = result;
                    return new SilupostAPIHttpActionResult<AppResponseModel<ItemBrandViewModel>>(Request, HttpStatusCode.Created, response);
                }
                else
                {
                    response.Message = Messages.Failed;
                    return new SilupostAPIHttpActionResult<AppResponseModel<ItemBrandViewModel>>(Request, HttpStatusCode.BadRequest, response);

                }
            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new SilupostAPIHttpActionResult<AppResponseModel<ItemBrandViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
        }
        [Route("")]
        [HttpPut]
        [ValidateModel]
        [SwaggerOperation("update")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IHttpActionResult Update([FromBody] UpdateItemBrandBindingModel model)
        {
            AppResponseModel<ItemBrandViewModel> response = new AppResponseModel<ItemBrandViewModel>();

            if (model != null && string.IsNullOrEmpty(model.ItemBrandId))
            {
                response.Message = string.Format(Messages.InvalidId, "Item Brand");
                return new SilupostAPIHttpActionResult<AppResponseModel<ItemBrandViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }
            if (model != null && model.IconFile == null)
            {
                response.Message = string.Format(Messages.CustomError, "Icon file");
                return new SilupostAPIHttpActionResult<AppResponseModel<ItemBrandViewModel>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    RecordedBy = identity.FindFirst("SystemUserId").Value;
                }

                var result = _itemBrandFacade.Find(model.ItemBrandId);
                if (result == null)
                {
                    response.Message = string.Format(Messages.InvalidId, "Item Brand");
                    return new SilupostAPIHttpActionResult<AppResponseModel<ItemBrandViewModel>>(Request, HttpStatusCode.BadRequest, response);
                }

                var root = HttpContext.Current.Server.MapPath(GlobalVariables.goDefaultSystemUploadRootDirectory);

                var storageDirectory = Path.Combine(root, @"Storage\", string.Format(@"{0}\", RecordedBy));
                var newFileName = string.Format("{0}{1}-{2}-{3}{4}", storageDirectory, RecordedBy, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH-mm"), GlobalFunctions.GetFileExtensionByFileRawFormat(model.IconFile.MimeType));
                model.IconFile.FileName = newFileName;
                Directory.CreateDirectory(storageDirectory);

                bool success = _itemBrandFacade.Update(model, RecordedBy);
                response.IsSuccess = success;

                if (success)
                {
                    result = _itemBrandFacade.Find(model.ItemBrandId);
                    response.Message = Messages.Updated;
                    response.Data = result;
                    return new SilupostAPIHttpActionResult<AppResponseModel<ItemBrandViewModel>>(Request, HttpStatusCode.OK, response);
                }
                else
                {
                    response.Message = Messages.Failed;
                    return new SilupostAPIHttpActionResult<AppResponseModel<ItemBrandViewModel>>(Request, HttpStatusCode.BadGateway, response);
                }
            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new SilupostAPIHttpActionResult<AppResponseModel<ItemBrandViewModel>>(Request, HttpStatusCode.BadRequest, response);
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
                response.Message = string.Format(Messages.InvalidId, "Item Brand");
                return new SilupostAPIHttpActionResult<AppResponseModel<object>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    RecordedBy = identity.FindFirst("SystemUserId").Value;
                }

                var result = _itemBrandFacade.Find(id);
                if (result == null)
                {
                    response.Message = string.Format(Messages.InvalidId, "Item Brand");
                    return new SilupostAPIHttpActionResult<AppResponseModel<object>>(Request, HttpStatusCode.BadRequest, response);
                }

                bool success = _itemBrandFacade.Remove(id, RecordedBy);
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
