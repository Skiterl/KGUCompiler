using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.RPN
{
    public class RPNGenerator
    {
        public Dictionary<Tag, int[]> Priority { get; private set; } = new Dictionary<Tag, int[]>()
        {
            {Tag.LPAR, new int[2]{100, 0 } },
            {Tag.LBRA, new int[2]{100, 0} },
            {Tag.ASSIGN, new int[2]{100, 0 } },
            {Tag.FOREACH, new int[2]{100, 0 } },
            {Tag.WHILE, new int[2]{100, 0 } },
            {Tag.IF, new int[2]{100, 0 } },
            {Tag.READLN, new int[2]{100, 0 } },
            {Tag.DIM, new int[2]{100, 0 } },

            {Tag.DO, new int[2] { 1, 0 } },
            {Tag.IN, new int[2] { 1, 0 } },
            {Tag.BASIC, new int[2]{1, 0 } }, 

            {Tag.ELSE, new int[2]{2, 2 } },

            {Tag.RPAR, new int[2]{1, -1 } },
            {Tag.RBRA, new int[2]{1, -1 } },
            {Tag.SEMICOLON, new int[2] { 1, -1 } },
            {Tag.COMMA, new int[2] { 1, -1 } },

            {Tag.POST_INC, new int[2]{5, 5 } },
            {Tag.POST_DEC, new int[2]{5, 5 } },
            {Tag.PRE_DEC, new int[2]{6, 6 } },
            {Tag.PRE_INC, new int[2]{6, 6 } },
            {Tag.UPLUS, new int[2]{7, 7 } },
            {Tag.UMINUS, new int[2]{7, 7 } },
            {Tag.NOT, new int [2] { 8, 8 } },
            {Tag.MUL, new int [2]{9, 9 } },
            {Tag.DIV, new int [2]{9, 9 } },
            {Tag.SUM, new int [2]{10, 10 } },
            {Tag.SUB, new int [2]{10, 10 } },
            {Tag.LESS, new int[2]{11, 11 } },
            {Tag.GREATER, new int[2]{11, 11 } },
            {Tag.LESS_EQUAL, new int[2]{11, 11 } },
            {Tag.GREATER_EQUAL, new int [2] { 11, 11 } },
            {Tag.EQUAL, new int [2]{12, 12 } },
            {Tag.NOT_EQUAL, new int[2]{12, 12 } },
            {Tag.AND, new int[2]{13, 13 } },
            {Tag.OR, new int[2]{14, 14 } }
        };

        public Dictionary<Tag, Tag> OpenCloseTags { get; private set; } = new Dictionary<Tag, Tag>()
        {
            {Tag.ASSIGN, Tag.SEMICOLON },
            {Tag.LBRA, Tag.RBRA },
            {Tag.LPAR, Tag.RPAR },
            {Tag.IF, Tag.ELSE },
            {Tag.ELSE, Tag.SEMICOLON },
            {Tag.IF, Tag.SEMICOLON },
            {Tag.WHILE, Tag.DO },
            {Tag.DO, Tag.SEMICOLON },
            {Tag.FOREACH, Tag.IN },
            {Tag.IN, Tag.SEMICOLON },
            {Tag.WRITE, Tag.SEMICOLON },
            {Tag.READLN, Tag.SEMICOLON },
            {Tag.DIM, Tag.BASIC },
            {Tag.BASIC, Tag.SEMICOLON }
        };

        public 
    }
}
