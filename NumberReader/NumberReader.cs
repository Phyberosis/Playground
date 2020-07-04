using ScreenInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NumberReader
{
    public class NumberReader
    {
        private char it = 'a';

        private class DistsComparer : IComparer<KeyValuePair<char, int[]>>
        {
            public int Compare(KeyValuePair<char, int[]> x, KeyValuePair<char, int[]> y)
            {
                return x.Value[0].CompareTo(y.Value[0]);
            }
        }

        private class LocComparer : IComparer<KeyValuePair<char, int>>
        {
            public int Compare(KeyValuePair<char, int> x, KeyValuePair<char, int> y)
            {
                return x.Value.CompareTo(y.Value);
            }

        }
        public string ReadFromImage(ref Bitmap b)
        {
            ScreenProcessor sp = new ScreenProcessor(ref b);
            
            Island BiPol = sp.Bipolarize(50f);
            List<Island> islands = BiPol.GetInnerIslands(350, 2200);
            List<KeyValuePair<Island, char>> candidates = new List<KeyValuePair<Island, char>>();
            IEnumerator<Island> islandEn = islands.GetEnumerator();
            for (int i = 0; i<islands.Count; i++)
            {
                islandEn.MoveNext();
                Island island = islandEn.Current;
                if (island.GetWidth() * 3f / 2f > island.GetHeight()) continue;
                char guess = doGuess(ref island);
                SavePolarized(island, " (" + it.ToString() + i + ") " + guess.ToString());
                if (!guess.Equals(' '))
                {
                    candidates.Add(new KeyValuePair<Island, char>(island, guess));
                }
            }

            List<KeyValuePair<char, int[]>> byDist = new List<KeyValuePair<char, int[]>>();
            foreach (KeyValuePair<Island, char> elem in candidates)
            {
                int sumDists = 0;
                foreach (KeyValuePair<Island, char> other in candidates)
                {
                    if (elem.Equals(other)) continue;
                    sumDists += Math.Abs(other.Key.Loc.X - elem.Key.Loc.X);
                }
                int[] sumAndLoc = { sumDists, elem.Key.Loc.X };
                byDist.Add(new KeyValuePair<char, int[]>(elem.Value, sumAndLoc));
            }

            byDist.Sort(new DistsComparer());
            List<KeyValuePair<char, int>> byLoc = new List<KeyValuePair<char, int>>();
            for(int i = 0; i<3 && i < byDist.Count; i++)
                byLoc.Add(new KeyValuePair<char, int>(byDist.ElementAt(i).Key, byDist.ElementAt(i).Value[1]));
            byLoc.Sort(new LocComparer());

            string ret = "";
            for (int i = 0; i < 3 && i < byLoc.Count; i++)
                ret += byLoc.ElementAt(i).Key.ToString();

            it++;
            return ret;
        }

        private char doGuess(ref Island orig)
        {
            List<IGuesser> guesers = new List<IGuesser>();
            guesers.Add(new GInnerIslands());
            guesers.Add(new GSevenSegments());

            char attempt = ' ';
            foreach(IGuesser g in guesers)
            {
                attempt = g.GuessNumber(ref orig);
                if (attempt != ' ') break;
            }

            return attempt;
        }

        public static void PrintIsland(ref Island i, ref Point[] pts, int hx = -1, int hy = -1)
        {
            for (int y = 0; y < i.GetHeight(); y++)
            {
                for (int x = 0; x < i.GetWidth(); x++)
                {
                    bool match = false;
                    if (hy == y && hx == x)
                    {
                        Console.Write("^v");
                        match = true;
                    }
                    for (int j = 0; j<pts.Length && !match; j++)
                    {
                        if (pts[j].Y == y && pts[j].X == x)
                        {
                            Console.Write("><");
                            match = true;
                        }
                    }
                    if (!match)
                        Console.Write(i.Morph[x, y] ? "##" : "  ");
                }
                Console.WriteLine();
            }
        }

        private void SavePolarized(Island island, string tag)
        {
            bool[,] b = island.Morph;
            Bitmap img = new Bitmap(b.GetLength(0), b.GetLength(1));
            for (int x = 0; x < img.Width; x++)
                for (int y = 0; y < img.Height; y++)
                {
                    Color c = b[x, y] ? Color.White : Color.Black;
                    img.SetPixel(x, y, c);
                }
            img.Save(@"C:\workspace\CSharp\Playground\NumberReaderTests\out\"+tag +" "+island.Loc+@".bmp");
        }
    }
}
