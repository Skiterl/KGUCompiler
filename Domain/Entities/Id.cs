using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Id : Word
    {
        public Id(string lexeme) : base(lexeme, Tag.ID, TokenType.ID)
        {
        }

        public override string? ToString() => "Id";
    }
}
