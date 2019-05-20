using Common;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MetricsProxyService.Utils
{

    public class DataProvider : IDisposable
    {
        private readonly HttpClient client;

        private readonly IMemoryCache cache;

        private readonly SemaphoreSlim locking;

        public DataProvider(IMemoryCache cache)
        {
            this.client = new HttpClient();

            this.cache = cache;

            this.locking = new SemaphoreSlim(1);
        }

        private void AddMetric(string id, IEnumerable<MetricDTO> metrics)
        {
            cache.Set(id, metrics,
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
        }

        private async Task<MetricsList> GetMetricsAsync(string id)
        {
            HttpResponseMessage response = await client.GetAsync($"http://localhost:59217/api/Metrics/{id}");
            string responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<MetricsList>(responseBody);
        }

        public async Task<MetricsList> GetMetricsCache(string id)
        {
            MetricsList metrics;

            await locking.WaitAsync();
            try
            {
                if (!cache.TryGetValue(id, out metrics))
                {
                    metrics = await GetMetricsAsync(id);
                    cache.Set(id, metrics);
                }

            }
            finally
            {
                locking.Release();
            }
            return metrics;
        }

        public void Dispose()
        {
            client.Dispose();
            cache.Dispose();
            locking.Dispose();
        }
    }
}
