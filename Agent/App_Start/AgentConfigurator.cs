using Agent.Controllers;
using Agent.Utils;
using Autofac;
using Autofac.Integration.WebApi;
using Common;
using OpenHardwareMonitor.Hardware;
using System.Reflection;
using System.Web.Http;

namespace Agent.App_Start
{
    public class AgentConfigurator : AbstractConfigurator
    {
        protected override IContainer Configure(ContainerBuilder builder)
        {
            ISensorWatcher watcher = new SensorWatcher();
            ServiceStarter.Start(watcher.WatchProc, "SensorProcess");
            builder.RegisterType<ValuesController>().InstancePerRequest();
            builder.RegisterType<Computer>().As<IComputer>();
            builder.RegisterInstance(watcher).As<ISensorWatcher>();
            return builder.Build();
        }
    }
}