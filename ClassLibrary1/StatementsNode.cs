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

        public abstract void Execute();

    }

    public class PrintStatementNode : StatementNode
    {
        public Expression? InsideParentesis { get; }

        public PrintStatementNode(int Location, Expression? insideof) : base(Location)
        {
            this.Location = Location;
            InsideParentesis = insideof;
        }

        public override void Execute()
        {
            InsideParentesis.Evaluate();

            Console.WriteLine(InsideParentesis.Value);
        }

        public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors) => true;

    }

    public class VariableDeclarationNode : StatementNode
    {
        public VariableDeclarationNode(string identifier, Expression? exp, VariableScope variables, int Location) : base(Location)
        {
            this.Location = Location;
            this.Exp=exp;
            this.identifier=identifier;
            this.DefVariables=variables;
        }
        public string identifier { get; set; }
        public Expression? Exp { get; set; }
        public  VariableScope DefVariables { get; set; }

        public override void Execute()
        {
            // Obtener el valor de la expresión
            Exp.Evaluate();
            // Declarar la variable y asignarle el valor
            DefVariables.AddVariable(identifier, Exp.Value);
        }

        public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors) => true;
    }
}
