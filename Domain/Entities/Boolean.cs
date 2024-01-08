using Domain.Enums;

namespace Domain.Entities
{
    public class Boolean:Word
    {
        public bool Value { get; init; }
        public Boolean(bool value, string lexeme) : base(lexeme, Tag.BOOLEAN_CONST, TokenType.CONST) => Value = value;

        public override string? ToString() => "boolean";
    }
}
