using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ServiceStarter // todo: refactoring
    {
        public static void Start(Action callback, string name)
        {
            BackWorker worker = new BackWorker(name, callback,20000);
        }
    }
}
