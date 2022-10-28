namespace ColourfulAssembly 
{
    public class Program 
    {
        public static void Main() 
        {
            Console.Write("Enter script file path: ");

            string? filePath = Console.ReadLine();

            if (File.Exists(filePath))
            {
                ScriptProcessor proc = new ScriptProcessor(filePath);
                List<Colour> commands = new();

                if (Path.GetExtension(filePath) == ".txt")
                {
                    commands = proc.ProcessScriptAsText();
                }
                else 
                {
                    commands = proc.ProcessScript();
                }

                commands.ToList().ForEach((c) => Console.WriteLine(c.colour));
                Compiler comp = new Compiler(commands);
                string code = comp.Compile();
                Console.WriteLine("\n############################\n");
                Console.WriteLine("Transpiled C++ Code:\n");
                Console.WriteLine("############################\n");
                Console.WriteLine(code);
                Console.WriteLine("\n############################\n");
            }
            else 
            {
                Console.WriteLine($"Couldn't find file at {filePath}");
            }
        }
    }
}
