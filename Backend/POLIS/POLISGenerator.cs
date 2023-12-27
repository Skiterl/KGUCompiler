using Backend.POLIS.Abstractions;
using Backend.POLIS.Entities;
using Backend.POLIS.Enums;
using Backend.RPN;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Frontend.Lexical;
using Frontend.Symbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace Backend.POLIS
{
    public class POLISGenerator
    {
        public Lexer Lexer { get; private set; }
        public SymbolTable SymbolTable { get; private set; } = null;

        public POLISGenerator(string path)
        {
            Lexer = new Lexer(path);
            SymbolTable = new SymbolTable(null);
        }

        public Dictionary<Tag, int[]> Priority { get; private set; } = new Dictionary<Tag, int[]>()
        {
            {Tag.LPAR,          [100, 0] },
            {Tag.LBRA,          [100, 0] },

            {Tag.FOREACH,       [100, 0] },
            {Tag.WHILE,         [100, 0] },
            {Tag.IF,            [100, 0] },

            {Tag.DIM,           [100, 0] },

            {Tag.DO,            [1, 0] },
            {Tag.IN,            [1, 0] },
            {Tag.ELSE,          [1, 0] },

            {Tag.RPAR,          [2, -1] },
            {Tag.RBRA,          [2, -1] },
            {Tag.SEMICOLON,     [2, -1] },
            {Tag.COMMA,         [2, -1] },

            {Tag.ASSIGN,        [6, 6] },

            {Tag.OR,            [8, 8] },
            {Tag.AND,           [9, 9] },

            {Tag.LESS,          [10, 10] },
            {Tag.GREATER,       [10, 10] },
            {Tag.LESS_EQUAL,    [10, 10] },
            {Tag.GREATER_EQUAL, [10, 10] },
            {Tag.EQUAL,         [10, 10] },
            {Tag.NOT_EQUAL,     [10, 10] },
            
            {Tag.NOT,           [11, 11] },

            {Tag.SUM,           [12, 12] },
            {Tag.SUB,           [12, 12] },
            
            {Tag.MUL,           [13, 13] },
            {Tag.DIV,           [13, 13] },

            {Tag.UPLUS,         [14, 14] },
            {Tag.UMINUS,        [14, 14] },

            {Tag.PRE_DEC,       [15, 15] },
            {Tag.PRE_INC,       [15, 15] },

            {Tag.POST_INC,      [16, 16] },
            {Tag.POST_DEC,      [16, 16] },

            {Tag.LABEL,         [-1, 0] }
        };

        private bool IsNumberOperand(PolisEntity entity)
        {
            if (entity.Tag == Tag.INTEGER_CONST || entity.Tag == Tag.REAL_CONST) return true;
            else if (entity.Tag == Tag.ID)
            {
                var foundType = SymbolTable.GetType(entity.Value);
                if (foundType.Tag != Tag.BOOLEAN_TYPE) return true;
            }
            return false;
        }

        private DataType ExpressionSemanticCheck(List<PolisEntity> expr)
        {
            for (int i = 0; i < expr.Count; i++)
            {
                if (expr[i].Type == PolisEntityType.Operator)
                {
                    var oper = expr[i];
                    if (DataType.UnaryOperators.Contains(expr[i].Tag))
                    {
                        if (DataType.NumberOperandsOperators.Contains(expr[i].Tag))
                        {
                            /*if (expr[i-1].Tag == Tag.INTEGER_CONST || expr[i-1].Tag == Tag.REAL_CONST) expr.RemoveAt(i);
                            else if (expr[i - 1].Tag == Tag.ID)
                            {
                                var foundType = SymbolTable.GetType(expr[i - 1].Value);
                                if(foundType.Tag != Tag.BOOLEAN_TYPE) expr.RemoveAt(i);
                            }*/
                            if (IsNumberOperand(expr[i - 1])) expr.RemoveAt(i); else return null;
                        }
                        else
                        {
                            /*if (expr[i - 1].Tag == Tag.BOOLEAN_CONST) expr.RemoveAt(i);
                            else if (expr[i - 1].Tag == Tag.ID)
                            {
                                var foundType = SymbolTable.GetType(expr[i - 1].Value);
                                if (foundType.Tag == Tag.BOOLEAN_TYPE) expr.RemoveAt(i);
                            }*/
                            if (!IsNumberOperand(expr[i - 1])) expr.RemoveAt(i); else return null;
                        }
                        i--;
                    }
                    else
                    {
                        if (expr[i].Tag == Tag.ASSIGN)
                        {
                            if (IsNumberOperand(expr[i - 1]) && IsNumberOperand(expr[i - 2])) return DataType.Real;
                            else if (!IsNumberOperand(expr[i - 1]) && !IsNumberOperand(expr[i - 2])) return DataType.Bool;
                            else return null;
                        }
                        if (DataType.NumberOperandsOperators.Contains(expr[i].Tag))
                        {
                            if (!IsNumberOperand(expr[i - 1]) || !IsNumberOperand(expr[i - 2])) return null;
                            //var var1 = expr[i - 1];
                            //var var2 = expr[i - 2];
                            if (DataType.BooleanResultOperators.Contains(oper.Tag)) expr[i - 2] = new PolisConst<bool>(true, Tag.BOOLEAN_CONST);
                            else expr[i - 2] = new PolisConst<double>(1, Tag.INTEGER_CONST);
                        }
                        else
                        {
                            if (IsNumberOperand(expr[i - 1]) || IsNumberOperand(expr[i - 2])) return null;
                            expr[i - 2] = new PolisConst<bool>(true, Tag.BOOLEAN_CONST);
                        }
                        expr.RemoveAt(i);
                        expr.RemoveAt(i - 1);
                        i -= 2;
                    }
                }
            }

            var el = expr.Last();

            if (el.Tag == Tag.BOOLEAN_CONST) return DataType.Bool;
            else if (el.Tag == Tag.REAL_CONST) return DataType.Real;
            else if (el.Tag == Tag.INTEGER_CONST) return DataType.Integer;
            else return SymbolTable.GetType(el.Value);
        }

        public List<PolisEntity> GenerateRPN()
        {
            var tokens = Lexer.GetTokens().ToList();

            Stack<PolisEntity> Stack = new Stack<PolisEntity>();
            List<PolisEntity> Result = new List<PolisEntity>();

            List<Id> IdsBuffer = new List<Id>();

            int ExprStartIndex = -1;

            Stack.Push(new PolisSeparator("{", Tag.LBRA));

            for(int i = 1; i < tokens.Count; i++)
            {
                switch (tokens[i].TokenType)
                {
                    case TokenType.CONST:
                        {
                            if (tokens[i] is Integer intConst)
                            {
                                Result.Add(new PolisConst<int>(intConst.Value, Tag.INTEGER_CONST));
                            }
                            else if (tokens[i] is Real realConst)
                            {
                                Result.Add(new PolisConst<double>(realConst.Value, Tag.REAL_CONST));
                            }
                            else if (tokens[i] is Domain.Entities.Boolean boolConst)
                            {
                                Result.Add(new PolisConst<bool>(boolConst.Value, Tag.BOOLEAN_CONST));
                            }
                            if(ExprStartIndex == -1) ExprStartIndex = Result.Count - 1;
                            break;
                        }
                    case TokenType.ID:
                        {
                            if (Stack.Peek().Tag == Tag.DIM)
                            {
                                var haveId = SymbolTable.GetType((Id)tokens[i]);
                                if (haveId is not null) throw new Exception("Переменная уже инициализирована.");
                                IdsBuffer.Add((Id)tokens[i]);
                                if (tokens[i + 1].Tag == Tag.COMMA) i++;
                            }
                            else
                            {
                                var IdToken = (Id)tokens[i];
                                var foundType = SymbolTable.GetType(IdToken);
                                if (foundType is null) throw new Exception("Переменная не инициализирована.");
                                var idkey = SymbolTable.Table.FirstOrDefault(x => x.Key.Value == IdToken.Value).Key;

                                if (ExprStartIndex == -1) ExprStartIndex = Result.Count;

                                Result.Add(new PolisId(idkey.Offset, IdToken.Value, foundType.Width));
                            }
                            break;
                        }
                    case TokenType.TYPE:
                        {
                            foreach (var id in IdsBuffer) SymbolTable.Put((DataType)tokens[i], id);
                            Stack.Pop();
                            i++;
                            break;
                        }
                    default:
                        {
                            if (Priority[tokens[i].Tag][0] > Priority[Stack.Peek().Tag][1])
                            {
                                switch (tokens[i].TokenType)
                                {
                                    case TokenType.UNARY_OPERATOR:
                                    case TokenType.BINARY_OPERATOR:
                                        {
                                            var oper = (Word)tokens[i];
                                            Stack.Push(new PolisOperator(oper.Value, oper.Tag));

                                            break;
                                        }
                                    case TokenType.SEPARATOR:
                                        {
                                            if (tokens[i].Tag == Tag.RBRA && Stack.Peek().Tag == Tag.LBRA || tokens[i].Tag == Tag.RPAR && Stack.Peek().Tag == Tag.LPAR)
                                                Stack.Pop();
                                            else if (tokens[i].Tag == Tag.SEMICOLON && Stack.Peek().Tag == Tag.ASSIGN)
                                            {
                                                Result.Add(Stack.Pop());
                                                var type = ExpressionSemanticCheck(Result[ExprStartIndex..Result.Count]);
                                                if (type is null) throw new Exception("Несовместимые типы");
                                                i++;
                                                ExprStartIndex = -1;
                                            }
                                            else if (tokens[i].Tag == Tag.SEMICOLON && Stack.Peek().Tag == Tag.LABEL)
                                            {
                                                var label = (PolisLabel)Stack.Pop();
                                                if (Stack.Peek().Tag == Tag.DO)
                                                {
                                                    var doentity = (PolisKeyword)Stack.Pop();
                                                    Result.Add(new PolisLabel(doentity.Index));
                                                    Result.Add(new PolisBP());
                                                }
                                                Result[label.Index] = new PolisLabel(Result.Count);
                                            }
                                            else if (tokens[i].Tag == Tag.LBRA && Stack.Peek().Tag == Tag.IF)
                                            {
                                                if (Result.Last().Type == PolisEntityType.Operator)
                                                {
                                                    if (!DataType.BooleanResultOperators.Contains(Result.Last().Tag))
                                                        throw new Exception("if должен содержать boolean в условии");
                                                }

                                                var type = ExpressionSemanticCheck(Result[ExprStartIndex..Result.Count]);
                                                if (type.Tag != Tag.BOOLEAN_TYPE) throw new Exception("Ошибка в if");
                                                ExprStartIndex = -1;

                                                Stack.Pop();
                                                Stack.Push(new PolisLabel(Result.Count));
                                                Result.Add(new PolisLabel(0));
                                                Result.Add(new PolisUPL());
                                                i--;
                                            }
                                            else if (tokens[i].Tag == Tag.SEMICOLON && Stack.Peek().Tag == Tag.ELSE)
                                            {
                                                var elseentity = (PolisKeyword)Stack.Pop();
                                                Result[elseentity.Index] = new PolisLabel(Result.Count);
                                            }
                                            else
                                            {
                                                var separ = (Word)tokens[i];
                                                Stack.Push(new PolisSeparator(separ.Value, separ.Tag));
                                            }

                                            break;
                                        }
                                    case TokenType.KEYWORD:
                                        {
                                            if (tokens[i].Tag == Tag.DO && Stack.Peek().Tag == Tag.WHILE)
                                            {
                                                if (Result.Last().Type == PolisEntityType.Operator)
                                                    if (!DataType.BooleanResultOperators.Contains(Result.Last().Tag))
                                                        throw new Exception("while должен содержать boolean в условии");

                                                var type = ExpressionSemanticCheck(Result[ExprStartIndex..Result.Count]);
                                                if (type.Tag != Tag.BOOLEAN_TYPE) throw new Exception("Ошибка в while");
                                                ExprStartIndex = -1;

                                                var dotok = (PolisKeyword)Stack.Pop();
                                                Stack.Push(new PolisKeyword(dotok.Index, "do", Tag.DO));
                                                Stack.Push(new PolisLabel(Result.Count));
                                                Result.Add(new PolisLabel(0));
                                                Result.Add(new PolisUPL());
                                            }
                                            else if (tokens[i].Tag == Tag.ELSE && Stack.Peek().Tag == Tag.LABEL)
                                            {
                                                var label = (PolisLabel)Stack.Pop();
                                                Result.Add(new PolisLabel(0));
                                                Result.Add(new PolisBP());
                                                Result[label.Index] = new PolisLabel(Result.Count);
                                                Stack.Push(new PolisKeyword(Result.Count - 2, "else", Tag.ELSE));
                                            }
                                            else
                                            {
                                                var word = (Word)tokens[i];
                                                Stack.Push(new PolisKeyword(Result.Count, word.Value, word.Tag));
                                            }
                                            break;
                                        }
                                }
                            }
                            else
                            {
                                switch (Stack.Peek().Type)
                                {
                                    case PolisEntityType.Operator:
                                        {
                                            if (tokens[i].Tag == Tag.SEMICOLON && Stack.Peek().Tag == Tag.ASSIGN)
                                            {
                                                Result.Add(Stack.Pop());
                                                var type = ExpressionSemanticCheck(Result[ExprStartIndex..Result.Count]);
                                                if (type is null) throw new Exception("Несовместимые типы");
                                                ExprStartIndex = -1;
                                                break;
                                            }
                                            Result.Add(Stack.Pop());
                                            i--;
                                            break;
                                        }
                                    }
                            }
                            break;
                        }
                }
            }

            return Result;
        }
    }
}
