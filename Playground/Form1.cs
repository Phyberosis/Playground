using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Playground
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs eArgs)
        {
            Bitmap img = new Bitmap("C:\\Users\\phybe\\Documents\\Blender\\out\\flare.png");
            Bitmap outImg = new Bitmap(img);

            float maxLSq = (new Vector3(255f, 255f, 255f)).LengthSquared();
            for(int x = 0; x<img.Width; x++)
            {
                for(int y = 0; y<img.Height; y++)
                {
                    Color pix = img.GetPixel(x, y);
                    Vector3 v = new Vector3(pix.R, pix.G, pix.B);
                    float dist = v.LengthSquared();

                    int al = (int) Math.Round(dist / maxLSq * 255);

                    Color pOut = Color.FromArgb(al, pix.R, pix.G, pix.B);

                    outImg.SetPixel(x, y, pOut);
                }
            }

            outImg.Save("C:\\Users\\phybe\\Documents\\Blender\\out\\flareOut.png", ImageFormat.Png);

            //int counter = 0;
            //int sum = 0;
            //int start = 1, end = 11;
            //double n = 14d;
            //double k = n + 1d;
            //double ans = 0.0001d * Math.Pow(3d, n - 1) * n;
            //double ovr = 0.0001d * Math.Pow(3d, k - 1) * k;

            //Console.WriteLine(ans + " " + ovr);
            //Console.WriteLine(24 * 60 * 60 * 36500d);

            //for(int a=start; a<end; a++)
            //{
            //    //Console.WriteLine(a+"0%");
            //    for(int b=start; b<end; b++)
            //    {
            //        //Console.WriteLine("###" + b);
            //        for(int c=start; c<end; c++)
            //        {
            //            for(int d=start; d<end; d++)
            //            {
            //                for(int e=start; e<end; e++)
            //                {
            //                    //Console.WriteLine(a + "" + b + "" + c + "" + d + "" + e);
            //                    sum++;
            //                    if(a<b && b<c && c<d && d<e)
            //                    {
            //                        counter++;
            //                        //Console.WriteLine(a + "" + b + "" + c + "" + d + "" + e);
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            //Console.WriteLine(counter + " / " + sum);
        }

        private struct RGB
        {
            private byte _r;
            private byte _g;
            private byte _b;

            public RGB(byte r, byte g, byte b)
            {
                this._r = r;
                this._g = g;
                this._b = b;
            }

            public byte R
            {
                get { return this._r; }
                set { this._r = value; }
            }

            public byte G
            {
                get { return this._g; }
                set { this._g = value; }
            }

            public byte B
            {
                get { return this._b; }
                set { this._b = value; }
            }

            public bool Equals(RGB rgb)
            {
                return (this.R == rgb.R) && (this.G == rgb.G) && (this.B == rgb.B);
            }
        }

        private struct HSL
        {
            private int _h;
            private float _s;
            private float _l;

            public HSL(int h, float s, float l)
            {
                this._h = h;
                this._s = s;
                this._l = l;
            }

            public int H
            {
                get { return this._h; }
                set { this._h = value; }
            }

            public float S
            {
                get { return this._s; }
                set { this._s = value; }
            }

            public float L
            {
                get { return this._l; }
                set { this._l = value; }
            }

            public bool Equals(HSL hsl)
            {
                return (this.H == hsl.H) && (this.S == hsl.S) && (this.L == hsl.L);
            }
        }

        private HSL RGBToHSL(RGB rgb)
        {
            HSL hsl = new HSL();

            float r = (rgb.R / 255.0f);
            float g = (rgb.G / 255.0f);
            float b = (rgb.B / 255.0f);

            float min = Math.Min(Math.Min(r, g), b);
            float max = Math.Max(Math.Max(r, g), b);
            float delta = max - min;

            hsl.L = (max + min) / 2;

            if (delta == 0)
            {
                hsl.H = 0;
                hsl.S = 0.0f;
            }
            else
            {
                hsl.S = (hsl.L <= 0.5) ? (delta / (max + min)) : (delta / (2 - max - min));

                float hue;

                if (r == max)
                {
                    hue = ((g - b) / 6) / delta;
                }
                else if (g == max)
                {
                    hue = (1.0f / 3) + ((b - r) / 6) / delta;
                }
                else
                {
                    hue = (2.0f / 3) + ((r - g) / 6) / delta;
                }

                if (hue < 0)
                    hue += 1;
                if (hue > 1)
                    hue -= 1;

                hsl.H = (int)(hue * 360);
            }

            return hsl;
        }
    }
}
