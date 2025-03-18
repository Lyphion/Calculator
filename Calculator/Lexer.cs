using System.Text.RegularExpressions;

namespace Calculator;

public static partial class Lexer
{
    public static readonly (TokenType type, Regex pattern)[] Pattern =
    [
        (TokenType.Number, NumberRegex()),
        (TokenType.Constant, ConstantRegex()),
        (TokenType.Plus, PlusRegex()),
        (TokenType.Minus, MinusRegex()),
        (TokenType.Star, MultiplyRegex()),
        (TokenType.Slash, DivideRegex()),
        (TokenType.Modulus, ModulusRegex()),
        (TokenType.Power, PowerRegex()),
        (TokenType.LeftParenthesis, LeftParenthesisRegex()),
        (TokenType.RightParenthesis, RightParenthesisRegex()),
        (TokenType.Function, FunctionRegex())
    ];

    public static List<Token> Tokenize(string source)
    {
        source = WhiteSpaceRegex().Replace(source, "");

        var tokens = new List<Token>();

        int position = 0;
        while (position < source.Length)
        {
            bool success = false;
            foreach (var (type, pattern) in Pattern)
            {
                var match = pattern.Match(source, position, source.Length - position);
                if (!match.Success || match.Index != position)
                    continue;

                tokens.Add(new Token(type, match.Value));
                position += match.Length;
                success = true;
                break;
            }

            if (!success)
                throw new ArgumentException($"Invalid character {source[position]}");
        }

        return tokens;
    }

    #region Regex

    [GeneratedRegex("\\s+")]
    private static partial Regex WhiteSpaceRegex();

    [GeneratedRegex(@"\d*\.?\d+((E|e)(\+|\-)?\d+)?")]
    private static partial Regex NumberRegex();

    [GeneratedRegex("e|(pi)|(phi)|(tau)", RegexOptions.IgnoreCase)]
    private static partial Regex ConstantRegex();
    
    [GeneratedRegex(@"\+")]
    private static partial Regex PlusRegex();

    [GeneratedRegex("-")]
    private static partial Regex MinusRegex();

    [GeneratedRegex(@"\*")]
    private static partial Regex MultiplyRegex();

    [GeneratedRegex("/")]
    private static partial Regex DivideRegex();

    [GeneratedRegex("%")]
    private static partial Regex ModulusRegex();

    [GeneratedRegex(@"\^")]
    private static partial Regex PowerRegex();

    [GeneratedRegex(@"\(")]
    private static partial Regex LeftParenthesisRegex();

    [GeneratedRegex(@"\)")]
    private static partial Regex RightParenthesisRegex();

    [GeneratedRegex("sin|cos|tan|sqrt", RegexOptions.IgnoreCase)]
    private static partial Regex FunctionRegex();

    #endregion
}
