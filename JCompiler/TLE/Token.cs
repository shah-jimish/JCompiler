using JCompiler.Helper.Token;

namespace JCompiler.TLE
{
    public class Token
    {
        public char tokenText;
        public string tokenSText;
        public TokenEnum tokenKind;
        public Token(char tokenText, TokenEnum tokenKind)
        {
            this.tokenText = tokenText;
            this.tokenKind = tokenKind;
        }
        public Token(string tokenSText, TokenEnum tokenKind)
        {
            this.tokenSText = tokenSText;
            this.tokenKind = tokenKind;
        }
    }
}
