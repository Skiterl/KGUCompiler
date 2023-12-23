
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;

namespace Frontend.Symbols
{
    public class DataType : Word
    {
        public int Width { get; set; } = 0;

        public DataType(string lexeme, Tag tag, int width) : base(lexeme, tag, TokenType.TYPE) => Width = width;
        public static DataType
            Integer = new DataType("integer", Tag.INTEGER_TYPE, 4),
            Real = new DataType("real", Tag.REAL_TYPE, 8),
            Char = new DataType("char", Tag.CHAR_TYPE, 1),
            Bool = new DataType("boolean", Tag.BOOLEAN_TYPE, 1);

        public static bool IsNumeric(Token t, SymbolTable st = null)
        {
            if (t == null) throw new ArgumentNullException("Токены не может быть null");
            switch (t.TokenType)
            {
                case TokenType.CONST: 
                    return t.Tag != Tag.BOOLEAN_CONST;
                case TokenType.ID:
                    return Tag.BOOLEAN_TYPE != st.GetType((Id)t)?.Tag;
                default:
                    return false;
            }
        }

        public static HashSet<Tag> NumberOperandsOperators { get; private set; } = new HashSet<Tag>()
        {
            Tag.POST_DEC, Tag.PRE_DEC, Tag.PRE_INC, 
            Tag.POST_INC, Tag.SUB, Tag.SUB, Tag.LESS, 
            Tag.EQUAL, Tag.GREATER, Tag.GREATER_EQUAL, 
            Tag.LESS_EQUAL, Tag.MUL, Tag.DIM, Tag.DIV, Tag.UMINUS, Tag.NOT_EQUAL
        };

        public static HashSet<Tag> BooleanResultOperators { get; private set; } = new HashSet<Tag>() {
            Tag.AND, Tag.OR, Tag.EQUAL, Tag.NOT_EQUAL, Tag.GREATER_EQUAL, 
            Tag.LESS_EQUAL, Tag.GREATER, Tag.LESS, Tag.NOT
        };

        public static bool Numeric(DataType p) => p is not null && (p == Char || p == Integer || p == Real);
        public static DataType Max(DataType p1, DataType p2)
        {
            if (!Numeric(p1) || !Numeric(p2)) return null;
            else if (p1 == Real || p2 == Real) return Real;
            else if (p1 == Integer || p2 == Integer) return Integer;
            else return Char;
        }

        public static bool Boolean(DataType p) => p is not null && p == Bool;
        public static bool IsNumericResult(Word op, DataType p1 = null, DataType p2 = null)
        {
            if (BooleanResultOperators.Contains(op.Tag)) return false;
            if (Boolean(p1) || Boolean(p2)) return false;
            return true;
        }
        
        public override string ToString() => "type";
    }
}
