using ScreenInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberReader
{
    internal class GInnerIslands : IGuesser
    {
        public char GuessNumber(ref Island orig)
        {
            Island island = new Island(orig);
            char g = makeGuess(ref island);
            if (g == ' ')
            {
                island = new Island(orig);
                island.Grow();
                return makeGuess(ref island);
            }
            return g;
        }

        private char makeGuess(ref Island island)
        {
            Point[] d = new Point[0];
            NumberReader.PrintIsland(ref island, ref d);
            
            island.PerimeterFill();
            island.Negate();
            List<Island> inners = island.GetInnerIslands(10);
            if (inners.Count == 0)
            {
                return ' ';
            }
            else if (inners.Count == 1)
            {
                Island inner = inners.First();
                if (inner.Morph.Length > island.Morph.Length / 4) return '0';
                int nearTop = gnInnerNearTop(ref inner, ref island);
                switch (nearTop)
                {
                    case 0: return ' ';
                    case 1: return '9';
                    case -1: return '6';
                }
            }
            else if (inners.Count == 2)
            {
                Island ilA = inners.ElementAt(0);
                Island ilB = inners.ElementAt(1);
                int ha = gnInnerNearTop(ref ilA, ref island);
                int hb = gnInnerNearTop(ref ilB, ref island);
                if((ha == 1 && hb == -1) || (ha == -1 && hb == 1)) return '8';
            }
            return ' ';
        }

        private int gnInnerNearTop(ref Island i, ref Island parent)
        {
            int ph = parent.GetHeight();
            int cy = (i.Loc.Y + i.GetHeight()) / 2;
            int pcy = ph / 2;
            //const float t = 12f;
            float up = cy;// + (ph / t);
            float dn = cy;// - (ph / t);
            return up < pcy ? 1 : dn > pcy ? -1 : 0;
        }

    }
}
