

public abstract class AtomExpression : Expression
{
    public AtomExpression(int Location) : base(Location){}

    public override bool CheckSemantic(Context context, Scope table, List<CompilingError> errors) => true;

    public override void Evaluate() { }

    public override string ToString() => String.Format("{0}",Value);
}
public class Boolean : AtomExpression
{
    public  override object? Value { get; set; }

    public Boolean(bool value,int Location):base(Location)
    {
        this.Value = value;
    }

    public Boolean Not()
    {
        return new Boolean(!(bool)Value!,Location-1);
    }

    public override ExpressionType Type
    {
        get
        {
            return ExpressionType.Number;
        }
        set { }
    }
}
public class StringLiteral : AtomExpression
{
    public override object? Value { get; set; }

    public StringLiteral(string value, int Location) : base(Location)
    {
        this.Value = value;
    }
    public override ExpressionType Type
    {
        get
        {
            return ExpressionType.Number;
        }
        set { }
    }
}

