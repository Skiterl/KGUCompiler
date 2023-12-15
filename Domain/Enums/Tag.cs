using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum Tag
    {
        // BOOLEAN CONSTANTS
        TRUE = 256, FALSE,
        // LOGICAL OPERATORS
        AND, OR,
        // EQUAL OPERATORS
        EQUAL, NOT_EQUAL,
        // RELATIONSHIP OPERATORS
        GREATER_EQUAL, LESS_EQUAL, GREATER, LESS,
        // ASSIGN
        ASSIGN,
        // TYPES
        INTEGER, REAL, BOOLEAN,
        BASIC,
        // DECL
        DIM,
        // ID
        ID,
        // MATH OPERATORS
        SUM, SUB, MUL, DIV, PRE_INC, POST_INC, PRE_DEC, POST_DEC,
        // UNARY
        UPLUS, UMINUS, NOT,
        // CONDITIONS
        IF, ELSE,
        // CYCLES
        WHILE, DO, FOREACH, IN,
        // BRACKETS
        LBRA, RBRA, // { }
        LPAR, RPAR, // ( )
        // OTHERSYMBOLS
        SEMICOLON, //;
        COMMA, //,
        // IO
        WRITE, READLN,
        // EOF
        EOF
    }
}
