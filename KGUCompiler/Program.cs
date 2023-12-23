using Backend.Asm;
using Frontend.Syntax;

Parser parser = new Parser("whilecode.txt");
parser.SyntaxCheck();

AsmGenerator gen = new AsmGenerator("whilecode.txt");
gen.GenerateNASM();
Console.Read();