using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace MapStudio.UI
{
    public class ConsoleWindowUtil
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        public static void Hide()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var handle = GetConsoleWindow();
                // Hide
                ShowWindow(handle, SW_HIDE);
            }
        }

        public static void Show()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var handle = GetConsoleWindow();
                // Show
                ShowWindow(handle, SW_SHOW);
            }
        }
    }
}
