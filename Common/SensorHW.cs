using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class SensorHW : ISensorHW
    {
        public string Id { get ; }
        public string Type { get; }
        public float Value { get ; set ; }

        public SensorHW(string id, string type, float value)
        {
            Id = id;
            Type = type;
            Value = value;
        }

        protected bool Equals(SensorHW other)
        {
            return string.Equals(Id, other.Id) && string.Equals(Type, other.Type);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SensorHW) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Id != null ? Id.GetHashCode() : 0) * 397) ^ (Type != null ? Type.GetHashCode() : 0);
            }
        }
    }
}
