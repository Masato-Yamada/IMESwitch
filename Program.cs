using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

namespace IMESwitcher
{
    class HiddenForm : Form
    {
        public HiddenForm()
        {
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Opacity = 0; // completely transparent
        }
    }

    class Program
    {

        // Win32 API
        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern IntPtr GetKeyboardLayout(uint idThread);

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        static extern short GetKeyState(int nVirtKey);

        const int SW_HIDE = 0;

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const uint KEYEVENTF_KEYUP = 0x0002;

        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        static void Main()
        {

            // for debug
            //DumpKeyboardLayouts();

            // hide console window
            IntPtr handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);

            // create tray icon
            NotifyIcon tray = new NotifyIcon();
            tray.Icon = SystemIcons.Application;   // change image if you want
            tray.Text = IMESwitch.Properties.Resources.StatusRunning;
            tray.Visible = true;

            // create menu
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Items.Add(IMESwitch.Properties.Resources.MenuStop, null, (s, e) =>
            {
                tray.Visible = false;
                Application.Exit();
            });

            tray.ContextMenuStrip = menu;

            // start keyboard hook
            _hookID = SetHook(_proc);

            // loop for background process
            System.Windows.Forms.Application.Run(new HiddenForm());
            // end process
            UnhookWindowsHookEx(_hookID);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    IntPtr.Zero, 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);

                // for debug temp code
                //Console.WriteLine($"Key: 0x{vkCode:X}");

                if (vkCode == 0x20) // when Space pushed
                {
                    // check if left ctrl is pushed
                    bool leftCtrlDown = (GetKeyState(0xA2) & 0x8000) != 0; // VK_LCONTROL = 0xA2
                    if (leftCtrlDown)
                    {
                        keybd_event(0x5B, 0, 0, 0);
                        keybd_event(0x20, 0, 0, 0);
                        keybd_event(0x20, 0, KEYEVENTF_KEYUP, 0);
                        keybd_event(0x5B, 0, KEYEVENTF_KEYUP, 0);
                        return (IntPtr)1;
                    }
                }
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }



        // for debug start
        //[DllImport("user32.dll")]
        //static extern uint GetKeyboardLayoutList(int nBuff, IntPtr[] lpList);

        //static void DumpKeyboardLayouts()
        //{
        //    IntPtr[] list = new IntPtr[16];
        //    int count = (int)GetKeyboardLayoutList(list.Length, list);

        //    Console.WriteLine("Installed keyboard layouts:");
        //    for (int i = 0; i < count; i++)
        //    {
        //        Console.WriteLine($"HKL: 0x{list[i].ToInt64():X}");
        //    }
        //}
        // for debug end

    }
}