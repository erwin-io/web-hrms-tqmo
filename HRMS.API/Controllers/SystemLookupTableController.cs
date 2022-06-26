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

namespace HRMS.API.Controllers
{
    //[Authorize]
    [HRMSAuthorizationFilter]
    [RoutePrefix("api/v1/SystemLookup")]
    public class SystemLookupTableController : ApiController
    {
        private readonly ILookupFacade _lookupFacade;
        private string RecordedBy { get; set; }
        #region CONSTRUCTORS
        public SystemLookupTableController(ILookupFacade lookupFacade)
        {
            _lookupFacade = lookupFacade ?? throw new ArgumentNullException(nameof(lookupFacade));
        }
        #endregion

        [Route("GetAllByTableNames")]
        [HttpGet]
        [SwaggerOperation("GetAllByTableNames")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IHttpActionResult GetAllByTableNames(string TableNames)
        {
            AppResponseModel<Dictionary<string, object>> response = new AppResponseModel<Dictionary<string, object>>();

            if (string.IsNullOrEmpty(TableNames))
            {
                response.Message = string.Format(Messages.InvalidId, "System Lookup Table");
                return new HRMSAPIHttpActionResult<AppResponseModel<Dictionary<string, object>>>(Request, HttpStatusCode.BadRequest, response);
            }

            try
            {
                var data = _lookupFacade.FindLookupByTableNames(TableNames);
                var result = new Dictionary<string, object>();
                foreach (var item in data)
                {
                    result.Add(item.LookupName, item.LookupData);
                }

                if (result != null)
                {
                    response.IsSuccess = true;
                    response.Data = result;
                    return new HRMSAPIHttpActionResult<AppResponseModel<Dictionary<string, object>>>(Request, HttpStatusCode.OK, response);
                }
                else
                {
                    response.Message = Messages.NoRecord;
                    return new HRMSAPIHttpActionResult<AppResponseModel<Dictionary<string, object>>>(Request, HttpStatusCode.NotFound, response);
                }

            }
            catch (Exception ex)
            {
                response.DeveloperMessage = ex.Message;
                response.Message = Messages.ServerError;
                //TODO Logging of exceptions
                return new HRMSAPIHttpActionResult<AppResponseModel<Dictionary<string, object>>>(Request, HttpStatusCode.BadRequest, response);
            }
        }

        
    }
}
