using Extensions.DependencyInjection.Factories;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace Extensions.DependencyInjection.Tests
{
    public class ServiceFactoryTests : Startup
    {
        public ServiceFactoryTests()
            : base(configure: false)
        { }

        [Fact]
        public void ParameterLessCtorWithouImplFactoryTest()
        {
            Services.Clear();
            Services.AddTransient<IBazService, BazService>();
            Services.AddServiceFactory<IBazService, BazService>(); // No Impl factory

            var factory = ServiceProvider.GetService<IServiceFactory<IBazService>>();
            var service = factory.GetService();

            Assert.NotNull(service);
        }

        [Fact]
        public void ParameterLessCtorWithManualImplFactoryTest()
        {
            Services.Clear();
            Services.AddTransient<IBazService, BazService>();
            Services.AddServiceFactory<IBazService, BazService>(() => new BazService()); // Manual instantiation

            // Or:
            // Services.AddServiceFactory<IBazService, BazService>((object[] args) => new BazService()); // Receives args
            // Services.AddServiceFactory<IBazService, BazService>((IServiceProvider provider, object[] args) => new BazService()); // Receives ServiceProvider and args

            var factory = ServiceProvider.GetService<IServiceFactory<IBazService>>();
            var service = factory.GetService();

            Assert.NotNull(service);
        }

        [Fact]
        public void ParameterLessCtorWithDIUtilitiesImplFactoryTest()
        {
            Services.Clear();
            Services.AddTransient<IBazService, BazService>();
            Services.AddServiceFactory<IBazService, BazService>((IServiceProvider provider, object[] args) => provider.GetServiceOrCreateInstance<IBazService>()); // Requires IBazService registration

            var factory = ServiceProvider.GetService<IServiceFactory<IBazService>>();
            var service = factory.GetService();

            Assert.NotNull(service);
        }

        [Fact]
        public void ParameterLessCtorWithDIUtilitiesImplFactoryWithoutServiceRegistrationTest()
        {
            Services.Clear();
            Services.AddServiceFactory<IBazService, BazService>((IServiceProvider provider, object[] args) => provider.GetServiceOrCreateInstance<BazService>()); // No matter IBazService was registered or not into DI container

            var factory = ServiceProvider.GetService<IServiceFactory<IBazService>>();
            var service = factory.GetService();

            Assert.NotNull(service);
        }

        [Fact]
        public void WithoutParameterLessCtorWithDIUtilitiesAndArgsImplFactoryWithoutServiceRegistrationTest()
        {
            Services.Clear();
            Services.AddServiceFactory<IAckService, AckService>((IServiceProvider provider, object[] args) => provider.CreateInstance<AckService>(args)); // IAckService must not be registered into DI container

            var factory = ServiceProvider.GetService<IServiceFactory<IAckService>>();
            var service = factory.GetService(new object());

            Assert.NotNull(service);
        }

        [Fact]
        public void WithoutParameterLessCtorWithDIUtilitiesAndMixedArgsImplFactoryWithoutServiceRegistrationTest()
        {
            Services.Clear();
            Services.AddLogging();
            Services.AddServiceFactory<IQuxService, QuxService>((IServiceProvider provider, object[] args) => provider.CreateInstance<QuxService>(args)); // IQuxService must not be registered into DI container

            var factory = ServiceProvider.GetService<IServiceFactory<IQuxService>>();
            var service = factory.GetService(new object());

            Assert.NotNull(service);
        }
    }
}
