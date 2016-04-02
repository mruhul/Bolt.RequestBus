using System;
using System.Net;
using System.Web.Http;
using System.Web.Http.Results;
using Bolt.RequestBus;

namespace Sample.Api.Infrastructure.Extensions
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

        public static IHttpActionResult ResponseResult<TValue>(this ApiController controller,
            IResponse<TValue> response,
            Func<TValue,Uri> uri)
        {
            if (response.Succeed)
            {
                if (response.Value == null)
                {
                    return new NotFoundResult(controller.Request);
                }

                return new CreatedNegotiatedContentResult<TValue>(uri.Invoke(response.Value), response.Value, controller);
            }

            return new NegotiatedContentResult<dynamic>(HttpStatusCode.BadRequest, new { response.Errors }, controller);

        }
    }
}