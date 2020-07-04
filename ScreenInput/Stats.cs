using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenInput
{
    class ImageStats
    {
        long[,] sumR, sumG, sumB;
        long[,] sumRs, sumGs, sumBs;

        public ImageStats(ref Bitmap img)
        {
            sumR = new long[img.Width, img.Height];
            sumG = new long[img.Width, img.Height];
            sumB = new long[img.Width, img.Height];
            sumRs = new long[img.Width, img.Height];
            sumGs = new long[img.Width, img.Height];
            sumBs = new long[img.Width, img.Height];

            for (int x = 0; x < img.Width; x++)
            {
                long sR = 0, sG = 0, sB = 0;
                long qR = 0, qG = 0, qB = 0;

                for (int y = 0; y < img.Height; y++)
                {
                    Color pix = img.GetPixel(x, y);
                    sR += pix.R;
                    sG += pix.G;
                    sB += pix.B;

                    qR += pix.R * pix.R;
                    qG += pix.G * pix.G;
                    qB += pix.B * pix.B;

                    long comR, comG, comB;
                    long comSR, comSG, comSB;

                    comR = x == 0 ? sR : sR + sumR[x - 1, y];
                    comG = x == 0 ? sG : sG + sumG[x - 1, y];
                    comB = x == 0 ? sB : sB + sumB[x - 1, y];

                    comSR = x == 0 ? qR : qR + sumRs[x - 1, y];
                    comSG = x == 0 ? qG : qG + sumGs[x - 1, y];
                    comSB = x == 0 ? qB : qB + sumBs[x - 1, y];

                    sumR[x, y] = comR;
                    sumG[x, y] = comG;
                    sumB[x, y] = comB;

                    sumRs[x, y] = comSR;
                    sumGs[x, y] = comSG;
                    sumBs[x, y] = comSB;
                }
            }
        }

        private void enforceBounds(ref Point p)
        {
            p.X = p.X < 0 ? 0 : p.X >= sumR.GetLength(0) ? sumR.GetLength(0) - 1 : p.X;
            p.Y = p.Y < 0 ? 0 : p.Y >= sumR.GetLength(1) ? sumR.GetLength(1) - 1 : p.Y;
        }

        private long getSum(char channel, Point ul, Point lr)
        {
            long[,] sum;
            switch (channel)
            {
                case 'r':
                    sum = sumR;
                    break;
                case 'g':
                    sum = sumG;
                    break;
                case 'b':
                    sum = sumB;
                    break;
                case 'R':
                    sum = sumRs;
                    break;
                case 'G':
                    sum = sumGs;
                    break;
                case 'B':
                    sum = sumBs;
                    break;
                default:
                    return -1;
            }

            int ux = ul.X;
            int uy = ul.Y;
            int lx = lr.X;
            int ly = lr.Y;

            int ulc = 1; // upper left corner counts
            long ret = sum[lr.X, lr.Y];

            if (ux != 0 || uy != 0)
            {
                if (ux != 0)
                {
                    ret -= sum[ux - 1, ly];
                    ulc--;
                }
                if (uy != 0)
                {
                    ret -= sum[lx, uy - 1];
                    ulc--;
                }
                if (ulc == -1)
                {
                    ret += sum[ux - 1, uy - 1];
                }
            }

            // std::cout << sumRed[lr.X, lr.Y] << " " << channel << " " << lr.X << lr.Y << " gs\n";
            return ret;
        }

        private long getSumSq(char channel, Point ul, Point lr)
        {
            return getSum((char)(channel - 32), ul, lr);
        }

        private long rectArea(Point ul, Point lr)
        {
            return (lr.X - ul.X + 1) * (lr.Y - ul.Y + 1);
        }

        public long getScore(Point ul, Point lr)
        {
            enforceBounds(ref ul);
            enforceBounds(ref lr);

            // std::cout << "stats::getScore\n";
            char[] ch = { 'r', 'g', 'b' };
            long sumSq = 0;
            double sum = 0;
            double a = rectArea(ul, lr);
            long ret = 0;
            foreach (char c in ch)
            {
                sumSq = getSumSq(c, ul, lr);
                sum = getSum(c, ul, lr);

                ret += sumSq - (long)((sum * sum) / a);

                // std::cout << sum << " " << sumSq << " @" << c << " "; 
            }
            // std::cout << "\n" << ret << "\n";
            // std::cout << (sum * sum) << " " << a << " " << ret << "\n";
            // std::cout << "stats::getScore end\n";
            return ret;
        }

        public Color getAvg(Point ul, Point lr)
        {
            enforceBounds(ref ul);
            enforceBounds(ref lr);

            int ar = (int)rectArea(ul, lr);
            int r = (int)(getSum('r', ul, lr) / ar);
            int g = (int)(getSum('g', ul, lr) / ar);
            int b = (int)(getSum('b', ul, lr) / ar);
            // if (ul.first == lr.first && ul.second == lr.second)
            //     cout<<"####" << r <<"####";
            Color p = Color.FromArgb(255, r, g, b);
            return p;
        }
    }
}
