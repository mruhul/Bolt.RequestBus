using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using Bolt.RequestBus;

namespace Sample.Api.Controllers
{
    public static class ResponseExtensions
    {
        public static IHttpActionResult ResponseResult<TValue>(this ApiController controller, 
            IResponse<TValue> response,
            Func<TValue, IHttpActionResult> buildResult = null)
        {
            if (response.Succeed)
            {
                if (response.Value == null)
                {
                    return new NotFoundResult(controller.Request);
                }
                
                return buildResult == null 
                    ? new NegotiatedContentResult<dynamic>(HttpStatusCode.OK, response.Value, controller)
                    : buildResult.Invoke(response.Value);
            }

            return new NegotiatedContentResult<dynamic>(HttpStatusCode.BadRequest, new {response.Errors }, controller);
            
        } 
    }
}