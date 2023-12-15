using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum TokenType
    {
        KEYWORD,
        BINARY_OPERATOR,
        UNARY_OPERATOR,
        CONST,
        ID,
        SEPARATOR,
        ERROR,
        EOF
    }
}
