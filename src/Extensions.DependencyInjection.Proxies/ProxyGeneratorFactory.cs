using Castle.DynamicProxy;
using System;
using System.Threading;

namespace Extensions.DependencyInjection.Proxies
{
    public class ProxyGeneratorFactory
    {
        private static readonly Lazy<ProxyGeneratorFactory> Factory = new Lazy<ProxyGeneratorFactory>(
            () => new ProxyGeneratorFactory(), LazyThreadSafetyMode.PublicationOnly
        );

        private ProxyGenerator _proxyGenerator;
        public static ProxyGeneratorFactory Instance => Factory.Value;

        private ProxyGeneratorFactory()
        { }

        public ProxyGenerator CreateProxyGenerator() => (_proxyGenerator = _proxyGenerator ?? new ProxyGenerator());
    }
}
