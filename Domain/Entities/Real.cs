using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities
{
    public class Real:Token
    {
        public double Value { get; init; }
        public Real(float value) : base(Tag.REAL_CONST, TokenType.CONST) => Value = value;
        public override string? ToString() => "real";
    }
}
