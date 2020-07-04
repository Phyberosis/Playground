using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;

namespace AsciiArt
{
    class Program
    {
        const int PixSize = 4;
        const int ss = PixSize * PixSize;
        const string Path = @"C:\workspace\CSharp\Playground\AsciiArt\";
        const string name = "img.png";

        List<Pair> Levels = new List<Pair>();

        static void Main(string[] args)
        {
            Program p = new Program();
            p.run();
            Console.WriteLine("done");
            Console.ReadLine();
        }

        public void run()
        {
            initLevels();

            Bitmap img = new Bitmap(Path + name);
            Console.WriteLine(img.Width + " x " + img.Height);

            StringBuilder sb = new StringBuilder();

            int y = 0;
            int x = 0;
            while(y <= img.Height - PixSize)
            {
                while(x <= img.Width - PixSize)
                {
                    int r = 0;
                    int g = 0;
                    int b = 0;

                    for (int dy = 0; dy < PixSize; dy++)
                    {
                        for (int dx = 0; dx < PixSize; dx++)
                        {
                            Color c = img.GetPixel(x + dx, y + dy);
                            r += c.R;
                            g += c.G;
                            b += c.B;
                        }
                    }

                    sb.Append(getpix((r + g + b) / (3 * ss)));

                    x += PixSize;
                }
                sb.Append("|\r\n");
                y += PixSize;
                x = 0;
            }

            string ascii = sb.ToString();
            File.WriteAllText(Path + "ascii.txt", ascii);
            Console.WriteLine(ascii);
        }

        struct Pair
        {
            public int Threshold;
            public string Val;

            public Pair(int t, string v)
            {
                Threshold = t;
                Val = v;
            }
        }

        void initLevels()
        {
            Levels.Add(new Pair(220, "  "));
            Levels.Add(new Pair(190, ". "));
            Levels.Add(new Pair(160, "/ "));
            Levels.Add(new Pair(130, "/."));
            Levels.Add(new Pair(100, "//"));
            Levels.Add(new Pair(70, "X/"));
            Levels.Add(new Pair(35, "XX"));
        }

        private string getpix(int v)
        {
            foreach(Pair p in Levels)
            {
                if (v > p.Threshold)
                    return p.Val;
            }

            return "▒▒";
        }
    }
}
