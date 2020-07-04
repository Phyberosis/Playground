using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ScreenInput
{
    public class ScreenProcessor
    {
        Bitmap img;
        ImageStats stats;

        static int InstanceCount = 0;
        private int it;

        public ScreenProcessor(): this(0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height)
        { }

        public ScreenProcessor(int x, int y, int w, int h): this (new Point(x, y), new Size(w, h))
        { }

        public ScreenProcessor(Point loc, Size s)
        {
            img = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics g = Graphics.FromImage(img);
            g.CopyFromScreen(loc.X, loc.Y, 0, 0, s);

            init();
        }

        public ScreenProcessor(ref Bitmap b)
        {
            img = b;
            init();
        }

        private void init()
        {
            stats = new ImageStats(ref img);

            InstanceCount++;
            it = InstanceCount;
        }

        private void savePolarized(bool[,] b, string path)
        {
            Bitmap img = new Bitmap(b.GetLength(0), b.GetLength(1));
            for (int x = 0; x < img.Width; x++)
                for (int y = 0; y < img.Height; y++)
                {
                    Color c = b[x, y] ? Color.White : Color.Black;
                    img.SetPixel(x, y, c);
                }
            img.Save(path);
        }

        public Island Bipolarize(float threshold)
        {
            Bitmap b = new Bitmap(img.Width, img.Height);
            long ss = 0;
            int r = 1;
            for (int x = r; x < b.Width - r; x++)
                for (int y = r; y < b.Height - r; y++)
                {
                    Color c = getColorSample(x, y, r);
                    b.SetPixel(x, y, c);
                    //ss += c.R ^ 2 + c.G ^ 2 + c.B ^ 2;
                    ss += Math.Max(c.R, Math.Max(c.G, c.B));
                }

            int sum = b.Width * b.Height;
            float av = ss / sum;
            bool[,] polarized = new bool[b.Width, b.Height];
            for (int x = 0; x < b.Width; x++)
                for (int y = 0; y < b.Height; y++)
                {
                    Color c = b.GetPixel(x, y);
                    int maxV = Math.Max(c.R, Math.Max(c.G, c.B));
                    Color bg = getColorSample(x, y, r + 6);
                    int locAv = Math.Max(bg.R, Math.Max(bg.G, bg.B));
                    //if (maxV >= av + threshold) // brighter than average 
                    if (maxV >= locAv + (threshold / 3) || maxV >= av + threshold) // brighter than local average
                        polarized[x, y] = true;
                    else
                        polarized[x, y] = false;
                }
            //FileInput.FileProcessor.SavePolarized(polarized,  "biPolTest");
            b.Save(@"C:\workspace\CSharp\Playground\NumberReaderTests\out\blured" + it + @".bmp");
            savePolarized(polarized, @"C:\workspace\CSharp\Playground\NumberReaderTests\out\biPol" + it + @".bmp");
            return new Island(polarized, 0, 0);
        }

        private Color getColorSample(int fx, int fy, int r)
        {
            Point ul = new Point(fx - r, fy - r);
            Point lr = new Point(fx + r, fy + r);
            return stats.getAvg(ul, lr);
        }

        private void Save()
        {
            img.Save("test.bmp");
        }
    }
}
