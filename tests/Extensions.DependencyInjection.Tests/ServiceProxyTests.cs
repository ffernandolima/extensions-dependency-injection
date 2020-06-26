using Castle.DynamicProxy;
using Extensions.DependencyInjection.Proxies;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Extensions.DependencyInjection.Tests
{
    public class ServiceProxyTests : Startup
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            // Many ways of registering services as proxies:
            services.AddTransientProxy<IFooService, FooService>();
            services.AddTransientProxy<IBarService, BarService>();

            // Or:
            // services.AddTransient<IFooService>(provider => provider.CreateProxy<IFooService, FooService>());
            // services.AddTransient<IBarService>(provider => provider.CreateProxy<IBarService, BarService>());
        }

        [Fact]
        public void ServiceProxyTest()
        {
            var service = ServiceProvider.GetService<IFooService>();

            var isProxy = ProxyUtil.IsProxy(service);

            Assert.True(isProxy);

            var executeResult = service.Execute();

            Assert.Equal("Foo-Bar", executeResult);

            var service2 = ServiceProvider.GetService<IFooService>();

            var equalsResult = ReferenceEquals(service, service2);

            Assert.False(equalsResult);
        }
    }
}
