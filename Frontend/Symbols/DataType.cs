
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
            Tag.POST_INC, Tag.SUB, Tag.SUM, Tag.LESS, 
            Tag.EQUAL, Tag.GREATER, Tag.GREATER_EQUAL, 
            Tag.LESS_EQUAL, Tag.MUL, Tag.DIM, Tag.DIV, Tag.UMINUS, Tag.NOT_EQUAL
        };

        public static HashSet<Tag> UnaryOperators { get; private set; } = new HashSet<Tag>()
        { 
            Tag.POST_INC, Tag.POST_DEC, Tag.PRE_DEC, 
            Tag.PRE_INC, Tag.NOT, Tag.UMINUS, Tag.UPLUS
        };

        public static HashSet<Tag> BooleanResultOperators { get; private set; } = new HashSet<Tag>() 
        {
            Tag.AND, Tag.OR, Tag.EQUAL, Tag.NOT_EQUAL, Tag.GREATER_EQUAL, 
            Tag.LESS_EQUAL, Tag.GREATER, Tag.LESS, Tag.NOT
        };
        
        public override string ToString() => "type";
    }
}
