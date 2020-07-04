using ScreenInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberReader
{
    internal class GSevenSegments : IGuesser
    {
        public char GuessNumber(ref Island orig)
        {
            //Console.WriteLine("by path");
            Point[] pts = gnGenCorners(ref orig);
            bool[] segs =   // like 7 seg display
            {
                gnHasPath(ref orig, pts[0], pts[1]),    // [' ] 0
                gnHasPath(ref orig, pts[0], pts[3]),    // [``] 1
                gnHasPath(ref orig, pts[3], pts[4]),    // [ '] 2
                gnHasPath(ref orig, pts[1], pts[4]),    // [--] 3
                gnHasPath(ref orig, pts[1], pts[2]),    // [, ] 4
                gnHasPath(ref orig, pts[2], pts[5]),    // [__] 5
                gnHasPath(ref orig, pts[4], pts[5])     // [ ,] 6
            };

            const char T = (char)1, F = (char)0;
            char[,] keys =
            {
                { F, T, T, F, F, F, T, '7' },    // 7
                { F, T, T, T, T, T, F, '2' },    // 2
                { F, T, T, T, F, T, T, '3' },    // 3
                { T, T, F, T, F, T, T, '5' },    // 5
                { T, T, T, T, T, T, T, '1' },    // 1
                { T, F, T, T, F, F, T, '4' },    // 4
            };

            char best = ' ';
            int bestMatches = 0;
            for (int t = 0; t < keys.GetLength(0); t++)
            {
                int matches = 0;
                for (int i = 0; i < keys.GetLength(1) - 1; i++)
                {
                    if (segs[i] == (keys[t, i] == T)) matches++;
                    //if (t == 0)
                    //    Console.Write(segs[i] + " ");
                }
                if (matches > bestMatches)
                {
                    best = keys[t, 7];
                    bestMatches = matches;
                }
            }

            //Console.WriteLine();
            //Console.WriteLine(best + ", " + bestMatches / 7.0 * 100.0 + "%");
            //printIsland(ref orig, ref pts);
            //Console.ReadLine();
            return bestMatches >= 7 ? best : ' ';

        }

        private Point[] gnGenCorners(ref Island orig)
        {
            //Console.WriteLine("gen corners");
            int w = orig.GetWidth();
            int h = orig.GetHeight();
            int m = h / 2;

            bool[,] morph = orig.Morph;
            Point[] pts = {
                new Point(0, 0), new Point(0, m), new Point(0, h),
                new Point(w, 0), new Point(w, m), new Point(w, h)
            };
            Point[] dirs = {
                new Point(2, 1), new Point(1, 0), new Point(2, -1),
                new Point(-2, 1), new Point(-1, 0), new Point(-2, -1)
            };

            const int padding = 2;
            for (int i = 0; i < dirs.Length; i++)
            {
                Point d = dirs[i];
                int l = 0;

                enforceBounds(ref pts[i], w, h, padding);
                while (!morph[pts[i].X, pts[i].Y] && l < w / 3)
                {
                    pts[i].X += d.X;
                    pts[i].Y += d.Y;
                    l += Math.Abs(d.X);
                    enforceBounds(ref pts[i], w, h, padding);
                    //Console.WriteLine(l);
                }
            }

            return pts;
        }

        private void enforceBounds(ref Point p, int w, int h, int padding)
        {
            p.X = p.X < padding ? padding : p.X;
            p.X = p.X >= w - padding ? w - padding - 1 : p.X;
            p.Y = p.Y < padding ? padding : p.Y;
            p.Y = p.Y >= h - padding ? h - padding - 1 : p.Y;
        }

        private bool gnHasPath(ref Island orig, Point a, Point b)
        {
            bool debug = false;
            if(debug)
                Console.WriteLine("has path" + a + " " + b);

            if (!orig.Morph[a.X, a.Y] || !orig.Morph[b.X, b.Y]) return false;

            const int res = 10; // checks along path
            double dx = (b.X - a.X) / (double)res;
            double dy = (b.Y - a.Y) / (double)res;

            int w = orig.GetWidth(), h = orig.GetHeight();
            if (dx * res < w / 5 && dy * res < h / 5) return false;

            int[,] rad =
            {
                { 0, 0 },
                { 1, 0 }, { 2, 0 },
                { 0, 1 }, { 0, 2 },
                {-1, 0 }, {-2, 0 },
                { 0,-1 }, { 0,-2 }
            };
            int x = 0, y = 0;
            for (int i = 0; i <= res; i++)
            {
                x = a.X + (int)Math.Round(i * dx);
                y = a.Y + (int)Math.Round(i * dy);

                if (debug)
                {
                    Console.WriteLine(x + " " + y);// + "|" + dx + " " + dy);
                    Point[] pts = { a, b, };
                    NumberReader.PrintIsland(ref orig, ref pts, x, y);
                    //debug = !(Console.ReadLine().Contains("x"));
                    string debugL = Console.ReadLine();
                    debug = !debugL.Contains("x");
                }

                bool ok = false;
                for (int s = 0; s < rad.GetLength(0); s++)
                {
                    int tx = x + rad[s, 0], ty = y + rad[s, 1];
                    if (tx >= 0 && ty >= 0 && tx < w && ty < h &&
                        orig.Morph[tx, ty])
                    {
                        ok = true;
                        break;
                    }
                }

                if (!ok) return false;
            }

            return true;
        }
    }
}
