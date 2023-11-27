
public abstract class BinaryExpression : Expression
{
    public BinaryExpression(int Location) : base(Location) { }

    public Expression? Left { get; set; }

    public Expression? Right { get; set; }

    public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
    {
        bool right = Right!.CheckSemantic(context, scope, errors);
        bool left = Left!.CheckSemantic(context, scope, errors);

        if (Right.Type == ExpressionType.ErrorType || Left.Type == ExpressionType.ErrorType)
        {
            Type = ExpressionType.ErrorType;
            return false;
        }

        if (Right.Type != Left.Type)
        {
            errors.Add(new CompilingError(Location, ErrorCode.Invalid, "Differents Expressions Types"));
            Type = ExpressionType.ErrorType;
            return false;
        }
        return left && right;
    }
    public static void CheckTypes(string operation, ExpressionType left, ExpressionType right)
    {
        if (!(left == ExpressionType.Anytype || right == ExpressionType.Anytype))
        {
            if (left != right)
            {
                throw new Exception($"!SEMANTIC ERROR : Invalid expression: Can't operate {left} with {right} using {operation}");
            }
        }
    }

    //public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
    //{
    //    bool right = Right!.CheckSemantic(context, scope, errors);
    //    bool left = Left!.CheckSemantic(context, scope, errors);

    //    if (Right.Type != ExpressionType.Number || Left.Type != ExpressionType.Number)
    //    {
    //        errors.Add(new CompilingError(location, ErrorCode.Invalid, "We don't do that here... "));
    //        Type = ExpressionType.ErrorType;
    //        return false;
    //    }

    //    Type = ExpressionType.Number;
    //    return right && left;
    //}
}
public class OpAnd : BinaryExpression
{

    public override object? Value { get; set; }

    public OpAnd(int Location) : base(Location) { }

    public override void Evaluate()
    {
        Left.Evaluate();
        bool x = (bool)Left.Value!;
        Right.Evaluate();
        bool y = (bool)Left.Value!;
        CheckTypes("&", Left.Type, Right.Type);
        Value = x && y;
    }
    public override ExpressionType Type
    {
        get
        {
            return ExpressionType.Boolean;
        }
        set { }
    }
}
public class OpOr : BinaryExpression
{

    public override object? Value { get; set; }

    public OpOr(int Location) : base(Location) { }

    public override void Evaluate()
    {
        Left.Evaluate();
        bool x = (bool)Left.Value!;
        Right.Evaluate();
        bool y = (bool)Left.Value!;
        CheckTypes("|", Left.Type, Right.Type);
        Value = x || y;
    }
    public override ExpressionType Type
    {
        get
        {
            return ExpressionType.Boolean;
        }
        set { }
    }
}

public class Igualdad : BinaryExpression
{

    public override object? Value { get; set; }

    public Igualdad(int Location) : base(Location) { }


    public override void Evaluate()
    {
        Left.Evaluate(); Right.Evaluate();
        Value = ((double)Left.Value! == (double)Right.Value!);
    }

    public override ExpressionType Type
    {
        get
        {
            return ExpressionType.Boolean;
        }
        set { }
    }
}
public class Mayor : BinaryExpression
{

    public override object? Value { get; set; }

    public Mayor(int Location) : base(Location) { }


    public override void Evaluate()
    {
        Left.Evaluate(); Right.Evaluate();
        Value = (bool)((double)Left.Value! > (double)Right.Value!);
    }

    public override ExpressionType Type
    {
        get
        {
            return ExpressionType.Boolean;
        }
        set { }
    }
}
public class Menor : BinaryExpression
{

    public override object? Value { get; set; }

    public Menor(int Location) : base(Location) { }


    public override void Evaluate()
    {
        Left.Evaluate(); Right.Evaluate();
        Value = (bool)((double)Left.Value! < (double)Right.Value!);
    }

    public override ExpressionType Type
    {
        get
        {
            return ExpressionType.Boolean;
        }
        set { }
    }
}
