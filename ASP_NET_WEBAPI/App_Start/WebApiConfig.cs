using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using WebApiContrib.Formatting.Jsonp;
using System.Web.Http.Cors;

namespace ASP_NET_WEBAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 設定和服務
            // 將 Web API 設定成僅使用 bearer 權杖驗證。
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // To enable basic authentication across the entire Web API application, 
            // register BasicAuthenticationAttribute as a filter using the Register() method in WebApiConfig class
            //config.Filters.Add(new BasicAuthenticationAttribute());

            // To fix cross-domain issue, we enable Jsonp format
            //var jsonpFormatter = new JsonpMediaTypeFormatter(config.Formatters.JsonFormatter);
            //config.Formatters.Insert(0, jsonpFormatter);

            // To fix cross-domain issue, We enable Cors function
            // enables CORS globally for the entire application i.e for all controllers and action methods
            //EnableCorsAttribute cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors();

            // Will return only JSON from ASP.NET Web API Service irrespective of the Accept header value
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // Will return only XML from ASP.NET Web API Service irrespective of the Accept header value
            //config.Formatters.Remove(config.Formatters.JsonFormatter);

            // Approach 1 : Include the following line in Register() method of WebApiConfig.cs file in App_Start folder. 
            // This tells ASP.NET Web API to use JsonFormatter when a request is made for text/html which is the default for most browsers. 
            // The problem with this approach is that Content-Type header of the response is set to text/html which is misleading.
            // config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("Text/html"));

            // Approach2 :
            //config.Formatters.Add(new CustomJsonFormatter());

            config.Formatters.JsonFormatter.SerializerSettings.Formatting =
                            Newtonsoft.Json.Formatting.Indented;
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();
        }

        public class CustomJsonFormatter : JsonMediaTypeFormatter
        {
            public CustomJsonFormatter()
            {
                this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            }

            public override void SetDefaultContentHeaders(Type type, HttpContentHeaders headers, MediaTypeHeaderValue mediaType)
            {
                base.SetDefaultContentHeaders(type, headers, mediaType);
                headers.ContentType = new MediaTypeHeaderValue("application/json");
            }
        }
    }
}
