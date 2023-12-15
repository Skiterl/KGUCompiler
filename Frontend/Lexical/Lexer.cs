using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Frontend.Symbols;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;

namespace Frontend.Lexical
{
    public class Lexer
    {
        public StreamReader StreamReader { get; set; }
        public LexerState State { get; private set; } = LexerState.DEFAULT;
        public Dictionary<string, Word> Words { get; private set; } = new Dictionary<string, Word>();
        public int Line { get; private set; } = 1;

        public delegate void LexerStateHandler(LexerState state);
        public event LexerStateHandler StateChanged;

        public Token LastToken { get; private set; }

        public Lexer(string path)
        {
            StreamReader = new StreamReader(path);
            StateChanged += (LexerState ls) => State = ls;

            Reserve(new Word("if", Tag.IF, TokenType.KEYWORD));
            Reserve(new Word("else", Tag.ELSE, TokenType.KEYWORD));
            Reserve(new Word("while", Tag.WHILE, TokenType.KEYWORD));
            Reserve(new Word("do", Tag.DO, TokenType.KEYWORD));
            Reserve(new Word("foreach", Tag.FOREACH, TokenType.KEYWORD));
            Reserve(new Word("dim", Tag.DIM, TokenType.KEYWORD));
            Reserve(new Word("in", Tag.IN, TokenType.KEYWORD));
            Reserve(new Word("readln", Tag.READLN, TokenType.KEYWORD));
            Reserve(new Word("write", Tag.WRITE, TokenType.KEYWORD));
            Reserve(Word.True); Reserve(Word.False);
            Reserve(DataType.Bool); Reserve(DataType.Integer);
            Reserve(DataType.Real); Reserve(DataType.Char);
        }

        public void Reserve(Word word) => Words.Add(word.Value, word);
        private bool HasNextSymbol() => IsNextSymbol((char c) => c != 65535);
        private bool IsNextSymbol(Predicate<char> p) => p((char)StreamReader.Peek());
        private char GetNextSymbol() => (char)StreamReader.Read();
        
        private Word HandleComplexToken(Word w)
        {
            StreamReader.Read();
            LastToken = w;
            return w;
        }

        private bool IsIdentifier(string str)
        {
            Regex id = new Regex(@"^[A-Za-z][A-Za-z]+\d");
            return id.IsMatch(str);
        }

        private Token HandleConst(char curChar)
        {
            int value = curChar - '0';
            while (IsNextSymbol(char.IsDigit))
            {
                curChar = GetNextSymbol();
                value = value * 10 + curChar;
            }
            if (IsNextSymbol((char c) => c != '.'))
            {
                LastToken = new Integer(value);
                return new Integer(value);
            }

            curChar = GetNextSymbol();
            float x = value; float d = 10;

            while (IsNextSymbol(char.IsDigit))
            {
                curChar = GetNextSymbol();
                x = x + curChar / d; d *= 10;
            }
            LastToken = new Real(x);
            return new Real(x);
        }

        private Word HandleWord(char curChar)
        {
            var posLex = new StringBuilder().Append(curChar);

            while (IsNextSymbol(char.IsLetterOrDigit))
            {
                posLex.Append(GetNextSymbol());
            }
            string str = posLex.ToString();
            Word posKW;



            if (Words.TryGetValue(str, out posKW))
            {
                if (posKW.Tag == Tag.DIM) StateChanged(LexerState.IN_DEFINITION);
                LastToken = posKW;
                return posKW;
            }
            if (IsIdentifier(str))
            {
                LastToken = new Id(str);
                return new Id(str);
            }
            else throw new Exception($"Lexical Error in identification in {Line} line");
        }

        private Word HandleToken(string lexeme, Tag tag, TokenType tokenType)
        {
            Word token = new Word(lexeme, tag, tokenType);
            LastToken = token;
            return token;
        }

