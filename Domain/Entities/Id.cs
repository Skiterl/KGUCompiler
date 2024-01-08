using Domain.Enums;

namespace Domain.Entities
{
    public class Id : Word
    {
        public int Offset { get; set; } = 0;
        public Id(string lexeme) : base(lexeme, Tag.ID, TokenType.ID)
        {
        }

        public static bool operator ==(Id left, Id right)
        {
            return left.Value == right.Value;
        }

        public static bool operator !=(Id left, Id right)
        {
            return left.Value != right.Value;
        }

        public override string? ToString() => "Id";
    }
}
