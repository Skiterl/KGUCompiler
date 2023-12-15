using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstractions
{
    public abstract class Token
    {
        public Tag Tag { get; init; }
        public TokenType TokenType { get; init; }
        public Token(Tag tag, TokenType tokenType) { Tag = tag; TokenType = tokenType; }
    }
}
