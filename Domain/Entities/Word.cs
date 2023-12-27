using Domain.Abstractions;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Word : Token
    {
        public string Value { get; init; }
        public Word(string lexeme, Tag tag, TokenType tokenType) : base(tag, tokenType) => Value = lexeme;
        public override string? ToString() => Value;
        public static Word
            and = new Word("and", Tag.AND, TokenType.BINARY_OPERATOR),
            or = new Word("or", Tag.OR, TokenType.BINARY_OPERATOR),
            ne = new Word("!=", Tag.NOT_EQUAL, TokenType.BINARY_OPERATOR),
            eq = new Word("==", Tag.EQUAL, TokenType.BINARY_OPERATOR),
            le = new Word("<=", Tag.LESS_EQUAL, TokenType.BINARY_OPERATOR),
            ge = new Word(">=", Tag.GREATER_EQUAL, TokenType.BINARY_OPERATOR),
            post_inc = new Word("++", Tag.POST_INC, TokenType.UNARY_OPERATOR),
            post_dec = new Word("--", Tag.POST_DEC, TokenType.UNARY_OPERATOR),
            pre_inc = new Word("++", Tag.PRE_INC, TokenType.UNARY_OPERATOR),
            pre_dec = new Word("--", Tag.PRE_DEC, TokenType.UNARY_OPERATOR),
            True = new Boolean(true, "true"),
            False = new Boolean(false, "false"),
            EOF = new Word("eof", Tag.EOF, TokenType.EOF);
    }
}
