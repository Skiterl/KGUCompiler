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
    public class PolisOperator:PolisEntity
    {
        public PolisOperator(string value, Tag tag) : base(PolisEntityType.Operator, value, tag) { }
    }
}
