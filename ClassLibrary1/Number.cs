
public class Number : AtomExpression
{
    public Number(double value, int Location) : base(Location)
    {
        Value = value;
    }

    public override ExpressionType Type
    {
        get
        {
            return ExpressionType.Number;
        }
        set { }
    }

    public override object? Value { get; set; }

    public bool IsInt
    {
        get
        {
            int a;
            return int.TryParse(Value!.ToString(), out a);
        }
    }
}
