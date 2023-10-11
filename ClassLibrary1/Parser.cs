using ClassLibrary1;
public class Parser
{
    public Parser(TokenStream stream)
    {
        Stream = stream;
    }

    public TokenStream Stream { get; private set; }


    public Expression? ParseExpression()
    {
        return ParseExpressionLv1();
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
        Expression? exp = ParseNumber();
        if (exp != null) return exp;

        exp = ParseParentesis();
        if (exp != null) return exp;

        return null;
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


}
