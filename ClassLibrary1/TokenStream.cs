//esta clase representa el flujo durante el parser en los tokens
using System.Collections;
using ClassLibrary1;
public class TokenStream
{
    public Token[] tokens;
    public int position;
    public int LastToken { get; set; }

    public TokenStream(Token[] token)
    {

        this.tokens = token;
        position = 0;
        LastToken = token.Length - 1;
        RevisarPComa();
    }
    public void RevisarPComa()   //revisa que la coma este bien en la expresion inicial
    {
        int CantidadPComas=0;
        int? PositionPcoma =null;
        int index=0;
        foreach (Token token in tokens)
        {
            index++;
            if (token.TipoDToken == TiposDToken.PuntoComa)
            {
                CantidadPComas++;
                if (PositionPcoma==null) PositionPcoma = index;
            }
        }
        if (CantidadPComas == 0)
        {
            throw new Exception("!SYNTAX ERROR : Not find valid last token \";\"");
        }
        if(PositionPcoma < tokens.Length-1)
        {
            string x = "";
            for(int i = (int)PositionPcoma; i < tokens.Length; i++)
            {
                x+= tokens[i].StringToken+" ";
            }
            throw new Exception("!SYNTAX ERROR : The expression "+x+" is out of context");

        }
    }
    public int Position { get { return position; } }
    public bool IsToken(TiposDToken type)
    {
        return (tokens[position].TipoDToken == type);
    }
    public Token CurrentToken()
    {
        return tokens[position];
    }
    public void NextToken(int k = 1)
    {
        if (position < LastToken) position += k;
    }
    public bool FindToken(TiposDToken type) // busca el token de un tipo y si lo encuentra avanza a el
    {
        if (position < LastToken && tokens[position + 1].TipoDToken == type)
        {
            NextToken();
            return true;
        }
        return false;
    }

    public void MoveBack(int k)
    {
        position -= k;
    }
    public int WhereCloseParentesis() // revisa que lo parentesis esten balanceados
    {
        int count = 1;
        int k = Position;
        while (count != 0 && k < tokens.Length - 1)
        {
            k++;
            if (tokens[k].TipoDToken == TiposDToken.OpenParentesis) count++;
            if (tokens[k].TipoDToken == TiposDToken.CloseParentesis) count--;
        }
        if (k == this.LastToken) throw new Exception("!SYNTAX ERROR: Parentheses are not balanced");
        return k;
    }

   

}
