# extensions-dependency-injection

Exposes service factories.

Exposes dependency injection modularization.

Also, exposes some AOP (Aspect Oriented Programming) extensions which help registering and resolving proxies instead of concrete implementations through Microsoft built-in container with the main purpose of providing lazy loading/instantiation of resources.

[![build-and-tests Workflow Status](https://github.com/ffernandolima/extensions-dependency-injection/actions/workflows/build-and-tests.yml/badge.svg?branch=master)](https://github.com/ffernandolima/extensions-dependency-injection/actions/workflows/build-and-tests.yml?branch=master)

[![build-and-publish Workflow Status](https://github.com/ffernandolima/extensions-dependency-injection/actions/workflows/build-and-publish.yml/badge.svg?branch=master)](https://github.com/ffernandolima/extensions-dependency-injection/actions/workflows/build-and-publish.yml?branch=master)

 | Package | NuGet |
 | ------- | ----- |
 | Extensions.DependencyInjection.Factories | [![Nuget](https://img.shields.io/badge/nuget-v2.2.1-blue) ![Nuget](https://img.shields.io/nuget/dt/Extensions.DependencyInjection.Factories)](https://www.nuget.org/packages/Extensions.DependencyInjection.Factories/2.2.1) |
 | Extensions.DependencyInjection.Modules | [![Nuget](https://img.shields.io/badge/nuget-v2.2.1-blue) ![Nuget](https://img.shields.io/nuget/dt/Extensions.DependencyInjection.Modules)](https://www.nuget.org/packages/Extensions.DependencyInjection.Modules/2.2.1) |
 | Extensions.DependencyInjection.Proxies | [![Nuget](https://img.shields.io/badge/nuget-v2.2.1-blue) ![Nuget](https://img.shields.io/nuget/dt/Extensions.DependencyInjection.Proxies)](https://www.nuget.org/packages/Extensions.DependencyInjection.Proxies/2.2.1) |

## Installation

It is available on Nuget.

```
Install-Package Extensions.DependencyInjection.Factories -Version 2.2.1
Install-Package Extensions.DependencyInjection.Modules -Version 2.2.1
Install-Package Extensions.DependencyInjection.Proxies -Version 2.2.1
```

P.S.: There's no dependency among the packages. Which one has its own features.

## Usage

The following code demonstrates basic usage of service factory.

```C#
    public interface IAckService
    { }

    public interface IBazService
    { }

    public interface IQuxService
    { }

    public class AckService : IAckService
    {
        private readonly object _source;

        public AckService(object source) => _source = source ?? throw new ArgumentNullException(nameof(source));
    }

    public class BazService : IBazService
    {
        public BazService()
        { }
    }

    public class QuxService : IQuxService
    {
        private readonly ILogger<QuxService> _logger;
        private readonly object _source;

        public QuxService(ILogger<QuxService> logger, object source)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _source = source ?? throw new ArgumentNullException(nameof(source));
        }
    }

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            Services.AddTransient<IBazService, BazService>();
            Services.AddServiceFactory<IBazService, BazService>();

            // Many ways of registering service factory:
            // Services.AddServiceFactory<IBazService, BazService>(() => new BazService()); // Manual instantiation
            // Services.AddServiceFactory<IBazService, BazService>((object[] args) => new BazService()); // Receives args
            // Services.AddServiceFactory<IBazService, BazService>((IServiceProvider provider, object[] args) => new BazService()); // Receives ServiceProvider and args
            // Services.AddServiceFactory<IBazService, BazService>((IServiceProvider provider, object[] args) => provider.GetServiceOrCreateInstance<IBazService>()); // Requires IBazService registration
            // Services.AddServiceFactory<IBazService, BazService>((IServiceProvider provider, object[] args) => provider.GetServiceOrCreateInstance<BazService>()); // No matter IBazService was registered or not into DI container

            Services.AddServiceFactory<IAckService, AckService>((IServiceProvider provider, object[] args) => provider.CreateInstance<AckService>(args)); // IAckService must not be registered into DI container
            Services.AddServiceFactory<IQuxService, QuxService>((IServiceProvider provider, object[] args) => provider.CreateInstance<QuxService>(args)); // IQuxService must not be registered into DI container
        }
    }

    public class DIController : Controller
    {
        private readonly IServiceFactory<IBazService> _bazServiceFactory;
        private readonly IServiceFactory<IAckService> _ackServiceFactory;
        private readonly IServiceFactory<IQuxService> _quxServiceFactory;

        public DIController(IServiceFactory<IBazService> bazServiceFactory, IServiceFactory<IAckService> ackServiceFactory, IServiceFactory<IQuxService> quxServiceFactory)
        {
            _bazServiceFactory = bazServiceFactory ?? throw new ArgumentNullException(nameof(bazServiceFactory));
            _ackServiceFactory = ackServiceFactory ?? throw new ArgumentNullException(nameof(ackServiceFactory));
            _quxServiceFactory = quxServiceFactory ?? throw new ArgumentNullException(nameof(quxServiceFactory));
        }

        public IActionResult Get()
        {
            var bazService = _bazServiceFactory.GetService();
            var ackService = _bazServiceFactory.GetService(new object());
            var quxService = _bazServiceFactory.GetService(new object());

            return Ok();
        }
    }
```

The following code demonstrates basic usage of modules.

```C#

    public class ServicesModule : IModuleRegistry
    {
        public string ModuleName => nameof(ServicesModule);

        public void Registry(IServiceCollection services, IConfiguration configuration = null, ILoggerFactory loggerFactory = null, IHostEnvironment hostEnvironment = null)
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

        public void Registry(IServiceCollection services, IConfiguration configuration = null, ILoggerFactory loggerFactory = null, IHostEnvironment hostEnvironment = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // Register your messaging services here.
        }
    }

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            IModuleCollection moduleCollection = new ModuleCollection();
            moduleCollection.AddModule<ServicesModule>()
                            .AddModule<MessagingModule>()
                            .Registry(services, Configuration); // Configuration, LoggerFactory and HostEnvironment are optional parameters.
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

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

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
