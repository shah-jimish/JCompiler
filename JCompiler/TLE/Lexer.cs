using JCompiler.Helper;
using JCompiler.Helper.Token;
using JCompiler.TLE;

namespace JCompiler
{
    public class Lexer
    {
        public string source;
        public char curChar;
        public int curPosition;

        public Lexer(string source)
        {
            this.source = source + "\n"; // Source code to lex as a string. Append a newline to simplify lexing/parsing the last token/statement.
            curChar = '\0';              // Initialize the current character.
            curPosition = -1;            // Initialize the current position.
            NextChar();                  // Call NextChar to set the first character.
        }

        //process the next character
        public void NextChar()
        {
            curPosition++;
            if (curPosition >= source.Length)
            {
                curChar = '\0';
            }
            else
            {
                curChar = source[curPosition];
            }
        }
        //return the lookagead character
        public char Peek()
        {
            if (curPosition + 1 >= source.Length)
                return '\0';
            return source[curPosition + 1];
        }
        //invalid token found , print error message
        public void Abort(string message)
        {
            Console.WriteLine("Lexing Error: " + message);
            // 1 is typically used to indicate an error.
            Environment.Exit(1);
        }
        //skip white-space except new line,which we will use to indicate the end of a statement.
        public void SkipWhiteSpace()
        {
            while (curChar == ' ' || curChar == '\t' || curChar == '\r')
            {
                NextChar();
            }
        }
        //skip comment in the code
        public void SkipComment()
        {

        }
        //return the next token
        public Token GetToken()
        {
            SkipWhiteSpace();
            Token token = null;
            switch (curChar)
            {
                case '+':
                    {
                        token = new(curChar, TokenEnum.PLUS);
                        break;
                    }
                case '-':
                    {
                        token = new(curChar, TokenEnum.MINUS);
                        break;
                    }
                case '*':
                    {
                        token = new(curChar, TokenEnum.ASTERISK);
                        break;
                    }
                case '/':
                    {
                        token = new(curChar, TokenEnum.SLASH);
                        break;
                    }
                case '\n':
                    {
                        token = new(curChar, TokenEnum.NEWLINE);
                        break;
                    }
                case '\0':
                    {
                        token = new(curChar, TokenEnum.EOF);
                        break;
                    }
                case '=':
                    {
                        if (Peek() == '=')
                        {
                            char lastChar = curChar;
                            NextChar();
                            token = new("" + lastChar + curChar, TokenEnum.EQEQ);
                        }
                        else
                        {
                            token = new(curChar, TokenEnum.EQ);
                        }
                        break;
                    }
                default:
                    {
                        // unknown token!
                        Abort("Unknown token: " + curChar);
                        break;
                    }
            }
            NextChar();
            return token;
        }
    }
}
