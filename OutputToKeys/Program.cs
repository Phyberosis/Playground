using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceOutput
{
    class Program
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);

        [DllImport("user32.dll")] private static extern int GetWindowText(IntPtr hWnd, StringBuilder title, int size);


        // send input
        [DllImport("user32.dll")]
        internal static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

        internal struct INPUT
        {
            public UInt32 Type;
            public MOUSEKEYBDHARDWAREINPUT Data;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct MOUSEKEYBDHARDWAREINPUT
        {
            [FieldOffset(0)]
            public MOUSEINPUT Mouse;
        }

        internal struct MOUSEINPUT
        {
            public Int32 X;
            public Int32 Y;
            public UInt32 MouseData;
            public UInt32 Flags;
            public UInt32 Time;
            public IntPtr ExtraInfo;

        }
        // ///

        static void Main(string[] args)
        {
            const int WM_SYSKEYDOWN = 0x100;
            const int VK_SPACE = 0x30;
            const int WM_UP = 0x101;

            Thread.Sleep(1000);
            Console.WriteLine("s");
            Thread.Sleep(1000);
            Console.WriteLine("s");
            Thread.Sleep(1000);
            Console.WriteLine("s");

            //Process[] notepads = Process.GetProcessesByName("notepad");
            //if (notepads.Length == 0) return;
            //if (notepads[0] != null)
            //{

            //}

            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(1000);

                IntPtr parent = GetForegroundWindow();
                //IntPtr child = FindWindowEx(parent, new IntPtr(0), "Edit", null);
                StringBuilder title = new StringBuilder(256);
                GetWindowText(parent, title, 256);
                Console.WriteLine(title);
                //SendMessage(child, 0x000C, 0, "est");
                //PostMessage(parent, WM_SYSKEYDOWN, VK_SPACE, 0);
                PostMessage(parent, WM_UP, VK_SPACE, 0);
            }

            Console.WriteLine("d");
            Console.ReadKey();
        }
    }
}
