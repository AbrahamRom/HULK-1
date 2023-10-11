using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class Potencia : BinaryExpression
    {
        public Potencia(int Location) : base(Location) { }

        public override ExpressionType Type { get; set; }

        public override object? Value { get; set; }

        public override void Evaluate()
        {
            Right!.Evaluate();
            Left!.Evaluate();

            Value = Math.Pow((double)Left.Value!, (double)Right.Value!);
        }

        public override string? ToString()
        {
            if (Value == null)
            {
                return String.Format("({0} ^ {1})", Left, Right);
            }
            return Value.ToString();
        }
    }

    //public class NumberPI : AtomExpression
    //{
    //    public NumberPI (int Location) : base(Location) { }

    //    public override object? Value { get { return Math.PI; } set { } }

    //    public override ExpressionType Type
    //    {
    //        get { return ExpressionType.Number; }
    //        set { }
    //    }
    //}

    public class Coseno : AtomExpression
    {
        public Coseno(int Location) : base(Location) { }

        public override object? Value { get; set; }
        public Expression? InsideParentesis { get; set; }

        public override ExpressionType Type
        {
            get
            {
                return ExpressionType.Number;
            }
            set { }
        }

        public override void Evaluate()
        {
            InsideParentesis!.Evaluate();

            Value = Math.Cos((double)InsideParentesis.Value!);
        }
        public Expression? NegateCos()
        {
            Evaluate();
            double x = (double)Value!;
            return new Number(-1 * x, Location);
        }
    }
    public class Seno : AtomExpression
    {
        public Seno(int Location) : base(Location) { }

        public override object? Value { get; set; }
        public Expression? InsideParentesis { get; set; }

        public override ExpressionType Type
        {
            get
            {
                return ExpressionType.Number;
            }
            set { }
        }

        public override void Evaluate()
        {
            InsideParentesis!.Evaluate();

            Value = Math.Sin((double)InsideParentesis.Value!);
        }
        public Expression? NegateSen()
        {
            Evaluate();
            double x = (double)Value!;
            return new Number(-1 * x, Location);
        }
    }

    public class Logaritmo : BinaryExpression
    {
        public Logaritmo(int Location) : base(Location) { }

        public override ExpressionType Type { get; set; }

        public override object? Value { get; set; }

        public override void Evaluate()
        {
            Right!.Evaluate();
            Left!.Evaluate();

            Value = Math.Log((double)Left.Value!, (double)Right.Value!);
        }

        public override string? ToString()
        {
            if (Value == null)
            {
                return String.Format("(log({0},{1}))", Left, Right);
            }
            return Value.ToString();
        }

    }

}
