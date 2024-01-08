using Backend.POLIS;
using Backend.POLIS.Abstractions;
using Backend.POLIS.Entities;
using Backend.POLIS.Enums;
using Domain.Enums;
using Frontend.Symbols;
using System.Text;

namespace Backend.Asm
{
    public class AsmGenerator
    {
        public POLISGenerator gen { get; set; }
        public AsmGenerator(string path) => gen = new POLISGenerator(path);

        public string GenerateNASM()
        {
            var elements = gen.GeneratePolis();
            StringBuilder sb = new StringBuilder();

            Stack<PolisEntity> stack = new Stack<PolisEntity>();

            var temp = false;
            int mind = 1;

            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].Type == PolisEntityType.Id || elements[i].Type == PolisEntityType.Const)
                {
                    stack.Push(elements[i]);
                }
                else if (elements[i].Type == PolisEntityType.Operator)
                {
                    if (DataType.UnaryOperators.Contains(elements[i].Tag))
                    {

                    }
                    else
                    {
                        if (elements[i].Tag == Tag.ASSIGN)
                        {
                            if (stack.Peek().Type == PolisEntityType.Const)
                            {
                                var t1 = (PolisConst<int>)stack.Pop();
                                var t2 = (PolisId)stack.Pop();
                                if(temp == false)sb.AppendLine($"mov eax, {t1.Value}");

                                sb.AppendLine($"mov dword[rbp -{t2.Offset}], eax");
                                temp = false;
                            }
                        }else if (elements[i].Tag == Tag.MUL)
                        {
                            var t1 = (PolisId)stack.Pop();
                            var t2 = (PolisId)stack.Pop();
                            sb.AppendLine($"mov eax, dword[rbp -{t1.Offset}]");
                            sb.AppendLine($"mul dword[rbp -{t2.Offset}]");
                            stack.Push(new PolisConst<int>(0, Tag.INTEGER_CONST));
                            temp = true;
                        }else if (elements[i].Tag == Tag.EQUAL)
                        {
                            var t1 = (PolisId)stack.Pop();
                            var t2 = (PolisConst<int>)stack.Pop();
                            sb.AppendLine($"M{mind++}:");
                            sb.AppendLine($"cmp dword[rbp-{t1.Offset}], {t2.Value}");
                        }
                    }
                }
            }

            return sb.ToString();
        }
    }
}
