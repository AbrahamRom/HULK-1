using ClassLibrary1;
public class Parser
{
    public Parser(TokenStream stream)                                //constructor de la clase los token vienen incluidos en stream
    {
        Stream = stream;
        DefVariables = new VariableScope();
    }
    public TokenStream Stream { get; private set; }                 //flujo de los token mientras avanza el programa
    public VariableScope DefVariables { get; private set; }           // aqui se guardan las variables locales
    public bool ParsingAFunction { get; private set; }               // mientas se parsea una funcion toma valor true, asi se utiliza un ambito global
    public StatementNode? ParseStament()                    // itera tratando de parsear una un Statement
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

        Statement = ParseExpStatement();
        if (Statement != null) return Statement;

        return null;
    }
    private StatementNode? ParseFunctionDeclaration()             // parsea una declaracion de funciion utilizando la estructura function f(x) => expression del cuerpo
    {
        if (!Stream.IsToken(TiposDToken.Function)) return null;
        var function = new FunctionDeclarationNode(Stream.Position - 3);
        if (!Stream.FindToken(TiposDToken.Identificador)) return null;
        function.Identifier = Stream.CurrentToken().StringToken;
        FunctionScope.AddFunctName(Stream.CurrentToken().StringToken);
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
        ParsingAFunction = true;
        function.Body = ParseStament();
        ParsingAFunction = false;
        return function;

    }
    private StatementNode? ParseIfElseStatement()             // parsea una expresion if-else 
    {
        if (!Stream.IsToken(TiposDToken.If)) return null;
        if (!Stream.FindToken(TiposDToken.OpenParentesis)) return null;
        Stream.NextToken();
        var statement = new IfStatementNode(Stream.Position - 2);
        statement.Condition = ParseExpressionBoolean();
        Stream.NextToken(2);
        statement.IfBody = ParseStament();
        if (!Stream.FindToken(TiposDToken.Else)) return null;
        Stream.NextToken();
        statement.ElseBody = ParseStament();
        return statement;
    }
    private StatementNode? ParsePrint()                              // parsea print()
    {
        if (!Stream.IsToken(TiposDToken.Print)) return null;
        if (!Stream.FindToken(TiposDToken.OpenParentesis)) return null;
        Stream.NextToken();
        var exp = ParseExpression();
        var Statement = new PrintStatementNode(Stream.Position - 2, exp);
        return Statement;
    }
    private void ParseParseVariableDeclaration()                      //parsea un ambito let-in
    {
        if (!Stream.IsToken(TiposDToken.Let)) return;
        int count = 1;
        while (count == 1)
        {
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
    private StatementNode? ParseExpStatement()                      // convierte las expresiones en stamments de la clase heredada de StatatementNode
    {
        var exp = ParseExpression();
        if (exp == null) return null;
        return new ExpStatement(Stream.Position, exp);
    }

    public Expression? ParseExpression()                     // metodo general para parsear una expresion
    {
        int temp = Stream.Position;
        var exp = ParseExpressionBoolean();
        if (exp != null)
        {
            if ((exp.Type == ExpressionType.Boolean) || Stream.Position == (Stream.LastToken - 1))
            {
                return exp;
            }
            Stream.position = temp;
        }

        exp = ParseExpressionLv1();
        if (exp != null) return exp;

        return null;
    }
    private Expression? ParseExpressionBoolean()                  // analogamente al parsea de expresiones aritmeticas se parsea las expresiosnes booleanas
    {                                                             // haciendo uso de un metodo analogo y un prioridad igualmente
        return ParseExpBooleanLv1();
    }
    private Expression? ParseExpBooleanLv1()
    {
        Expression? left = ParseExpBooleanLv2();
        Expression? exp = ParseExpBooleanLv1_(left);

        exp = ParseExpBooleanLv1_(exp);

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

        return null;
    }
    private Expression? ParseString()
    {
        if (Stream.IsToken(TiposDToken.StringLiteral))
        {
            return new StringLiteral(Stream.CurrentToken().StringToken.Substring(1, Stream.CurrentToken().StringToken.Length - 2), Stream.Position);
        }
        return null;
    }
    private Expression? ParseBooleanOP(Expression? left)                 //parsea los operadores booleanos >,<,==, no sea han implemetado <=,>=, aunque su implemetacion es sencilla
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
    private Expression? ParseBoolean()                         //parsea la expression booleana true o false
    {
        if (Stream.IsToken(TiposDToken.Bool))
        {
            return new Boolean(bool.Parse(Stream.CurrentToken().StringToken), Stream.Position);
        }
        return null;
    }
    private Expression? ParseOR(Expression? left)                         //parsea la expression booleana |
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
    private Expression? ParseAnd(Expression? left)                            //parsea la expression booleana &
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



    private Expression? ParseExpressionLv1()              // EXPRESIONES lV1 trata de parsear una adicion o subtraccion
    {
        Expression? newLeft = ParseExpressionLv2();
        Expression? exp = ParseExpressionLv1_(newLeft);
        //if (Stream.FindToken(TiposDToken.CloseParentesis))
        // {
        exp = ParseExpressionLv1_(exp);
        //}
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
    private Expression? ParseExpressionLv2()                   // EXPRESIONES lV2 trata de parsear una multiplicacion o division
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
    private Expression? ParseExpressionLv3()         // EXPRESIONES lV3 trata de parsear una potencia
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
    private Expression? ParseExpressionLv4()              // EXPRESIONES lV 4 son las mas prioritarias al resolver una expression
    {
        Expression? exp = ParseNumber();
        if (exp != null) return exp;

        exp = ParsePI();
        if (exp != null) return exp;


        exp = ParseFunctionReference();
        if (exp != null) return exp;

        exp = ParseFunctionRecRef();
        if (exp != null) return exp;

        exp = ParseVariableReference();
        if (exp != null) return exp;

        exp = ParseCoseno();
        if (exp != null) return exp;

        exp = ParseSeno();
        if (exp != null) return exp;

        exp = ParseLog();
        if (exp != null) return exp;

        exp = ParseParentesis();
        if (exp != null) return exp;

        exp = ParseString();
        if (exp != null) return exp;

        return null;
    }

    private Expression? ParseFunctionRecRef()
    {
        if (!Stream.IsToken(TiposDToken.Identificador) || !FunctionScope.ContainsFunctName(Stream.CurrentToken().StringToken)) return null;
        var funct = new FunctionReference(Stream.Position);
        funct.Identifier = Stream.CurrentToken().StringToken;
        if (!Stream.FindToken(TiposDToken.OpenParentesis)) return null;
        var arguments = ParseArgumentsFunc();
        funct.arguments = arguments;

        return funct;
    }

    public Expression? ParseFunctionReference()                                  //parsea una referencia a funcion por ejemplo f(3)
    {
        if (!Stream.IsToken(TiposDToken.Identificador) || !FunctionScope.ContainsFunction(Stream.CurrentToken().StringToken)) return null;
        var identificador = Stream.CurrentToken().StringToken;
        var function = FunctionScope.GetFunction(Stream.CurrentToken().StringToken);
        if (!Stream.FindToken(TiposDToken.OpenParentesis)) return null;
        var arguments = ParseArgumentsFunc();
        return ConvertObjToExp(function.Invoke(arguments));

    }
    private List<Expression> ParseArgumentsFunc()                      //parsea los argumentos        
    {
        var Arguments = new List<Expression>();
        int count = 1;
        while (count == 1)
        {
            Stream.NextToken();
            Arguments.Add(ParseExpression());
            if (!Stream.FindToken(TiposDToken.Coma)) count = 0;

        }
        Stream.NextToken();
        return Arguments;
    }

    private Expression? ConvertObjToExp(object? obj)                              //convierte los tipo object en objetos Expressions
    {
        if (obj is bool) return new Boolean((bool)obj, Stream.Position);
        if (obj is double) return new Number((double)obj, Stream.Position);
        if (obj is string) return new StringLiteral((string)obj, Stream.Position);
        return null;
    }


    private Expression? ParseAdd(Expression? left)                         //parsea una substraccion con miembro izquierdo y miembro derecho
    {
        Add sum = new Add(Stream.Position);

        if (!Stream.FindToken(TiposDToken.OpSuma)) return null;

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
    private Expression? ParseSub(Expression? left)                                 //parsea una substraccion con miembro izquierdo y miembro derecho
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
    private Expression? ParseMul(Expression? left)                    //parsea una multiplicacion con miembro izquierdo y miembro derecho
    {
        Mul mul = new Mul(Stream.Position);

        if (!Stream.FindToken(TiposDToken.OpMultiply)) return null;

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
    private Expression? ParseDiv(Expression? left)                                   //parsea una division con miembro izquierdo y miembro derecho
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
    private Expression? ParsePotencia(Expression? left)                          //parsea la potencia
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
    private Expression? ParseNumber()                                       //parsea un numero o su negativo
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
    }
    private Expression? ParseParentesis()                                                 //parsea un parentesis
    {
        if (Stream.IsToken(TiposDToken.OpenParentesis))
        {
            var exp = new Parentesis(Stream.Position);

            int CloseP = Stream.WhereCloseParentesis();
            Stream.NextToken();
            exp.InsideParentesis = ParseExpression();
            if (CloseP == Stream.Position) return exp;
        }
        if (Stream.IsToken(TiposDToken.OpResta) && Stream.FindToken(TiposDToken.OpenParentesis))
        {
            var exp = new Parentesis(Stream.Position);
            int CloseP = Stream.WhereCloseParentesis();
            Stream.NextToken();
            exp.InsideParentesis = ParseExpression();
            if (CloseP == Stream.Position) return exp.NegateParentesis();

        }
        return null;
    }
    private Expression? ParseCoseno()                                   //  parsea el llamado a la funcion coseno
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
    private Expression? ParseSeno()                       // parsea el llamado a la funcion seno
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
    private Expression? ParsePI()                          //parsea el numero PI
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
    private Expression? ParseLog()          //parsea un logaritmo
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
    private Expression? ParseVariableReference()     // parsea la referencia de una variable
    {
        if (Stream.IsToken(TiposDToken.Identificador))
        {
            return new VariableReference(Stream.CurrentToken().StringToken, DefVariables, ParsingAFunction, Stream.Position);
        }
        return null;
    }

}


public class VariableScope
{
    public Dictionary<string, object> variables = new Dictionary<string, object>();

    public void AddVariable(string identifier, object value)  // añade las variables al diccionario
    {
        if (!variables.ContainsKey(identifier))
        {
            variables.Add(identifier, value);
        }
        else throw new Exception($"! SEMANTIC ERROR : A parameter with the name '{identifier}' already exists insert another parameter name");
    }

    public void AssignVariable(string identifier, object value)
    {
        variables[identifier] = value;
    }
    public object GetVariableValue(string identifier) //obtiene la variable si esta definida
    {
        if (variables.ContainsKey(identifier))
        {
            return variables[identifier];
        }
        else
        {
            throw new Exception($"! SEMANTIC ERROR : Variable '{identifier}' does not exist.");
        }
    }

    public bool ContainsVariable(string identifier) // dice si el diccionario contiene la variable
    {
        return variables.ContainsKey(identifier);
    }
}

public class FunctionVariableScope  // esta clase representa un stack<variableScope> donde se guardan los valores de las funciones que se ejecutan
{
    public static Stack<VariableScope> variables = new Stack<VariableScope>();
    // public static VariableScope variables = new VariableScope();
    public static int CountOverFlow { get; set; }
    public static void AddVariable(string identifier, object value) // este metodo verifica si para una variable existe un valor en un diccionario
    {                                                               // si no existe lo agrega en ese diccionario y si existe crea uno nuevo, agrega el valor y lo 
        if (CountOverFlow > 1000)                                   // pone en el stack<>
        {
            throw new Exception("!OVERFLOW ERROR : Hulk Stack overflow");
        }
        else CountOverFlow++;
        // variables.Add(identifier, value);
        if (!variables.Peek().ContainsVariable(identifier))
        {
            variables.Peek().AddVariable(identifier, value);
        }
        else
        {
            var NewVarScope = new VariableScope();
            NewVarScope.variables.Add(identifier, value);
            variables.Push(NewVarScope);
        }
        // variables.variables[identifier] = value;
    }

    public static bool ContainsVariable(string identifier)
    {
        return variables.Peek().ContainsVariable(identifier);
    }
}
