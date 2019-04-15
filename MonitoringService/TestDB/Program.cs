using Server.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDB
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new DataContext();
            foreach (var a in db.Agents)
                Console.Write(a.Endpoint);
            
        }
    }
}
