using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Sample.Api.Features.Shared
{
    public interface ISettings
    {
        string ApplicationId { get; }
    }

    public class Settings : ISettings
    {
        public Settings()
        {
            ApplicationId = ConfigurationManager.AppSettings["ApplicationId"];
        }

        public string ApplicationId { get; }
    }
}