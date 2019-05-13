using Common;
using MetricsProxyService.Utils;
using Microsoft.AspNetCore.Mvc;

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
        //[Route("GetMetrics/{id?}")]
        public MetricsList GetMetrics(string id)
        {
            return provider.GetMetricsCache(id);
        }
    }
}