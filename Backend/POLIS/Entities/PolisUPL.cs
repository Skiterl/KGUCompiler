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
    public class PolisUPL : PolisEntity
    {
        public PolisUPL() : base(PolisEntityType.Upl, "UPL", Tag.UPL) { }
    }
}
