using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;
using System.Web.Routing;

namespace FTC_MES_MVC
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
           
            config.EnableCors();
            //Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
                //,namespaces:new string [] { "FTC_MES_MVC" }
            );
        }
    }
    //public static class HttpRouteCollectionEx
    //{
    //    public static IHttpRoute MapHttpRoute(this HttpRouteCollection routes, string name, string routeTemplate, object defaults, string[] namespaces)
    //    {
    //        return routes.MapHttpRoute(name, routeTemplate, defaults, null, null, namespaces);
    //    }
    //    public static IHttpRoute MapHttpRoute(this HttpRouteCollection routes, string name, string routeTemplate, object defaults, object constraints, HttpMessageHandler handler, string[] namespaces)
    //    {
    //        if (routes == null)
    //        {
    //            throw new ArgumentNullException("routes");
    //        }
    //        var routeValue = new HttpRouteValueDictionary(new { Namespace = namespaces });//设置路由值
    //        var route = routes.CreateRoute(routeTemplate, new HttpRouteValueDictionary(defaults), new HttpRouteValueDictionary(constraints), routeValue, handler);
    //        routes.Add(name, route);
    //        return route;
    //    }
    //}
}
