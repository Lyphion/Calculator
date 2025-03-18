using System.Globalization;
using Calculator;

while (true)
{
    string? line = Console.ReadLine();
    if (line is null || string.IsNullOrWhiteSpace(line))
        continue;
    if (line.Equals("exit"))
        break;

    try
    {
        var tokens = Lexer.Tokenize(line);
#if DEBUG
        Console.WriteLine(string.Join('\n', tokens));
        Console.WriteLine("---");
#endif

        var node = Parser.Parse(tokens);

#if DEBUG
        Console.WriteLine(node);
        Console.WriteLine("---");
#endif

        double result = node.Evaluate();
        Console.WriteLine('=' + result.ToString(CultureInfo.InvariantCulture));
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }
}
