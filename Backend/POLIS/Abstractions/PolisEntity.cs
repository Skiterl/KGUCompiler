using Backend.POLIS.Enums;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.POLIS.Abstractions
{
    public abstract class PolisEntity
    {
        public Tag Tag { get; set; }
        public PolisEntityType Type { get; set; }
        public string Value { get; set; }
        public PolisEntity(PolisEntityType type, string value, Tag tag)
        {
            Type = type;
            Value = value;
            Tag = tag;
        }
    }
}
