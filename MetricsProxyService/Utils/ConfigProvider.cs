using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsProxyService.Utils
{
    public interface IConfigProvider
    {
        string ServerUrl { get; }
    }

    public class ConfigProvider : IConfigProvider
    {
        private readonly IConfiguration config;

        public ConfigProvider(IConfiguration config)
        {
            this.config = config;
        }

        public string ServerUrl => config.GetSection("Server").GetValue<string>("ServerUrl");
    }
}
