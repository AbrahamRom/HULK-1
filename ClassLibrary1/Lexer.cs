using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ClassLibrary1
{
    public class Lexer
    {
        static string TokenUnion = ExpresionesRegulares.OpSuma.ToString() + "|" + ExpresionesRegulares.OpResta.ToString() + "|" + ExpresionesRegulares.OpMultiply.ToString() + "|" + ExpresionesRegulares.OpDivide.ToString() + "|" + ExpresionesRegulares.Potencia.ToString() + "|" + ExpresionesRegulares.NumberPI.ToString() + "|" + ExpresionesRegulares.Seno.ToString() + "|" + ExpresionesRegulares.Coseno.ToString() + "|" + ExpresionesRegulares.Logaritmo.ToString() + "|" + ExpresionesRegulares.OpenParentesis.ToString() + "|" + ExpresionesRegulares.CloseParentesis.ToString() + "|" + ExpresionesRegulares.Coma.ToString() + "|" + ExpresionesRegulares.PuntoComa.ToString() + "|" + ExpresionesRegulares.numero.ToString()+"|" + ExpresionesRegulares.Print.ToString() + "|" + ExpresionesRegulares.Let.ToString() + "|" + ExpresionesRegulares.In.ToString() + "|" + ExpresionesRegulares.Assign.ToString() + "|" + ExpresionesRegulares.Identificador.ToString();
        static Regex separador = new Regex(TokenUnion); // el regex que se usa para separar los tokens // se lleva los errores de errores invalidos ARREGLAR

        public Token[] ArrayObjectToken;
        // public string[] ArrayStringToken;

        public Lexer(string code, List<CompilingError> errors)
        {
            MatchCollection collection = separador.Matches(code);
            int ArrayDimension = collection.Count;
            // ArrayStringToken = new string[ArrayDimension];
            ArrayObjectToken = new Token[ArrayDimension];
            Match match = separador.Match(code);
            int indexador = 0;
            while (match.Success)
            {
                // ArrayStringToken[indexador] = match.Value;
                Token x = new Token(match.Index, match.Value);
                ArrayObjectToken[indexador] = x;
                if (x.TipoDToken == TiposDToken.InvalidToken)
                {
                    errors.Add(new CompilingError(match.Index, ErrorCode.Invalid, "Token"));
                }
                indexador++;
                match = match.NextMatch();
            }
        }
    }
    public enum TiposDToken { OpSuma, OpResta, OpMultiply, OpDivide, OpenParentesis, CloseParentesis, PuntoComa, Number, Potencia, NumberPI, Seno, Coseno, Logaritmo,Coma, Print,Identificador,Let,In,Assign,InvalidToken }
    public class Token
    {
        public TiposDToken TipoDToken;
        public int Location;
        public string StringToken;

        public Token(int position, string stringtoken)
        {
            this.Location = position;
            this.StringToken = stringtoken;
            if (ExpresionesRegulares.OpSuma.IsMatch(stringtoken)) this.TipoDToken = TiposDToken.OpSuma;
            else if (ExpresionesRegulares.OpResta.IsMatch(stringtoken)) this.TipoDToken = TiposDToken.OpResta;
            else if (ExpresionesRegulares.OpMultiply.IsMatch(stringtoken)) this.TipoDToken = TiposDToken.OpMultiply;
            else if (ExpresionesRegulares.OpDivide.IsMatch(stringtoken)) this.TipoDToken = TiposDToken.OpDivide;
            else if (ExpresionesRegulares.Potencia.IsMatch(stringtoken)) this.TipoDToken = TiposDToken.Potencia;
            else if (ExpresionesRegulares.NumberPI.IsMatch(stringtoken)) this.TipoDToken = TiposDToken.NumberPI;
            else if (ExpresionesRegulares.Seno.IsMatch(stringtoken)) this.TipoDToken = TiposDToken.Seno;
            else if (ExpresionesRegulares.Coseno.IsMatch(stringtoken)) this.TipoDToken = TiposDToken.Coseno;
            else if (ExpresionesRegulares.Logaritmo.IsMatch(stringtoken)) this.TipoDToken = TiposDToken.Logaritmo;
            else if (ExpresionesRegulares.OpenParentesis.IsMatch(stringtoken)) this.TipoDToken = TiposDToken.OpenParentesis;
            else if (ExpresionesRegulares.CloseParentesis.IsMatch(stringtoken)) this.TipoDToken = TiposDToken.CloseParentesis;
            else if (ExpresionesRegulares.Coma.IsMatch(stringtoken)) this.TipoDToken = TiposDToken.Coma;
            else if (ExpresionesRegulares.PuntoComa.IsMatch(stringtoken)) this.TipoDToken = TiposDToken.PuntoComa;
            else if (ExpresionesRegulares.numero.IsMatch(stringtoken)) this.TipoDToken = TiposDToken.Number;
            else if (ExpresionesRegulares.Print.IsMatch(stringtoken)) this.TipoDToken = TiposDToken.Print;
            else if (ExpresionesRegulares.Assign.IsMatch(stringtoken)) this.TipoDToken = TiposDToken.Assign;
            else if (ExpresionesRegulares.Let.IsMatch(stringtoken)) this.TipoDToken = TiposDToken.Let;
            else if (ExpresionesRegulares.In.IsMatch(stringtoken)) this.TipoDToken = TiposDToken.In;
            else if (ExpresionesRegulares.Identificador.IsMatch(stringtoken)) this.TipoDToken = TiposDToken.Identificador;
            else this.TipoDToken = TiposDToken.InvalidToken;
        }
    }

    public class ExpresionesRegulares
    {
        static public Regex OpSuma = new Regex(@"\+");
        static public Regex OpResta = new Regex(@"\-");
        static public Regex OpMultiply = new Regex(@"\*");
        static public Regex OpDivide = new Regex(@"/");
        static public Regex Potencia = new Regex(@"\^");
        static public Regex NumberPI = new Regex("PI");
        static public Regex Seno = new Regex("sin");
        static public Regex Coseno = new Regex("cos");
        static public Regex Logaritmo = new Regex("log");
        static public Regex OpenParentesis = new Regex(@"\(");
        static public Regex CloseParentesis = new Regex(@"\)");
        static public Regex Coma = new Regex(",");
        static public Regex PuntoComa = new Regex(";");
        static public Regex numero = new Regex(@"\d+");// ^[0-9]+([,][0-9]+)?$ para numeros con coma, revisar
        static public Regex Print = new Regex("print");
        static public Regex Let = new Regex("let");
        static public Regex In = new Regex("in");
        static public Regex Assign = new Regex("=");
        static public Regex Identificador = new Regex("[a-zA-Z_][0-9a-zA-Z_]*");
    }
}
