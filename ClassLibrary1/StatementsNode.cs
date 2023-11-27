using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public abstract class StatementNode : ASTNode
    {

        public StatementNode(int Location) : base(Location)
        {
            this.Location = Location;
        }

        public abstract object? Execute();

    }

    public class PrintStatementNode : StatementNode
    {
        public Expression? InsideParentesis { get; }

        public PrintStatementNode(int Location, Expression? insideof) : base(Location)
        {
            this.Location = Location;
            InsideParentesis = insideof;
        }

        public override object? Execute()
        {
            InsideParentesis.Evaluate();
            var x = InsideParentesis.Value;
            Console.WriteLine(x);
            return x;
        }

        public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors) => true;

    }

    public class VariableDeclarationNode : StatementNode
    {
        public VariableDeclarationNode(string identifier, Expression? exp, VariableScope variables, int Location) : base(Location)
        {
            this.Location = Location;
            this.Exp = exp;
            this.identifier = identifier;
            this.DefVariables = variables;
        }
        public string identifier { get; set; }
        public Expression? Exp { get; set; }
        public VariableScope DefVariables { get; set; }

        public override object? Execute()
        {
            // Obtener el valor de la expresión
            Exp.Evaluate();
            // Declarar la variable y asignarle el valor
            DefVariables.AddVariable(identifier, Exp.Value);
            return null;
        }

        public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors) => true;
    }

    public class IfStatementNode : StatementNode
    {
        public Expression? Condition { get; set; }
        public StatementNode? IfBody { get; set; }
        public StatementNode? ElseBody { get; set; }

        public IfStatementNode(int Location) : base(Location)
        {
            this.Location = Location;
        }

        public override object? Execute()
        {
            Condition.Evaluate();
            var conditionValue = Condition.Value;
            if (conditionValue == null) throw new Exception("! SEMANTIC ERROR : If-ELSE expressions must have a boolean condition");
            if (Convert.ToBoolean(conditionValue))
            {
                return IfBody.Execute();
            }
            else
            {
                return ElseBody.Execute();
            }
        }
        public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors) => true;
    }

    public class FunctionDeclarationNode : StatementNode
    {
        public string Identifier { get; set; }
        public List<string> Parameters { get; set; }
        public StatementNode? Body { get; set; }

        public FunctionDeclarationNode(int Location) : base(Location)
        {
            Parameters = new List<string>();
        }


        public override object? Execute()
        {
            var function = new FunctionDefinition();
            function.FunctionName = Identifier;
            function.Parameters = Parameters;
            function.Body = Body;
            FunctionScope.AddFunctName(Identifier);
            FunctionScope.AddFunction(Identifier, function);
            //return Identifier; // Devuelve el nombre de la función declarada
            Console.WriteLine(Identifier);
            return null;
        }

        public void AddParameter(string parameter)
        {
            this.Parameters.Add(parameter);
        }

        public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors) => true;

    }

    public class FunctionDefinition
    {
        public string FunctionName { get; set; }
        public List<string> Parameters { get; set; }
        public StatementNode? Body { get; set; }

        //public VariableScope VariablesInFuntion { get; set; }

        public FunctionDefinition() { }

        public object? Invoke(List<Expression> arguments)
        {
            if (arguments.Count != Parameters.Count)
            {
                throw new Exception($"!SEMANTIC ERROR : Function {FunctionName} does not have {arguments.Count} parameters but {Parameters.Count} parameters");
            }

            // Establece los valores de los argumentos en el ámbito de variables
            for (int i = 0; i < Parameters.Count; i++)
            {

                string parameter = Parameters[i];
                Expression argument = arguments[i];
                argument.Evaluate();
                var value = argument.Value;
                if (FunctionVariableScope.variables.Count == 0)
                {
                    var x = new VariableScope();
                    FunctionVariableScope.variables.Push(x);
                }
                //if (!FunctionVariableScope.ContainsVariable(parameter))
                //{
                    FunctionVariableScope.AddVariable(parameter, value);//error
                //}
            }

            object? exp = Body.Execute();

            // Limpia las variables del ámbito de variables
            // FunctionVariableScope.ClearVariables();
            //agregar pop
            FunctionVariableScope.variables.Pop();

            if (FunctionVariableScope.variables.Count() == 0 && FunctionVariableScope.CountOverFlow > 0)
            {
                Console.WriteLine(exp.ToString());
            }

            return exp;

            // Ejecuta el cuerpo de la función
            //  Body.Evaluate();
            //var bo = Body;
            //return Body;
            //// Obtiene el valor de retorno de la función
            //var returnValue = VariableScope.GetVariableValue(FunctionScope.ReturnVariable);



            //return returnValue;
        }


    }

    public class ExpStatement : StatementNode
    {
        public Expression? exp { get; }
        public ExpStatement(int Location, Expression? exp) : base(Location)
        {
            this.Location = Location;
            this.exp = exp;
        }

        public override object? Execute()
        {
            exp.Evaluate();
            var x = exp.Value;
            return x;
        }
        public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors) => true;
    }

    public static class FunctionScope
    {
        public static Dictionary<string, FunctionDefinition> functions = new Dictionary<string, FunctionDefinition>();
        //  public static string ReturnVariable { get; } = "__return__";
        public static List<string> Identificadores = new List<string>();

        public static void AddFunction(string identifier, FunctionDefinition function)
        {
            functions.Add(identifier, function);
        }
        public static void AddFunctName(string iden)
        {
            if (!Identificadores.Contains(iden))
            {
                Identificadores.Add(iden);
            }
        }

        public static FunctionDefinition GetFunction(string identifier)
        {
            if (functions.ContainsKey(identifier))
            {
                return functions[identifier];
            }
            else
            {
                throw new Exception($"!SEMANTIC ERROR : Function {identifier} is not defined");
            }
        }

        public static bool ContainsFunction(string identifier)
        {
            return functions.ContainsKey(identifier);
        }
        public static bool ContainsFunctName(string identifier)
        {
            return Identificadores.Contains(identifier);
        }
    }
    //class FunctionScopeTry
    //{
    //    public static Dictionary<string, FunctionDeclarationNode> Functions = new Dictionary<string, FunctionDeclarationNode>();

    //    public static Dictionary<string, object> Variables = new Dictionary<string, object>();

    //}
}
