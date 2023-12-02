
public class Add : BinaryExpression
{
    public Add(int Location) : base(Location){}

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
        Right!.Evaluate();                                     //debe guardarse las evaluaciones de las ramas en variables locales, de los contrario la referencia en la 
        double y = (double)Right.Value!;                       // recursividad puede hacer que devuelve un valores erroneos en las capas mas altas de la misma
        CheckTypes("+",Left.Type,Right.Type);
        Value = x+y;
    }

    public override string? ToString()
    {
        if (Value == null)
        {
            return String.Format("({0} + {1})", Left, Right);
        }
        return Value.ToString();
    }
}
