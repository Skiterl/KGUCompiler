using Backend.POLIS.Abstractions;
using Backend.POLIS.Enums;
using Backend.RPN;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.POLIS.Entities
{
    public class PolisSeparator:PolisEntity
    {
        public PolisSeparator(string value, Tag tag) : base(PolisEntityType.Separator, value, tag) { }
    }
}
