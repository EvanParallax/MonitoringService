using Autofac;
using Autofac.Integration.WebApi;
using Server.Entities;
using System.Reflection;
using System.Web.Http;

namespace Server.Utils
{
    public class AutoConfigurator
    {
        public static void Configure()
        {
            var builder = new ContainerBuilder();
            var config = GlobalConfiguration.Configuration;
            builder.RegisterType<DataContext>().As<IDataContext>();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}
