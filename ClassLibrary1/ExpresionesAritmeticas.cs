namespace ClassLibrary1
{
    public  abstract class ExpresionesAritmeticas : BinaryExpression
    {
        public ExpresionesAritmeticas(int Location) : base(Location) { }

        public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
        {
            bool right = Right!.CheckSemantic(context, scope, errors);
            bool left = Left!.CheckSemantic(context, scope, errors);

            if (Right.Type != ExpressionType.Number || Left.Type != ExpressionType.Number)
            {
                errors.Add(new CompilingError(Location, ErrorCode.Invalid, "Expression must represent a number"));
                Type = ExpressionType.ErrorType;
                return false;
            }
            
            Type = ExpressionType.Number;
            return right && left;
        }
        
    }
}