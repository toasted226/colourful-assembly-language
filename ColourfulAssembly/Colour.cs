using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColourfulAssembly
{
    public class Colour
    {
        public string colour { get; set; }

        public Colour(string colour)
        {
            string fixedColour = colour;
            if (colour[0] == '#') 
            {
                fixedColour = colour[1..];
            }

            //if (fixedColour.Length != 6) Console.WriteLine($"Warning: {colour} is not a valid colour. Ignoring.");

            this.colour = fixedColour;
        }

        /// <summary>
        /// Returns the Hex value of the Red channel as a string.
        /// </summary>
        /// <returns></returns>
        public string R()
        {
            return colour[0..2];
        }

        /// <summary>
        /// Returns the Hex value of the Green channel as a string.
        /// </summary>
        /// <returns></returns>
        public string G() 
        {
            return colour[2..4];
        }

        /// <summary>
        /// Returns the Hex value of the Blue channel as a string.
        /// </summary>
        /// <returns></returns>
        public string B() 
        {
            return colour[4..6];
        }

        /// <summary>
        /// Returns the Integer value of the Red channel.
        /// </summary>
        /// <returns></returns>
        public int RedValue() 
        {
            return int.Parse(R(), NumberStyles.HexNumber);
        }

        /// <summary>
        /// Returns the Integer value of the Green channel.
        /// </summary>
        /// <returns></returns>
        public int GreenValue() 
        {
            return int.Parse(G(), NumberStyles.HexNumber);
        }

        /// <summary>
        /// Returns the Integer value of the Blue channel.
        /// </summary>
        /// <returns></returns>
        public int BlueValue() 
        {
            return int.Parse(B(), NumberStyles.HexNumber);
        }

        /// <summary>
        /// Returns an int[] containing the Red, Green and Blue channels as Integers respectively.
        /// </summary>
        /// <returns></returns>
        public int[] ChannelValues()
        {
            int rInt = RedValue();
            int gInt = GreenValue();
            int bInt = BlueValue();

            return new int[] { rInt, gInt, bInt };
        }

        /// <summary>
        /// Treats the entire colour as a single Hex value and returns the converted Integer value.
        /// </summary>
        /// <returns></returns>
        public int WholeValue() 
        {
            return int.Parse(colour, NumberStyles.HexNumber);
        }
    }
}
