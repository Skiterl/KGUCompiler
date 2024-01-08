using Backend.Asm;
using Backend.POLIS;
using Frontend.Lexical;
using Frontend.Syntax;
using System.Text.Json;

namespace Utils
{
    public class SourceManager
    {
        public FileInfo SourceFile { get; private set; }
        public Lexer Lexer { get; private set; }
        public Parser Parser { get; private set; }
        public POLISGenerator POLISGenerator { get; private set; }
        public AsmGenerator AssemblerGenerator { get; private set; }

        public SourceManager(string path)
        {
            try
            {
                SourceFile = new FileInfo(path);
                Lexer = new Lexer(path);
                Parser = new Parser(path);
                POLISGenerator = new POLISGenerator(path);
                AssemblerGenerator = new AsmGenerator(path);
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void LexerCompile()
        {
            try
            {
                var tokens = Lexer.GetTokens().Select(t => t.Tag.ToString());
                var jsonText = JsonSerializer.Serialize(tokens);
                File.WriteAllText("lexer.json", jsonText);
            }
            catch(Exception ex)
            {
                var jsonText = JsonSerializer.Serialize(ex.Message);
                File.WriteAllText("lexer.json", jsonText);
            }
        }

        public void ParserCompile()
        {
            try
            {
                Parser.SyntaxCheck();
                File.WriteAllText("parser.txt", "Syntax: OK");
            }
            catch(Exception e)
            {
                var jsonText = JsonSerializer.Serialize(e.Message);
                File.WriteAllText("parser.txt", jsonText);
            }
        }

        public void PolisCompile()
        {
            try
            {
                var jsonText = JsonSerializer.Serialize(POLISGenerator.GeneratePolis());
                File.WriteAllText("semantic.json", jsonText);
            }
            catch(Exception e)
            {
                var jsonText = JsonSerializer.Serialize(e.Message);
                File.WriteAllText("semantic.json", jsonText);
            }
        }

        public void AssemblerCompile()
        {
            try
            {
                var jsonText = JsonSerializer.Serialize(AssemblerGenerator.GenerateNASM());
                File.WriteAllText("assembler.txt", jsonText);
            }
            catch (Exception e)
            {
                var jsonText = JsonSerializer.Serialize(e.Message);
                File.WriteAllText("assembler.txt", jsonText);
            }
        }
    }
}
