using System;

namespace Extensions.DependencyInjection.Factories
{
    public class ServiceFactory<TService, TImplementation> : IServiceFactory<TService, TImplementation>
        where TService : class
        where TImplementation : class, TService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Func<IServiceProvider, object[], TService> _implementationFactory;

        public ServiceFactory(IServiceProvider serviceProvider, Func<IServiceProvider, object[], TService> implementationFactory = null)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _implementationFactory = implementationFactory;
        }

        public TService GetService(params object[] parameters)
        {
            TService service;

            if (_implementationFactory != null)
            {
                service = _implementationFactory(_serviceProvider, parameters);
            }
            else
            {
                service = _serviceProvider.GetServiceOrCreateInstance<TImplementation>();
            }

            return service;
        }
    }
}
