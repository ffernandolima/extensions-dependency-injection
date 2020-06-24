using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Extensions.DependencyInjection
{
    public interface IModuleRegistry
    {
        string ModuleName { get; }
        void Registry(IServiceCollection services, IConfiguration configuration = null, ILoggerFactory loggerFactory = null, IHostingEnvironment hostingEnvironment = null);
    }
}
