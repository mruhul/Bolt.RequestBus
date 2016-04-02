using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Bolt.Common.Extensions;
using Owin;
using Sample.Api.Infrastructure.ContextStores;

namespace Sample.Api.Infrastructure.RequestWrappers
{
    public interface IRequestContext
    {
        string CorrelationId();
    }

    //TODO: move to owin context so that it works even for self hosting
    public class HttpRequestContext : IRequestContext
    {
        private const string CorrelationIdContextKey = "_CorrelationId";
        private const string CorrelationIdHeaderName = "x-correlation-id";
        private readonly IContextStore contextStore;

        public HttpRequestContext(IContextStore contextStore)
        {
            this.contextStore = contextStore;
        }

        public string CorrelationId()
        {
            return contextStore.Get(CorrelationIdContextKey, () =>
            {
                var correlationIdFromHeader = HttpContext.Current?.Request.Headers[CorrelationIdHeaderName];

                return correlationIdFromHeader.HasValue() 
                        ? correlationIdFromHeader 
                        : Guid.NewGuid().ToString();
            });
        }
    }
}