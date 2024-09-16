using JCompiler.Helper.Token;
using JCompiler.TLE;

namespace JCompiler
{
    public class Lexer
    {
        public string source;
        public char curChar;
        public int curPosition;
        private readonly Emitter emitter;
        public Lexer(string source, Emitter emitter)
        {
            this.source = source + "\n"; // Source code to lex as a string. Append a newline to simplify lexing/parsing the last token/statement.
            curChar = '\0';              // Initialize the current character.
            curPosition = -1;            // Initialize the current position.
            NextChar();                  // Call NextChar to set the first character.
            this.emitter = emitter;
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
            if (curChar == '#')
            {
                while (curChar != '\n')
                {
                    NextChar();
                }
            }
        }
        //return the next token
        public Token GetToken()
        {
            SkipWhiteSpace();
            SkipComment();
            Token token = null;
            switch (curChar)
            {
                case '+':
                    {
                        token = new(curChar.ToString(), TokenEnum.PLUS);
                        break;
                    }
                case '-':
                    {
                        token = new(curChar.ToString(), TokenEnum.MINUS);
                        break;
                    }
                case '*':
                    {
                        token = new(curChar.ToString(), TokenEnum.ASTERISK);
                        break;
                    }
                case '/':
                    {
                        token = new(curChar.ToString(), TokenEnum.SLASH);
                        break;
                    }
                case '\n':
                    {
                        token = new(curChar.ToString(), TokenEnum.NEWLINE);
                        break;
                    }
                case '\0':
                    {
                        token = new(curChar.ToString(), TokenEnum.EOF);
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
                            token = new(curChar.ToString(), TokenEnum.EQ);
                        }
                        break;
                    }
                case '>':
                    {
                        if (Peek() == '=')
                        {
                            char lastChar = curChar;
                            NextChar();
                            token = new("" + lastChar + curChar, TokenEnum.GTEQ);
                        }
                        else
                        {
                            token = new(curChar.ToString(), TokenEnum.GT);
                        }
                        break;
                    }
                case '<':
                    {
                        if (Peek() == '=')
                        {
                            string lastChar = curChar.ToString();
                            NextChar();
                            token = new("" + lastChar + curChar, TokenEnum.LTEQ);
                        }
                        else
                        {
                            token = new(curChar.ToString(), TokenEnum.LT);
                        }
                        break;
                    }
                case '!':
                    {
                        if (Peek() == '=')
                        {
                            char lastChar = curChar;
                            NextChar();
                            token = new("" + lastChar + curChar, TokenEnum.NOTEQ);
                        }
                        else
                        {
                            Abort("Expected !=, got !" + Peek());
                        }
                        break;
                    }
                case '\"':
                    {
                        NextChar();
                        HashSet<char> illegalChars = ['\r', '\n', '\t', '\\', '%'];
                        int startPos = curPosition;
                        while (curChar != '\"')
                        {
                            //Don't allow special characters in the string. No escape characters, newlines, tabs, or %.
                            //We will be using C's printf on this string.
                            if (illegalChars.Contains(curChar))
                            {
                                Abort("Illegal charter in string!");
                            }
                            NextChar();
                        }
                        string tokenText = source.Substring(startPos, curPosition - startPos);
                        token = new(tokenText, TokenEnum.STRING);
                        break;
                    }
                case char c when char.IsDigit(c):
                    {
                        //Leading character is a digit, so this must be a number.
                        // Get all consecutive digits and decimal if there is one.
                        int startPos = curPosition;
                        while (char.IsDigit(Peek()))
                        {
                            NextChar();
                        }
                        if (Peek() == '.')
                        {
                            NextChar();
                            //Must have atlest one digit after the decimal.
                            if (!char.IsDigit(Peek()))
                            {
                                //Error
                                Abort("Illegal character in number.");
                            }
                            while (char.IsDigit(Peek()))
                            {
                                NextChar();
                            }
                        }
                        string tokenText = source.Substring(startPos, curPosition - startPos + 1);
                        token = new(tokenText, TokenEnum.NUMBER);
                        break;
                    }
                case char c when char.IsLetter(c):
                    {
                        // Leading character is a letter, so this must be an identifier or a keyword.
                        // Get all consecutive alpha numeric characters.
                        int startPos = curPosition;
                        while (char.IsLetterOrDigit(Peek()))
                        {
                            NextChar();
                        }
                        // Check if the token is in the list of keywords.
                        string tokenText = source.Substring(startPos, (curPosition - startPos) + 1);
                        TokenEnum keyword = TokenIdentifyHelper.CheckIfTokenIsKeyword(tokenText);
                        if (keyword == TokenEnum.NONE)
                        {
                            token = new(tokenText, TokenEnum.IDENT);
                        }
                        else
                        {
                            token = new(tokenText, keyword);
                        }
                        break;
                    }
                case '[':
                    {
                        Console.WriteLine("inside the [");
                        token = new(curChar.ToString(),TokenEnum.SBO);
                        break;
                    }
                case ']':
                    {
                        Console.WriteLine("inside the ]");
                        token = new(curChar.ToString(), TokenEnum.SBC);
                        break;
                    }
                case ',':
                    {
                        token = new(curChar.ToString(), TokenEnum.COMMA);
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
