using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;

namespace Extensions.DependencyInjection.Proxies
{
    public class ServiceActivationInterceptor<TService, TImplementation> : IInterceptor, IDisposable
        where TService : class
        where TImplementation : class, TService
    {
        private TService _service;

        private readonly IServiceProvider _serviceProvider;
        private readonly Func<TImplementation> _implementationFactory;

        public ServiceActivationInterceptor(IServiceProvider serviceProvider, Func<TImplementation> implementationFactory = null)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _implementationFactory = implementationFactory;
        }

        public void Intercept(IInvocation invocation)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }

            if (invocation.Method.Name.Equals(nameof(Dispose), StringComparison.OrdinalIgnoreCase))
            {
                Dispose();
            }
            else
            {
                var service = GetServiceOrCreateInstance();

                if (service == null)
                {
                    throw new ArgumentNullException(nameof(service));
                }

                try
                {
                    invocation.ReturnValue = invocation.Method.Invoke(service, invocation.Arguments);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        throw ex.InnerException;
                    }

                    throw new Exception($"An error has occurred while invoking {typeof(TService).Name}.{invocation.Method.Name}.", ex);
                }
            }
        }

        private TService GetServiceOrCreateInstance()
        {
            if (_service == null)
            {
                if (_implementationFactory != null)
                {
                    _service = _implementationFactory();
                }
                else
                {
                    _service = (TService)ActivatorUtilities.GetServiceOrCreateInstance(_serviceProvider, typeof(TImplementation));
                }
            }

            return _service;
        }

        private void DisposeService()
        {
            if (_service != null)
            {
                if (_service is IDisposable disposable)
                {
                    try
                    {
                        disposable.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(
                            $"An error has occurred while disposing service '{typeof(TService).Name}'. Exception -> {ex.ToString()}"
                        );
                    }
                }

                _service = null;
            }
        }

        #region IDisposable Members

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    DisposeService();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
