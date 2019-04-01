using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;

namespace Common
{
    public abstract class AbstractConfigurator
    {
        protected abstract IContainer Configure(ContainerBuilder builder);

        public void RegisterInMvvc()
        {
            

            var config = GlobalConfiguration.Configuration;
            var builder = new ContainerBuilder();
            var container = Configure(builder);
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}
