using System;

namespace Extensions.DependencyInjection.Tests
{
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

        public FooService(IBarService barService) => _barService = barService ?? throw new ArgumentNullException(nameof(barService));

        public string Execute() => $"Foo-{_barService.Execute()}";
    }

    public class BarService : IBarService
    {
        public string Execute() => $"Bar";
    }
}