        private Token GetNextToken()
        {
            while (HasNextSymbol())
                switch (State)
                {
                    case LexerState.IN_BLOCK_COMMENT:
                        {
                            if (IsNextSymbol((char c) => c == '\n')) Line++;
                            if (GetNextSymbol() == '*' && IsNextSymbol((char c) => c == ')'))
                            {
                                StateChanged(LexerState.DEFAULT);
                                GetNextSymbol();
                            }
                            break;
                        }
                    case LexerState.IN_LINE_COMMENT:
                        {
                            if (GetNextSymbol() == '\n')
                            {
                                Line++;
                                StateChanged(LexerState.DEFAULT);
                            }
                            break;
                        }
                    case LexerState.IN_DEFINITION:
                        {
                            char curChar = GetNextSymbol();
                            if (char.IsWhiteSpace(curChar)) continue;
                            if (LastToken.Tag == Tag.COMMA || LastToken.Tag == Tag.DIM)
                                if (char.IsLetter(curChar))
                                {
                                    Word Id = HandleWord(curChar);
                                    if (Id.Tag == Tag.ID) return Id;
                                }
                                else throw new Exception($"Expected Identifier in {Line} line");
                            else if (LastToken.Tag == Tag.ID)
                            {
                                if (curChar == ',') return HandleToken(",", Tag.COMMA, TokenType.SEPARATOR);
                                Word Type = HandleWord(curChar);
                                if (Type.Tag == Tag.BASIC)
                                {
                                    StateChanged(LexerState.DEFAULT);
                                    return Type;
                                }
                            }
                            throw new Exception($"Expected \",\" or Type in {Line} line");
                        }
                    case LexerState.DEFAULT:
                        {
                            char curChar = GetNextSymbol();

                            switch (curChar)
                            {
                                case '\n':
                                    Line++;
                                    continue;
                                case ' ':
                                case '\r':
                                case '\t':
                                    continue;
                                case '/':
                                    if (IsNextSymbol((char c) => c == '/'))
                                    {
                                        StateChanged(LexerState.IN_LINE_COMMENT);
                                        continue;
                                    };
                                    return HandleToken("/", Tag.DIV, TokenType.BINARY_OPERATOR);
                                case '(':
                                    if (IsNextSymbol((char c) => c == '*'))
                                    {
                                        StateChanged(LexerState.IN_BLOCK_COMMENT);
                                        continue;
                                    }
                                    return HandleToken("(", Tag.LPAR, TokenType.SEPARATOR);
                                case '{':
                                    return HandleToken("{", Tag.LBRA, TokenType.SEPARATOR);
                                case '}':
                                    return HandleToken("}", Tag.RBRA, TokenType.SEPARATOR);
                                case ')':
                                    return HandleToken(")", Tag.RPAR, TokenType.SEPARATOR);
                                case '=':
                                    if (IsNextSymbol((char c) => c == '=')) return HandleComplexToken(Word.eq);
                                    else return HandleToken("=", Tag.ASSIGN, TokenType.BINARY_OPERATOR);
                                case '<':
                                    if (IsNextSymbol((char c) => c == '=')) return HandleComplexToken(Word.le);
                                    else return HandleToken("<", Tag.LESS, TokenType.BINARY_OPERATOR);
                                case '>':
                                    if (IsNextSymbol((char c) => c == '=')) return HandleComplexToken(Word.ge);
                                    else return HandleToken(">", Tag.GREATER, TokenType.BINARY_OPERATOR);
                                case '!':
                                    if (IsNextSymbol((char c) => c == '=')) return HandleComplexToken(Word.ne);
                                    else return HandleToken("!", Tag.NOT, TokenType.BINARY_OPERATOR);
                                case '+':
                                    if (IsNextSymbol((char c) => c == '+')) 
                                    {
                                        if (LastToken.TokenType == TokenType.CONST || LastToken.TokenType == TokenType.ID) return HandleComplexToken(Word.post_inc);
                                        else return HandleComplexToken(Word.pre_inc);
                                    } 
                                    else if (LastToken.TokenType == TokenType.CONST || LastToken.TokenType == TokenType.ID)
                                        return HandleToken("+", Tag.SUM, TokenType.BINARY_OPERATOR);
                                    else return HandleToken("@+", Tag.UPLUS, TokenType.UNARY_OPERATOR);
                                case '-':
                                    if (IsNextSymbol((char c) => c == '-'))
                                    {
                                        if (LastToken.TokenType == TokenType.CONST || LastToken.TokenType == TokenType.ID) return HandleComplexToken(Word.post_dec);
                                        else return HandleComplexToken(Word.pre_dec);
                                    }
                                    else if (LastToken.TokenType == TokenType.CONST || LastToken.TokenType == TokenType.ID)
                                        return HandleToken("-", Tag.SUB, TokenType.BINARY_OPERATOR);
                                    else return HandleToken("@-", Tag.UMINUS, TokenType.UNARY_OPERATOR);
                                case '*':
                                    return HandleToken("*", Tag.MUL, TokenType.BINARY_OPERATOR);
                                case ',':
                                    return HandleToken(",", Tag.COMMA, TokenType.SEPARATOR);
                                case ';':
                                    return HandleToken(";", Tag.SEMICOLON, TokenType.SEPARATOR);
                                default:
                                    break;
                            }

                            if (char.IsDigit(curChar)) return HandleConst(curChar);
                            if (char.IsLetter(curChar)) return HandleWord(curChar);

                            throw new Exception($"Unknown symbol {curChar} in {Line} line");
                        }
                }
            Console.WriteLine("Lexical analys: OK");
            return Word.EOF;
        }

        public IEnumerable<Token> GetTokens()
        {
            Token token;
            while ((token = GetNextToken()).Tag != Tag.EOF)
            {
                yield return token;
            }
        }
    }
}
