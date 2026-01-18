using Autofac;
using Core.Core.Domain.Entities;
using Resurrection.Domain.Interfaces;
using Resurrection.Domain.Services;

namespace Resurrection.Configurations
{
    public class InfraModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AccountService>()
                  .As<IAccountService>()
                  .InstancePerLifetimeScope();

            builder.RegisterType<HttpClientRessurrection>()
                   .AsSelf()
                   .InstancePerLifetimeScope();

            builder.RegisterType<HttpClient_RessurrectionService>()
                   .As<IHttpClient_RessurrectionService>()
                   .InstancePerLifetimeScope();

            builder.RegisterType<ArchiveService>()
                .As<IArchiveService>()
                .InstancePerLifetimeScope();
        }
    }

}
