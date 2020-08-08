
namespace Extensions.DependencyInjection.Factories
{
    public interface IServiceFactory<TService, TImplementation>
        where TService : class
        where TImplementation : class, TService
    {
        TService GetService(params object[] parameters);
    }
}
