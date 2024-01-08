using Utils;

SourceManager sm = new SourceManager("testnetcomp.txt");
sm.LexerCompile();
sm.ParserCompile();
sm.PolisCompile();
sm.AssemblerCompile();
