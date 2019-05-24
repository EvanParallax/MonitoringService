using System;

namespace Common
{
    public class MetricDTO : IComparable<MetricDTO>
    {
        public DateTime Session { get; set; } 
        public string SId { get; set; }
        public string Stype { get; set; }
        public float Svalue { get; set; }

        public int CompareTo(MetricDTO other)
        {
            return Session.CompareTo(other.Session);
        }
    }
}
