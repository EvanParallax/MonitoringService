using Common;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace Server.Utils
{
    public interface IDataReceiver
    {
        Task<(Envelope env, int del)> GetDataAsync(string endpoint);
    }

    public class DataReceiver : IDataReceiver
    {
        private static readonly HttpClient Client = new HttpClient();

        public async Task<(Envelope env, int del)> GetDataAsync(string endpoint)
        {
            var watch = Stopwatch.StartNew();
            HttpResponseMessage response = await Client.GetAsync(endpoint);
            var responseBody = response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : String.Empty;
            watch.Stop();
            int delay;
            Envelope envelope;

            if (response.IsSuccessStatusCode)
            {
                delay = watch.Elapsed.Milliseconds;
                envelope = JsonConvert.DeserializeObject<Envelope>(responseBody);
                return (envelope, delay);
            }

            delay = watch.Elapsed.Milliseconds;
            envelope = new Envelope()
            {
                Header = new Header()
                {
                    ErrorMsg = response.StatusCode.ToString(),
                    AgentTime = DateTime.Now
                }
            };

            return (envelope, delay);
        }
    }
}
