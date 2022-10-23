using System.Globalization;

namespace ColourfulAssembly
{
    public class Compiler
    {
        public List<Colour> commands { get; set; }

        private const string code_InNext = "CC";
        private string indent = "    ";
        private int currentCommand;
        private string? compiledCode;

        public Compiler(List<Colour> commands) 
        {
            this.commands = commands.ToList();
        }

        //returns a string of C++ code of the compiled (transpiled) program
        public string Compile() 
        {
            //loop through each command and process them
            for (int i = 0; i < commands.Count; i++) 
            {
                currentCommand = i;
                CheckRegister(commands[i], out _);
            }

            //boilerplate C++ code plus given compiled code
            string code = 
                $"#include<iostream>\n" +
                $"int main()\n" +
                "{\n" +
                $"{compiledCode}\n" +
                $"    return 0;\n" +
                "}";

            return code;
        }

        public string GetUniqueVarName() 
        {
            string uuid = Guid.NewGuid().ToString();
            for (int i = 0; i < uuid.Length; i++) 
            {
                if (uuid[i] == '-') 
                {
                    uuid = uuid.Remove(i, 1);
                }
            }

            return uuid;
        }

        /// <summary>
        /// Returns true if the given colour is a register and passes out the register it was identified as.
        /// Calls the method linked to the identified register by default, can be disabled.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="register"></param>
        /// <param name="callRegister"></param>
        /// <returns></returns>
        public bool CheckRegister(Colour command, out string register, bool callRegister = true) 
        {
            string r = command.R();
            bool isRegister = false;

            switch (r)
            {
                case "FF":
                    isRegister = true;
                    register = "FF";
                    if (callRegister) Assignment(command);
                    break;
                case "FA":
                    isRegister = true;
                    register = "FA";
                    if (callRegister) Addition(command);
                    break;
                case "F0":
                    isRegister = true;
                    register = "F0"; 
                    if (callRegister) Subtraction(command);
                    break;
                case "CA":
                    isRegister = true;
                    register = "CA";
                    if (callRegister) Input(command);
                    break;
                case "C0":
                    isRegister = true;
                    register = "C0";
                    if (callRegister) Output(command);
                    break;
                case "FE":
                    isRegister = true;
                    register = "FE";
                    if (callRegister) LoopOpen(command);
                    break;
                case "CE":
                    isRegister = true;
                    register = "CE";
                    if (callRegister) ScopeClose(command);
                    break;
                default:
                    register = string.Empty;
                    break;
            }

            return isRegister;
        }

        public int[] CheckInNext(Colour command) 
        {
            int g = command.GreenValue();
            int b = command.BlueValue();
            int nextIndex = currentCommand + 1;

            if (command.G() == code_InNext)
            {
                g = commands[nextIndex].WholeValue();
                commands.RemoveAt(nextIndex);
            }
            if (command.B() == code_InNext)
            {
                b = commands[nextIndex].WholeValue();
                commands.RemoveAt(nextIndex);
            }

            return new int[] { g, b };
        }

        public void Assignment(Colour command) 
        {
            int[] vals = CheckInNext(command);

            //eg.
            //int v = 5;
            string line = $"{indent}int v_{vals[0]} = {vals[1]};\n";
            compiledCode += line;
        }

        public void Addition(Colour command)
        {
            int[] vals = CheckInNext(command);

            //eg.
            //v += 5;
            string line = $"{indent}v_{vals[0]} += {vals[1]};\n";
            compiledCode += line;
        }

        public void Subtraction(Colour command)
        {
            int[] vals = CheckInNext(command);

            //eg.
            //v -= 5;
            string line = $"{indent}v_{vals[0]} -= {vals[1]};\n";
            compiledCode += line;
        }

        public void Output(Colour command)
        {
            int[] vals = CheckInNext(command);

            //eg.
            //std::cout << v;
            string line = $"{indent}std::cout << char(v_{vals[0]});\n";
            compiledCode += line;
        }

        //gets only first character from input buffer with stdin
        public void Input(Colour command) 
        {
            int[] vals = CheckInNext(command);

            //eg.
            //char c{};
            //std::cin.get(c);
            //v = int(c);
            string uuid = GetUniqueVarName();
            string line = indent + "char v_" + uuid + "{};" +
                "\n" + indent + "std::cin.get(v_" + uuid + $");" +
                $"\n{indent}v_{vals[0]} = int(v_{uuid});\n";
            compiledCode += line;
        }

        public void LoopOpen(Colour command) 
        {
            int[] vals = CheckInNext(command);

            //eg.
            //for (int i = 0; i < g; i++)
            //{
            string uuid = GetUniqueVarName();
            string line = $"{indent}for (int v_{uuid} = 0; v_{uuid} < v_{vals[0]}; v_{uuid}++)\n{indent}{{\n";
            compiledCode += line;

            //add indent
            indent += "    ";
        }

        public void ScopeClose(Colour command)
        {
            //remove indent
            indent = indent.Substring(indent.Length - 4);

            //eg.
            //}
            string line = indent + "}\n";
            compiledCode += line;
        }
    }
}
