using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Playground
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            string rp = @"C:\Users\phybe\Documents\UBC2019W\CPSC221\pa3\pa3\Rbuild.txt";
            string tp = @"C:\Users\phybe\Documents\UBC2019W\CPSC221\pa3\pa3\Tbuild.txt";

            string rTest = File.ReadAllText(rp);
            string tTest = File.ReadAllText(tp);

            StringReader rr = new StringReader(rTest);
            StringReader tr = new StringReader(tTest);

            HashSet<string> set = new HashSet<string>();

            string line = null;
            while(true)
            {
                line = tr.ReadLine();
                if (line == null) break;
                line = line.Substring(0, line.IndexOf('|') - 1);

                set.Add(line);
            }
            Console.WriteLine(set.Count);
            while (true)
            {
                line = rr.ReadLine();
                if (line == null) break;

                
                set.Remove(line);
            }

            Console.WriteLine(set.Count);
            Console.ReadLine();
        }
    }
}
