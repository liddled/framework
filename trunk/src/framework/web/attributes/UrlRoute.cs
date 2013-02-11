using System;

namespace DL.Framework.Web
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class UrlRouteAttribute : Attribute
    {
        /// <summary>
        /// Optional name of the route.  If not specified, the route name will
        /// be set to [controller name].[action name].
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Path of the URL route.  This is relative to the root of the web site.
        /// Do not append a "/" prefix.  Specify empty string for the root page.
        /// </summary>
        public string Path { get; set; }
        
        /// <summary>
        /// Optional order in which to add the route (default is 0).  Routes
        /// with lower order values will be added before those with higher.
        /// Routes that have the same order value will be added in undefined
        /// order with respect to each other.
        /// </summary>
        public int Order { get; set; }
    }

    /// <summary>
    /// Assigns a constraint to a route parameter in a UrlRouteAttribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class UrlRouteParameterConstraintAttribute : Attribute
    {
        /// <summary>
        /// Name of the route parameter on which to apply the constraint.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Regular expression constraint to test on the route parameter value in the URL.
        /// </summary>
        public string Regex { get; set; }
    }

    /// <summary>
    /// Assigns a default value to a route parameter in a UrlRouteAttribute if not specified in the URL.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class UrlRouteParameterDefaultAttribute : Attribute
    {
        /// <summary>
        /// Name of the route parameter for which to supply the default value.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Default value to set on the route parameter if not specified in the URL.
        /// </summary>
        public object Value { get; set; }
    }
}
