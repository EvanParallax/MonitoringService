using Common;
using MetricsProxyService.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MetricsProxyService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetricsProxyController : Controller
    {
        private readonly DataProvider provider;

        public MetricsProxyController(DataProvider provider)
        {
            this.provider = provider;
        }

        [HttpGet]
        [Route("GetMetrics/{id?}")]
        public async Task<MetricsList> GetMetrics(string id)
        {
            return await provider.GetMetricsCache(id);
        }
    }
}