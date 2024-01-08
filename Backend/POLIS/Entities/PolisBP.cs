using Backend.POLIS.Abstractions;
using Backend.POLIS.Enums;
using Domain.Enums;

namespace Backend.POLIS.Entities
{
    public class PolisBP : PolisEntity
    {
        public PolisBP() : base(PolisEntityType.Bp, "BP", Tag.BP) { }
    }
}
