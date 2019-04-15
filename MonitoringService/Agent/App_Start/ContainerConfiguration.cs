using Agent.Utils;
using Autofac;
using Autofac.Integration.WebApi;
using Common;
using System.Reflection;
using System.Web.Http;

namespace Agent.App_Start
{
    public class ContainerConfiguration
    {
        public static void Configure()
        {
            ISensorWatcher watcher = new SensorWatcher();
            BackWorker worker = new BackWorker("SensorWatcher", watcher.WatchProc, 2000);
            var builder = new ContainerBuilder();
            var config = GlobalConfiguration.Configuration;
            builder.RegisterInstance(watcher).As<ISensorWatcher>();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}