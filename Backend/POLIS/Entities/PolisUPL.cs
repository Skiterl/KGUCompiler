using Backend.POLIS.Abstractions;
using Backend.POLIS.Enums;
using Domain.Enums;

namespace Backend.POLIS.Entities
{
    public class PolisUPL : PolisEntity
    {
        public PolisUPL() : base(PolisEntityType.Upl, "UPL", Tag.UPL) { }
    }
}
