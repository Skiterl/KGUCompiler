
using Domain.Entities;
using Domain.Enums;

namespace Frontend.Symbols
{
    public class DataType : Word
    {
        public int Width { get; set; } = 0;

        public DataType(string lexeme, Tag tag, int width) : base(lexeme, tag, TokenType.KEYWORD) => Width = width;
        public static DataType
            Integer = new DataType("integer", Tag.BASIC, 4),
            Real = new DataType("real", Tag.BASIC, 8),
            Char = new DataType("char", Tag.BASIC, 1),
            Bool = new DataType("boolean", Tag.BASIC, 1);

        public static List<Tag> BooleanResultOperators { get; private set; } = new List<Tag>() {
        Tag.AND, Tag.OR,
        Tag.EQUAL, Tag.NOT_EQUAL,
        Tag.GREATER_EQUAL, Tag.LESS_EQUAL, Tag.GREATER, Tag.LESS
        };

        public static bool Numeric(DataType p) => p is not null && (p == Char || p == Integer || p == Real);
        public static DataType Max(DataType p1, DataType p2)
        {
            if (!Numeric(p1) || !Numeric(p2)) return null;
            else if (p1 == Real || p2 == Real) return Real;
            else if (p1 == Integer || p2 == Integer) return Integer;
            else return Char;
        }

        public static bool Boolean(DataType p) => p == Bool;
        public static bool IsNumericResult(Word op, DataType p1 = null, DataType p2 = null)
        {
            if (BooleanResultOperators.Contains(op.Tag)) return false;
            if (Boolean(p1) || Boolean(p2)) return false;
            return true;
        }
        

        public override string ToString() => "type";
    }
}
