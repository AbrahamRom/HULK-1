
public abstract class Expression : ASTNode
{
    public Expression(int Location) : base (Location) { }

    public abstract ExpressionType Type { get; set; }

    public abstract object? Value { get; set; }

    public abstract void Evaluate();
}

public enum ExpressionType
{
    Anytype,
    Text,
    Number,
    Boolean,
    ErrorType
}
