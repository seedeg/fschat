using FsChat.Services.APIs.Public.App_Start;
using FsChat.Services.APIs.Public.App_Start.Ioc.Plumbing;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Threading.Tasks;

namespace FsChat.Services.APIs.Public
{
    public class WebApiApplication : HttpApplication
    {
        private WindsorContainer container;

        protected void Application_Start()
        {
            RegisterJsonConvertToUseCamelCase();
            RegesterDependencyResolvers();
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        private void RegesterDependencyResolvers()
        {
            container = new WindsorContainer();
            container.Install(FromAssembly.This());

            GlobalConfiguration.Configuration.DependencyResolver = new CastleDependencyResolver(container.Kernel);
        }

        private void RegisterJsonConvertToUseCamelCase()
        {
            HttpConfiguration config = GlobalConfiguration.Configuration;
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;
        }

        protected virtual void Application_End()
        {
            container?.Dispose();
        }
    }
}
