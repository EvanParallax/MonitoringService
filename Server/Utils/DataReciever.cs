﻿using Common;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Server.Utils
{
    public interface IDataReciever
    {
        Task<Envelope> GetDataAsync(string Endpoint);
    }

    public class DataReciever : IDataReciever
    {
        private static readonly HttpClient client = new HttpClient();

        public async Task<Envelope> GetDataAsync(string endpoint) // naming!
        {
            HttpResponseMessage response = client.GetAsync(endpoint).Result;
            string responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Envelope>(responseBody);
        }
    }
}