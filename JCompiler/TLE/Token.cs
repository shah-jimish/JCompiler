using JCompiler.Helper.Token;

namespace JCompiler.TLE
{
    public class Token
    {
        public string tokenText;
        public TokenEnum tokenKind;
        public Token(string tokenText, TokenEnum tokenKind)
        {
            this.tokenText = tokenText;
            this.tokenKind = tokenKind;
        }
    }
}
