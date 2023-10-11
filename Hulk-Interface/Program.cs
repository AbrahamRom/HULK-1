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
                if (string.IsNullOrWhiteSpace(code)) return;
                var errors = new List<CompilingError>();
                var lexer = new Lexer(code,errors);
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
                var x = Parser.ParseExpression();
                x.Evaluate();
                Console.WriteLine(x.Value);


            }
        }
    }
}