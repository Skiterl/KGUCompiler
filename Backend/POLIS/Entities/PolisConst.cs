using Backend.POLIS.Abstractions;
using Backend.POLIS.Enums;
using Domain.Enums;

namespace Backend.POLIS.Entities
{
    public class PolisConst<T> : PolisEntity
    {
        T Value { get; set; }
        public PolisConst(T value, Tag tag) : base(PolisEntityType.Const, value.ToString(), tag)
        {
            Value = value;
        }
    }
}
