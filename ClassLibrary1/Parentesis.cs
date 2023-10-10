using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
     class Parentesis : AtomExpression
    {
        public Parentesis(double value, int Location) : base(Location)
        {
            Value = value;
        }
        public override ExpressionType Type { get; set; }

        public override object? Value { get; set; }
    }
}
