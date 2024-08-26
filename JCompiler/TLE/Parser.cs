using JCompiler.Helper.Token;
using System.Security.Cryptography.X509Certificates;

namespace JCompiler.TLE
{
    // Parser object keeps track of current token and checks if the code matches the grammar.
    public class Parser
    {
        private readonly Lexer lexer;
        Token curToken;
        Token peekToken;
        public Parser(Lexer lexer)
        {
            this.lexer = lexer;
            curToken = null;
            peekToken = null;
            NextToken();
            NextToken();
        }
        //return true if the current token matches the grammer
        public bool CheckToken(TokenEnum tokenKind)
        {
            return tokenKind == curToken.tokenKind;
        }
        //return true if the next token matches the grammer
        public bool CheckPeek(TokenEnum tokenKind)
        {
            return tokenKind == peekToken.tokenKind;
        }

        //match current token , if not then advances the current token
        public void Match(TokenEnum tokenKind)
        {
            if (!CheckToken(tokenKind))
            {
                Abort("Expected " + tokenKind.ToString() + ", got " + curToken.tokenKind.ToString());
            }
            NextToken();
        }

        //Advance the current token
        public void NextToken()
        {
            curToken = peekToken;
            peekToken = lexer.GetToken();

        }
        public void Abort(string message)
        {
            Console.WriteLine("Lexing Error: " + message);
            // 1 is typically used to indicate an error.
            Environment.Exit(1);
        }

        //Production rules.
        //program ::= {statement}
        public void Program()
        {
            Console.WriteLine("PROGRAM");
            while (!CheckToken(TokenEnum.EOF))
            {
                Statement();
            }
        }
        //one of the following statements...
        public void Statement()
        {
            //check if the first token to see what kind of statement this is
            // "PRINT" (expression | string)
            if (CheckToken(TokenEnum.PRINT))
            {
                Console.WriteLine("STATEMENT-PRINT");
                NextToken();
                if (CheckToken(TokenEnum.STRING))
                {
                    //simple string
                    NextToken();
                }
                else
                {
                    //Except an expression
                    Expression();
                }
            }
            // "IF" comparison "THEN" nl {statement} "ENDIF" nl
            else if (CheckToken(TokenEnum.IF))
            {
                Console.WriteLine("STATEMENT-IF");
                NextToken();
                Comparasion();
                Match(TokenEnum.THEN);
                NewLine();
                while (!CheckToken(TokenEnum.ENDIF))
                {
                    Statement();
                }
                Match(TokenEnum.ENDIF);
            }

            //"WHILE" comparison "REPEAT" nl {statement nl} "ENDWHILE" nl
            else if (CheckToken(TokenEnum.WHILE))
            {
                Console.WriteLine("STATEMENT-WHILE");
                NextToken();
                Comparasion();
                Match(TokenEnum.REPEAT);
                while (!CheckToken(TokenEnum.ENDWHILE))
                {
                    Statement();
                }
                Match(TokenEnum.ENDWHILE);
            }
            //"LABEL" ident nl
            else if (CheckToken(TokenEnum.LABEL))
            {
                Console.WriteLine("STATEMENT-LABEL");
                NextToken();
                Match(TokenEnum.IDENT);
            }
            //"GOTO" ident nl
            else if (CheckToken(TokenEnum.GOTO))
            {
                Console.WriteLine("STATEMENT-GOTO");
                NextToken();
                Match(TokenEnum.IDENT);
            }
            //"LET" ident "=" expression nl
            else if (CheckToken(TokenEnum.LET))
            {
                Console.WriteLine("STATEMENT-LET");
                NextToken();
                Match(TokenEnum.IDENT);
                Match(TokenEnum.EQ);
                Expression();
            }
            //"INPUT" ident
            else if (CheckToken(TokenEnum.INPUT))
            {
                Console.WriteLine("STATEMENT-INPUT");
                NextToken();
                Match(TokenEnum.IDENT);
            }
            //this is not a valid statement Error!
            else
            {
                Abort("Invalid statement at " + curToken.tokenSText + " (" + curToken.tokenKind.ToString() + ")");
            }
            NewLine();
        }
        public void NewLine()
        {
            Console.WriteLine("NEWLINE");

            //require at least one new line
            Match(TokenEnum.NEWLINE);
            //but we will allow extra new line too,of course
            while (CheckToken(TokenEnum.NEWLINE))
            {
                NextToken();
            }
        }

        //expression ::= term {( "-" | "+" ) term}
        public void Expression()
        {
            Console.WriteLine("EXPRESSION");
            Term();
            while (CheckToken(TokenEnum.PLUS) || CheckToken(TokenEnum.MINUS))
            {
                NextToken();
                Term();
            }
        }

        //comparison ::= expression (("==" | "!=" | ">" | ">=" | "<" | "<=") expression)+
        public void Comparasion()
        {
            Console.WriteLine("COMPARISON");
            Expression();
            // Must be at least one comparison operator and another expression.
            if (IsComparisonOperator())
            {
                NextToken();
                Expression();
            }
            else
            {
                Abort("Expected comparison operator at: " + curToken.ToString());
            }
            while (IsComparisonOperator())
            {
                NextToken();
                Expression();
            }
        }
        //Return true if the current token is a comparison operator.
        public bool IsComparisonOperator()
        {
            return CheckToken(TokenEnum.GT) || CheckToken(TokenEnum.GTEQ) ||
                    CheckToken(TokenEnum.LT) || CheckToken(TokenEnum.LTEQ) ||
                    CheckToken(TokenEnum.EQEQ) || CheckToken(TokenEnum.NOTEQ);
        }
        //term ::= unary {( "/" | "*" ) unary}
        public void Term()
        {
            Console.WriteLine("TERM");
            Unary();
            //can have 0 or more *// and expression
            while (CheckToken(TokenEnum.ASTERISK) || CheckToken(TokenEnum.SLASH))
            {
                NextToken();
                Unary();
            }
        }
        // unary ::= ["+" | "-"] primary
        public void Unary()
        {
            Console.WriteLine("UNARY");
            if (CheckToken(TokenEnum.PLUS) || CheckToken(TokenEnum.MINUS))
            {
                NextToken();
            }
            Primary();
        }
        //primary ::= number | ident
        public void Primary()
        {
            Console.WriteLine("PRIMARY (" + curToken.tokenSText.ToString() + ")");
            if (CheckToken(TokenEnum.NUMBER) || CheckToken(TokenEnum.IDENT))
            {
                NextToken();
            }
            else
            {
                Abort("Unexpected token at " + curToken.ToString());
            }
        }
    }
}
