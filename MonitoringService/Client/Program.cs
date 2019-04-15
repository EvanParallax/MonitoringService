using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        private static readonly string endpoint = "Http://localhost:59217/api/Server";
        private static async Task Get_data()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(endpoint);
                string responseBody = response.Content.ReadAsStringAsync().Result;
                Envelope c = JsonConvert.DeserializeObject<Envelope>(responseBody);
                Console.WriteLine("Показания с устройства " + c.Header.AgentId + " за время " + c.Header.AgentTime.ToLongDateString());
                foreach(var sensor in c.HardwareTree.Sensors)
                {
                    Console.WriteLine(sensor.Type + " " + sensor.Value );
                }
            }
        }
        static void Main(string[] args)
        {
            MainInternal().Wait();
        }

        private static async Task MainInternal()
        {
            while (true)
            {
                Console.WriteLine("Enter command");
                var command = Console.ReadLine();
                switch (command)
                {
                    case "GetData":
                        await Get_data();
                        break;
                }
            }
        }
    }
}
