using System.Diagnostics;

namespace SirHurtCleaner
{
    public static class Cleaner
    {
        private static List<string> _cmds = new List<string>
        {
            "/c del /Q %systemdrive%\\Users\\%username%\\AppData\\LocalLow\\rbxcsettings.rbx",
            "/c del /Q %systemdrive%\\Users\\%username%\\AppData\\Local\\Roblox\\GlobalBasicSettings_13.xml",
            "/c del /Q %systemdrive%\\Users\\%username%\\AppData\\Local\\Roblox\\RobloxCookies.dat",
            "/c del /Q %systemdrive%\\Users\\%username%\\AppData\\Local\\Roblox\\frm.cfg",
            "/c del /Q %systemdrive%\\Users\\%username%\\AppData\\Local\\Roblox\\AnalysticsSettings.xml",
            "/c del /Q %systemdrive%\\Users\\%username%\\AppData\\Local\\Roblox\\LocalStorage\\*",
            "/c del /S /Q %systemdrive%\\Users\\%username%\\AppData\\Local\\Roblox\\logs\\*",
            "/c del /Q %temp%\\RBX-*.log",
            "/c del /S /Q %systemdrive%\\Windows\\Temp\\*",
            "/c del /S /Q %systemdrive%\\Users\\%username%\\AppData\\Local\\Microsoft\\CLR_v4.0_32\\UsageLogs\\*",
            "/c del /S /Q %systemdrive%\\Users\\%username%\\AppData\\Local\\Microsoft\\CLR_v4.0\\UsageLogs\\*"
        };

        public static void Main(String[] args)
        {
            try
            {
                Console.WriteLine($"[LOG][{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}] Cleaning started");

                foreach (string cmd in _cmds)
                {
                    ProcessStartInfo processStartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        Arguments = cmd
                    };

                    using (Process process = Process.Start(processStartInfo))
                    {
                        Console.WriteLine($"[LOG][{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}] Cleaning argument: {processStartInfo.Arguments}");
                        process.WaitForExit();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EXCEPTION][{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}] Cleaning exception: {ex.Message}");
            }
            finally
            {
                Console.WriteLine($"[LOG][{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}] Cleaning ended");
                Console.ReadKey();
            }
        }
    }
}