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
                List<Colour> commands = proc.ProcessScript();
                commands.ToList().ForEach((c) => Console.WriteLine(c.colour));
                Compiler comp = new Compiler(commands);
                string code = comp.Compile();
                Console.WriteLine("\nTranspiled C++ Code:\n\n" + code);
            }
            else 
            {
                Console.WriteLine($"Couldn't find file at {filePath}");
            }
        }
    }
}
