using Microsoft.Extensions.Logging;
using System;

namespace Extensions.DependencyInjection.Tests
{
    public interface IAckService
    { }

    public interface IBarService
    {
        string Execute();
    }

    public interface IBazService
    { }

    public interface IFooService
    {
        string Execute();
    }

    public interface IQuxService
    { }

    public class AckService : IAckService
    {
        private readonly object _source;

        public AckService(object source) => _source = source ?? throw new ArgumentNullException(nameof(source));
    }

    public class BarService : IBarService
    {
        public BarService()
        { }

        public string Execute() => $"Bar";
    }

    public class BazService : IBazService
    {
        public BazService()
        { }
    }

    public class FooService : IFooService
    {
        private readonly IBarService _barService;

        public FooService(IBarService barService) => _barService = barService ?? throw new ArgumentNullException(nameof(barService));

        public string Execute() => $"Foo-{_barService.Execute()}";
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
}
