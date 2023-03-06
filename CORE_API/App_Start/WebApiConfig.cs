
using CORE_BASE.CORE;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;

namespace CORE_API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
         //   config.DependencyResolver = new NinjectResolver();
            // Web API routes
            CoreConnection.ConnectionString = ConfigurationManager.ConnectionStrings["connsetting"].ToString();
            CoreEngineConnection.ConnectionString = ConfigurationManager.ConnectionStrings["connsettingEngine"].ToString();
            WPSConnection.ConnectionString = ConfigurationManager.ConnectionStrings["WPSFileProcessing"].ToString();
            config.MapHttpAttributeRoutes();
          // config.EnableCors(new System.Web.Http.Cors.EnableCorsAttribute("*", headers: "*", methods: "*"));
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{areas}/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("text/html"));
            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
            config.Formatters.Remove(config.Formatters.XmlFormatter);



          //  config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("text/html"));
        }
    }
}
