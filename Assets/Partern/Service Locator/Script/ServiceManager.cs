using System;
using System.Collections.Generic;
using UnityEngine;

namespace ServiceLocator
{
    public class ServiceManager
    {
        Dictionary<Type, object> services = new Dictionary<Type, object>();
        public IEnumerable<object> RegisteredServices => services.Values;

        public bool TryGet<T>(out T service)
            where T : class
        {
            Type type = typeof(T);
            if (services.TryGetValue(type, out object obj))
            {
                service = obj as T;
                return true;
            }
            service = null;
            return false;
        }

        public T Get<T>()
            where T : class
        {
            Type type = typeof(T);
            if (services.TryGetValue(type, out object service))
            {
                return service as T;
            }
            throw new AggregateException(
                $"ServiceManager.Get: Service of type {type.FullName} not found"
            );
        }

        public ServiceManager Register<T>(T service)
        {
            Type type = typeof(T);

            if (!services.TryAdd(type, service))
            {
                Debug.LogError(
                    $"ServiceManager.Register: Service of type {type.FullName} already registered"
                );
            }

            return this;
        }

        public ServiceManager Register(Type type, object service)
        {
            if (!type.IsInstanceOfType(service))
            {
                throw new ArgumentException(
                    "Type of service does not match the specified type",
                    nameof(service)
                );
            }

            if (!services.TryAdd(type, service))
            {
                Debug.LogError(
                    $"ServiceManager.Register: Service of type {type.FullName} already registered"
                );
            }

            return this;
        }
    }
}
