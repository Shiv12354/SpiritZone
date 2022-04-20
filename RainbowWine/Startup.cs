
using Microsoft.Owin;
using Owin;
using Microsoft.Extensions.DependencyInjection;
using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using SZInfrastructure.ConfigurationManage.Interfaces;
using SZData.Interfaces;
using SZData.Repo;
using SZData;
using RainbowWine.Providers;

[assembly: OwinStartupAttribute(typeof(RainbowWine.Startup))]
namespace RainbowWine
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var services = new ServiceCollection();
            ConfigureAuth(app);
            ConfigureServices(services);
            var resolver = new DefaultDependencyResolver(services.BuildServiceProvider());
            DependencyResolver.SetResolver(resolver);
        }
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddTransient(typeof(IConfigService),typeof(ConfigService));
            //services.AddTransient(typeof(IConfigRepo),typeof(ConfigRepo));
            SZIoc.Register(services);

            services.AddControllersAsServices(typeof(Startup).Assembly.GetExportedTypes()
                  .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition)
                  .Where(t => typeof(IController).IsAssignableFrom(t)
                     || t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)));
        }
    }
    public static class ServiceProviderExtensions
    {
        public static IServiceCollection AddControllersAsServices(this IServiceCollection services,
           IEnumerable<Type> controllerTypes)
        {
            foreach (var type in controllerTypes)
            {
                services.AddTransient(type);
            }

            return services;
        }
    }
    public class DefaultDependencyResolver : IDependencyResolver
    {
        protected IServiceProvider serviceProvider;

        public DefaultDependencyResolver(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public object GetService(Type serviceType)
        {
            return this.serviceProvider.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return this.serviceProvider.GetServices(serviceType);
        }
    }
}
