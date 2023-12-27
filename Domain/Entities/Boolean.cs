using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Boolean:Word
    {
        public bool Value { get; init; }
        public Boolean(bool value, string lexeme) : base(lexeme, Tag.BOOLEAN_CONST, TokenType.CONST) => Value = value;

        public override string? ToString() => "boolean";
    }
}
