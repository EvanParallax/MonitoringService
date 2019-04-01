using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
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


            builder.RegisterType<DataReciever>().As<IDataReciever>();
            builder.RegisterType<DataProvider>().As<IDataProvider>();
            builder.RegisterType<HierarchyWriter>().As<IHierarchyWriter>();
            builder.RegisterType<MetricWriter>().As<IMetricWriter>();
            builder.RegisterType<SessionWriter>().AsSelf();
            builder.RegisterType<ServerController>().InstancePerRequest();
            var config = builder.Build();
            var writer = config.Resolve<SessionWriter>();
            //ServiceStarter.Start(writer.WriteSession, "WriteDB");
            return config;


            // Common part
            //var config = GlobalConfiguration.Configuration;
            //builder.RegisterApiControllers(Assembly.GetExecutingAssembly()).SingleInstance();
            //var container = builder.Build();
            //config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            //

            // Another class
            //var writer = container.Resolve<SessionWriter>();
            //SessionWriter writer = new SessionWriter(ctx, rcvr, provider, hWriter, mWriter);
            //BackWorker worker = new BackWorker("WritingProcess", writer.WriteSession, 2000);

        }
    }
}