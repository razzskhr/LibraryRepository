using Common.EncryptionRepository;
using Loggers;
using ServiceRepository;
using System.Web.Http;
using Unity;
using Unity.WebApi;

namespace LibraryManagement
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<ILoggers, Logger>();
            container.RegisterType<IConfigRepository, ConfigRepository>();
            container.RegisterType<IUserRepository, UserRepository>();
            container.RegisterType<IBooksRepository, BooksRepository>();
            container.RegisterType<IPasswordRepository,PasswordRepository>();

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}