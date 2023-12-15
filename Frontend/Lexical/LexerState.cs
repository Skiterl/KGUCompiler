using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Lexical
{
    public enum LexerState
    {
        DEFAULT, IN_STRING, IN_SYMBOL, IN_LINE_COMMENT, IN_BLOCK_COMMENT, IN_DEFINITION
    }
}
