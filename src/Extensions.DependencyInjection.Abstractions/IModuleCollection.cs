using System.Reflection;

namespace Extensions.DependencyInjection
{
    public interface IModuleCollection : IModuleRegistry
    {
        IModuleCollection AddAllModules(Assembly assembly);
        IModuleCollection AddModule(IModuleRegistry module);
        IModuleCollection RemoveAllModules();
        IModuleCollection RemoveModule(IModuleRegistry module);
        IModuleCollection RemoveModule(string moduleName);
    }
}
