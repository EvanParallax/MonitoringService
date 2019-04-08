using Autofac;
using Common;
using Server.Controllers;
using Server.Utils;

namespace Server
{
    public class ServerConfigurator : AbstractConfigurator
    {
        protected override IContainer Configure(ContainerBuilder builder)
        {
           
            builder.RegisterType<DataContext>()
                .As<IDataContext>().As<IReadOnlyDataContext>();


            builder.RegisterType<DataReceiver>().As<IDataReceiver>();
            builder.RegisterType<DataProvider>().As<IDataProvider>();
            builder.RegisterType<HierarchyWriter>().As<IHierarchyWriter>();
            builder.RegisterType<MetricWriter>().As<IMetricWriter>();
            builder.RegisterType<SessionWriter>().AsSelf();
            builder.RegisterType<AgentController>().InstancePerRequest();
            builder.RegisterType<MetricsController>().InstancePerRequest();
            var config = builder.Build();
            var writer = config.Resolve<SessionWriter>();
            ServiceStarter.Start(writer.WriteSession, "WriteDB");
            return config;
        }
    }
}