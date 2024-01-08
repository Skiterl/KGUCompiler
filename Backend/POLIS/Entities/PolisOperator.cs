using Backend.POLIS.Abstractions;
using Backend.POLIS.Enums;
using Domain.Enums;

namespace Backend.POLIS.Entities
{
    public class PolisOperator : PolisEntity
    {
        public PolisOperator(string value, Tag tag) : base(PolisEntityType.Operator, value, tag) { }
    }
}
