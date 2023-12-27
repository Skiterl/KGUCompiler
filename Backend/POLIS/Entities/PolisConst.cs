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
    public class PolisConst<T>: PolisEntity
    {
        T Value { get; set; }
        public PolisConst(T value, Tag tag) : base(PolisEntityType.Const, value.ToString(), tag)
        {
            Value = value;
        }
    }
}
