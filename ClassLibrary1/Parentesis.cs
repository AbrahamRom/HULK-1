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
            //var parser = new Parser(stream);
            //var exp = parser.ParseExpression();
            // exp.Evaluate();
            // exp.Value;
        }
        public override ExpressionType Type { get; set; }

        public override object? Value { get; set; }
        public Expression? InsideParentesis { get; set; }

        public override void Evaluate()
        {
            InsideParentesis!.Evaluate();

            Value = (double)InsideParentesis.Value!;
        }
        public Expression? NegateParentesis()
        {
            InsideParentesis.Evaluate();
            double x = (double)InsideParentesis.Value!;
            return new Number(-1*x,Location);
        }
    }
}
