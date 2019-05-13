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
        Task<Envelope> GetDataAsync(string endpoint);
    }

    public class DataReceiver : IDataReceiver
    {
        private static readonly HttpClient Client = new HttpClient();

        public async Task<Envelope> GetDataAsync(string endpoint)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            HttpResponseMessage response = Client.GetAsync(endpoint).Result;
            watch.Stop();
            int delay;
            Envelope envelope;
            string responseBody;

            if (response.IsSuccessStatusCode)
            {
                delay = watch.Elapsed.Milliseconds;
                responseBody = await response.Content.ReadAsStringAsync();
                envelope = JsonConvert.DeserializeObject<Envelope>(responseBody);
                envelope.Header.Delay = delay;
                return envelope;
            }

            delay = watch.Elapsed.Milliseconds;
            responseBody = await response.Content.ReadAsStringAsync();
            envelope = new Envelope()
            {
                Header = new Header()
                {
                    ErrorMsg = responseBody.ToString(),
                    AgentTime = DateTime.Now
                }
            };

            envelope.Header.Delay = delay;
            return envelope;
        }
    }
}