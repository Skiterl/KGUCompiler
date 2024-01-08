using Backend.POLIS;
using Backend.POLIS.Abstractions;
using Backend.POLIS.Entities;
using Backend.POLIS.Enums;
using Domain.Enums;
using Frontend.Symbols;
using System.Text;

namespace Backend.Asm
{
    public enum Operation
    {
        SUM, SUB,MUL,DIV, GREATER, EQUAL, GREATER_EQUAL, LESS_EQUAL,LESS,EMPTY, NOT_EQUAL
    }
    public class AsmGenerator
    {
        public POLISGenerator gen { get; set; }
        public AsmGenerator(string path) => gen = new POLISGenerator(path);
        public List<string> asmElements { get; set; } = new List<string>();
        public List<string> asmCode { get; set; } = new List<string>();
        

        public void GenerateNASM()
        {
            asmCode.Clear();
            var elements = gen.GeneratePolis();
            int mind = 1;
            Stack<PolisEntity> stack = new Stack<PolisEntity>();
            int curupl = 1;

            Dictionary<int, int> bpLabels = new Dictionary<int, int>();
            Dictionary<int, int> uplLabels = new Dictionary<int, int>();

            var temp = false;
            var lastOperation = Operation.EMPTY;

            asmCode.Add("section .text");
            asmCode.Add("global main");
            asmCode.Add("main:");
            for (int i = 0;i<elements.Count;i++)
            {
                if (elements[i].Type == PolisEntityType.Upl)
                {
                    var t = (PolisLabel)elements[i - 1];
                    uplLabels.Add(t.Index, curupl++);
                }else if (elements[i].Type == PolisEntityType.Bp)
                {
                    var t = (PolisLabel)elements[i - 1];
                    bpLabels.Add(t.Index, curupl++);
                }
            }

            for (int i = 0; i < elements.Count; i++)
            {
                if(bpLabels.ContainsKey(i))
                {
                    asmCode.Add($"M{bpLabels[i]}:");
                }else if (uplLabels.ContainsKey(i))
                {
                    asmCode.Add($"M{uplLabels[i]}:");
                }
                if (elements[i].Type == PolisEntityType.Upl)
                {
                    var t = (PolisLabel)elements[i - 1];
                    if(lastOperation == Operation.GREATER)
                        asmCode.Add($"jle M{uplLabels[t.Index]}");
                    if (lastOperation == Operation.LESS)
                        asmCode.Add($"jge M{uplLabels[t.Index]}");
                    if (lastOperation == Operation.GREATER_EQUAL)
                        asmCode.Add($"jl M{uplLabels[t.Index]}");
                    if (lastOperation == Operation.LESS_EQUAL)
                        asmCode.Add($"jg M{uplLabels[t.Index]}");
                    if (lastOperation == Operation.EQUAL)
                        asmCode.Add($"jne M{uplLabels[t.Index]}");
                    if (lastOperation == Operation.NOT_EQUAL)
                        asmCode.Add($"je M{uplLabels[t.Index]}");
                }
                else if (elements[i].Type == PolisEntityType.Bp)
                {
                    var t = (PolisLabel)elements[i - 1];
                    asmCode.Add($"jmp M{bpLabels[t.Index]}");
                }
                else if (elements[i].Type == PolisEntityType.Id || elements[i].Type == PolisEntityType.Const)
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
                                if(temp == false)asmCode.Add($"mov eax, {t1.Value}");

                                asmCode.Add($"mov dword[rbp -{t2.Offset}], eax");
                                temp = false;
                                lastOperation = Operation.EMPTY;
                            }
                            else
                            {
                                var t1 = (PolisId)stack.Pop();
                                var t2 = (PolisId)stack.Pop();
                                asmCode.Add($"mov eax, dword[rbp -{t1.Offset}]");
                                asmCode.Add($"mov dword[rbp -{t2.Offset}], eax");
                                temp = false;
                                lastOperation = Operation.EMPTY;
                            }
                        }else if (elements[i].Tag == Tag.MUL)
                        {
                            
                            if(stack.Peek().Tag == Tag.INTEGER_CONST)
                            {
                                PolisConst<int> t1 = (PolisConst<int>)stack.Pop();
                                if(stack.Peek().Tag == Tag.INTEGER_CONST)
                                {
                                    PolisConst<int> t2 = (PolisConst<int>)stack.Pop();
                                    var tempConst = new PolisConst<int>(int.Parse(t1.Value) * int.Parse(t2.Value), Tag.INTEGER_CONST);
                                    stack.Push(tempConst);
                                    asmCode.Add($"mov eax, {tempConst.Value}");
                                }
                                else
                                {
                                    PolisId t2 = (PolisId)stack.Pop();
                                    asmCode.Add($"mov eax, {t1.Value}");
                                    asmCode.Add($"mul dword[rbp -{t2.Offset}]");
                                    stack.Push(new PolisConst<int>(0, Tag.INTEGER_CONST));
                                }
                            }
                            else
                            {
                                PolisId t1 = (PolisId)stack.Pop();
                                if (stack.Peek().Tag == Tag.INTEGER_CONST)
                                {
                                    PolisConst<int> t2 = (PolisConst<int>)stack.Pop();
                                    asmCode.Add($"mov eax, {t2.Value}");
                                    asmCode.Add($"mul dword[rbp -{t1.Offset}]");
                                    stack.Push(new PolisConst<int>(0, Tag.INTEGER_CONST));
                                }
                                else
                                {
                                    PolisId t2 = (PolisId)stack.Pop();
                                    asmCode.Add($"mov eax, dword[rbp -{t1.Offset}]");
                                    asmCode.Add($"mul dword[rbp -{t2.Offset}]");
                                    stack.Push(new PolisConst<int>(0, Tag.INTEGER_CONST));
                                }
                            }
                            temp = true;
                            lastOperation = Operation.MUL;
                        }
                        else if (elements[i].Tag == Tag.DIV)
                        {

                            if (stack.Peek().Tag == Tag.INTEGER_CONST)
                            {
                                PolisConst<int> t1 = (PolisConst<int>)stack.Pop();
                                if (stack.Peek().Tag == Tag.INTEGER_CONST)
                                {
                                    PolisConst<int> t2 = (PolisConst<int>)stack.Pop();
                                    var tempConst = new PolisConst<int>(int.Parse(t2.Value) / int.Parse(t1.Value), Tag.INTEGER_CONST);
                                    stack.Push(tempConst);
                                    asmCode.Add($"mov eax, {tempConst.Value}");
                                }
                                else
                                {
                                    PolisId t2 = (PolisId)stack.Pop();
                                    asmCode.Add($"mov eax, dword[rbp -{t2.Offset}]");
                                    asmCode.Add($"mov ebx, {t1.Value}");
                                    asmCode.Add($"div ebx");
                                    stack.Push(new PolisConst<int>(0, Tag.INTEGER_CONST));
                                }
                            }
                            else
                            {
                                PolisId t1 = (PolisId)stack.Pop();
                                if (stack.Peek().Tag == Tag.INTEGER_CONST)
                                {
                                    PolisConst<int> t2 = (PolisConst<int>)stack.Pop();
                                    asmCode.Add($"mov eax, {t2.Value}");
                                    asmCode.Add($"mov ebx, dword[rbp -{t1.Offset}]");
                                    asmCode.Add($"div ebx");
                                    stack.Push(new PolisConst<int>(0, Tag.INTEGER_CONST));
                                }
                                else
                                {
                                    PolisId t2 = (PolisId)stack.Pop();
                                    asmCode.Add($"mov eax, dword[rbp -{t2.Offset}]");
                                    asmCode.Add($"mov ebx, dword[rbp -{t1.Offset}]");
                                    asmCode.Add($"div ebx");
                                    stack.Push(new PolisConst<int>(0, Tag.INTEGER_CONST));
                                }
                            }
                            temp = true;
                            lastOperation = Operation.MUL;
                        }
                        else if (elements[i].Tag == Tag.GREATER)
                        {
                            if (stack.Peek().Tag == Tag.INTEGER_CONST)
                            {
                                PolisConst<int> t1 = (PolisConst<int>)stack.Pop();
                                if (stack.Peek().Tag == Tag.INTEGER_CONST)
                                {
                                    PolisConst<int> t2 = (PolisConst<int>)stack.Pop();
                                    asmCode.Add($"mov eax, {t2.Value}");
                                    asmCode.Add($"cmp eax, {t1.Value}");
                                }
                                else
                                {
                                    PolisId t2 = (PolisId)stack.Pop();
                                    asmCode.Add($"mov eax, dword[rbp -{t2.Offset}]");
                                    asmCode.Add($"cmp eax, {t1.Value}");
                                }
                            }
                            else
                            {
                                PolisId t1 = (PolisId)stack.Pop();
                                if (stack.Peek().Tag == Tag.INTEGER_CONST)
                                {
                                    PolisConst<int> t2 = (PolisConst<int>)stack.Pop();
                                    asmCode.Add($"mov eax, {t2.Value}");
                                    asmCode.Add($"cmp eax, dword[rbp -{t1.Offset}]");
                                }
                                else
                                {
                                    PolisId t2 = (PolisId)stack.Pop();
                                    asmCode.Add($"mov eax, dword[rbp -{t2.Offset}]");
                                    asmCode.Add($"cmp eax, dword[rbp -{t1.Offset}]");
                                }
                            }
                            temp = true;
                            lastOperation = Operation.GREATER;
                        }
                        else if (elements[i].Tag == Tag.LESS_EQUAL)
                        {
                            if (stack.Peek().Tag == Tag.INTEGER_CONST)
                            {
                                PolisConst<int> t1 = (PolisConst<int>)stack.Pop();
                                if (stack.Peek().Tag == Tag.INTEGER_CONST)
                                {
                                    PolisConst<int> t2 = (PolisConst<int>)stack.Pop();
                                    asmCode.Add($"mov eax, {t2.Value}");
                                    asmCode.Add($"cmp eax, {t1.Value}");
                                }
                                else
                                {
                                    PolisId t2 = (PolisId)stack.Pop();
                                    asmCode.Add($"mov eax, dword[rbp -{t2.Offset}]");
                                    asmCode.Add($"cmp eax, {t1.Value}");
                                }
                            }
                            else
                            {
                                PolisId t1 = (PolisId)stack.Pop();
                                if (stack.Peek().Tag == Tag.INTEGER_CONST)
                                {
                                    PolisConst<int> t2 = (PolisConst<int>)stack.Pop();
                                    asmCode.Add($"mov eax, {t2.Value}");
                                    asmCode.Add($"cmp eax, dword[rbp -{t1.Offset}]");
                                }
                                else
                                {
                                    PolisId t2 = (PolisId)stack.Pop();
                                    asmCode.Add($"mov eax, dword[rbp -{t2.Offset}]");
                                    asmCode.Add($"cmp eax, dword[rbp -{t1.Offset}]");
                                }
                            }
                            temp = true;
                            lastOperation = Operation.LESS_EQUAL;
                        }
                        else if (elements[i].Tag == Tag.GREATER_EQUAL)
                        {
                            if (stack.Peek().Tag == Tag.INTEGER_CONST)
                            {
                                PolisConst<int> t1 = (PolisConst<int>)stack.Pop();
                                if (stack.Peek().Tag == Tag.INTEGER_CONST)
                                {
                                    PolisConst<int> t2 = (PolisConst<int>)stack.Pop();
                                    asmCode.Add($"mov eax, {t2.Value}");
                                    asmCode.Add($"cmp eax, {t1.Value}");
                                }
                                else
                                {
                                    PolisId t2 = (PolisId)stack.Pop();
                                    asmCode.Add($"mov eax, dword[rbp -{t2.Offset}]");
                                    asmCode.Add($"cmp eax, {t1.Value}");
                                }
                            }
                            else
                            {
                                PolisId t1 = (PolisId)stack.Pop();
                                if (stack.Peek().Tag == Tag.INTEGER_CONST)
                                {
                                    PolisConst<int> t2 = (PolisConst<int>)stack.Pop();
                                    asmCode.Add($"mov eax, {t2.Value}");
                                    asmCode.Add($"cmp eax, dword[rbp -{t1.Offset}]");
                                    stack.Push(new PolisConst<int>(0, Tag.INTEGER_CONST));
                                }
                                else
                                {
                                    PolisId t2 = (PolisId)stack.Pop();
                                    asmCode.Add($"mov eax, dword[rbp -{t2.Offset}]");
                                    asmCode.Add($"cmp eax, dword[rbp -{t1.Offset}]");
                                    stack.Push(new PolisConst<int>(0, Tag.INTEGER_CONST));
                                }
                            }
                            temp = true;
                            lastOperation = Operation.GREATER_EQUAL;
                        }
                        else if (elements[i].Tag == Tag.EQUAL)
                        {
                            if (stack.Peek().Tag == Tag.INTEGER_CONST)
                            {
                                PolisConst<int> t1 = (PolisConst<int>)stack.Pop();
                                if (stack.Peek().Tag == Tag.INTEGER_CONST)
                                {
                                    PolisConst<int> t2 = (PolisConst<int>)stack.Pop();
                                    asmCode.Add($"mov eax, {t2.Value}");
                                    asmCode.Add($"cmp eax, {t1.Value}");
                                }
                                else
                                {
                                    PolisId t2 = (PolisId)stack.Pop();
                                    asmCode.Add($"mov eax, dword[rbp -{t2.Offset}]");
                                    asmCode.Add($"cmp eax, {t1.Value}");
                                }
                            }
                            else
                            {
                                PolisId t1 = (PolisId)stack.Pop();
                                if (stack.Peek().Tag == Tag.INTEGER_CONST)
                                {
                                    PolisConst<int> t2 = (PolisConst<int>)stack.Pop();
                                    asmCode.Add($"mov eax, {t2.Value}");
                                    asmCode.Add($"cmp eax, dword[rbp -{t1.Offset}]");
                                    stack.Push(new PolisConst<int>(0, Tag.INTEGER_CONST));
                                }
                                else
                                {
                                    PolisId t2 = (PolisId)stack.Pop();
                                    asmCode.Add($"mov eax, dword[rbp -{t2.Offset}]");
                                    asmCode.Add($"cmp eax, dword[rbp -{t1.Offset}]");
                                    stack.Push(new PolisConst<int>(0, Tag.INTEGER_CONST));
                                }
                            }
                            temp = true;
                            lastOperation = Operation.EQUAL;
                        }
                        else if (elements[i].Tag == Tag.NOT_EQUAL)
                        {
                            if (stack.Peek().Tag == Tag.INTEGER_CONST)
                            {
                                PolisConst<int> t1 = (PolisConst<int>)stack.Pop();
                                if (stack.Peek().Tag == Tag.INTEGER_CONST)
                                {
                                    PolisConst<int> t2 = (PolisConst<int>)stack.Pop();
                                    asmCode.Add($"mov eax, {t2.Value}");
                                    asmCode.Add($"cmp eax, {t1.Value}");
                                }
                                else
                                {
                                    PolisId t2 = (PolisId)stack.Pop();
                                    asmCode.Add($"mov eax, dword[rbp -{t2.Offset}]");
                                    asmCode.Add($"cmp eax, {t1.Value}");
                                }
                            }
                            else
                            {
                                PolisId t1 = (PolisId)stack.Pop();
                                if (stack.Peek().Tag == Tag.INTEGER_CONST)
                                {
                                    PolisConst<int> t2 = (PolisConst<int>)stack.Pop();
                                    asmCode.Add($"mov eax, {t2.Value}");
                                    asmCode.Add($"cmp eax, dword[rbp -{t1.Offset}]");
                                    stack.Push(new PolisConst<int>(0, Tag.INTEGER_CONST));
                                }
                                else
                                {
                                    PolisId t2 = (PolisId)stack.Pop();
                                    asmCode.Add($"mov eax, dword[rbp -{t2.Offset}]");
                                    asmCode.Add($"cmp eax, dword[rbp -{t1.Offset}]");
                                    stack.Push(new PolisConst<int>(0, Tag.INTEGER_CONST));
                                }
                            }
                            temp = true;
                            lastOperation = Operation.NOT_EQUAL;
                        }
                        else if (elements[i].Tag == Tag.LESS)
                        {
                            if (stack.Peek().Tag == Tag.INTEGER_CONST)
                            {
                                PolisConst<int> t1 = (PolisConst<int>)stack.Pop();
                                if (stack.Peek().Tag == Tag.INTEGER_CONST)
                                {
                                    PolisConst<int> t2 = (PolisConst<int>)stack.Pop();
                                    asmCode.Add($"mov eax, {t2.Value}");
                                    asmCode.Add($"cmp eax, {t1.Value}");
                                }
                                else
                                {
                                    PolisId t2 = (PolisId)stack.Pop();
                                    asmCode.Add($"mov eax, dword[rbp -{t2.Offset}]");
                                    asmCode.Add($"cmp eax, {t1.Value}");
                                }
                            }
                            else
                            {
                                PolisId t1 = (PolisId)stack.Pop();
                                if (stack.Peek().Tag == Tag.INTEGER_CONST)
                                {
                                    PolisConst<int> t2 = (PolisConst<int>)stack.Pop();
                                    asmCode.Add($"mov eax, {t2.Value}");
                                    asmCode.Add($"cmp eax, dword[rbp -{t1.Offset}]");
                                }
                                else
                                {
                                    PolisId t2 = (PolisId)stack.Pop();
                                    asmCode.Add($"mov eax, dword[rbp -{t2.Offset}]");
                                    asmCode.Add($"cmp eax, dword[rbp -{t1.Offset}]");
                                }
                            }
                            temp = true;
                            lastOperation = Operation.LESS;
                        }
                        else if (elements[i].Tag == Tag.SUB)
                        {
                            if (stack.Peek().Tag == Tag.INTEGER_CONST)
                            {
                                PolisConst<int> t1 = (PolisConst<int>)stack.Pop();
                                if (stack.Peek().Tag == Tag.INTEGER_CONST)
                                {
                                    PolisConst<int> t2 = (PolisConst<int>)stack.Pop();
                                    var tempConst = new PolisConst<int>(int.Parse(t1.Value) - int.Parse(t2.Value), Tag.INTEGER_CONST);
                                    stack.Push(tempConst);
                                    asmCode.Add($"mov eax, {tempConst.Value}");
                                }
                                else
                                {
                                    PolisId t2 = (PolisId)stack.Pop();
                                    asmCode.Add($"mov eax, dword[rbp -{t2.Offset}]");
                                    asmCode.Add($"sub eax, {t1.Value}");
                                    stack.Push(new PolisConst<int>(0, Tag.INTEGER_CONST));
                                }
                            }
                            else
                            {
                                PolisId t1 = (PolisId)stack.Pop();
                                if (stack.Peek().Tag == Tag.INTEGER_CONST)
                                {
                                    PolisConst<int> t2 = (PolisConst<int>)stack.Pop();
                                    asmCode.Add($"mov eax, {t2.Value}");
                                    asmCode.Add($"sub eax, dword[rbp -{t1.Offset}]");
                                    stack.Push(new PolisConst<int>(0, Tag.INTEGER_CONST));
                                }
                                else
                                {
                                    PolisId t2 = (PolisId)stack.Pop();
                                    asmCode.Add($"mov eax, dword[rbp -{t2.Offset}]");
                                    asmCode.Add($"sub eax, dword[rbp -{t1.Offset}]");
                                    stack.Push(new PolisConst<int>(0, Tag.INTEGER_CONST));
                                }
                            }
                            temp = true;
                            lastOperation = Operation.SUB;
                        }
                        else if (elements[i].Tag == Tag.SUM)
                        {
                            if (stack.Peek().Tag == Tag.INTEGER_CONST)
                            {
                                PolisConst<int> t1 = (PolisConst<int>)stack.Pop();
                                if (stack.Peek().Tag == Tag.INTEGER_CONST)
                                {
                                    PolisConst<int> t2 = (PolisConst<int>)stack.Pop();
                                    var tempConst = new PolisConst<int>(int.Parse(t1.Value) + int.Parse(t2.Value), Tag.INTEGER_CONST);
                                    stack.Push(tempConst);
                                    asmCode.Add($"mov eax, {tempConst.Value}");
                                }
                                else
                                {
                                    PolisId t2 = (PolisId)stack.Pop();
                                    asmCode.Add($"mov eax, dword[rbp -{t2.Offset}]");
                                    asmCode.Add($"add eax, {t1.Value}");
                                    stack.Push(new PolisConst<int>(0, Tag.INTEGER_CONST));
                                }
                            }
                            else
                            {
                                PolisId t1 = (PolisId)stack.Pop();
                                if (stack.Peek().Tag == Tag.INTEGER_CONST)
                                {
                                    PolisConst<int> t2 = (PolisConst<int>)stack.Pop();
                                    asmCode.Add($"mov eax, {t2.Value}");
                                    asmCode.Add($"add eax, dword[rbp -{t1.Offset}]");
                                    stack.Push(new PolisConst<int>(0, Tag.INTEGER_CONST));
                                }
                                else
                                {
                                    PolisId t2 = (PolisId)stack.Pop();
                                    asmCode.Add($"mov eax, dword[rbp -{t2.Offset}]");
                                    asmCode.Add($"add eax, dword[rbp -{t1.Offset}]");
                                    stack.Push(new PolisConst<int>(0, Tag.INTEGER_CONST));
                                }
                            }
                            temp = true;
                            lastOperation = Operation.SUM;
                        }
                    }
                }
            }

            if (uplLabels.ContainsKey(elements.Count))
            {
                asmCode.Add($"M{uplLabels[elements.Count]}:");
            }
            asmCode.Add("mov rsp, rbp");
            asmCode.Add("pop rbp");
            asmCode.Add("ret");
        }
    }
}
