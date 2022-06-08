using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace HRMS.API.Filters
{
    /// <summary>
    /// Jinisys implementation of IHttpActionResult to return custom content
    /// </summary>
    /// <typeparam name="T">Return content data type</typeparam>
    public class SilupostAPIHttpActionResult<T> : IHttpActionResult
    {
        private T content;
        private HttpStatusCode status;
        private HttpRequestMessage request;
        /// <summary>
        /// Jinisys implementation of IHttpActionResult to return custom content
        /// </summary>
        /// <param name="request">HttpRequestMessage of current ApiController</param>
        /// <param name="status">Response status code</param>
        /// <param name="content">Response content</param>
        public SilupostAPIHttpActionResult(HttpRequestMessage request, HttpStatusCode status, T content)
        {
            this.request = request;
            this.status = status;
            this.content = content;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var _response = this.request.CreateResponse<T>(this.status, this.content);
            return Task.FromResult(_response);
        }
    }
}