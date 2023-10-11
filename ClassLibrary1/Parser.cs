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
}
