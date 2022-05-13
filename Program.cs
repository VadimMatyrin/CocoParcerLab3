using WhileAnalyzer;

LexicalAnalyzer();
//AnalyzeWhile();

static void LexicalAnalyzer()
{
    Token t;
    Scanner scanner = new Scanner("analyze-while.txt");
    while ((t = scanner.Scan()).kind != 0)
    {
        Console.WriteLine("Token:{0}, Lexeme {1} ;", t.kind, t.val);
    }
}


static void AnalyzeWhile()
{
    Scanner scanner = new Scanner("analyze-while.txt");
    Parser parser = new Parser(scanner);
    parser.Parse();
    Console.Write(parser.errors.count + " errors detected");
}
