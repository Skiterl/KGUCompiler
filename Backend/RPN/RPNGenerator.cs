using Domain.Entities;
using Domain.Enums;
using Frontend.Lexical;
using Frontend.Symbols;
using System.Reflection.Emit;

namespace Backend.RPN
{
    public enum RPNEntityType
    {
        Id, Const, Keyword, Operator, Upl, Bp, Label, Separator
    }

    public enum ConstType
    {
        INTEGER, BOOLEAN, REAL, CHAR
    }

    public class RPNEntity
    {
        public Tag tag;
        public RPNEntityType Type { get; set; }
        public string Value { get; set; }
        public RPNEntity(RPNEntityType type, string value, Tag tag)
        {
            Type = type;
            Value = value;
            this.tag = tag;
        }
    }

    class OperatorRPN:RPNEntity
    {
        public OperatorRPN(string value, Tag tag) : base(RPNEntityType.Operator, value, tag) { }
    }

    class Label : RPNEntity
    {
        public int Index { get; set; }
        public Label(int index):base(RPNEntityType.Label, "Label", Tag.LABEL) => Index = index;
    }

    class UPL : RPNEntity
    {
        public int Index { get; set; }
        public UPL(int index) : base(RPNEntityType.Upl, "UPL", Tag.UPL) => Index = index;
    }

    class BP : RPNEntity
    {
        public int Index { get; set; }
        public BP(int index) : base(RPNEntityType.Bp, "BP", Tag.BP) => Index = index;
    }

    class IdRPN : RPNEntity
    {
        public int Offset { get; set; }
        public int Width { get; set; }
        public IdRPN(int offset, string value, int width):base(RPNEntityType.Id, value, Tag.ID)
        {
            Offset = offset;
            Width = width;
        }
    }

    class ConstRPN : RPNEntity
    {
        public ConstType DataType { get; set; }
        public ConstRPN(string value, ConstType dataType, Tag tag):base(RPNEntityType.Const, value, tag) => DataType = dataType;
    }

    class KeywordRPN : Label
    {
        public KeywordRPN(int index, string value, Tag tag) : base(index)
        {
            Type = RPNEntityType.Keyword;
            Value = value;
            this.tag = tag;
        }
    }

    class SeparatorRPN: RPNEntity
    {
        public SeparatorRPN(string value, Tag tag) : base(RPNEntityType.Separator, value, tag) { }
    }

    public class RPNGenerator
    {
        public Lexer Lexer { get; private set; }
        public SymbolTable st { get; set; } = null;

        public RPNGenerator(string path)
        {
            Lexer = new Lexer(path);
        }

