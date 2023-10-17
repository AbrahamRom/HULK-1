using ClassLibrary1;
public class Parser
{
    public Parser(TokenStream stream)
    {
        Stream = stream;
        DefVariables = new VariableScope();
    }

    public TokenStream Stream { get; private set; }

    public VariableScope DefVariables { get; private set; }

    public StatementNode? ParseStament()
    {
        StatementNode? Statement = ParsePrint();
        if (Statement != null) return Statement;

        Statement = ParseFunctionDeclaration();
        if (Statement != null) return Statement;

        Statement = ParseIfElseStatement();
        if (Statement != null) return Statement;

        ParseParseVariableDeclaration();
        if (Stream.FindToken(TiposDToken.In))
        {
            Stream.NextToken();
            Statement = ParseStament();
            if (Statement != null) return Statement;
        }

        return null;
    }

    private StatementNode? ParseFunctionDeclaration()
    {
        if (!Stream.IsToken(TiposDToken.Function)) return null;
        var function = new FunctionDeclarationNode(Stream.Position - 3);
        if (!Stream.FindToken(TiposDToken.Identificador)) return null;
        function.Identifier = Stream.CurrentToken().StringToken;
        if (!Stream.FindToken(TiposDToken.OpenParentesis)) return null;

        int count = 1;
        while (count == 1)
        {
            if (!Stream.FindToken(TiposDToken.Identificador)) return null;
            function.AddParameter(Stream.CurrentToken().StringToken);
            if (!Stream.FindToken(TiposDToken.Coma)) count = 0;
        }
        if (!Stream.FindToken(TiposDToken.CloseParentesis)) return null;
        if (!Stream.FindToken(TiposDToken.FunctionDef)) return null;

        Stream.NextToken();

        function.Body = ParseExpression();
        return function;

    }

    private StatementNode? ParseIfElseStatement()
    {
        if (!Stream.IsToken(TiposDToken.If)) return null;
        if (!Stream.FindToken(TiposDToken.OpenParentesis)) return null;
        Stream.NextToken();
        var statement = new IfStatementNode(Stream.Position - 2);
        statement.Condition = ParseExpressionBoolean();
        //  if (!Stream.FindToken(TiposDToken.CloseParentesis)) return null;
        Stream.NextToken();
        statement.IfBody = ParseStament();
        if (!Stream.FindToken(TiposDToken.Else)) return null;
        Stream.NextToken();
        statement.ElseBody = ParseStament();
        return statement;
    }

    private StatementNode? ParsePrint()
    {
        if (!Stream.IsToken(TiposDToken.Print)) return null;
        if (!Stream.FindToken(TiposDToken.OpenParentesis)) return null;
        Stream.NextToken();
        var exp = ParseExpression();
        var Statement = new PrintStatementNode(Stream.Position - 2, exp);
        return Statement;
    }
    private void ParseParseVariableDeclaration()
    {
        if (!Stream.IsToken(TiposDToken.Let)) return;
        int count = 1;
        while (count == 1)
        { // revisar si se declaran 2 variables con el mismo nombre
            if (!Stream.FindToken(TiposDToken.Identificador)) return;
            if (!Stream.FindToken(TiposDToken.Assign)) return;
            Stream.MoveBack(1);
            string variable = Stream.CurrentToken().StringToken;
            int location = Stream.Position;
            Stream.NextToken(2);
            var exp = ParseExpression();
            var DeclVar = new VariableDeclarationNode(variable, exp, DefVariables, location);
            DeclVar.Execute();
            if (!Stream.FindToken(TiposDToken.Coma)) count = 0;

        }
    }
    public Expression? ParseExpression()
    {
        var exp = ParseExpressionBoolean();
        if (exp != null) return exp;

        exp = ParseExpressionLv1();
        if (exp != null) return exp;

        return null;
    }
    private Expression? ParseExpressionBoolean()
    {
        return ParseExpBooleanLv1();
    }
    private Expression? ParseExpBooleanLv1()
    {
        Expression? left = ParseExpBooleanLv2();
        Expression? exp = ParseExpBooleanLv1_(left);
        if (Stream.FindToken(TiposDToken.CloseParentesis))
        {
            exp = ParseExpBooleanLv1_(exp);
        }
        return exp;

    }
    private Expression? ParseExpBooleanLv1_(Expression? left)
    {
        Expression? exp = ParseOR(left);
        if (exp != null) return exp;
        return left;
    }
    private Expression? ParseExpBooleanLv2()
    {
        Expression? newLeft = ParseExpBooleanLv3();
        return ParseExpBooleanLv2_(newLeft);
    }
    private Expression? ParseExpBooleanLv2_(Expression? left)
    {
        Expression? exp = ParseAnd(left);
        if (exp != null) return exp;
        return left;
    }
    private Expression? ParseExpBooleanLv3()
    {
        Expression? exp = ParseString();
        if (exp != null) return exp;

        exp = ParseBoolean();
        if (exp != null) return exp;

        exp = ParseParentesis();
        if (exp != null) return exp;


        Expression? left = ParseExpressionLv1();
        exp = ParseBooleanOP(left);
        if (exp != null) return exp;


        //exp = ParseVariableReference();
        //if (exp != null) return exp;

        return null;
    }

