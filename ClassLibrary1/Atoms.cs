

public abstract class AtomExpression : Expression
{
    public AtomExpression(int Location) : base(Location){}

    public override bool CheckSemantic(Context context, Scope table, List<CompilingError> errors) => true;

    public override void Evaluate() { }

    public override string ToString() => String.Format("{0}",Value);
}
