
public abstract class ASTNode
{
    public ASTNode(int Location)
    {
      this.Location   = Location;
    }

    public int Location {get; set;}
    
    public abstract bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors);    
}
