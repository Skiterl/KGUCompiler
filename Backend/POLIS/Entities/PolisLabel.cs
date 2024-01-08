using Backend.POLIS.Abstractions;
using Backend.POLIS.Enums;
using Domain.Enums;

namespace Backend.POLIS.Entities
{
    public class PolisLabel : PolisEntity
    {
        public int Index { get; set; }
        public PolisLabel(int index) : base(PolisEntityType.Label, "Label", Tag.LABEL) => Index = index;
    }
}
