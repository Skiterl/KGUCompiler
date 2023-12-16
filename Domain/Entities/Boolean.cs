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

        public Boolean(string lexeme) : base(lexeme, Tag.BOOLEAN_CONST, TokenType.CONST)
        {
        }

        public override string? ToString() => "boolean";
    }
}
