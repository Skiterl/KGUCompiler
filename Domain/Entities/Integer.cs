using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities
{
    public class Integer:Token
    {
        public int Value { get; init; }
        public Integer(int value) : base(Tag.INTEGER_CONST, TokenType.CONST) => Value = value;
        public override string ToString() => "integer";
    }
}
