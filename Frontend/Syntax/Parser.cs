using Frontend.Lexical;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Syntax
{
    public class Parser
    {
        public Lexer Lexer { get; private set; }
        private Dictionary<string, IEnumerable<string>> Matrix { get; set; } = new Dictionary<string, IEnumerable<string>>();
        private List<string> Rules { get; set; } = new List<string>();

        public Parser(string path)
        {
            Lexer = new Lexer(path);
            ReadMatrix();
            ReadRules();
        }

        public void ReadRules()
        {
            using (TextFieldParser parser = new TextFieldParser(@"file3.csv"))
            {
                var line = parser.ReadLine();
                while (!parser.EndOfData)
                {
                    line = parser.ReadLine();
                    var values = line.Split(",").ToList();
                    if (values[0] == "\"")
                    {
                        values.RemoveAll(x => x == "\"");
                        values.Insert(0, ",");
                    }

                    Matrix.Add(values[0].Trim(), values.ToArray()[1..]);
                }
            }
        }

        public void ReadMatrix()
        {
            using (StreamReader streamReader = new StreamReader("newgrammatic.txt"))
            {
                string str = streamReader.ReadLine();
                var rule = str.Split(":");
                Rules.AddRange(rule[1].Split("|").Select(str => str.Trim()));
            }
        }

        private bool RuleCheck(string prod)
        {
            foreach (var rule in Rules)
            {
                if (prod == rule)
                {
                    return true;
                }
            }
            return false;
        }

        public void SyntaxCheck()
        {
            var tokens = Lexer.GetTokens().ToList();
            var stack = new Stack<string>();

            string noterm = "E";
            List<string> terminals = Matrix.Keys.ToList();

            stack.Push("/b/");

            for(int i = 0;i<tokens.Count; i++)
            {
                string predRel, prod = "", topTerm = "";

                if (stack.Peek() == noterm)
                {
                    prod = stack.Pop();
                }
                predRel = Matrix[stack.Peek()].ToList()[terminals.FindIndex(str => str == tokens[i].ToString())];

                switch (predRel)
                {
                    case "<":
                    case "=":
                        {
                            if(prod != "")stack.Push(prod);
                            stack.Push(tokens[i].ToString());
                            break;
                        }
                    case ">":
                        {
                            topTerm = stack.Pop();
                            prod = topTerm + " " + prod;

                            while (stack.Peek() == noterm || Matrix[stack.Peek()].ToList()[terminals.FindIndex(str => str == topTerm)] == "=")
                            {
                                if(stack.Peek() != noterm) topTerm = stack.Peek();
                                prod = stack.Pop() + " " + prod;
                            }

                            prod = prod.Trim();

                            if (RuleCheck(prod)) stack.Push(noterm);
                            else throw new Exception("Syntax error: " + i + " Token: " + tokens[i]);
                            i--;
                            break;
                        }
                    default:
                        throw new Exception("Syntax error: " + i + " Token: " + tokens[i]);
                }
            }

            string finalCheck = stack.Pop();

            while(stack.Count > 1)finalCheck = stack.Pop() + " " + finalCheck;
            if (RuleCheck(finalCheck)) stack.Push(noterm);

            stack.Push("/e/");

            if (stack.Count == 3) Console.WriteLine("Syntax analys: OK");
        }
    }
}
