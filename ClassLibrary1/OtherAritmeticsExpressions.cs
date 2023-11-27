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

        public override ExpressionType Type
        {
            get
            {
                return ExpressionType.Number;
            }
            set { }
        }

        public override object? Value { get; set; }

        public override void Evaluate()
        {
            Left!.Evaluate();
            double x = (double)Left.Value!;
            Right!.Evaluate();
            double y = (double)Right.Value!;
            Value = Math.Pow(x,y);
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

        public override ExpressionType Type
        {
            get
            {
                return ExpressionType.Number;
            }
            set { }
        }

        public override object? Value { get; set; }

        public override void Evaluate()
        {
            Left!.Evaluate();
        double x = (double)Left.Value!;
        Right!.Evaluate();
        double y = (double)Right.Value!;
        Value = Math.Log(x, y);
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

    public class VariableReference : AtomExpression
    {
        public VariableReference(string identifier, VariableScope variables, bool IsFunct, int Location) : base(Location)
        {
            this.Identifier = identifier;
            this.IsFunction = IsFunct;
            if (IsFunct)
            {
                if (FunctionVariableScope.variables.Count == 0)
                {
                    var x = new VariableScope();
                    FunctionVariableScope.variables.Push(x);
                }
                VariableScope = FunctionVariableScope.variables.Peek();
                if (VariableScope.ContainsVariable(Identifier)) valor = VariableScope.GetVariableValue(Identifier);
            }
            else VariableScope = variables;

        }
        public string Identifier { get; set; }
          public bool IsFunction { get; set; }

        public override ExpressionType Type
        {
            get
            {
                return ExpressionType.Anytype;
            }
            set { }
        }
        public VariableScope VariableScope { get; set; }

        public object? valor = null;

        public override object? Value
        {
            get
            {
                //if (valor == null)
                // {
                if (VariableScope.ContainsVariable(Identifier))
                {
                    object x;
                    if (IsFunction)
                    {
                        x=FunctionVariableScope.variables.Peek().GetVariableValue(Identifier);
                    }
                    else { x = VariableScope.GetVariableValue(Identifier); }
                     
                    return x;// VariableScope.GetVariableValue(Identifier);
                }
                //else if (FunctionScope.ContainsFunction(Identifier))
                //{
                //    return FunctionScope.GetFunction;
                //}
                else if (FunctionVariableScope.variables.Count()!=0)
                {
                    FunctionVariableScope.variables.Pop();
                    return this.Value;
                }
                else
                {
                   throw new Exception($"Semantic error: Variable '{Identifier}' does not exist.");
                }

                // }

                //  return valor;
            }
            set { }
        }

    }
    public class FunctionReference : AtomExpression
    {
        public FunctionReference(int Location) : base(Location) { }
        public string Identifier { get; set; }
        public FunctionDefinition Definition { get; set; }
        public List<Expression> arguments;
        public override ExpressionType Type
        {
            get
            {
                return ExpressionType.Anytype;
            }
            set { }
        }
        public override void Evaluate()
        {
            Definition = FunctionScope.GetFunction(Identifier);
            Value = Definition.Invoke(arguments);
        }

        public override object? Value { get; set; }


    }



}
