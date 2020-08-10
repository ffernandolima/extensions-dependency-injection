
namespace Extensions.DependencyInjection.Factories
{
    public interface IServiceFactory<TService>
        where TService : class
    {
        TService GetService(params object[] parameters);
    }

    public interface IServiceFactory<TService, TImplementation> : IServiceFactory<TService>
        where TService : class
        where TImplementation : class, TService
    { }
}
