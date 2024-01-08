using Backend.POLIS.Enums;
using Domain.Enums;

namespace Backend.POLIS.Entities
{
    internal class PolisKeyword : PolisLabel
    {
        public PolisKeyword(int index, string value, Tag tag) : base(index)
        {
            Type = PolisEntityType.Keyword;
            Value = value;
            Tag = tag;
        }
    }
}
