using Domain.Abstractions;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Symbols
{
    public class SymbolTable
    {
        public Dictionary<Token, Id> Table { get; private set; } = new Dictionary<Token, Id>();
        private SymbolTable Prev { get; set; } = null;
        public SymbolTable(SymbolTable prev)
        {
            Table = new Dictionary<Token, Id>();
            Prev = prev;
        }

        public void Put(Word word, Id id) => Table.Add(word, id);
        public Id? Get(Word w)
        {
            for(SymbolTable t = this; t != null; t = t.Prev)
            {
                Id found = t.Table[w];
                if (found != null) return found;
            }
            return null;
        }
    }
}
