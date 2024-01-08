using Backend.POLIS.Abstractions;
using Backend.POLIS.Enums;
using Domain.Enums;

namespace Backend.POLIS.Entities
{
    public class PolisId : PolisEntity
    {
        public int Offset { get; private set; }
        public int Width { get; private set; }
        public PolisId(int offset, string value, int width) : base(PolisEntityType.Id, value, Tag.ID)
        {
            Offset = offset;
            Width = width;
        }
    }
}
