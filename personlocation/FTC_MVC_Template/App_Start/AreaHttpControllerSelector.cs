using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EnterprisePortal.Infrastructure.Dispatcher
{
    
   using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Dispatcher;

    public class AreaHttpControllerSelector : DefaultHttpControllerSelector
    {
        private const string ControllerSuffix = "Controller";
        private const string AreaRouteVariableName = "area";

        private readonly HttpConfiguration _configuration;

        public AreaHttpControllerSelector(HttpConfiguration configuration)
            : base(configuration)
        {
            _configuration = configuration;
        }

        private Dictionary<string, Type> _apiControllerTypes;

        private Dictionary<string, Type> ApiControllerTypes
        {
            get { return _apiControllerTypes ?? (_apiControllerTypes = GetControllerTypes()); }
        }

        private static Dictionary<string, Type> GetControllerTypes()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var types = assemblies.SelectMany(a => a.GetTypes().Where(t => !t.IsAbstract && t.Name.EndsWith(ControllerSuffix) && typeof(IHttpController).IsAssignableFrom(t)))
                .ToDictionary(t => t.FullName, t => t);

            return types;
        }

        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            return GetApiController(request) ?? base.SelectController(request);
        }

        private static string GetAreaName(HttpRequestMessage request)
        {
            var data = request.GetRouteData();

            if (!data.Values.ContainsKey(AreaRouteVariableName))
            {
                return null;
            }

            return data.Values[AreaRouteVariableName].ToString().ToLower();
        }

        private Type GetControllerTypeByArea(string areaName, string controllerName)
        {
            var areaNameToFind = string.Format(".{0}.", areaName.ToLower());
            var controllerNameToFind = string.Format(".{0}{1}", controllerName, ControllerSuffix);

            return ApiControllerTypes.Where(t => t.Key.ToLower().Contains(areaNameToFind) && t.Key.EndsWith(controllerNameToFind, StringComparison.OrdinalIgnoreCase))
                    .Select(t => t.Value).FirstOrDefault();
        }

        private HttpControllerDescriptor GetApiController(HttpRequestMessage request)
        {
            var controllerName = base.GetControllerName(request);

            var areaName = GetAreaName(request);
            if (string.IsNullOrEmpty(areaName))
            {
                return null;
            }

            var type = GetControllerTypeByArea(areaName, controllerName);
            if (type == null)
            {
                return null;
            }

            return new HttpControllerDescriptor(_configuration, controllerName, type);
        }
    }
}
