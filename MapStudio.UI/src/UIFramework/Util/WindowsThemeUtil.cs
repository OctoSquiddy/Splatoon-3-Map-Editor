using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;

namespace MapStudio.UI
{
    internal class WindowsThemeUtil
    {
        #region WINAPI
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);


        [DllImport("dwmapi.dll", SetLastError = true)]
        private static extern bool DwmSetWindowAttribute(IntPtr handle, int param, in int value, int size);

        [DllImport("dwmapi.dll", SetLastError = true)]
        private static extern bool DwmGetWindowAttribute(IntPtr handle, int param, out int value, int size);


        [DllImport("uxtheme.dll", SetLastError = true)]
        private static extern bool SetWindowTheme(IntPtr handle, string? subAppName, string? subIDList);
        #endregion

        public static void Init(IntPtr handle)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 10) //only works on windows 10+
            {
                //Support dark mode on Windows
                //ported from https://github.com/libsdl-org/SDL/issues/4776#issuecomment-926976455

                static void ToggleDarkmode(IntPtr handle, bool value)
                {
                    if (!DwmSetWindowAttribute(handle, 20, value ? 1 : 0, sizeof(int)))
                        DwmSetWindowAttribute(handle, 19, value ? 1 : 0, sizeof(int));
                }

                SetWindowTheme(handle, "DarkMode_Explorer", null);


                //"bind" darkmode to console window
                var consoleHandle = FindWindow(null, Console.Title);

                bool isDarkMode = false;

                var windowHandle = handle;

                void CheckDarkmode()
                {
                    int value = 0;

                    if (!DwmGetWindowAttribute(consoleHandle, 20, out value, sizeof(int)))
                        DwmGetWindowAttribute(consoleHandle, 19, out value, sizeof(int));

                    bool shouldBeDarkMode = value == 1;

                    if (isDarkMode != shouldBeDarkMode)
                    {
                        ToggleDarkmode(windowHandle, shouldBeDarkMode);
                        isDarkMode = shouldBeDarkMode;
                    }
                }

                System.Timers.Timer timer = new System.Timers.Timer(1000);
                timer.Elapsed += (o, e) => CheckDarkmode();

                timer.Start();
            }
        }
    }
}
