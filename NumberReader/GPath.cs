using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScreenInput;

using System.Numerics;
using System.Drawing;

namespace NumberReader
{
    internal class GPath : IGuesser
    {
        static int it = 0;

        private enum Shape
        {
            O, S, I, M
        }

        public char GuessNumber(ref Island orig)
        {
            Point start = getStart(ref orig);
            List<Vector2> perimeter = tracePerimeter(ref orig, start);
            return ' ';
        }

        private List<Vector2> tracePerimeter(ref Island orig, Point start)
        {
            Bitmap debugImg = orig.ToBitmap();

            List<Vector2> path = new List<Vector2>();
            const float THRESHOLD = 3f;
            double tsq = Math.Pow(THRESHOLD, 2);
            const int precision = 3;
            int w = orig.GetWidth(), h = orig.GetHeight();
            const float TURN = 3.1415f / 35f;

            Vector2 startingPos = new Vector2(start.X, start.Y);
            Vector2 startingDir = new Vector2(precision, 0);
            Vector2 startingPDir = new Vector2(0, precision);

            float angle = 0f;
            Vector2 currPos = startingPos;
            Vector2 currDir = startingDir;
            Vector2 currPDir = startingPDir;
            bool[,] m = orig.Morph;
            bool lastFailed = true;
            int tries = 500;
            Color c = Color.FromArgb(255, 255, 0, 0);
            while ((Vector2.DistanceSquared(currPos, startingPos) > tsq || path.Count < THRESHOLD) && tries > 0)
            {
                Vector2 testPos = currPos + currDir;
                int x1 = (int) Math.Round(testPos.X), x2 = (int) Math.Round(testPos.X - currPDir.X);
                int y1 = (int) Math.Round(testPos.Y), y2 = (int) Math.Round(testPos.Y - currPDir.Y);
                bool outside = inBounds(x1, 0, w-1) && inBounds(y1, 0, h-1) ? !m[x1, y1] : true;
                bool inside = inBounds(x2, 0, w - 1) && inBounds(y2, 0, h - 1) ? m[x2, y2] : false;
                if (outside && inside)
                {
                    if (lastFailed) path.Add(new Vector2(currDir.X, currDir.Y));
                    currPos = testPos;
                    const int j = 2;
                    c = Color.FromArgb(c.A, c.R<j? 0 : c.R-j, c.G, c.B>255-j? 255: c.B+j);
                    if (inBounds(x1, 0, w - 1) && inBounds(y1, 0, h - 1))
                        debugImg.SetPixel(x1, y1, c);
                    if (inBounds(x2, 0, w - 1) && inBounds(y2, 0, h - 1))
                        debugImg.SetPixel(x2, y2, Color.Green);
                    lastFailed = false;
                }
                else
                {
                    if(outside && !inside)
                        angle -= TURN;
                    else
                        angle += TURN;
                    Matrix3x2 turn = Matrix3x2.CreateRotation(angle);
                    currDir = Vector2.Transform(startingDir, turn);
                    currPDir = Vector2.Transform(startingPDir, turn);
                    lastFailed = true;
                }
                tries--;
                //Console.WriteLine(currPos.X + " " + currPos.Y + " | " + currDir.X + " " + currDir.Y);
            }

            debugImg.SetPixel(start.X, start.Y, Color.Yellow);
            debugImg.Save(@"C:\workspace\CSharp\Playground\NumberReaderTests\paths\" + it + " - "+tries+".bmp");
            it++;
            return path;
        }

        private bool inBounds(int a, int l, int r)
        {
            return a >= l && a <= r;
        }

        private Point getStart(ref Island orig)
        {
            int x = 0;
            while (!orig.Morph[x, 0])
                x++;

            return new Point(x, 0);
        }
    }
}
