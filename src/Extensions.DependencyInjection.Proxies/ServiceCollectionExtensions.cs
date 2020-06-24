using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;
using System.Reflection;

namespace Extensions.DependencyInjection.Proxies
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddScopedProxy<TService, TImplementation>(this IServiceCollection services, Func<TImplementation> implementationFactory = null)
            where TService : class
            where TImplementation : class, TService
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddScoped<TService>(provider => provider.CreateProxy<TService, TImplementation>(implementationFactory));

            return services;
        }

        public static IServiceCollection AddSingletonProxy<TService, TImplementation>(this IServiceCollection services, Func<TImplementation> implementationFactory = null)
            where TService : class
            where TImplementation : class, TService
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<TService>(provider => provider.CreateProxy<TService, TImplementation>(implementationFactory));

            return services;
        }

        public static IServiceCollection AddTransientProxy<TService, TImplementation>(this IServiceCollection services, Func<TImplementation> implementationFactory = null)
            where TService : class
            where TImplementation : class, TService
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddTransient<TService>(provider => provider.CreateProxy<TService, TImplementation>(implementationFactory));

            return services;
        }

        public static IServiceCollection AddAllReferencesAsServices<T>(this IServiceCollection services, Assembly assembly, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            services.AddAllReferencesAsServices(typeof(T), assembly, serviceLifetime);

            return services;
        }

        public static IServiceCollection AddAllReferencesAsServices(this IServiceCollection services, Type serviceType, Assembly assembly, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            var implementationTypes = assembly.GetTypes().Where(type => !type.IsInterface && !type.IsAbstract && type.IsPublic && serviceType.IsAssignableFrom(type)).ToArray();

            if (implementationTypes.Any())
            {
                foreach (var implementationType in implementationTypes)
                {
                    switch (serviceLifetime)
                    {
                        case ServiceLifetime.Singleton:
                            {
                                services.TryAddSingleton(implementationType, implementationType);
                            }
                            break;
                        case ServiceLifetime.Scoped:
                            {
                                services.TryAddScoped(implementationType, implementationType);
                            }
                            break;
                        default:
                        case ServiceLifetime.Transient:
                            {
                                services.TryAddTransient(implementationType, implementationType);
                            }
                            break;
                    }
                }
            }

            return services;
        }
    }
}
