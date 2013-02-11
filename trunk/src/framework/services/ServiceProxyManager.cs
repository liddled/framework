using System;
using System.Collections.Generic;
using DL.Framework.Services;

namespace DL.Framework.Services
{
    public static class ServiceProxyManager
    {
        private static readonly IDictionary<string, object> ServiceProxyList = new Dictionary<string, object>();

        public static T GetProxy<T>(string endPoint) where T : class
        {
            if (!ServiceProxyList.ContainsKey(endPoint))
            {
                var proxy = new ServiceProxy<T>(endPoint);
                ServiceProxyList.Add(endPoint, proxy.Instance);
            }

            return (T) ServiceProxyList[endPoint];
        }

        public static void Use<T>(string endPoint, Action<T> action) where T : class
        {
            var proxy = new ServiceProxy<T>(endPoint);
            var success = false;
            try
            {
                action(proxy.Instance);
                proxy.Close();
                success = true;
            }
            finally
            {
                if (!success)
                    proxy.Abort();
            }
        }
    }
}
