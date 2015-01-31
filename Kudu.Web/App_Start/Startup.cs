using System;
using System.Configuration;
using System.IO;
using System.Web;
using Kudu.Client.Infrastructure;
using Kudu.SiteManagement;
using Kudu.Web.Infrastructure;
using Kudu.Web.Models;
using Ninject;
using Ninject.Web.Common;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Kudu.Web.App_Start.Startup), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(Kudu.Web.App_Start.Startup), "Stop")]

namespace Kudu.Web.App_Start
{
    public static class Startup
    {
        private static readonly Bootstrapper _bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            HttpApplication.RegisterModule(typeof(OnePerRequestHttpModule));
            HttpApplication.RegisterModule(typeof(NinjectHttpModule));
            _bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            _bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

            RegisterServices(kernel);
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            SetupKuduServices(kernel);
        }

        private static void SetupKuduServices(IKernel kernel)
        {
            string root = HttpRuntime.AppDomainAppPath;
            string serviceSitePath = ConfigurationManager.AppSettings["serviceSitePath"];
            string sitesPath = ConfigurationManager.AppSettings["sitesPath"];
            string sitesBaseUrl = ConfigurationManager.AppSettings["urlBaseValue"];
            string serviceSitesBaseUrl = ConfigurationManager.AppSettings["serviceUrlBaseValue"];
            string customHostNames = ConfigurationManager.AppSettings["enableCustomHostNames"];

            serviceSitePath = Path.Combine(root, serviceSitePath);
            sitesPath = Path.Combine(root, sitesPath);

            var pathResolver = new DefaultPathResolver(serviceSitePath, sitesPath);
            var settingsResolver = new DefaultSettingsResolver(sitesBaseUrl, serviceSitesBaseUrl, customHostNames);

            kernel.Bind<IPathResolver>().ToConstant(pathResolver);
            kernel.Bind<ISettingsResolver>().ToConstant(settingsResolver);
            kernel.Bind<ISiteManager>().To<SiteManager>().InSingletonScope();
            kernel.Bind<KuduEnvironment>().ToMethod(_ => new KuduEnvironment
            {
                RunningAgainstLocalKuduService = true,
                IsAdmin = IdentityHelper.IsAnAdministrator(),
                ServiceSitePath = pathResolver.ServiceSitePath,
                SitesPath = pathResolver.SitesPath
            });

            // TODO: Integrate with membership system
            kernel.Bind<ICredentialProvider>().ToConstant(new BasicAuthCredentialProvider("admin", "kudu"));
            kernel.Bind<IApplicationService>().To<ApplicationService>().InRequestScope();
            kernel.Bind<ISettingsService>().To<SettingsService>();

            // Sql CE setup
            Directory.CreateDirectory(Path.Combine(root, "App_Data"));
        }
    }
}
