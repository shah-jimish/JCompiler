using JCompiler.TLE;
using System.Diagnostics;

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
                Emitter emitter = new("out.c");
                Lexer lexer = new(source,emitter);
                Parser parse = new(lexer, emitter);
                parse.Program(); //Start the parser.
                emitter.WriteFile(); //write the output to the file
                Console.WriteLine("Parsing completed.");

                //run the generated `c` code
                /*
                    step 1 - install the min-gw compiler
                    step 2 - run the `gcc out.c` command  --> it will generate e.exe file
                    step 3 - run a.exe 
                */
                //RunGCCCompiler();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Environment.Exit(1);
                return;
            }
        }
        public static void RunGCCCompiler()
        {
            try
            {
                // 1. Compile the C code using gcc
                string mingwBinPath = @"C:\MinGW\bin";
                //this is output file path in my local this code is in another directory because access issue 
                // i was unable to run this code so i've changed the output to desktop
                string outputPath = @"C:\Users\shahj\Desktop\output\out.exe";
                Process compileProcess = new();
                compileProcess.StartInfo.FileName = "gcc";
                compileProcess.StartInfo.Arguments = $"out.c -o {outputPath}";
                compileProcess.StartInfo.RedirectStandardOutput = true;
                compileProcess.StartInfo.RedirectStandardError = true;
                compileProcess.StartInfo.UseShellExecute = false;
                compileProcess.StartInfo.CreateNoWindow = true;

                // Set the PATH environment variable explicitly
                compileProcess.StartInfo.EnvironmentVariables["PATH"] = Environment.GetEnvironmentVariable("PATH");

                compileProcess.Start();
                compileProcess.WaitForExit();

                if (compileProcess.ExitCode != 0)
                {
                    Console.WriteLine("Error compiling C code:");
                    Console.WriteLine(compileProcess.StandardError.ReadToEnd());
                    return;
                }

                // 2. Execute the compiled C program
                Process runProcess = new();
                runProcess.StartInfo.FileName = outputPath;
                runProcess.StartInfo.UseShellExecute = true;
                runProcess.Start();
                runProcess.WaitForExit();
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