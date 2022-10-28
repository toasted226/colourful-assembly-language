using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColourfulAssembly
{
    public class ScriptProcessor
    {
        public string filePath { get; set; }

        public ScriptProcessor(string filePath)
        {
            this.filePath = filePath;
        }

        public List<Colour> ProcessScriptAsText() 
        {
            List<Colour> colours = new();
            File.ReadAllLines(filePath).ToList().ForEach((s) => colours.Add(new Colour(s[1..])));
            return colours;
        }

        public List<Colour> ProcessScript() 
        {
            List<Colour> colours = new();
            GetPixels().ToList().ForEach((p) => colours.Add(new Colour(p)));
            return colours;
        }

        public string[] GetPixels()
        {
            List<string> pixels = new List<string>();

            if (File.Exists(filePath))
            {
                var bitmap = new Bitmap(filePath);

                for (int x = 0; x < bitmap.Width; x++)
                {
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        var pixel = bitmap.GetPixel(y, x);
                        string pixelData = ColorTranslator.ToHtml(pixel);
                        //if the pixel is not fully opaque, it is not a command
                        if (pixel.A == 255)
                        {
                            string command = pixelData[1..];
                            pixels.Add(command);
                        }
                    }
                }
            }
            else
            {
                throw new FileNotFoundException("Could not find a file at path " + filePath);
            }

            return pixels.ToArray();
        }
    }
}
