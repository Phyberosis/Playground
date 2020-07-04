using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileInput
{
    class Program
    {
        static void Main(string[] args)
        {
            FileProcessor fp = new FileProcessor();
            fp.GetNumberReaderTests();

            Console.Read();
        }
    }
}