        public Dictionary<Tag, int[]> Priority { get; private set; } = new Dictionary<Tag, int[]>()
        {
            {Tag.LPAR, new int[2]{100, 0 } },
            {Tag.LBRA, new int[2]{100, 0} },
            {Tag.FOREACH, new int[2]{100, 0 } },
            {Tag.WHILE, new int[2]{100, 0 } },
            {Tag.IF, new int[2]{100, 0 } },
            {Tag.READLN, new int[2]{100, 0 } },
            {Tag.WRITE, new int[2]{100, 0 } },
            {Tag.DIM, new int[2]{100, 0 } },

            {Tag.DO, new int[2] { 1, 0 } },
            {Tag.IN, new int[2] { 1, 0 } },
            {Tag.BOOLEAN_TYPE, new int[2]{1, 0 } },
            {Tag.CHAR_TYPE, new int[2]{1, 0 } },
            {Tag.REAL_TYPE, new int[2]{1, 0 } },
            {Tag.INTEGER_TYPE, new int[2]{1, 0 } },

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
            {Tag.AND, new int[2]{8, 8 } },
            {Tag.OR, new int[2]{8, 8 } },
            {Tag.NOT, new int [2] { 9, 9 } },

            {Tag.LESS, new int[2]{10, 10 } },
            {Tag.GREATER, new int[2]{10, 10 } },
            {Tag.LESS_EQUAL, new int[2]{10, 10 } },
            {Tag.GREATER_EQUAL, new int [2] { 10, 10 } },
            {Tag.EQUAL, new int [2]{10, 10 } },
            {Tag.NOT_EQUAL, new int[2]{10, 10 } },
            {Tag.ASSIGN, new int[2]{10, 10 } },

            {Tag.SUM, new int [2]{11, 11 } },
            {Tag.SUB, new int [2]{11, 11 } },

            {Tag.MUL, new int [2]{12, 12 } },
            {Tag.DIV, new int [2]{12, 12 } },

            {Tag.LABEL, new int[2]{-1, 0 } }
        };
        /*
        public HashSet<Tag> IfCloseTags { get; private set; } = new HashSet<Tag>() { Tag.LBRA, Tag.SEMICOLON, Tag.ELSE };
        public HashSet<Tag> ElseCloseTags { get; private set; } = new HashSet<Tag>() { Tag.SEMICOLON };
        public HashSet<Tag> WhileCloseTags { get; private set; } = new HashSet<Tag>() { Tag.DO };
        public HashSet<Tag> DoCloseTags { get; private set; } = new HashSet<Tag>() { Tag.SEMICOLON };
        public HashSet<Tag> AssignCloseTags { get; private set; } = new HashSet<Tag>() { Tag.SEMICOLON };
        public HashSet<Tag> LBRACloseTags { get; private set; } = new HashSet<Tag>() { Tag.RBRA };

        public Dictionary<Tag, Tag> OpenCloseTags { get; private set; } = new Dictionary<Tag, Tag>()
        {
            {Tag.ASSIGN, Tag.SEMICOLON },
            {Tag.LBRA, Tag.RBRA },
            {Tag.LPAR, Tag.RPAR },
            {Tag.IF, Tag.LBRA },
            {Tag.ELSE, Tag.SEMICOLON },
            {Tag.WHILE, Tag.DO },
            {Tag.DO, Tag.SEMICOLON },
        };
        */
        public List<RPNEntity> GenerateRPN()
        {
            DataType lastType = null;
            DataType resultType = null;
            var tokens = Lexer.GetTokens().ToList();
            Stack<RPNEntity> Magazine = new Stack<RPNEntity>();
            List<RPNEntity> RPNResult = new List<RPNEntity>();

            List<Id> Ids = new List<Id>(); 

            st = new SymbolTable(null);
            Magazine.Push(new SeparatorRPN("{", Tag.LBRA));

            for (int i = 1;i< tokens.Count;i++)
            {
                switch(tokens[i].TokenType)
                {
                    case TokenType.CONST:
                        {
                            if (Magazine.Peek().Type == RPNEntityType.Operator)
                            {
                                if (tokens[i].Tag == Tag.INTEGER_CONST)
                                {
                                    if (RPNResult.Last().tag == Tag.ID)
                                    { 
                                        var resid = (IdRPN)RPNResult.Last();
                                        if (resid.Width != 4) throw new Exception("Несовместимые типы");
                                    }else if (RPNResult.Last().Type == RPNEntityType.Const)
                                    {
                                        if (tokens[i].Tag != RPNResult.Last().tag) throw new Exception("Несовместимые типы");
                                    }
                                }else if(tokens[i].Tag == Tag.REAL_CONST)
                                {
                                    if (RPNResult.Last().tag == Tag.ID)
                                    {
                                        var resid = (IdRPN)RPNResult.Last();
                                        if (resid.Width != 8) throw new Exception("Несовместимые типы");
                                    }
                                    else if (RPNResult.Last().Type == RPNEntityType.Const)
                                    {
                                        if (tokens[i].Tag != RPNResult.Last().tag) throw new Exception("Несовместимые типы");
                                    }
                                }
                                else if (tokens[i].Tag == Tag.BOOLEAN_CONST)
                                {
                                    if (RPNResult.Last().tag == Tag.ID)
                                    {
                                        var resid = (IdRPN)RPNResult.Last();
                                        if (resid.Width != 1) throw new Exception("Несовместимые типы");
                                    }
                                    else if (RPNResult.Last().Type == RPNEntityType.Const)
                                    {
                                        if (tokens[i].Tag != RPNResult.Last().tag) throw new Exception("Несовместимые типы");
                                    }
                                }
                            }
                            if (tokens[i] is Domain.Entities.Boolean boolConst) RPNResult.Add(new ConstRPN(boolConst.Value, ConstType.BOOLEAN, Tag.BOOLEAN_CONST));
                            else if (tokens[i] is Integer intConst) RPNResult.Add(new ConstRPN(intConst.Value.ToString(), ConstType.INTEGER, Tag.INTEGER_CONST));
                            else if (tokens[i] is Real realConst) RPNResult.Add(new ConstRPN(realConst.Value.ToString(), ConstType.REAL, Tag.REAL_CONST));
                            break;
                        }
                    case TokenType.ID:
                        {
                            if(Magazine.Peek().tag == Tag.DIM)
                            {
                                var haveId = st.GetType((Id)tokens[i]);
                                if (haveId is not null) throw new Exception("Переменная уже инициализирована.");
                                Ids.Add((Id)tokens[i]);
                                if (tokens[i + 1].Tag == Tag.COMMA) i++;
                                break;
                            }else if(Magazine.Peek().Type == RPNEntityType.Operator)
                            {
                                if (RPNResult.Last().tag == Tag.ID)
                                {
                                    var curid = (Id)tokens[i];
                                    var foundCurType = st.GetType(curid);
                                    var resid =(IdRPN) RPNResult.Last();
                                    if (foundCurType.Width != resid.Width) throw new Exception("Несовместимые типы");
                                }else if(RPNResult.Last().tag == Tag.BOOLEAN_CONST)
                                {
                                    var curid = (Id)tokens[i];
                                    var foundCurType = st.GetType(curid);
                                    if (foundCurType.Tag != Tag.BOOLEAN_TYPE) throw new Exception("Несовместимые типы");
                                }
                                else if (RPNResult.Last().tag == Tag.INTEGER_CONST)
                                {
                                    var curid = (Id)tokens[i];
                                    var foundCurType = st.GetType(curid);
                                    if (foundCurType.Tag != Tag.INTEGER_TYPE) throw new Exception("Несовместимые типы");
                                }
                                else if (RPNResult.Last().tag == Tag.REAL_CONST)
                                {
                                    var curid = (Id)tokens[i];
                                    var foundCurType = st.GetType(curid);
                                    if (foundCurType.Tag != Tag.REAL_TYPE) throw new Exception("Несовместимые типы");
                                }
                            }
                            
                            var IdToken = (Id)tokens[i];
                            var foundType = st.GetType(IdToken);
                            if (foundType is null) throw new Exception("Переменная не инициализирована.");
                            var idkey = st.Table.FirstOrDefault(x => x.Key.Value == IdToken.Value).Key;

                            lastType = foundType;

                            RPNResult.Add(new IdRPN(idkey.Offset, IdToken.Value, foundType.Width));
                            break;
                        }
                    case TokenType.TYPE:
                        {
                            foreach(var id in Ids)
                            {
                                st.Put((DataType)tokens[i], id);
                            }
                            Magazine.Pop();
                            i++;
                            break;
                        }
                    default:
                        {
                            if (Priority[tokens[i].Tag][0] > Priority[Magazine.Peek().tag][1])
                            {
                                switch (tokens[i].TokenType)
                                {
                                    case TokenType.UNARY_OPERATOR:
                                    case TokenType.BINARY_OPERATOR:
                                        {
                                            var oper = (Word)tokens[i];
                                            Magazine.Push(new OperatorRPN(oper.Value, oper.Tag));

                                            break;
                                        }
                                    case TokenType.SEPARATOR:
                                        {
                                            if (tokens[i].Tag == Tag.RBRA && Magazine.Peek().tag == Tag.LBRA || tokens[i].Tag == Tag.RPAR && Magazine.Peek().tag == Tag.LPAR)
                                            {
                                                Magazine.Pop();
                                                break;
                                            }
                                            else if (tokens[i].Tag == Tag.SEMICOLON && Magazine.Peek().tag == Tag.ASSIGN)
                                            {
                                                RPNResult.Add(Magazine.Pop());
                                                i++;
                                                break;
                                            }
                                            else if (tokens[i].Tag == Tag.SEMICOLON && Magazine.Peek().tag == Tag.LABEL)
                                            {
                                                var label = (Label)Magazine.Pop();
                                                if (Magazine.Peek().tag == Tag.DO)
                                                {
                                                    var doentity = (KeywordRPN)Magazine.Pop();
                                                    RPNResult.Add(new Label(doentity.Index));
                                                    RPNResult.Add(new BP(doentity.Index));
                                                }
                                                RPNResult[label.Index] = new Label(RPNResult.Count);
                                                break;
                                            }
                                            else if (tokens[i].Tag == Tag.LBRA && Magazine.Peek().tag == Tag.IF)
                                            {
                                                if (RPNResult.Last().Type == RPNEntityType.Operator)
                                                {
                                                    if (!DataType.BooleanResultOperators.Contains(RPNResult.Last().tag))
                                                        throw new Exception("if должен содержать boolean в условии");
                                                }
                                                Magazine.Pop();
                                                Magazine.Push(new Label(RPNResult.Count));
                                                RPNResult.Add(new Label(0));
                                                RPNResult.Add(new UPL(RPNResult.Count));
                                                i--;
                                                break;
                                            }
                                            else if (tokens[i].Tag == Tag.SEMICOLON && Magazine.Peek().tag == Tag.ELSE)
                                            {
                                                var elseentity = (KeywordRPN)Magazine.Pop();
                                                RPNResult[elseentity.Index] = new Label(RPNResult.Count);
                                                break;
                                            }

                                            var separ = (Word)tokens[i];
                                            Magazine.Push(new SeparatorRPN(separ.Value, separ.Tag));
                                            break;
                                        }
                                    case TokenType.KEYWORD:
                                        {
                                            if (tokens[i].Tag == Tag.DO && Magazine.Peek().tag == Tag.WHILE)
                                            {
                                                if (RPNResult.Last().Type == RPNEntityType.Operator)
                                                {
                                                    if (!DataType.BooleanResultOperators.Contains(RPNResult.Last().tag))
                                                        throw new Exception("while должен содержать boolean в условии");
                                                }
                                                var dotok = (KeywordRPN)Magazine.Pop();
                                                Magazine.Push(new KeywordRPN(dotok.Index, "do", Tag.DO));
                                                Magazine.Push(new Label(RPNResult.Count));
                                                RPNResult.Add(new Label(0));
                                                RPNResult.Add(new UPL(RPNResult.Count));
                                                break;
                                            }
                                            else if (tokens[i].Tag == Tag.ELSE && Magazine.Peek().tag == Tag.LABEL)
                                            {
                                                var label = (Label)Magazine.Pop();
                                                RPNResult.Add(new Label(0));
                                                RPNResult.Add(new BP(RPNResult.Count));
                                                RPNResult[label.Index] = new Label(RPNResult.Count);
                                                Magazine.Push(new KeywordRPN(RPNResult.Count - 2, "else", Tag.ELSE));
                                                break;
                                            }
                                            var word = (Word)tokens[i];
                                            Magazine.Push(new KeywordRPN(RPNResult.Count, word.Value, word.Tag));
                                            break;
                                        }
                                }
                            }
                            else
                            {
                                switch (Magazine.Peek().Type)
                                {
                                    case RPNEntityType.Operator:
                                        {
                                            if (tokens[i].Tag == Tag.SEMICOLON && Magazine.Peek().tag == Tag.ASSIGN)
                                            {
                                                var foundtype = lastType;

                                                if (RPNResult.Last().tag == Tag.INTEGER_CONST)
                                                {
                                                    if (foundtype.Tag != Tag.INTEGER_TYPE) throw new Exception("Идентификатор типа integer");
                                                }
                                                else if (RPNResult.Last().tag == Tag.REAL_CONST)
                                                {
                                                    if (foundtype.Tag != Tag.REAL_TYPE) throw new Exception("Идентификатор типа real");
                                                }
                                                else if (RPNResult.Last().tag == Tag.BOOLEAN_CONST)
                                                {
                                                    if (foundtype.Tag != Tag.BOOLEAN_TYPE) throw new Exception("Идентификатор типа boolean");
                                                }
                                                else
                                                    RPNResult.Add(Magazine.Pop());
                                                break;
                                            }
                                            RPNResult.Add(Magazine.Pop());
                                            i--;
                                            break;
                                        }
                                    case RPNEntityType.Keyword:
                                        {
                                            if (tokens[i].Tag == Tag.SEMICOLON && Magazine.Peek().tag == Tag.ELSE)
                                            {
                                                var elseentity = (KeywordRPN)Magazine.Pop();
                                                RPNResult[elseentity.Index] = new Label(RPNResult.Count);
                                                break;
                                            }
                                            break;
                                        }
                                }
                            }
                            break;
                        }
                }
            }

            return RPNResult;
        }
    }
}
