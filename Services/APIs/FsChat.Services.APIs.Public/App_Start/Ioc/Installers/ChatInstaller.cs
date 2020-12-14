using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using FsChat.Providers.Chat;
using FsChat.Providers.Chat.Interfaces;

namespace FsChat.Services.APIs.Public.App_Start.Ioc.Installers
{
    public class ChatInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IChatSessionQueue>()
                .ImplementedBy<ChatSessionQueue>()
                .Named("ChatSessionQueue")
                .LifeStyle.Singleton);

            container.Register(Component.For<IAgentChatCoordinatorService>()
                .ImplementedBy<AgentChatCoordinatorService>()
                .Named("AgentChatCoordinatorService")
                .LifeStyle.Singleton);
        }
    }
}