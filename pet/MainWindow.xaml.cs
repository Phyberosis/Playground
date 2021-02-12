using System;
using System.Windows;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using InputHook;
using System.Collections.Generic;

namespace pet
{

    public partial class 精灵传说 : Window
    {
        public static double SW = 0;
        public static double SH = 0;
        public static IntPtr gh;
        public struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        public 精灵传说()
        {

            SW = System.Windows.SystemParameters.PrimaryScreenWidth;
            SH = System.Windows.SystemParameters.PrimaryScreenHeight;

            InitializeComponent();

            Process[] processes = Process.GetProcesses();
            var myid = Process.GetCurrentProcess().Id;
            var gid = 0;
            foreach (Process p in processes)
            {
                if (p.Id == myid) continue;
                if (p.ProcessName.Equals("pet"))
                {
                    gid = p.Id;
                    gh = p.Handle;
                }
            }

            var name = "";
            var title = "";
            this.Loaded += (object sender, RoutedEventArgs args) =>
            {
                title = this.Title;
                name = title;
            };
            name += $"|{gid}";


            string mm = "";
            List<string> kds = new List<string>();

            Action update = () =>
            {
                Dispatcher.Invoke(() =>
                {
                    string ka = "";
                    bool first = true;
                    foreach(string k in kds)
                    {
                        if (!first)
                            ka += ", ";
                        ka += k;
                        first = false;
                    }
                    this.Title = $"{title} {mm}  -  {ka}";
                });
            };

            Hook hook = Hook.I();
            hook.AddKeyHook((key) =>
            {
                kds.Add(key.ToString());
                update();
            }, (key) =>
            {
                while (kds.Remove(key.ToString())) { };
                update();
            });

            hook.AddMouseHook((act, x, y) =>
            {
                mm = $"{act} {x}, {y}";
                update();
            });



            //Robot ko = new Robot();
            //VirtualMouse mo = new VirtualMouse();

            //var name = "";
            //var title = "";
            //this.Loaded += (object sender, RoutedEventArgs args) =>
            //{
            //    title = this.Title;
            //    name = title;
            //};
            //name += $"|{gid}";

            //this.KeyDown += (object sender, KeyEventArgs args) =>
            //{
            //    Dispatcher.Invoke(() =>
            //    {
            //        this.Title = name + " - " + args.Key.ToString();
            //    });
            //};

            //this.MouseDown += (object sender, MouseButtonEventArgs args) =>
            //{
            //    var b = args.ChangedButton;
            //    var p = args.GetPosition(this);
            //    p.X += this.Left;
            //    p.Y += this.Top;
            //    Dispatcher.Invoke(() =>
            //    {
            //        this.Title = $"{name} - ({p.X}, {p.Y}) {b}";
            //    });
            //};

            Action<Bitmap> setbg = (Bitmap b) =>
            {
                Dispatcher.Invoke(() =>
                {
                    var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(b.GetHbitmap(),
                                                                      IntPtr.Zero,
                                                                      Int32Rect.Empty,
                                                                      BitmapSizeOptions.FromEmptyOptions()
                    );
                    b.Dispose();
                    var brush = new ImageBrush(bitmapSource);
                    this.Background = brush;
                });
            };

            //Thread t = new Thread(new ThreadStart(() => {
            //    while (true)
            //    {
            //        setbg(snip());

            //        try
            //        {
            //            Thread.Sleep(30);
            //        }
            //        catch (ThreadInterruptedException e)
            //        {
            //            break;
            //        }
            //    }
            //}));
        }

        public static Bitmap snip()
        {
            Rect r = new Rect();
            if (!GetWindowRect(gh, ref r)) return new Bitmap(0,0);

            //System.Drawing.Point loc = new System.Drawing.Point();
            //System.Drawing.Size s = new System.Drawing.Size();
            var img = new Bitmap((int)SW, (int)SH);
            Graphics g = Graphics.FromImage(img);
            g.CopyFromScreen(r.Left, r.Top, 0, 0, new System.Drawing.Size(r.Right - r.Left, r.Bottom - r.Top));

            return img;
        }
    }
}
