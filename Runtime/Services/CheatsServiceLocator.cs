using System;
using System.Collections.Generic;

namespace SMCheats
{
    public sealed class CheatsServiceLocator : IDisposable
    {
        private readonly Dictionary<Type, object> _services = new();
        private readonly List<object> _serviceList = new();

        public IReadOnlyList<object> Services => _serviceList;

        public T Get<T>() where T : class
        {
            return _services.TryGetValue(typeof(T), out var service)
                ? service as T
                : null;
        }

        public void Register<T>(T service) where T : class
        {
            _services[typeof(T)] = service;
            if (service != null && !_serviceList.Contains(service))
            {
                _serviceList.Add(service);
            }
        }

        public void Dispose()
        {
            foreach (var service in _services.Values)
            {
                if (service is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            _services.Clear();
            _serviceList.Clear();
        }
    }
}
