//namespace JCompiler.Helper.Token
//{
//    public class TokenIdentifyHelper
//    {
//        public TokenIdentifyHelper()
//        {
            
//        }
//        public Token IdentifyToken(string curChar)
//        {
//            switch (curChar)
//            {
//                case "+":
//                    {
//                        token = new(curChar, TokenEnum.PLUS);
//                        break;
//                    }
//                case "-":
//                    {
//                        token = new(curChar, TokenEnum.MINUS);
//                        break;
//                    }
//                case "*":
//                    {
//                        token = new(curChar, TokenEnum.ASTERISK);
//                        break;
//                    }
//                case "/":
//                    {
//                        token = new(curChar, TokenEnum.SLASH);
//                        break;
//                    }
//                case "\n":
//                    {
//                        token = new(curChar, TokenEnum.NEWLINE);
//                        break;
//                    }
//                case "\0":
//                    {
//                        token = new(curChar, TokenEnum.EOF);
//                        break;
//                    }
//                case "=":
//                    {
//                        if (Peek() == '=')
//                        {
//                            char lastChar = curChar;
//                            NextChar();
//                            token = new("" + lastChar + curChar, TokenEnum.EQEQ);
//                        }
//                        else
//                        {
//                            token = new(curChar, TokenEnum.EQ);
//                        }
//                        break;
//                    }
//                case '>':
//                    {

//                    }
//                default:
//                    {
//                        // unknown token!
//                        Abort("Unknown token: " + curChar);
//                        break;
//                    }
//            }
//        }

//        //invalid token found , print error message
//        public void Abort(string message)
//        {
//            Console.WriteLine("Lexing Error: " + message);
//            // 1 is typically used to indicate an error.
//            Environment.Exit(1);
//        }
//        //return the lookagead character
//        public char Peek()
//        {
//            if (curPosition + 1 >= source.Length)
//                return '\0';
//            return source[curPosition + 1];
//        }
//    }
//}
