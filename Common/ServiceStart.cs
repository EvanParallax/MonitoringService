using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    class ServiceStart
    {
        public void start(Action callback, string name)
        {
            BackWorker worker = new BackWorker();
        }
    }
}
