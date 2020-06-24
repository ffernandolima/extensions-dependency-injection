using Microsoft.Extensions.DependencyInjection;
using System;

namespace Extensions.DependencyInjection.Proxies
{
    public static class ServiceProviderExtensions
    {
        public static TService CreateProxy<TService, TImplementation>(this IServiceProvider serviceProvider, Func<TImplementation> implementationFactory = null)
            where TService : class
            where TImplementation : class, TService
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var proxyResult = ServiceProxyFactory.Instance.CreateProxy<TService, TImplementation>(serviceProvider, implementationFactory);

            return proxyResult;
        }

        public static object CreateInstance(this IServiceProvider serviceProvider, Type instanceType, params object[] parameters)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            if (instanceType == null)
            {
                throw new ArgumentNullException(nameof(instanceType));
            }

            var instance = ActivatorUtilities.CreateInstance(serviceProvider, instanceType, parameters);

            return instance;
        }

        public static T CreateInstance<T>(this IServiceProvider serviceProvider, params object[] parameters)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var instance = ActivatorUtilities.CreateInstance<T>(serviceProvider, parameters);

            return instance;
        }


        public static T GetServiceOrCreateInstance<T>(this IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var instance = ActivatorUtilities.GetServiceOrCreateInstance<T>(serviceProvider);

            return instance;
        }

        public static object GetServiceOrCreateInstance(this IServiceProvider serviceProvider, Type type)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var instance = ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, type);

            return instance;
        }
    }
}
