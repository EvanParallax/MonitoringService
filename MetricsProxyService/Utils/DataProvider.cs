using Common;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MetricsProxyService.Utils
{

    public class DataProvider : IDisposable
    {
        private readonly HttpClient client;

        private readonly IMemoryCache cache;

        private readonly object locking;

        public DataProvider(IMemoryCache cache)
        {
            this.client = new HttpClient();

            this.cache = cache;

            this.locking = new object();
        }

        private void AddMetric(string endpoint, IEnumerable<MetricDTO> metrics)
        {
            cache.Set(endpoint, metrics,
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
        }

        public AgentsList GetAgents()
        {
            HttpResponseMessage response = client.GetAsync("http://localhost:59217/api/Agent").Result;
            string responseBody = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<AgentsList>(responseBody);
        }

        private MetricsList GetMetrics(Guid id)
        {
            HttpResponseMessage response = client.GetAsync($"http://localhost:59217/api/Metrics/{id}").Result;
            string responseBody = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<MetricsList>(responseBody);
        }

        public MetricsList GetMetricsCache(string id)
        {
            lock(locking)
                return (MetricsList)cache.Get(id);
        }

        public void CachingProcess()
        {
            var agents = GetAgents();

            foreach(var item in agents.Agents)
            {
                var metrics = GetMetrics(item.Id);
                lock(locking)
                    cache.Set(item.Id, metrics);
            }
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}
