using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MapStudio.UI
{
    public static class Clipboard
    {
        public static void Copy(string val)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                $"echo {val} | clip".Bat();
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                $"echo {val} | clip".Bat();
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                $"echo \"{val}\" | pbcopy".Bash();
            }
        }

        public static void SetText(string text)
        {
            var powershell = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell",
                    Arguments = $"-command \"Set-Clipboard -Value \\\"{text}\\\"\""
                }
            };
            powershell.Start();
            powershell.WaitForExit();
        }

        public static string GetText()
        {
            var powershell = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    RedirectStandardOutput = true,
                    FileName = "powershell",
                    Arguments = "-command \"Get-Clipboard\""
                }
            };

            powershell.Start();
            string text = powershell.StandardOutput.ReadToEnd();
            powershell.StandardOutput.Close();
            powershell.WaitForExit();
            return text.TrimEnd();
        }
    }
}
