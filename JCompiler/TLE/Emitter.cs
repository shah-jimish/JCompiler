using System.Text;

namespace JCompiler.TLE
{
    public class Emitter
    {
        private readonly string fullPath;
        private readonly StringBuilder codes;
        private readonly StringBuilder header;

        public Emitter(string fullPath, string initialCode = "")
        {
            this.fullPath = fullPath ?? throw new ArgumentNullException(nameof(fullPath));
            codes = new StringBuilder(initialCode);
            header = new StringBuilder();
        }

        public void Emit(string code)
        {
            if (code == null) throw new ArgumentNullException(nameof(code));
            codes.Append(code);
        }

        public void EmitLine(string code)
        {
            if (code == null) throw new ArgumentNullException(nameof(code));
            codes.AppendLine(code);
        }

        public void HeaderLine(string code)
        {
            if (code == null) throw new ArgumentNullException(nameof(code));
            header.AppendLine(code);
        }

        public void WriteFile()
        {
            try
            {
                using (var writer = new StreamWriter(fullPath, false, Encoding.UTF8))
                {
                    writer.Write(header.ToString());
                    writer.Write(codes.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to write to file: {ex.Message}");
                // Consider logging the exception or rethrowing if needed
            }
        }
    }
}
