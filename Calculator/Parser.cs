using System.Globalization;

namespace Calculator;

public static class Parser
{
    public static INode Parse(List<Token> tokens)
    {
        var queue = new Queue<Token>(tokens);

        var node = ParseE(queue);
        if (queue.Count != 0)
            throw new Exception($"Unexpected tokens: {string.Join(", ", queue)}");

        return node;
    }

    private static INode ParseE(Queue<Token> tokens)
    {
        if (tokens.Count == 0)
            throw new Exception("Unexpected end of tokens");
        
        var current = tokens.Peek();
        switch (current.Type)
        {
            case TokenType.Function:
            case TokenType.LeftParenthesis:
            case TokenType.Number:
            case TokenType.Constant:
            case TokenType.Plus:
            case TokenType.Minus:
                var left = ParseT(tokens);
                return ParseEp(left, tokens);

            default:
                throw new Exception($"Unexpected token: {current.Type}");
        }
    }

    private static INode ParseEp(INode left, Queue<Token> tokens)
    {
        if (tokens.Count == 0)
            return left;

        var current = tokens.Peek();
        switch (current.Type)
        {
            case TokenType.RightParenthesis:
                return left;

            case TokenType.Plus:
            case TokenType.Minus:
                ConsumeToken(tokens);
                var right = ParseT(tokens);
                var operation = current.Type switch
                {
                    TokenType.Plus => OperationType.Addition,
                    _ => OperationType.Subtraction
                };

                var result = new BinaryOperation(operation, left, right);
                return ParseEp(result, tokens);

            default:
                throw new Exception($"Unexpected token: {current.Type}");
        }
    }

    private static INode ParseT(Queue<Token> tokens)
    {
        if (tokens.Count == 0)
            throw new Exception("Unexpected end of tokens");
        
        var current = tokens.Peek();

        switch (current.Type)
        {
            case TokenType.Function:
            case TokenType.LeftParenthesis:
            case TokenType.Number:
            case TokenType.Constant:
            case TokenType.Plus:
            case TokenType.Minus:
                var left = ParseU(tokens);
                return ParseTp(left, tokens);

            default:
                throw new Exception($"Unexpected token: {current.Type}");
        }
    }

    private static INode ParseTp(INode left, Queue<Token> tokens)
    {
        if (tokens.Count == 0)
            return left;

        var current = tokens.Peek();
        switch (current.Type)
        {
            case TokenType.RightParenthesis:
            case TokenType.Plus:
            case TokenType.Minus:
                return left;

            case TokenType.Star:
            case TokenType.Slash:
            case TokenType.Modulus:
                ConsumeToken(tokens);
                var right = ParseU(tokens);
                var operation = current.Type switch
                {
                    TokenType.Star => OperationType.Multiplication,
                    TokenType.Slash => OperationType.Division,
                    _ => OperationType.Modulus
                };

                var result = new BinaryOperation(operation, left, right);
                return ParseTp(result, tokens);

            default:
                throw new Exception($"Unexpected token: {current.Type}");
        }
    }

    private static INode ParseU(Queue<Token> tokens)
    {
        if (tokens.Count == 0)
            throw new Exception("Unexpected end of tokens");
        
        var current = tokens.Peek();

        switch (current.Type)
        {
            case TokenType.Function:
            case TokenType.LeftParenthesis:
            case TokenType.Number:
            case TokenType.Constant:
            case TokenType.Plus:
            case TokenType.Minus:
                var left = ParseF(tokens);
                return ParseUp(left, tokens);

            default:
                throw new Exception($"Unexpected token: {current.Type}");
        }
    }

    private static INode ParseUp(INode left, Queue<Token> tokens)
    {
        if (tokens.Count == 0)
            return left;

        var current = tokens.Peek();
        switch (current.Type)
        {
            case TokenType.RightParenthesis:
            case TokenType.Plus:
            case TokenType.Minus:
            case TokenType.Star:
            case TokenType.Slash:
            case TokenType.Modulus:
                return left;

            case TokenType.Power:
                ConsumeToken(tokens);
                var right = ParseF(tokens);
                const OperationType operation = OperationType.Power;

                var result = new BinaryOperation(operation, left, right);
                return ParseUp(result, tokens);

            default:
                throw new Exception($"Unexpected token: {current.Type}");
        }
    }

    private static INode ParseF(Queue<Token> tokens)
    {
        if (tokens.Count == 0)
            throw new Exception("Unexpected end of tokens");

        var current = ConsumeToken(tokens);
        switch (current.Type)
        {
            case TokenType.Function:
            {
                var result = ParseF(tokens);
                var operation = current.Value.ToLower() switch
                {
                    "sin" => OperationType.Sin,
                    "cos" => OperationType.Cos,
                    "tan" => OperationType.Tan,
                    "sqrt" => OperationType.Sqrt,
                    _ => throw new Exception($"Invalid function type {current.Value}")
                };

                return new UnaryOperation(operation, result);
            }

            case TokenType.Plus:
                return ParseF(tokens);

            case TokenType.Minus:
            {
                var result = ParseF(tokens);
                const OperationType operation = OperationType.Negate;
                return new UnaryOperation(operation, result);
            }

            case TokenType.LeftParenthesis:
            {
                var result = ParseE(tokens);
                _ = AcceptToken(tokens, TokenType.RightParenthesis);
                return result;
            }

            case TokenType.Number:
                return new Literal(double.Parse(current.Value, CultureInfo.InvariantCulture));

            case TokenType.Constant:
                double value = current.Value.ToLower() switch
                {
                    "e" => Math.E,
                    "pi" => Math.PI,
                    "phi" => 1.618033988749895,
                    "tau" => Math.Tau,
                    _ => throw new Exception($"Invalid constant type {current.Value}")
                };
                return new Literal(value);
            
            default:
                throw new Exception($"Unexpected token: {current.Type}");
        }
    }

    private static Token ConsumeToken(Queue<Token> tokens)
    {
        if (tokens.Count == 0)
            throw new Exception("Unexpected end of tokens");

        return tokens.Dequeue();
    }

    private static Token AcceptToken(Queue<Token> tokens, TokenType expectedType)
    {
        if (tokens.Count == 0)
            throw new Exception("Unexpected end of tokens");

        var token = tokens.Dequeue();
        if (token.Type != expectedType)
            throw new Exception($"Expected token {expectedType} but found {token.Type}");
        return token;
    }
}
