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
        // CONSTANTS
        INTEGER_CONST, REAL_CONST, BOOLEAN_CONST,
        // TYPES
        INTEGER_TYPE, REAL_TYPE, BOOLEAN_TYPE, CHAR_TYPE,
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
        EOF,


        // GENERATION POLIS
        BP, UPL, LABEL
    }
}