    private Expression? ParseString()
    {
        //var x =  Stream.CurrentToken().StringToken;
        if (Stream.IsToken(TiposDToken.StringLiteral))
        {
            return new StringLiteral(Stream.CurrentToken().StringToken.Substring(1, Stream.CurrentToken().StringToken.Length - 2), Stream.Position);
        }
        return null;
    }

    private Expression? ParseBooleanOP(Expression? left)
    {
        if (left == null) return null;
        if (Stream.FindToken(TiposDToken.Igual))
        {
            var exp = new Igualdad(Stream.Position);
            exp.Left = left;
            Stream.NextToken();
            Expression? right = ParseExpressionLv1();
            if (right == null) return null;
            exp.Right = right;
            return exp;
        }
        else if (Stream.FindToken(TiposDToken.Mayorq))
        {
            var exp = new Mayor(Stream.Position);
            exp.Left = left;
            Stream.NextToken();
            Expression? right = ParseExpressionLv1();
            if (right == null) return null;
            exp.Right = right;
            return exp;
        }
        else if (Stream.FindToken(TiposDToken.Menorq))
        {
            var exp = new Menor(Stream.Position);
            exp.Left = left;
            Stream.NextToken();
            Expression? right = ParseExpressionLv1();
            if (right == null) return null;
            exp.Right = right;
            return exp;
        }
        else return left;

    }

    private Expression? ParseBoolean()
    {
        if (Stream.IsToken(TiposDToken.Bool))
        {
            return new Boolean(bool.Parse(Stream.CurrentToken().StringToken), Stream.Position);
        }
        return null;
    }

    private Expression? ParseOR(Expression? left)
    {
        var Or = new OpOr(Stream.Position);
        if (left == null || !Stream.FindToken(TiposDToken.Or)) return null;
        Or.Left = left;
        Stream.NextToken();
        Expression? right = ParseExpBooleanLv2();
        if (right == null) return null;
        Or.Right = right;
        return Or;
    }
    private Expression? ParseAnd(Expression? left)
    {
        var And = new OpAnd(Stream.Position);
        if (left == null || !Stream.FindToken(TiposDToken.And)) return null;
        And.Left = left;
        Stream.NextToken();
        Expression? right = ParseExpBooleanLv3();
        if (right == null) return null;
        And.Right = right;
        return And;
    }

    private Expression? ParseExpressionLv1()
    {
        Expression? newLeft = ParseExpressionLv2();
        Expression? exp = ParseExpressionLv1_(newLeft);
        if (Stream.FindToken(TiposDToken.CloseParentesis))
        {
            exp = ParseExpressionLv1_(exp);
        }
        return exp;
    }

    private Expression? ParseExpressionLv1_(Expression? left)
    {
        Expression? exp = ParseAdd(left);
        if (exp != null) return exp;

        exp = ParseSub(left);
        if (exp != null) return exp;

        return left;
    }

    private Expression? ParseExpressionLv2()
    {
        Expression? newLeft = ParseExpressionLv3();
        return ParseExpressionLv2_(newLeft);
    }

    private Expression? ParseExpressionLv2_(Expression? left)
    {
        Expression? exp = ParseMul(left);
        if (exp != null) return exp;

        exp = ParseDiv(left);
        if (exp != null) return exp;

        return left;
    }
    private Expression? ParseExpressionLv3()
    {
        Expression? newLeft = ParseExpressionLv4();
        return ParseExpressionLv3_(newLeft);
    }
    private Expression? ParseExpressionLv3_(Expression? left)
    {
        Expression? exp = ParsePotencia(left);
        if (exp != null) return exp;
        return left;
    }

    private Expression? ParseExpressionLv4()
    {
        Expression? exp = ParseNumber();
        if (exp != null) return exp;

        exp = ParsePI();
        if (exp != null) return exp;

        exp = ParseVariableReference();
        if (exp != null) return exp;

        exp = ParseFunctionReference();
        if (exp != null) return exp;

        exp = ParseCoseno();
        if (exp != null) return exp;

        exp = ParseSeno();
        if (exp != null) return exp;

        exp = ParseLog();
        if (exp != null) return exp;

        exp = ParseParentesis();
        if (exp != null) return exp;
        return null;
    }

