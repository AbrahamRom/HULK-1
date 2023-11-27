using ClassLibrary1;
namespace Hulk_Interface
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write(">");
                string code = Console.ReadLine();
                try
                {


                    if (string.IsNullOrWhiteSpace(code)) return;
                    var errors = new List<CompilingError>();
                    var lexer = new Lexer(code, errors);
                    if (errors.Count > 0)
                    {
                        foreach (var error in errors)
                        {
                            Console.WriteLine("{0}, {1}, {2}", error.Location, error.Code, error.Argument);
                        }
                        return;
                    }

                    var TokenizedCode = lexer.ArrayObjectToken;
                    //foreach (var Token in TokenizedCode)
                    //{
                    //    Console.WriteLine(Token.StringToken);
                    //}

                    var Stream = new TokenStream(TokenizedCode);
                    var Parser = new Parser(Stream);
                    //var x = Parser.ParseExpression();
                    //x.Evaluate();
                    //Console.WriteLine(x.Value);
                    var Statement = Parser.ParseStament();
                    var x = Statement.Execute();
                    FunctionVariableScope.CountOverFlow = 0;
                    //  Console.WriteLine(FunctionScope.functions.Count);
                }
                catch (Exception ex)
                {
                    string[] message = ex.ToString().Split();
                    for (int i = 0; i < message.Length; i++)
                    {
                        if (message[i] == "OVERFLOW")
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                        }
                        if (message[i] == "SINTAX")
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            break;
                        }
                        if (message[i] == "SEMANTIC")
                        {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            break;
                        }
                        if (message[i] == "LEXICAL")
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            break;
                        }
                    }
                    Console.WriteLine(ex.Message);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }
    }
}