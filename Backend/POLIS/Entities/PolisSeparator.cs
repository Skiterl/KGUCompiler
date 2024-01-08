using Backend.POLIS.Abstractions;
using Backend.POLIS.Enums;
using Domain.Enums;

namespace Backend.POLIS.Entities
{
    public class PolisSeparator : PolisEntity
    {
        public PolisSeparator(string value, Tag tag) : base(PolisEntityType.Separator, value, tag) { }
    }
}
