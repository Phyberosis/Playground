using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenInput
{
    public struct Island
    {
        public bool[,] Morph;
        public Point Loc;

        public Island(Island other)
        {
            int width = other.Morph.GetLength(0), height = other.Morph.GetLength(1);
            Morph = new bool[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    Morph[x, y] = other.Morph[x, y];
                }
            Loc = new Point(other.Loc.X, other.Loc.Y);
        }

        public int GetWidth()
        {

            return Morph.GetLength(0);
        }

        public int GetHeight()
        {
            return Morph.GetLength(1);
        }

        public Island(bool[,] morph, Point loc)
        {
            Morph = morph;
            Loc = loc;
        }

        public Island(bool[,] morph, int x, int y) : this(morph, new Point(x, y))
        { }

        public void Negate()
        {
            int width = Morph.GetLength(0), height = Morph.GetLength(1);
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    Morph[x, y] = !Morph[x, y];
                }
        }

        public void Grow()
        {
            HashSet<Point> visited = new HashSet<Point>();
            Queue<Point> toVisit = new Queue<Point>();
            int width = Morph.GetLength(0), height = Morph.GetLength(1);
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    if (Morph[x, y])
                    {
                        toVisit.Enqueue(new Point(x, y));
                        x = width;
                        break;
                    }
                }

            while (toVisit.Count > 0)
            {
                Point p = toVisit.Dequeue();
                if (visited.Contains(p)) continue;
                int x = p.X, y = p.Y;
                if (x < 0 || y < 0 || x >= width || y >= height) continue;

                visited.Add(p);
                if (Morph[x, y])
                {
                    toVisit.Enqueue(new Point(x - 1, y));
                    toVisit.Enqueue(new Point(x + 1, y));
                    toVisit.Enqueue(new Point(x, y - 1));
                    toVisit.Enqueue(new Point(x, y + 1));
                }
                Morph[x, y] = true;
            }
        }

        public void PerimeterFill()
        {
            HashSet<Point> visited = new HashSet<Point>();
            Queue<Point> toVisit = new Queue<Point>();
            int width = Morph.GetLength(0), height = Morph.GetLength(1);
            toVisit.Enqueue(new Point(-1, -1));

            while(toVisit.Count > 0)
            {
                Point p = toVisit.Dequeue();
                if (visited.Contains(p)) continue;
                int x = p.X, y = p.Y;
                if (x < -1 || y < -1 || x > width || y > height) continue;  // outer perimeter
                visited.Add(p);

                bool inPerimeter = (x < 0 || y < 0 || x >= width || y >= height);

                if(inPerimeter || !Morph[x,y])
                {
                    if(!inPerimeter)
                        Morph[x, y] = true;
                    toVisit.Enqueue(new Point(x - 1, y));
                    toVisit.Enqueue(new Point(x + 1, y));
                    toVisit.Enqueue(new Point(x, y - 1));
                    toVisit.Enqueue(new Point(x, y + 1));
                }
            }
        }

        public List<Island> GetInnerIslands(int minSize, int maxSize = -1)
        {
            HashSet<Point> visited = new HashSet<Point>();
            List<Island> islands = new List<Island>();
            int width = Morph.GetLength(0), height = Morph.GetLength(1);
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    Point p = new Point(x, y);
                    if (visited.Contains(p)) continue;
                    if (Morph[x, y])
                    {
                        Island island = getIsland(Morph, x, y, ref visited);
                        if (island.Morph.Length >= minSize &&
                            (maxSize == -1 || island.Morph.Length <= maxSize))
                            islands.Add(island);
                    }
                    visited.Add(p);
                }

            return islands;
        }

        private Island getIsland(bool[,] b, int fx, int fy, ref HashSet<Point> visited)
        {
            Queue<Point> isWhite = new Queue<Point>();
            Queue<Point> toVisit = new Queue<Point>();
            toVisit.Enqueue(new Point(fx, fy));
            int width = b.GetLength(0), height = b.GetLength(1);

            int minX = width, maxX = 0;
            int minY = height, maxY = 0;
            while (toVisit.Count > 0)
            {
                Point p = toVisit.Dequeue();
                if (visited.Contains(p)) continue;
                int x = p.X, y = p.Y;
                if (x < 0 || y < 0 || x >= width || y >= height) continue;
                visited.Add(p);
                    
                if (b[x, y])
                {
                    minX = x < minX ? x : minX;
                    maxX = x > maxX ? x : maxX;
                    minY = y < minY ? y : minY;
                    maxY = y > maxY ? y : maxY;

                    isWhite.Enqueue(p);

                    toVisit.Enqueue(new Point(x - 1, y));
                    toVisit.Enqueue(new Point(x + 1, y));
                    toVisit.Enqueue(new Point(x, y - 1));
                    toVisit.Enqueue(new Point(x, y + 1));
                }
            }

            int iw = maxX - minX + 1, ih = maxY - minY + 1;
            bool[,] morph = new bool[iw, ih];
            foreach (Point p in isWhite)
            {
                morph[p.X - minX, p.Y - minY] = true;
            }

            return new Island(morph, minX, minY);
        }

        public Bitmap ToBitmap()
        {
            Bitmap b = new Bitmap(GetWidth(), GetHeight());
            for (int x = 0; x < GetWidth(); x++)
                for (int y = 0; y < GetHeight(); y++)
                {
                    Color c = Morph[x, y] ? Color.White : Color.Black;
                    b.SetPixel(x, y, c);
                }

            return b;
        }
    }
}
