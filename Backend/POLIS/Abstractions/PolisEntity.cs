using Backend.POLIS.Enums;
using Domain.Enums;

namespace Backend.POLIS.Abstractions
{
    public abstract class PolisEntity
    {
        public Tag Tag { get; set; }
        public PolisEntityType Type { get; set; }
        public virtual string Value { get; set; }
        public PolisEntity(PolisEntityType type, string value, Tag tag)
        {
            Type = type;
            Value = value;
            Tag = tag;
        }
    }
}
