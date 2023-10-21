﻿using System;
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

        public abstract Expression? Execute();

    }

    public class PrintStatementNode : StatementNode
    {
        public object? InsideParentesis { get; }

        public PrintStatementNode(int Location, Expression? insideof) : base(Location)
        {
            this.Location = Location;
            InsideParentesis = insideof;
        }

        public override Expression? Execute()
        {
            InsideParentesis.Evaluate();

            Console.WriteLine(InsideParentesis.Value);
            return InsideParentesis.Value;
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

        public override void Execute()
        {
            // Obtener el valor de la expresión
            Exp.Evaluate();
            // Declarar la variable y asignarle el valor
            DefVariables.AddVariable(identifier, Exp.Value);
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

        public override void Execute()
        {
            Condition.Evaluate();
            var conditionValue = Condition.Value;
            if (Convert.ToBoolean(conditionValue))
            {
                IfBody.Execute();
            }
            else
            {
                ElseBody.Execute();
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


        public override void Execute()
        {
            var function = new FunctionDefinition();
            function.Parameters = Parameters;
            function.Body = Body;
            FunctionScope.AddFunction(Identifier, function);
            //return Identifier; // Devuelve el nombre de la función declarada
            Console.WriteLine(Identifier);
        }

        public void AddParameter(string parameter)
        {
            this.Parameters.Add(parameter);
        }

        public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors) => true;

    }

    public class FunctionDefinition
    {
        public List<string> Parameters { get; set; }
        public StatementNode? Body { get; set; }

        public FunctionDefinition() { }

        //public void AddParameter(string parameter)
        //{
        //    this.Parameters.Add(parameter);
        //}

        public Expression? Invoke(List<Expression> arguments)
        {
            if (arguments.Count != Parameters.Count)
            {
                throw new Exception($"Semantic error: Function '{Parameters[0]}' receives {Parameters.Count} argument(s), but {arguments.Count} were given.");
            }

            // Establece los valores de los argumentos en el ámbito de variables
            for (int i = 0; i < Parameters.Count; i++)
            {
                string parameter = Parameters[i];
                Expression argument = arguments[i];
                argument.Evaluate();
                var value = argument.Value;
                FunctionVariableScope.AddVariable(parameter, value);
            }

            // Ejecuta el cuerpo de la función
              Body.Evaluate();
            var bo = Body;
            return Body;
            //// Obtiene el valor de retorno de la función
            //var returnValue = VariableScope.GetVariableValue(FunctionScope.ReturnVariable);

            //// Limpia las variables del ámbito de variables
            //VariableScope.ClearVariables();

            //return returnValue;
        }


    }

    public static class FunctionScope
    {
        public static Dictionary<string, FunctionDefinition> functions = new Dictionary<string, FunctionDefinition>();
        //  public static string ReturnVariable { get; } = "__return__";

        public static void AddFunction(string identifier, FunctionDefinition function)
        {
            functions.Add(identifier, function);
        }

        public static FunctionDefinition GetFunction(string identifier)
        {
            if (functions.ContainsKey(identifier))
            {
                return functions[identifier];
            }
            else
            {
                throw new Exception($"Semantic error: Function '{identifier}' does not exist.");
            }
        }

        public static bool ContainsFunction(string identifier)
        {
            return functions.ContainsKey(identifier);
        }
    }
}