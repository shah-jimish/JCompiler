using JCompiler.Helper.Token;
using System.Text;

namespace JCompiler.TLE
{
    // Parser object keeps track of current token and checks if the code matches the grammar.
    public class Parser
    {
        private readonly Lexer lexer;
        private readonly Emitter emitter;
        Token curToken;
        Token peekToken;
        HashSet<string> symbols;
        HashSet<string> labelsDeclared;
        HashSet<string> labelsGotoed;
        public Parser(Lexer lexer, Emitter emitter)
        {
            this.lexer = lexer;
            this.emitter = emitter;
            symbols = [];
            labelsDeclared = [];
            labelsGotoed = [];

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
        public void MatchTwoToken(TokenEnum firstTokenKind,TokenEnum secondTokenKind)
        {
            if (!CheckToken(firstTokenKind) && !CheckToken(secondTokenKind))
            {
                Abort("Expected " + firstTokenKind.ToString()+" or "+ secondTokenKind.ToString() + ", got " + curToken.tokenKind.ToString());
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
            emitter.HeaderLine("#include<stdio.h>");
            emitter.HeaderLine("int main(void){");
            //since some newlines are required in our grammer , need to skip the excess
            while (CheckToken(TokenEnum.NEWLINE))
            {
                NextToken();
            }
            //parse all the statement in the program
            while (!CheckToken(TokenEnum.EOF))
            {
                Statement();
            }
            emitter.EmitLine("printf(\"Press Enter to exit...\\n\");");
            emitter.EmitLine("getchar();"); // This will capture the newline character from the last input
            emitter.EmitLine("getchar();");
            emitter.EmitLine("return 0;");
            emitter.EmitLine("}");
            Console.WriteLine("PROGRAM");
            while (!CheckToken(TokenEnum.EOF))
            {
                Statement();
            }
            foreach (var label in labelsGotoed)
            {
                if (!labelsDeclared.Contains(label))
                {
                    Abort("Attempting to GOTO to undeclared label: " + label);
                }
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
                    //simple string so print it
                    emitter.EmitLine("printf(\"" + curToken.tokenText.ToString() + "\\n\");");
                    NextToken();
                }
                else
                {
                    //Except an expression and print result as float
                    emitter.Emit("printf(\"%" + ".2f\\n\", (float)(");
                    Expression();
                    emitter.EmitLine("));");
                }
            }
            // "IF" comparison "THEN" nl {statement} "ENDIF" nl
            else if (CheckToken(TokenEnum.IF))
            {
                Console.WriteLine("STATEMENT-IF");
                NextToken();
                emitter.Emit("if(");
                Comparasion();
                Match(TokenEnum.THEN);
                NewLine();
                emitter.EmitLine("){");
                while (!CheckToken(TokenEnum.ENDIF))
                {
                    Statement();
                }
                Match(TokenEnum.ENDIF);
                emitter.EmitLine("}");
            }
            //"WHILE" comparison "REPEAT" nl {statement nl} "ENDWHILE" nl
            else if (CheckToken(TokenEnum.WHILE))
            {
                Console.WriteLine("STATEMENT-WHILE");
                NextToken();
                emitter.Emit("while(");
                Comparasion();
                Match(TokenEnum.REPEAT);
                NewLine();
                emitter.EmitLine("){");
                while (!CheckToken(TokenEnum.ENDWHILE))
                {
                    Statement();
                }
                Match(TokenEnum.ENDWHILE);
                emitter.EmitLine("}");
            }
            //"LABEL" ident
            else if (CheckToken(TokenEnum.LABEL))
            {
                Console.WriteLine("STATEMENT-LABEL");
                NextToken();
                //make sure the label does not exists
                if (labelsDeclared.Contains(curToken.tokenText.ToString()))
                {
                    Abort("Label already exists: " + curToken.tokenText.ToString());
                }
                labelsDeclared.Add(curToken.tokenText.ToString());
                emitter.EmitLine(curToken.tokenText.ToString() + ":");
                Match(TokenEnum.IDENT);
            }
            //"GOTO" ident nl
            else if (CheckToken(TokenEnum.GOTO))
            {
                Console.WriteLine("STATEMENT-GOTO");
                NextToken();
                labelsGotoed.Add(curToken.tokenText.ToString());
                emitter.EmitLine("goto " + curToken.tokenText.ToString() + ";");
                Match(TokenEnum.IDENT);
            }
            //"LET" ident "=" expression nl
            else if (CheckToken(TokenEnum.LET))
            {
                Console.WriteLine("STATEMENT-LET");
                NextToken();
                if (!symbols.Contains(curToken.tokenText.ToString()))
                {
                    symbols.Add(curToken.tokenText.ToString());
                    emitter.HeaderLine("float " + curToken.tokenText.ToString() + ";");
                }
                emitter.Emit(curToken.tokenText.ToString() + "=");
                Match(TokenEnum.IDENT);
                Match(TokenEnum.EQ);
                Expression();
                emitter.EmitLine(";");
            }
            //"INPUT" ident
            else if (CheckToken(TokenEnum.INPUT))
            {
                Console.WriteLine("STATEMENT-INPUT");
                NextToken();
                if (!symbols.Contains(curToken.tokenText.ToString()))
                {
                    symbols.Add(curToken.tokenText.ToString());
                    emitter.HeaderLine("float " + curToken.tokenText.ToString() + ";");
                }
                emitter.EmitLine("if(0 == scanf(\"%" + "f\",&" + curToken.tokenText.ToString() + ")) {");
                emitter.EmitLine(curToken.tokenText.ToString() + " = 0;");
                emitter.Emit("scanf(\"%");
                emitter.EmitLine("*s\");");
                emitter.EmitLine("}");
                Match(TokenEnum.IDENT);
            }
            
            //datatype[] ident "=" [1,2,3,4]
            else if (CheckToken(TokenEnum.NUMBERS))
            {
                //[numbers]
                //4) = 
                //5) [
                //6) value sepeated by commans ]
                //[1,2,3,4,5]
                Console.WriteLine("Array-statement");
                NextToken();
                if (!symbols.Contains(curToken.tokenText.ToString()))
                {
                    symbols.Add(curToken.tokenText.ToString());
                    //int ident[] = {1,2,3,4};
                    //in the second section i want the array 
                    emitter.Emit("int " + curToken.tokenText.ToString()+"[] = ");
                }
                //ident[]
                Match(TokenEnum.IDENT);
                Match(TokenEnum.EQ);
                Match(TokenEnum.SBO);
                StringBuilder sb = new();
                sb.Append("{");
                sb.Append(curToken.tokenText.ToString());
                MatchTwoToken(TokenEnum.IDENT,TokenEnum.NUMBER);
                while (!CheckToken(TokenEnum.SBC))
                {
                    NextToken();
                    sb.Append(",");
                    sb.Append(curToken.tokenText.ToString());
                    var t = curToken.tokenKind.ToString();
                    if (curToken.tokenKind == TokenEnum.IDENT)
                    {
                        if (!symbols.Contains(curToken.tokenText.ToString()))
                        {
                            Abort("Invalid statement at " + curToken.tokenText.ToString() + " (" + curToken.tokenKind.ToString() + ")");
                        }
                    }
                    MatchTwoToken(TokenEnum.IDENT, TokenEnum.NUMBER);
                }
                NextToken();
                sb.Append("};");
                Console.WriteLine(sb.ToString());
                emitter.EmitLine(sb.ToString());
                // from here we have to match the entire string with this rule
                //[1,a]
                //number
                //int variable
                //sepeated by `,`
                //`[` and `]` bracket
            }
            //this is not a valid statement Error!
            else
            {
                Abort("Invalid statement at " + curToken.tokenText.ToString() + " (" + curToken.tokenKind.ToString() + ")");
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
            //Can have 0 or more +/- and expressions.
            while (CheckToken(TokenEnum.PLUS) || CheckToken(TokenEnum.MINUS))
            {
                emitter.Emit(curToken.tokenText.ToString());
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
                emitter.Emit(curToken.tokenText.ToString());
                NextToken();
                Expression();
            }
            else
            {
                Abort("Expected comparison operator at: " + curToken.ToString());
            }
            //Can have 0 or more comparison operator and expressions.
            while (IsComparisonOperator())
            {
                emitter.Emit(curToken.tokenText.ToString());
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
                emitter.Emit(curToken.tokenText.ToString());
                NextToken();
                Unary();
            }
        }
        // unary ::= ["+" | "-"] primary
        public void Unary()
        {
            Console.WriteLine("UNARY");
            //Optional unary +/-
            if (CheckToken(TokenEnum.PLUS) || CheckToken(TokenEnum.MINUS))
            {
                emitter.Emit(curToken.tokenText.ToString());
                NextToken();
            }
            Primary();
        }
        //primary ::= number | ident
        public void Primary()
        {
            Console.WriteLine("PRIMARY (" + curToken.tokenText.ToString() + ")");
            if (CheckToken(TokenEnum.NUMBER))
            {
                emitter.Emit(curToken.tokenText.ToString());
                NextToken();
            }
            else if (CheckToken(TokenEnum.IDENT))
            {
                //Ensure the variable already exists.
                if (!symbols.Contains(curToken.tokenText.ToString()))
                {
                    Abort("Referencing variable before assignment: " + curToken.tokenText.ToString());
                }
                emitter.Emit(curToken.tokenText.ToString());
                NextToken();
            }
            else
            {
                Abort("Unexpected token at " + curToken.tokenText.ToString());
            }
        }
    }
}
