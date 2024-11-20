using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;

namespace MapStudio.UI
{
    public class WebUtil
    {
        public static void OpenDonation() {
            OpenURLEncoded("aHR0cHM6Ly93d3cucGF5cGFsLmNvbS9kb25hdGUvP2J1c2luZXNzPVlQNUtRTUs1Q1VOVTYmY3VycmVuY3lfY29kZT1VU0Q=");
        }

        public static void OpenURLEncoded(string encodedString)
        {
            byte[] data = Convert.FromBase64String(encodedString);
            OpenURL(Encoding.UTF8.GetString(data));
        }

        public static void OpenURL(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true }); // Works ok on windows
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                Process.Start("xdg-open", url);  // Works ok on linux
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                Process.Start("open", url); // Not tested
            }
        }   
    }
}
