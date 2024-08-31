using JCompiler.TLE;

namespace JCompiler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            args[0] = "code.jimish";
            if (args.Length != 1)
            {
                Console.WriteLine("Error: Compiler needs source file as argument.");
                Environment.Exit(1);
            }
            string source = "";
            try
            {
                using (StreamReader inputFile = new(args[0]))
                {
                    source = inputFile.ReadToEnd();
                }
                Lexer lexer = new(source);
                Emitter emitter = new("out.c");
                Parser parse = new(lexer,emitter);
                parse.Program(); //Start the parser.
                emitter.WriteFile(); //write the output to the file
                Console.WriteLine("Parsing completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Environment.Exit(1);
                return;
            }
        }
    }
}