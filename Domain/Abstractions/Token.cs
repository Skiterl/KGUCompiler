using Domain.Enums;

namespace Domain.Abstractions
{
    public abstract class Token
    {
        public Tag Tag { get; init; }
        public TokenType TokenType { get; init; }
        public Token(Tag tag, TokenType tokenType) { Tag = tag; TokenType = tokenType; }
    }
}
