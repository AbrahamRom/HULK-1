
// las expresiones atomicas son el nivel mas bajo de una expresion y por tanto poseen prioridad ante las demas operaciones
// entre ellas estan los numeros, los llamados de funciones y evaluaciones de variables, booleanos,literales...
public abstract class AtomExpression : Expression
{
    public AtomExpression(int Location) : base(Location){}

    public override bool CheckSemantic(Context context, Scope table, List<CompilingError> errors) => true;

    public override void Evaluate() { }

    public override string ToString() => String.Format("{0}",Value);
}
public class Boolean : AtomExpression                             // representation de los booleanos
{
    public  override object? Value { get; set; }

    public Boolean(bool value,int Location):base(Location)
    {
        this.Value = value;
    }

    public Boolean Not()
    {
        return new Boolean(!(bool)Value!,Location-1); // el metodo not devuelve la negacion del valor del booleano
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

    public StringLiteral(string value, int Location) : base(Location)          // representacion de una cadena de texto
    {
        this.Value = value;
    }
    public override ExpressionType Type
    {
        get
        {
            return ExpressionType.StringLiteral;
        }
        set { }
    }
}

