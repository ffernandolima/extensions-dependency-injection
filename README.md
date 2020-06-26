# extensions-dependency-injection

Exposes dependency injection modularization.

Also, exposes some AOP (Aspect Oriented Programming) extensions which help registering and resolving proxies instead of concrete implementations through Microsoft built-in container with the main purpose of providing lazy loading/instantiation of resources.

 | Package | NuGet |
 | ------- | ----- |
 | Extensions.DependencyInjection.Modules | [![Nuget](https://img.shields.io/badge/nuget-v1.0.0-blue) ![Nuget](https://img.shields.io/nuget/dt/Extensions.DependencyInjection.Modules)](https://www.nuget.org/packages/Extensions.DependencyInjection.Modules/1.0.0) |
 | Extensions.DependencyInjection.Proxies | [![Nuget](https://img.shields.io/badge/nuget-v1.0.0-blue) ![Nuget](https://img.shields.io/nuget/dt/Extensions.DependencyInjection.Proxies)](https://www.nuget.org/packages/Extensions.DependencyInjection.Proxies/1.0.0) |

## Installation

It is available on Nuget.

```
Install-Package Extensions.DependencyInjection.Modules -Version 1.0.0
Install-Package Extensions.DependencyInjection.Proxies -Version 1.0.0
```

P.S.: There's no dependency between the packages. Which one has its own features.

## Usage

The following code demonstrates basic usage of modules.

```C#

    public class ServicesModule : IModuleRegistry
    {
        public string ModuleName => nameof(ServicesModule);

        public void Registry(IServiceCollection services, IConfiguration configuration = null, ILoggerFactory loggerFactory = null, IHostingEnvironment hostingEnvironment = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // Register your services here.
        }
    }

    public class MessagingModule : IModuleRegistry
    {
        public string ModuleName => nameof(MessagingModule);

        public void Registry(IServiceCollection services, IConfiguration configuration = null, ILoggerFactory loggerFactory = null, IHostingEnvironment hostingEnvironment = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // Register your messaging services here.
        }
    }

    public class Startup : IStartup
    {
        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            LoggerFactory = loggerFactory;
            HostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }
        public ILoggerFactory LoggerFactory { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            IModuleCollection moduleCollection = new ModuleCollection();
            moduleCollection.AddModule<ServicesModule>()
                            .AddModule<MessagingModule>()
                            .Registry(services, Configuration, LoggerFactory, HostingEnvironment); // Configuration, LoggerFactory and HostingEnvironment are optional parameters.
        }
    }

```

The following code demonstrates basic usage of proxies.

```C#

    public interface IBarService
    {
        string Execute();
    }

    public interface IFooService
    {
        string Execute();
    }

    public class FooService : IFooService
    {
        private readonly IBarService _barService;

        // At this moment barService is just a proxy, it wasn't instantiated yet
        public FooService(IBarService barService) => _barService = barService ?? throw new ArgumentNullException(nameof(barService));

        // When invoked, it gets instantiated
        public string Execute() => $"Foo-{_barService.Execute()}";
    }

    public class BarService : IBarService
    {
        public string Execute() => $"Bar";
    }

    public class Startup : IStartup
    {
        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            LoggerFactory = loggerFactory;
            HostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }
        public ILoggerFactory LoggerFactory { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // Many ways of registering services as proxies:
            services.AddTransientProxy<IFooService, FooService>();
            services.AddTransientProxy<IBarService, BarService>();

            // You can also provide an ImplementationFactory that will be used to create the service.
            // services.AddTransientProxy<IFooService, FooService>(() => new FooService(new BarService()));
            // services.AddTransientProxy<IBarService, BarService>(() => new BarService());

            // Or:
            // services.AddTransient<IFooService>(provider => provider.CreateProxy<IFooService, FooService>());
            // services.AddTransient<IBarService>(provider => provider.CreateProxy<IBarService, BarService>());

            // You can also provide an ImplementationFactory that will be used to create the service.
            // services.AddTransient<IFooService>(provider => provider.CreateProxy<IFooService, FooService>(() => new FooService(provider.GetService<IBarService>())));
            // services.AddTransient<IBarService>(provider => provider.CreateProxy<IBarService, BarService>(() => new BarService()));

            // All Lifetimes are available (Transient, Scoped and Singleton).
        }
    }

    public class FooController : Controller
    {
        private readonly IFooService _fooService;

        // At this moment fooService is just a proxy, it wasn't instantiated yet 
        // Also its dependency barService is just a proxy, it wasn't instantiated too
        public FooController(IFooService fooService) => _fooService = fooService ?? throw new ArgumentNullException(nameof(fooService));

        public IActionResult Get()
        {
            // When invoked, it gets instantiated
            // barService is still a proxy
            var result = fooService.Execute();

            return Ok(result);
        }
    }

```

## Support / Contributing
If you want to help with the project, feel free to open pull requests and submit issues. 

## Donate

If you would like to show your support for this project, then please feel free to buy me a coffee.

<a href="https://www.buymeacoffee.com/fernandolima" target="_blank"><img src="https://www.buymeacoffee.com/assets/img/custom_images/white_img.png" alt="Buy Me A Coffee" style="height: auto !important;width: auto !important;" ></a>