    private Expression? ParseFunctionReference()
    {
        if (!Stream.IsToken(TiposDToken.Identificador) || !FunctionScope.ContainsFunction(Stream.CurrentToken().StringToken)) return null;
        var identificador = Stream.CurrentToken().StringToken;
        var function = FunctionScope.GetFunction(Stream.CurrentToken().StringToken);
        if (!Stream.FindToken(TiposDToken.OpenParentesis)) return null;
        var arguments = ParseArgumentsFunc();
        return function.Invoke(arguments, DefVariables);

    }
    private List<Expression> ParseArgumentsFunc()
    {
        var Arguments = new List<Expression>();
        int count = 1;
        while (count == 1)
        {
            Stream.NextToken();
            Arguments.Add(ParseExpression());
            if (!Stream.FindToken(TiposDToken.Coma)) count = 0;

        }
        Stream.NextToken();//revisar
        return Arguments;
    }

    private Expression? ParseAdd(Expression? left)
    {
        Add sum = new Add(Stream.Position);

        if (left == null || !Stream.FindToken(TiposDToken.OpSuma)) return null;

        sum.Left = left;
        Stream.NextToken();
        Expression? right = ParseExpressionLv2();
        if (right == null)
        {
            Stream.MoveBack(2);
            return null;
        }
        sum.Right = right;

        return ParseExpressionLv1_(sum);
    }

    private Expression? ParseSub(Expression? left)
    {
        Sub sub = new Sub(Stream.Position);

        if (left == null || !Stream.FindToken(TiposDToken.OpResta)) return null;

        sub.Left = left;
        Stream.NextToken();
        Expression? right = ParseExpressionLv2();
        if (right == null)
        {
            Stream.MoveBack(2);
            return null;
        }
        sub.Right = right;

        return ParseExpressionLv1_(sub);
    }

    private Expression? ParseMul(Expression? left)
    {
        Mul mul = new Mul(Stream.Position);

        if (left == null || !Stream.FindToken(TiposDToken.OpMultiply)) return null;

        mul.Left = left;
        Stream.NextToken();
        Expression? right = ParseExpressionLv3();
        if (right == null)
        {
            Stream.MoveBack(2);
            return null;
        }
        mul.Right = right;

        return ParseExpressionLv2_(mul);
    }

    private Expression? ParseDiv(Expression? left)
    {
        Div div = new Div(Stream.Position);

        if (left == null || !Stream.FindToken(TiposDToken.OpDivide)) return null;

        div.Left = left;
        Stream.NextToken();
        Expression? right = ParseExpressionLv3();
        if (right == null)
        {
            Stream.MoveBack(2);
            return null;
        }
        div.Right = right;

        return ParseExpressionLv2_(div);
    }
    private Expression? ParsePotencia(Expression? left)
    {
        Potencia pot = new Potencia(Stream.Position);
        if (left == null || !Stream.FindToken(TiposDToken.Potencia)) return null;
        pot.Left = left;
        Stream.NextToken();
        Expression? right = ParseExpressionLv4();
        if (right == null)
        {
            Stream.MoveBack(2);
            return null;
        }
        pot.Right = right;

        return ParseExpressionLv3_(pot);
    }

    private Expression? ParseNumber()
    {
        if (Stream.IsToken(TiposDToken.Number))
        {
            return new Number(double.Parse(Stream.CurrentToken().StringToken), Stream.Position);
        }
        if (Stream.IsToken(TiposDToken.OpResta) && Stream.FindToken(TiposDToken.Number))
        {
            return new Number(-1 * double.Parse(Stream.CurrentToken().StringToken), Stream.Position - 1);

        }
        return null;
        //    if (!Stream.IsToken(TiposDToken.Number)) return null;

        //    return new Number(double.Parse(Stream.CurrentToken().StringToken), Stream.Position);
    }
    private Expression? ParseParentesis()
    {
        if (Stream.IsToken(TiposDToken.OpenParentesis))
        {
            var exp = new Parentesis(Stream.Position);

            Stream.LastToken = Stream.WhereCloseParentesis();
            Stream.NextToken();
            exp.InsideParentesis = ParseExpression();
            Stream.LastToken = Stream.tokens.Length - 1;
            return exp;
        }
        if (Stream.IsToken(TiposDToken.OpResta) && Stream.FindToken(TiposDToken.OpenParentesis))
        {
            var exp = new Parentesis(Stream.Position);
            Stream.NextToken();
            exp.InsideParentesis = ParseExpression();
            return exp.NegateParentesis();

        }
        return null;
    }

