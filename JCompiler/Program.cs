using JCompiler;
using JCompiler.Helper.Token;
using JCompiler.TLE;

public class Program
{
    public static void Main()
    {
        string source = "IF+-123 foo*THEN/";
        Lexer lexer = new(source);
        Token token = lexer.GetToken();
        while (token.tokenKind != TokenEnum.EOF)
        {
            Console.WriteLine(token.tokenKind);
            token = lexer.GetToken();
        }
    }
}