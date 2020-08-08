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
    }
}
