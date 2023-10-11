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


}
