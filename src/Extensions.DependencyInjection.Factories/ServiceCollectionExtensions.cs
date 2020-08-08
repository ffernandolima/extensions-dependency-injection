using Microsoft.Extensions.DependencyInjection;
using System;

namespace Extensions.DependencyInjection.Factories
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServiceFactory<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<IServiceFactory<TService, TImplementation>>(provider => new ServiceFactory<TService, TImplementation>(provider));

            return services;
        }

        public static IServiceCollection AddServiceFactory<TService, TImplementation>(this IServiceCollection services, Func<TService> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            Func<IServiceProvider, object[], TService> implementationFactoryInternal = null;

            if (implementationFactory != null)
            {
                implementationFactoryInternal = (IServiceProvider sp, object[] args) => implementationFactory.Invoke();
            }

            services.AddSingleton<IServiceFactory<TService, TImplementation>>(provider => new ServiceFactory<TService, TImplementation>(provider, implementationFactoryInternal));

            return services;
        }

        public static IServiceCollection AddServiceFactory<TService, TImplementation>(this IServiceCollection services, Func<object[], TService> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            Func<IServiceProvider, object[], TService> implementationFactoryInternal = null;

            if (implementationFactory != null)
            {
                implementationFactoryInternal = (IServiceProvider sp, object[] args) => implementationFactory.Invoke(args);
            }

            services.AddSingleton<IServiceFactory<TService, TImplementation>>(provider => new ServiceFactory<TService, TImplementation>(provider, implementationFactoryInternal));

            return services;
        }

        public static IServiceCollection AddServiceFactory<TService, TImplementation>(this IServiceCollection services, Func<IServiceProvider, object[], TService> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<IServiceFactory<TService, TImplementation>>(provider => new ServiceFactory<TService, TImplementation>(provider, implementationFactory));

            return services;
        }
    }
}
