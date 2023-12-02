using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    class Parentesis : AtomExpression
    {
        public Parentesis(int Location) : base(Location)
        {
        }
        public override ExpressionType Type
        {
            get
            {
                if (InsideParentesis == null) return ExpressionType.Anytype;
                else return InsideParentesis.Type;
            }
            set { }
        }

        public override object? Value { get; set; }
        public Expression? InsideParentesis { get; set; }

        public override void Evaluate()
        {
            InsideParentesis!.Evaluate();


            if (double.TryParse(InsideParentesis.Value!.ToString(), out double result))
            {
                Value = result;
            }
            else if (bool.TryParse(InsideParentesis.Value!.ToString(), out bool boolResult))
            {
                Value = boolResult;
            }
            else
            {
                Value = null;
            }

        }
        public Expression? NegateParentesis()
        {
            InsideParentesis.Evaluate();
            double x = (double)InsideParentesis.Value!;
            return new Number(-1 * x, Location);
        }
    }
}
