using Backend.POLIS.Abstractions;
using Backend.POLIS.Enums;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.POLIS.Entities
{
    public class PolisLabel:PolisEntity
    {
        public int Index { get; set; }
        public PolisLabel(int index) : base(PolisEntityType.Label, "Label", Tag.LABEL) => Index = index;
    }
}
