using Backend.Asm;
using Frontend.Syntax;

Parser parser = new Parser("ifcode.txt");
parser.SyntaxCheck();

AsmGenerator gen = new AsmGenerator("ifcode.txt");
gen.GenerateNASM();