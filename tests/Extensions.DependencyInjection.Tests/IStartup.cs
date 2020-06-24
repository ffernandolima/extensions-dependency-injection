using Microsoft.Extensions.DependencyInjection;

namespace Extensions.DependencyInjection.Tests
{
    public interface IStartup
    {
        void ConfigureServices(IServiceCollection services);
    }
}
