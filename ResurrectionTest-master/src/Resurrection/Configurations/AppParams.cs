using Autofac;

namespace Resurrection.Configurations
{
    public static class AppParams
    {
        public static IContainer Container { get; private set; }

        public static void LoadAppParams()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<InfraModule>();

            Container = builder.Build();
        }
    }

}
