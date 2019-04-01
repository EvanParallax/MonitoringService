using Common;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Server.Utils
{
    public interface IDataReceiver
    {
        Task<Envelope> GetDataAsync(string endpoint);
    }

    public class DataReceiver : IDataReceiver
    {
        private static readonly HttpClient Client = new HttpClient();

        public async Task<Envelope> GetDataAsync(string endpoint)
        {
            HttpResponseMessage response = Client.GetAsync(endpoint).Result;
            string responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Envelope>(responseBody);
        }
    }
}