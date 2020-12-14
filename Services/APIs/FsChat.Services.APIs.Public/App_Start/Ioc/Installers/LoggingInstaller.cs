using FsChat.Providers.Logging;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FsChat.Interfaces;
using FsChat.Interfaces.Logging;

namespace FsChat.Services.APIs.Public.App_Start.Ioc.Installers
{
    public class LoggingInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<ILogProvider>()
                .ImplementedBy<NLogProvider>()
                .Named("LogProvider")
                .DynamicParameters((k, d) => d["loggerName"] = "PublicApi")
                .LifeStyle.Singleton);
        }
    }
}