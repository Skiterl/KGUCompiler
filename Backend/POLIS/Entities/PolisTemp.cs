using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.POLIS.Entities
{
    public class PolisTemp<T> : PolisConst<T>
    {
        string id;
        public PolisTemp(T value) : base(value, Tag.TEMP)
        {
        }
    }
}
