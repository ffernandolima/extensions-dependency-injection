using Castle.DynamicProxy;
using System;
using System.Threading;

namespace Extensions.DependencyInjection.Proxies
{
    public class ServiceProxyFactory
    {
        private static readonly Lazy<ServiceProxyFactory> Factory = new Lazy<ServiceProxyFactory>(
            () => new ServiceProxyFactory(), LazyThreadSafetyMode.PublicationOnly
        );

        public static ServiceProxyFactory Instance => Factory.Value;

        private ServiceProxyFactory()
        { }

        public TService CreateProxy<TService, TImplementation>(IServiceProvider serviceProvider, Func<TImplementation> implementationFactory = null)
            where TService : class
            where TImplementation : class, TService
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var proxyGenerator = ProxyGeneratorFactory.Instance.CreateProxyGenerator();

            if (proxyGenerator == null)
            {
                throw new ArgumentNullException(nameof(proxyGenerator));
            }

            var proxyResult = proxyGenerator.CreateInterfaceProxyWithoutTarget<TService>(
                new IInterceptor[] {
                    new ServiceActivationInterceptor<TService, TImplementation>(serviceProvider, implementationFactory)
                }
            );

            return proxyResult;
        }
    }
}
