using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Extensions.DependencyInjection.Modules
{
    public class ModuleCollection : IModuleCollection, IModuleRegistry
    {
        private readonly IList<IModuleRegistry> _modules;

        public string ModuleName => nameof(ModuleCollection);

        public ModuleCollection() => _modules = new List<IModuleRegistry>();

        public ModuleCollection(IList<IModuleRegistry> modules)
        {
            if (modules == null)
            {
                throw new ArgumentNullException(nameof(modules));
            }

            _modules = new List<IModuleRegistry>(modules);
        }

        public void Registry(IServiceCollection services, IConfiguration configuration = null, ILoggerFactory loggerFactory = null, IHostingEnvironment hostingEnvironment = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            foreach (var module in _modules)
            {
                module.Registry(services, configuration, loggerFactory, hostingEnvironment);
            }
        }

        public IModuleCollection AddAllModules(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            var modules = assembly.GetTypes()
                                  .Where(type => !type.IsInterface && !type.IsAbstract && type.IsPublic && typeof(IModuleRegistry).IsAssignableFrom(type))
                                  .Select(type => (IModuleRegistry)Activator.CreateInstance(type));

            if (modules.Any())
            {
                foreach (var module in modules)
                {
                    AddModule(module);
                }
            }

            return this;
        }

        public IModuleCollection AddModule(IModuleRegistry module)
        {
            if (module == null)
            {
                throw new ArgumentNullException(nameof(module));
            }

            if (!_modules.Contains(module))
            {
                _modules.Add(module);
            }

            return this;
        }

        public IModuleCollection RemoveAllModules()
        {
            _modules.Clear();

            return this;
        }

        public IModuleCollection RemoveModule(IModuleRegistry module)
        {
            if (module == null)
            {
                throw new ArgumentNullException(nameof(module));
            }

            if (_modules.Contains(module))
            {
                _modules.Remove(module);
            }

            return this;
        }

        public IModuleCollection RemoveModule(string moduleName)
        {
            if (string.IsNullOrWhiteSpace(moduleName))
            {
                throw new ArgumentException(nameof(moduleName));
            }

            var modules = _modules.Where(module => string.Equals(module.ModuleName, moduleName, StringComparison.OrdinalIgnoreCase));

            foreach (var module in modules)
            {
                _modules.Remove(module);
            }

            return this;
        }
    }
}
