
public abstract class ASTNode
{
    public ASTNode(int Location)
    {
      this.Location   = Location;
    }

    public int Location {get; set;}
    
    public abstract bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors);    
}
// de esta clase heredan todos los nodos del AST sus ramas son StatementNode y ExpressionNode
