using Utils;

SourceManager sm = new SourceManager("testcode.txt");
sm.LexerCompile();
sm.ParserCompile();
sm.PolisCompile();
sm.AssemblerCompile();
