using System;
using System.Data.Entity.Core.Common.EntitySql;
using System.Web;
using Bolt.Common.Extensions;
using Microsoft.Owin;
using Owin;

namespace Sample.Api.Infrastructure.ContextStores
{
    public interface IContextStore
    {
        object Get(string key);
        void Set(string key, object value);
        bool Exists(string key);
    }

    public class HttpContextStore : IContextStore
    {
        public object Get(string key)
        {
            return Exists(key) ? HttpContext.Current.Items[key] : null;
        }

        public void Set(string key, object value)
        {
            HttpContext.Current.NullSafeDo(context => context.Items[key] = value);
        }

        public bool Exists(string key)
        {
            return HttpContext.Current.NullSafeGet(context => context.Items.Contains(key));
        }
    }

    public static class ContextStoreExtensions
    {
        public static T Get<T>(this IContextStore source, string key)
        {
            return Get(source, key, () => default(T));
        }

        public static T Get<T>(this IContextStore source, string key, Func<T> fetch)
        {
            var result = source.Get(key);

            if (result != null) return (T)result;

            var fetchResult = fetch.Invoke();

            if (fetchResult == null) return default(T);

            source.Set(key, fetchResult);

            return fetchResult;
        }
    }
}