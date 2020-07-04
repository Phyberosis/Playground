using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileInput
{
    public class FileProcessor
    {
        const string NR_TESTFOLDER = @"C:\workspace\CSharp\Playground\NumberReaderTests";
        public KeyValuePair<Bitmap, string>[] GetNumberReaderTests()
        {
            string folder = NR_TESTFOLDER;
            string[] files = Directory.GetFiles(folder);
            KeyValuePair<Bitmap, string>[] testCases = new KeyValuePair<Bitmap, string>[files.Length];
            for(int i = 0; i<files.Length; i++)
            {
                string[] split = files[i].Split('\\');
                string name = split[split.Length-1].Substring(0, split[split.Length-1].Length - 4);
                Bitmap b = new Bitmap(Image.FromFile(files[i]));
                testCases[i] = new KeyValuePair<Bitmap, string>(b, name);
                //Console.WriteLine(name);
            }

            return testCases;
        }

        public static void SavePolarized(bool[,] b, string name)
        {
            Bitmap img = new Bitmap(b.GetLength(0), b.GetLength(1));
            for (int x = 0; x < img.Width; x++)
                for (int y = 0; y < img.Height; y++)
                {
                    Color c = b[x, y] ? Color.White : Color.Black;
                    img.SetPixel(x, y, c);
                }
            img.Save(NR_TESTFOLDER + "\\out\\" + name + ".bmp");
        }
    }
}
