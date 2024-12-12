using System;
using System.Collections.Generic;
using script.Decorator;

namespace Partern.Service_Locator.Script
{
    public class ServiceLocator : Singleton<ServiceLocator>
    {
        private static readonly Dictionary<Type, object> ServiceCache;
        private static ServiceLocator _instance;

        static ServiceLocator()
        {
            ServiceCache = new Dictionary<Type, object>();
        }

        public void Register<T>(T service)
        {
            var key = typeof(T);
            if (!ServiceCache.ContainsKey(key))
            {
                ServiceCache.Add(key, service);
            }
            else
            {
                ServiceCache[key] = service;
            }
        }

        public T GetService<T>()
        {
            var key = typeof(T);
            if (ServiceCache.ContainsKey(key))
            {
                return (T)ServiceCache[key];
            }
            return default;
        }
    }
}
