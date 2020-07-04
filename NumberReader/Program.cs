using FileInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberReader
{
    class Program
    {
        static void Main(string[] args)
        {
            FileProcessor fp = new FileProcessor();
            NumberReader nr = new NumberReader();
            KeyValuePair<Bitmap, string>[] testCases = fp.GetNumberReaderTests();
            foreach(KeyValuePair<Bitmap, string> test in testCases)
            {
                Console.WriteLine("Test: " + test.Value + "");
                Bitmap b = new Bitmap(test.Key);
                Console.WriteLine(nr.ReadFromImage(ref b));
                //Console.ReadLine();
            }
   
        }
    }
}
