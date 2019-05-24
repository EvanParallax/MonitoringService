using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class AgentDTO
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Endpoint { get; set; }
        public string OsType { get; set; }
        public string AgentVersion { get; set; }
        public bool IsEnabled { get; set; }
    }
}
