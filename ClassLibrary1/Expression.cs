
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
    StringLiteral,
    Number,                              //diferentes tipos de expresiones con las que trabajo en el codigo
    Boolean,
    ErrorType
}
