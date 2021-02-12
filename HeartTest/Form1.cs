using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using MouseSimulator;
using ScreenInput;

namespace HeartTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Point ml = new Point();

        private void Form1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            textBox2.Text = "60";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Task.Delay(0).ContinueWith((t) =>
            {
                int y = 0, w = 500;
                Int32.TryParse(textBox1.Text, out y);
                ScreenProcessor sp = new ScreenProcessor(0, y, w, 500);
                Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    Bitmap b = sp.Get();
                    for(int x = 0; x < w; x++)
                    {
                        b.SetPixel(x, 100, Color.Red);
                        b.SetPixel(x, 101, Color.Red);
                        b.SetPixel(x, 102, Color.Red);
                    }

                    pictureBox1.Image = sp.Get();
                });
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Task.Delay(2000).ContinueWith((t) =>
            {
                var p = Cursor.Position;
                ml = p;
                Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    label1.Text = p.ToString();
                });
            });

        }

        private void button3_Click(object sender, EventArgs e)
        {
            label2.Text = "...";

            Task.Delay(0).ContinueWith((t) =>
            {
                VirtualMouse vm = new VirtualMouse();
                int kk = 0;
                Int32.TryParse(textBox2.Text, out kk);
                for (int i = 0; i< 5; i++)
                {
                    label2.Text = i.ToString();

                    int y = 0, w = 500, h = 400;
                    Int32.TryParse(textBox1.Text, out y);
                    ScreenProcessor sp = new ScreenProcessor(0, y, w, 400);
                    
                    Bitmap b = sp.Get();
                    for(int yy = 100; yy < 102; yy++)
                        for (int x = 0; x < w; x++)
                        {
                            label2.Text = i.ToString() + ">>" + x.ToString();
                            if (b.GetPixel(x, yy).R < kk)
                            {
                                b.SetPixel(x, yy, Color.Red);
                            }
                        }
                    pictureBox1.Image = b;

                    vm.LClick(ml.X, ml.Y);
                    vm.LClick(ml.X, ml.Y);
                    Thread.Sleep(250);
                }

                label2.Text = "cool beans";
            });

        }
    }
}
