using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Common.Logging;

namespace DL.Framework.Web
{
    public static class RouteHelper
    {
		private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Uses reflection to enumerate all Controller classes in the
        /// assembly and registers a route for each method declaring a UrlRoute attribute.
        /// </summary>
        /// <param name="routes">Route collection to add routes to.</param>
        public static void RegisterUrlRoutesFromAttributes(RouteCollection routes)
        {
            // Enumerate assembly for UrlRoute attributes.
            var routeParams = GetRouteParamsFromAttributes(Assembly.GetCallingAssembly());
            
            // Sort the routes based on order.
            routeParams.Sort((a, b) => a.Order.CompareTo(b.Order));
            
            // Add the routes to the routes collection.
            foreach (var rd in routeParams)
            {
                // Route priority not set, add it now.
				if (Log.IsDebugEnabled)
                    Log.DebugFormat("Adding route {{ Priority = {0}, Name = {1}, Path = {2}, Controller = {3}, Action = {4} }}", rd.Order, rd.RouteName, rd.Path, rd.ControllerName, rd.ActionName);
                
                // Controller and action is always the class/method that
                // the UrlRoute attribute was declared on (if user sets
                // these values it will be overridden here).
                rd.Defaults["controller"] = rd.ControllerName;
                rd.Defaults["action"] = rd.ActionName;

                RouteHelper.MapRoute(routes, rd.RouteName, rd.Path, rd.Defaults, rd.Constraints, null);
                
                // TODO: set namespace of the controller
            }
        }

        /// <summary>
        /// Uses reflection to enumerate all Controller classes in the
        /// assembly and registers a route for each method declaring a
        /// UrlRoute attribute.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        private static List<MapRouteParams> GetRouteParamsFromAttributes(Assembly a)
        {
            var routeParams = new List<MapRouteParams>();
            
            // Enumerate all non-abstract Controller classes in the assembly of the caller.
            var controllerClasses = from t in a.GetTypes()
                                    where t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Controller))
                                    select t;
            
            foreach (var controllerClass in controllerClasses)
            {
                // Enumerate public methods on the controller class.
                foreach (var methodInfo in controllerClass.GetMethods())
                {
                    // Enumerate UrlRoute attributes on the method.
                    foreach (UrlRouteAttribute routeAttrib in methodInfo.GetCustomAttributes(typeof(UrlRouteAttribute), false))
                    {
                        var controllerName = controllerClass.Name;
                        
                        if (!controllerName.EndsWith("Controller", StringComparison.InvariantCultureIgnoreCase))
                        {
                            // Failed MVC framework requirement for controller names.
                            throw new ApplicationException(String.Format("Invalid controller class name {0}: name must end with \"Controller\"", controllerName));
                        }
                        
                        // MVC framework does not seem to like the "Controller" suffix in the route so strip it off.
                        controllerName = controllerName.Substring(0, controllerName.Length - "Controller".Length);
                        
                        if (routeAttrib.Path.StartsWith("/") || routeAttrib.Path.Contains("?"))
                        {
                            throw new ApplicationException(String.Format("Invalid UrlRoute attribute \"{0}\" on method {1}.{2}: Path cannot start with \"/\" or contain \"?\".", routeAttrib.Path, controllerName, methodInfo.Name));
                        }
                        
                        if (String.IsNullOrEmpty(routeAttrib.Name))
                        {
                            routeAttrib.Name = methodInfo.Name;
                        }
                        
                        var routeName = String.Format("{0}.{1}", controllerName, routeAttrib.Name);
                        
                        // Add to list of routes.
                        routeParams.Add(new MapRouteParams
                        {
                            RouteName = routeName,
                            Path = routeAttrib.Path,
                            ControllerName = controllerName,
                            ActionName = methodInfo.Name,
                            Order = routeAttrib.Order,
                            Constraints = GetConstraints(methodInfo),
                            Defaults = GetDefaults(methodInfo),
                        });
                    
                    }
                }
            }
            
            return routeParams;
        }
        
        /// <summary>
        /// This was copied from System.Web.Mvc.RouteCollectionExtensions and modified to suit our purposes.
        /// </summary>
        /// <param name="routes"></param>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <param name="defaults"></param>
        /// <param name="constraints"></param>
        /// <param name="namespaces"></param>
        /// <returns></returns>
        private static Route MapRoute(RouteCollection routes, string name, string url, IDictionary<string, object> defaults, Dictionary<string, object> constraints, string[] namespaces)
        {
            if (routes == null)
            {
                throw new ArgumentNullException("routes");
            }
            
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }
            
            if (constraints == null)
            {
                throw new ArgumentNullException("constraints");
            }
            
            var route = new Route(url, new MvcRouteHandler())
            {
                Defaults = new RouteValueDictionary(defaults),
                Constraints = new RouteValueDictionary(constraints)
            };
            
            if ((namespaces != null) && (namespaces.Length > 0))
            {
                route.DataTokens = new RouteValueDictionary();
                route.DataTokens["Namespaces"] = namespaces;
            }
            
            routes.Add(name, route);
            
            return route;
        }
        
        private static Dictionary<string, object> GetConstraints(MethodInfo mi)
        {
            var constraints = new Dictionary<string, object>();
            
            foreach (UrlRouteParameterConstraintAttribute attrib in mi.GetCustomAttributes(typeof(UrlRouteParameterConstraintAttribute), true))
            {
                if (String.IsNullOrEmpty(attrib.Name))
                {
                    throw new ApplicationException(String.Format("UrlRouteParameterContraint attribute on {0}.{1} is missing the Name property.", mi.DeclaringType.Name, mi.Name));
                }
                
                if (String.IsNullOrEmpty(attrib.Regex))
                {
                    throw new ApplicationException(String.Format("UrlRouteParameterContraint attribute on {0}.{1} is missing the RegEx property.", mi.DeclaringType.Name, mi.Name));
                }
                
                constraints.Add(attrib.Name, attrib.Regex);
            }
            
            return constraints;
        }
        
        private static Dictionary<string, object> GetDefaults(MethodInfo mi)
        {
            var defaults = new Dictionary<string, object>();
            
            foreach (UrlRouteParameterDefaultAttribute attrib in mi.GetCustomAttributes(typeof(UrlRouteParameterDefaultAttribute), true))
            {
                if (String.IsNullOrEmpty(attrib.Name))
                {
                    throw new ApplicationException(string.Format("UrlRouteParameterDefault attribute on {0}.{1} is missing the Name property.", mi.DeclaringType.Name, mi.Name));
                }
                
                if (attrib.Value == null)
                {
                    throw new ApplicationException(string.Format("UrlRouteParameterDefault attribute on {0}.{1} is missing the Value property.", mi.DeclaringType.Name, mi.Name));
                }
                
                defaults.Add(attrib.Name, attrib.Value);
            }
            
            return defaults;
        }
        
        class MapRouteParams
        {
            public int Order { get; set; }
            public string RouteName { get; set; }
            public string Path { get; set; }
            public string ControllerNamespace { get; set; }
            public string ControllerName { get; set; }
            public string ActionName { get; set; }
            public Dictionary<string, object> Defaults { get; set; }
            public Dictionary<string, object> Constraints { get; set; }
        }
    }
}