    private Expression? ParseCoseno()
    {
        if (Stream.IsToken(TiposDToken.Coseno))
        {
            if (Stream.FindToken(TiposDToken.OpenParentesis))
            {
                var exp = new Coseno(Stream.Position);
                Stream.NextToken();
                exp.InsideParentesis = ParseExpression();
                return exp;
            }
            Stream.NextToken();
            return null;
        }
        if (Stream.IsToken(TiposDToken.OpResta) && Stream.FindToken(TiposDToken.Coseno))
        {
            if (Stream.FindToken(TiposDToken.OpenParentesis))
            {
                var exp = new Coseno(Stream.Position);
                Stream.NextToken();
                exp.InsideParentesis = ParseExpression();
                return exp.NegateCos();
            }
            Stream.NextToken();
            return null;
        }
        return null;
    }
    private Expression? ParseSeno()
    {
        if (Stream.IsToken(TiposDToken.Seno))
        {
            if (Stream.FindToken(TiposDToken.OpenParentesis))
            {
                var exp = new Seno(Stream.Position);
                Stream.NextToken();
                exp.InsideParentesis = ParseExpression();
                return exp;
            }
            Stream.NextToken();
            return null;
        }
        if (Stream.IsToken(TiposDToken.OpResta) && Stream.FindToken(TiposDToken.Seno))
        {
            if (Stream.FindToken(TiposDToken.OpenParentesis))
            {
                var exp = new Seno(Stream.Position);
                Stream.NextToken();
                exp.InsideParentesis = ParseExpression();
                return exp.NegateSen();
            }
            Stream.NextToken();
            return null;
        }
        return null;
    }

    private Expression? ParsePI()
    {
        if (Stream.IsToken(TiposDToken.NumberPI))
        {
            return new Number(Math.PI, Stream.Position - 1);
        }
        if (Stream.IsToken(TiposDToken.OpResta) && Stream.FindToken((TiposDToken.NumberPI)))
        {
            return new Number(-1 * Math.PI, Stream.Position - 1);
        }
        return null;
    }

    private Expression? ParseLog()
    {
        if (Stream.IsToken(TiposDToken.Logaritmo))
        {
            var exp = new Logaritmo(Stream.Position);
            if (Stream.FindToken(TiposDToken.OpenParentesis))
            {
                Stream.NextToken();
                var left = ParseExpression();
                left.Evaluate();
                double x = (double)left.Value!;
                bool BaseValida = left != null && x > 0 && x != 1;
                if (!BaseValida || !Stream.FindToken(TiposDToken.Coma)) return null;
                Stream.NextToken();
                var right = ParseExpression();
                if (right == null) return null;
                right.Evaluate();
                double y = (double)right.Value!;
                if (y <= 0) return null;
                exp.Left = left;
                exp.Right = right;
                return exp;

            }
            Stream.NextToken();
            return null;
        }
        return null;
    }

    private Expression? ParseVariableReference()
    {
        if (Stream.IsToken(TiposDToken.Identificador) && DefVariables.ContainsVariable(Stream.CurrentToken().StringToken))
        {
            return new VariableReference(Stream.CurrentToken().StringToken, DefVariables, Stream.Position);
        }
        return null;
    }

}


public class VariableScope
{
    public Dictionary<string, object> variables = new Dictionary<string, object>();

    public void AddVariable(string identifier, object value)
    {
        variables.Add(identifier, value);
    }

    public void AssignVariable(string identifier, object value)
    {
        variables[identifier] = value;
    }

    public void ClearVariables()
    {
        variables.Clear();
    }

    public object GetVariableValue(string identifier)
    {
        if (variables.ContainsKey(identifier))
        {
            return variables[identifier];
        }
        else
        {
            throw new Exception($"Semantic error: Variable '{identifier}' does not exist.");///ver
        }
    }

    public bool ContainsVariable(string identifier)
    {
        return variables.ContainsKey(identifier);
    }
}

//public static class VariableScope
//{
//    private static Dictionary<string, object> variables = new Dictionary<string, object>();

//    public static void AddVariable(string identifier, object value)
//    {
//        variables.Add(identifier, value);
//    }

//    public static void AssignVariable(string identifier, object value)
//    {
//        variables[identifier] = value;
//    }

//    public static void ClearVariables()
//    {
//        variables.Clear();
//    }

//    public static object GetVariableValue(string identifier)
//    {
//        if (variables.ContainsKey(identifier))
//        {
//            return variables[identifier];
//        }
//        else
//        {
//            throw new Exception($"Semantic error: Variable '{identifier}' does not exist.");///ver
//        }
//    }

//    public static bool ContainsVariable(string identifier)
//    {
//        return variables.ContainsKey(identifier);
//    }
//}
