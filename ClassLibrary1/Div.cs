
public class Div : BinaryExpression
{
    public Div(int Location) : base(Location) { }

    public override ExpressionType Type
    {
        get
        {
            return ExpressionType.Number;
        }
        set { }
    }

    public override object? Value {get; set;}

    public override void Evaluate()
    {
        Left!.Evaluate();
        double x = (double)Left.Value!;
        Right!.Evaluate();
        double y = (double)Right.Value!;
        if (y==0) throw new Exception("! SEMANTIC ERROR : Cannot divide by zero");
        Value = x / y;
    }

    public override string? ToString()
    {
        if (Value == null)
        {
            return String.Format("({0} / {1})", Left, Right);
        }
        return Value.ToString();
    }
}
