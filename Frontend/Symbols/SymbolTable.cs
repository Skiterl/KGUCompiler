using Domain.Entities;

namespace Frontend.Symbols
{
    public class SymbolTable
    {
        public int Offset { get; set; } = 0;
        public Dictionary<Id, DataType> Table { get; private set; }
        public SymbolTable Prev { get; set; } = null;
        public SymbolTable(SymbolTable prev)
        {
            Table = new Dictionary<Id, DataType>();
            Prev = prev;
        }

        public DataType? GetType(string lexeme)
        {
            for (SymbolTable t = this; t != null; t = t.Prev)
            {
                DataType found;
                try
                {
                    //found = t.Table[w];
                    found = Table.First(id => id.Key.Value == lexeme).Value;
                }
                catch (Exception e)
                {
                    return null;
                }
                return found;
            }
            return null;
        }

        public void Put(DataType type, Id id)
        {
            Offset += type.Width;
            id.Offset = Offset;
            Table.Add(id, type);
        }
        public DataType? GetType(Id w)
        {
            for(SymbolTable t = this; t != null; t = t.Prev)
            {
                DataType found;
                try
                {
                    //found = t.Table[w];
                    found = Table.First(id => id.Key.Value == w.Value).Value;
                }
                catch (Exception e)
                {
                    return null;
                }
                return found;
            }
            return null;
        }
    }
}
