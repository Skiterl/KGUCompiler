using Backend.POLIS.Enums;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.POLIS.Entities
{
    internal class PolisKeyword:PolisLabel
    {
        public PolisKeyword(int index, string value, Tag tag) : base(index)
        {
            Type = PolisEntityType.Keyword;
            Value = value;
            Tag = tag;
        }
    }
}
