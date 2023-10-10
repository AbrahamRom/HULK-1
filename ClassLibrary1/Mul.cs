﻿
public class Mul : BinaryExpression
{
    public Mul(int Location) : base(Location) { }

    public override ExpressionType Type {get; set;}

    public override object? Value {get; set;}

    public override void Evaluate()
    {
        Right!.Evaluate();
        Left!.Evaluate();
        
        Value = (double)Right.Value! * (double)Left.Value!;
    }

    public override string? ToString()
    {
        if (Value == null)
        {
            return String.Format("({0} * {1})", Left, Right);
        }
        return Value.ToString();
    }
}
