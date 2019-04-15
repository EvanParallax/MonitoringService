using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace Server.Utils
{
    public interface IDataReciever
    {
        Envelope GetData(string Endpoint);
    }

    public class DataReciever : IDataReciever
    {
        private static HttpClient client = new HttpClient();

        public Envelope GetData(string Endpoint)
        {
            HttpResponseMessage response = client.GetAsync(Endpoint).Result;
            string responseBody = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<Envelope>(responseBody);
        }
    }
}