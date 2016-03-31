using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using Bolt.RequestBus;

namespace Sample.Api.Controllers
{
    public static class ResponseExtensions
    {
        public static IHttpActionResult ResponseResult<TValue>(this ApiController controller, IResponse<TValue> response)
        {
            if (response.Succeed)
            {
                if (response.Value == null)
                {
                    return new NotFoundResult(controller.Request);
                }

                return new OkResult(controller.Request);
            }

            foreach (var error in response.Errors)
            {
                controller.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            return new BadRequestResult(controller.Request);
        } 
    }
}