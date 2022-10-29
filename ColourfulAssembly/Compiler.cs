using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

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

        /// <summary>
        /// Returns a string of C++ code of the compiled (transpiled) program
        /// </summary>
        /// <returns></returns>
        public string Compile() 
        {
            //preprocess common string pattern commands for optimisation
            Console.WriteLine("\nPreprocessing commands...");
            string[] strComs = PreprocessCommands();
            Console.WriteLine("\nPreprocessed commands: ");
            for (int i = 0; i < strComs.Length; i++) 
            {
                Console.WriteLine(strComs[i]);
            }

            //loop through each command and process them
            for (int i = 0; i < strComs.Length; i++) 
            {
                currentCommand = i;
                bool isRegister = CheckRegister(new Colour(strComs[i]), out _);
                if (!isRegister) 
                {
                    compiledCode += strComs[i];
                }
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

        /// <summary>
        /// Returns a list of strings containing pre-generated string code along with the rest of the commands.
        /// Looks for commands in a pattern of nFF, nC0; n > 1 where n is the number of given registers used.
        /// Pattern example: FF FF C0 C0
        /// </summary>
        private string[] PreprocessCommands()
        {
            string prevReg = "";

            int assignsFound = 0;
            int printsFound = 0;
            List<Colour> APPairs = new();
            int startIndex = -1;

            //commands in string form, those that are part of a string will be removed
            List<string> strCommands = new();

            for (int i = 0; i < commands.Count; i++) 
            {
                CheckRegister(commands[i], out string reg, false);
                strCommands.Add(commands[i].colour);

                if (reg == "FF") //assign register
                {
                    //not found prints, add this assign
                    if (printsFound == 0)
                    {
                        //add assign to pair
                        APPairs.Add(commands[i]);
                        assignsFound++;
                        if (startIndex == -1) 
                        {
                            startIndex = strCommands.Count - 1;
                        }
                    }
                    else if (printsFound < 2) //found pair either doesn't exist or isn't valid
                    {
                        //clear current pair info
                        assignsFound = 0;
                        printsFound = 0;
                        APPairs.Clear();
                        startIndex = -1;
                    }
                    else //found valid pair, process pair for string
                    {
                        bool hasString = TryParseAPPair(APPairs, out string code, out int startingIndex, out int matches);
                        if (hasString)
                        {
                            strCommands.Insert(startIndex + startingIndex, code);
                            strCommands.RemoveRange(startIndex + startingIndex + 1, matches);

                            //clear current pair info
                            assignsFound = 0;
                            printsFound = 0;
                            APPairs.Clear();
                            startIndex = -1;

                            Console.WriteLine("Found paired assignment and print registers:");
                            APPairs.ForEach((i) => Console.WriteLine(i.colour));
                            Console.WriteLine("Found matching variable addresses." +
                                "\nOptimised string code:\n" + code);
                        }
                    }
                }
                else if (reg == "C0") //print register
                {
                    //enough assigns found to potentially make up a pair
                    if (prevReg == "FF" && assignsFound > 1)
                    {
                        APPairs.Add(commands[i]);
                        printsFound++;
                    }
                    else if (printsFound > 0) //if a print has already been found, add this
                    {
                        APPairs.Add(commands[i]);
                        printsFound++;
                    }
                    else //this is a random print
                    {
                        //clear current pair info
                        assignsFound = 0;
                        printsFound = 0;
                        APPairs.Clear();
                        startIndex = -1;
                    }

                    if (i == commands.Count - 1 && printsFound > 1)
                    {
                        bool hasString = TryParseAPPair(APPairs, out string code, out int startingIndex, out int matches);
                        if (hasString)
                        {
                            strCommands.Insert(startIndex + startingIndex, code);
                            strCommands.RemoveRange(startIndex + startingIndex + 1, matches);

                            //clear current pair info
                            assignsFound = 0;
                            printsFound = 0;
                            APPairs.Clear();
                            startIndex = -1;

                            Console.WriteLine("Found paired assignment and print registers:");
                            APPairs.ForEach((i) => Console.WriteLine(i.colour));
                            Console.WriteLine("Found matching variable addresses." +
                                "\nOptimised string code:\n" + code);
                        }
                    }
                }
                else //different unrelated register
                {
                    if (printsFound > 1) //found valid pair, process pair for string
                    {
                        bool hasString = TryParseAPPair(APPairs, out string code, out int startingIndex, out int matches);
                        if (hasString)
                        {
                            strCommands.Insert(startIndex + startingIndex, code);
                            strCommands.RemoveRange(startIndex + startingIndex + 1, matches);

                            //clear current pair info
                            assignsFound = 0;
                            printsFound = 0;
                            APPairs.Clear();
                            startIndex = -1;

                            Console.WriteLine("Found paired assignment and print registers:");
                            APPairs.ForEach((i) => Console.WriteLine(i.colour));
                            Console.WriteLine("Found matching variable addresses." +
                                "\nOptimised string code:\n" + code);
                        }
                    }
                    else //no pair found, random register
                    {
                        //clear current pair info
                        assignsFound = 0;
                        printsFound = 0;
                        APPairs.Clear();
                        startIndex = -1;
                    }
                }

                prevReg = reg;
            }

            return strCommands.ToArray();
        }

        private bool TryParseAPPair(List<Colour> APPairs, out string code, out int startIndex, out int length)
        {
            startIndex = 0;
            length = 0;
            int matches = 0;
            bool stringFound = false;
            List<Colour> stringCommands = new();

            List<(Colour, bool)> assigns = new();
            List<(Colour, bool)> prints = new();

            List<Colour> sA = new();
            List<Colour> sP = new();

            code = "";

            foreach (var col in APPairs) 
            {
                if (col.R() == "FF")
                {
                    assigns.Add((col, false));
                }
                else 
                {
                    prints.Add((col, false));
                }
            }

            for (int i = 0; i < assigns.Count; i++) 
            {
                for (int j = 0; j < prints.Count; j++) 
                {
                    if (assigns[i].Item1.GreenValue() == prints[j].Item1.GreenValue())
                    {
                        if (!stringFound) startIndex = i;
                        stringFound = true;
                        matches++;
                        assigns[i] = (assigns[i].Item1, true);
                        prints[j] = (prints[j].Item1, true);

                        sA.Add(assigns[i].Item1);
                        sP.Add(prints[j].Item1);

                        if (i == assigns.Count - 1 || j == prints.Count) 
                        {
                            //string found!
                            List<Colour> cols = new();
                            sA.ForEach((i) => cols.Add(i));
                            sP.ForEach((i) => cols.Add(i));
                            length = cols.Count;

                            code = GetStringCode(cols);
                        }
                    }
                    else if (matches > 1)
                    {
                        //string found!
                        List<Colour> cols = new();
                        sA.ForEach((i) => cols.Add(i));
                        sP.ForEach((i) => cols.Add(i));
                        length = cols.Count;

                        code = GetStringCode(cols);
                    }
                    else 
                    {
                        if (!assigns[i].Item2 && !prints[j].Item2)
                        {
                            //not part of a string
                            stringFound = false;
                            startIndex = 0;
                            matches = 0;

                            sA.Clear();
                            sP.Clear();
                        }
                    }
                }
            }

            return stringFound;
        }

        private string GetStringCode(List<Colour> cols)
        {
            List<char> chars = new();
            string varName = GetUniqueVarName();

            for (int i = 0; i < cols.Count; i++) 
            {
                if (cols[i].R() == "FF")
                {
                    chars.Add(Convert.ToChar(cols[i].BlueValue()));
                }
                else 
                {
                    break;
                }
            }
            string charsString = new string(chars.ToArray());

            string code = $"{indent}std::string v_{varName} = \"{charsString}\";" +
                $"\n{indent}std::cout << v_{varName};\n";

            return code;
        }


        /// <summary>
        /// Returns a UUID with the - characters removed to be suitable as a variable name.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Returns values to be used as variable address / value.
        /// If the CC parameter was specified, return the value from the following colour.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
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
