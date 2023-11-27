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
    public bool FindToken(TiposDToken type)
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
    public int WhereCloseParentesis()
    {
        int count = 1;
        int k = Position;
        while (count != 0 && k < tokens.Length - 1)
        {
            k++;
            if (tokens[k].TipoDToken == TiposDToken.OpenParentesis) count++;
            if (tokens[k].TipoDToken == TiposDToken.CloseParentesis) count--;
        }
        if (k == this.LastToken) throw new Exception("parentesis sin cerrar");
        return k;
    }

    // public bool Next()
    // {
    //     if (position < tokens.Length - 1)
    //     {
    //         position++;
    //     }

    //     return position < tokens.Length;
    // }

    // public bool Next( TiposDToken type )
    // {
    //     if (position < tokens.Length-1 && LookAhead(1).TipoDToken == type)
    //     {
    //         position++;
    //         return true;
    //     }

    //     return false;
    // }



    // public bool CanLookAhead(int k = 0)
    // {
    //     return tokens.Length - position > k;
    // }

    // public Token LookAhead(int k = 0)
    // {
    //     return tokens[position + k];
    // }


}
