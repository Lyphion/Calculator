using System.Globalization;
using System.Text;

namespace Calculator;

public interface INode
{
    double Evaluate();

    string ToString();

    static string Indent(string text, int amount = 3, bool last = true)
    {
        string[] strArray = text.Split('\n');
        var sb = new StringBuilder();

        sb.Append("+-").Append(new string(' ', amount - 2)).Append(strArray[0]).AppendLine();
        string spacing = new(' ', amount - 1);

        for (int i = 1; i < strArray.Length; i++)
            sb.Append(last ? ' ' : '|').Append(spacing).Append(strArray[i]).AppendLine();

        sb.Remove(sb.Length - 1, 1);
        return sb.ToString();
    }
}

public sealed record Literal(double Value) : INode
{
    public double Evaluate() => Value;

    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }
}

public sealed record UnaryOperation(OperationType Type, INode Member) : INode
{
    public double Evaluate()
    {
        return Type switch
        {
            OperationType.Negate => -Member.Evaluate(),
            OperationType.Sin => Math.Sin(Member.Evaluate()),
            OperationType.Cos => Math.Cos(Member.Evaluate()),
            OperationType.Tan => Math.Tan(Member.Evaluate()),
            OperationType.Sqrt => Math.Sqrt(Member.Evaluate()),
            _ => throw new ArgumentOutOfRangeException(nameof(Type), Type, "Unsupported operation type")
        };
    }

    public override string ToString()
    {
        return Type + "\n"
                    + INode.Indent(Member.ToString(), last: true);
    }
}

public sealed record BinaryOperation(OperationType Type, INode Left, INode Right) : INode
{
    public double Evaluate()
    {
        return Type switch
        {
            OperationType.Addition => Left.Evaluate() + Right.Evaluate(),
            OperationType.Subtraction => Left.Evaluate() - Right.Evaluate(),
            OperationType.Multiplication => Left.Evaluate() * Right.Evaluate(),
            OperationType.Division => Left.Evaluate() / Right.Evaluate(),
            OperationType.Modulus => Left.Evaluate() % Right.Evaluate(),
            OperationType.Power => Math.Pow(Left.Evaluate(), Right.Evaluate()),
            _ => throw new ArgumentOutOfRangeException(nameof(Type), Type, "Unsupported operation type")
        };
    }

    public override string ToString()
    {
        return Type + "\n"
                    + INode.Indent(Left.ToString(), last: false) + '\n'
                    + INode.Indent(Right.ToString(), last: true);
    }
}
