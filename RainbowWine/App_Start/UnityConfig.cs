using Microsoft.AspNet.Identity.Owin;
using System.Web.Http;
using Unity;
using Unity.WebApi;

namespace RainbowWine
{
    public static class UnityConfig
    {
        private static IUnityContainer _container;
        public static IUnityContainer Container
        {
            get
            {
                if (_container == null) _container = new UnityContainer();
                return _container;
            }
        }
        public static void RegisterComponents()
        {
            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(Container);
        }
    }
}