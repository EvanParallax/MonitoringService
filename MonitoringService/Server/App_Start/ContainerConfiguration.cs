using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Common;
using Server.Utils;

namespace Server
{
    public class ContainerConfiguration
    {
        public static void Configure()
        {
            DataWriter writer = new DataWriter(new DataContext(), new DataReciever());
            BackWorker worker = new BackWorker("WritingProcess", writer.WriteProc, 2000);
            var builder = new ContainerBuilder();
            var config = GlobalConfiguration.Configuration;
            builder.RegisterType<DataContext>().As<IDataContext>();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly()).SingleInstance();
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}