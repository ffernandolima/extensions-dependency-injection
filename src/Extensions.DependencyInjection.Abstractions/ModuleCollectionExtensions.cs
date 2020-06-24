
namespace Extensions.DependencyInjection
{
    public static class ModuleCollectionExtensions
    {
        public static IModuleCollection AddAllModules<TAssembly>(this IModuleCollection modules)
            =>
            modules.AddAllModules(typeof(TAssembly).Assembly);

        public static IModuleCollection AddModule<TModule>(this IModuleCollection modules)
            where TModule : IModuleRegistry, new()
            =>
            modules.AddModule(new TModule());
    }
}
