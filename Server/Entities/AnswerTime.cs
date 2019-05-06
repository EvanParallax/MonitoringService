using System;
using System.Collections.Generic;

namespace Server.Utils
{
    public class AnswerTime
    {
        public Guid Id { get; set; }

        public int Delay { get; set; }

        public ICollection<Agent> Agents { get; set; }

        public AnswerTime()
        {
            Agents = new List<Agent>();
        }
    }
}