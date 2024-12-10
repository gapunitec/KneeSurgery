# KneeSurgery
## UI preview
![UI](https://i.imghippo.com/files/ovkm1671OI.PNG)
![UI1](https://i.imghippo.com/files/BFb1432CeM.PNG)
## Dll configuration
- Download KneeSurgeryDll_X.X.X.X.zip from the [releases](https://github.com/gapunitec/KneeSurgery/releases/) page
- Create or open your solution in Visual Studio
- Right-click on References
- Left-click on Add reference...
- At the bottom right left-click the Browse... button
- Add KneeSurgeryDll.dll
## Dll usage
- ### int Initialize(string path = null)
```csharp
        //SOURCE

        /// <summary>
        /// Initializes the KneeSurgery environment by creating necessary directories.
        /// </summary>
        /// <param name="path">Optional path to the base directory. Defaults to the Application Data folder if not provided.</param>
        /// <returns>1 if successful, -1 if an error occurs.</returns>
        public static int Initialize(string path = null)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                    path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                path = Path.Combine(path, "KneeSurgery");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string autoexecPath = Path.Combine(path, "Autoexec");

                if (!Directory.Exists(autoexecPath))
                    Directory.CreateDirectory(autoexecPath);

                BootstrapService.BootstrapExtraction();

                string bootstrapperExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bootstrapper.exe");
                ProcessStartInfo bootstrapperStartInfo = new ProcessStartInfo
                {
                    FileName = bootstrapperExePath,
                    UseShellExecute = false
                };

                using (Process bootstrapper = Process.Start(bootstrapperStartInfo))
                {
                    bootstrapper.WaitForExit();
                }

                foreach (Process process in Process.GetProcesses())
                {
                    if (process.ProcessName.Contains("SirHurt V5"))
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                }

                return 1;
            }
            catch
            {
                return -1;
            }
        }


        //USAGE

        int result = KneeSurgeryDll.KneeSurgery.Initialize()
```
- ### int Injection()
```csharp
        //SOURCE

        /// <summary>
        /// Injects the SirHurt executable.
        /// </summary>
        /// <returns>1 if successful, -1 if an error occurs.</returns>
        public static int Injection()
        {
            try
            {
                string sirhurtExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sirhurt.exe");
                ProcessStartInfo sirhurtStartInfo = new ProcessStartInfo
                {
                    FileName = sirhurtExePath,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process sirhurt = Process.Start(sirhurtStartInfo))
                {
                    sirhurt.WaitForExit();

                    return sirhurt.ExitCode == 0 ? 1 : -1;
                }
            }
            catch
            {
                return -1;
            }
        }


        //USAGE

        int result = KneeSurgeryDll.KneeSurgery.Injection()
```
- ### int Autoexec()
```csharp
        //SOURCE

        /// <summary>
        /// Executes all Lua scripts in the Autoexec folder.
        /// </summary>
        /// <returns>1 if successful, -1 if an error occurs.</returns>
        public static int Autoexec()
        {
            try
            {
                string autoexecPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "KneeSurgery", "Autoexec");
                string[] autoexecFiles = Directory.GetFiles(autoexecPath, "*.lua");

                foreach (string autoexecFile in autoexecFiles)
                {
                    if (Execution(File.ReadAllText(autoexecFile)) == -1)
                        throw new Exception();

                    Task.Delay(500).Wait();
                }

                return 1;
            }
            catch
            {
                return -1;
            }
        }


        //USAGE

        int result = KneeSurgeryDll.KneeSurgery.Autoexec()
```
- ### int Startup()
```csharp
        //SOURCE

        /// <summary>
        /// Runs the startup sequence which includes initializing, injecting, and executing autoexec scripts.
        /// </summary>
        /// <returns>1 if successful, -1 if an error occurs.</returns>
        public static int Startup()
        {
            try
            {
                int status = Initialize();

                if (status == 1)
                {
                    status = Injection();

                    if (status == 1)
                    {
                        status = Autoexec();
                    }
                }

                return status;
            }
            catch
            {
                return -1;
            }
        }


        //USAGE

        int result = KneeSurgeryDll.KneeSurgery.Startup()
```
- ### int Execution([MarshalAs(UnmanagedType.LPStr)] string text)
```csharp
        //SOURCE

        /// <summary>
        /// Executes the provided Lua script text by writing it to the SirHurt data file.
        /// </summary>
        /// <param name="text">The Lua script text to be executed.</param>
        /// <returns>1 if successful, -1 if an error occurs.</returns>
        public static int Execution([MarshalAs(UnmanagedType.LPStr)] string text)
        {
            try
            {
                string sirhurtDatPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "sirhurt", "sirhui", "sirhurt.dat");
                string sirhurtDatParentPath = Path.GetDirectoryName(sirhurtDatPath);

                if (!Directory.Exists(sirhurtDatParentPath))
                    Directory.CreateDirectory(sirhurtDatParentPath);

                File.WriteAllText(sirhurtDatPath, KsfService.KsfConverter(text));

                return 1;
            }
            catch
            {
                return -1;
            }
        }


        //USAGE

        string text = "print(\"HelloWorld!\")"
        int result = KneeSurgeryDll.KneeSurgery.Execution(text)
```
- ### int OpenLogsFolder()
```csharp
        //SOURCE

        /// <summary>
        /// Opens the Logs folder in File Explorer.
        /// </summary>
        /// <returns>1 if successful, -1 if an error occurs.</returns>
        public static int OpenLogsFolder()
        {
            try
            {
                string logsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "sirhurt", "sirhui");

                Process.Start("explorer.exe", logsPath);

                return 1;
            }
            catch
            {
                return -1;
            }
        }


        //USAGE

        int result = KneeSurgeryDll.KneeSurgery.OpenLogsFolder()
```
- ### int OpenAutoexecFolder()
```csharp
        //SOURCE

        /// <summary>
        /// Opens the Autoexec folder in File Explorer.
        /// </summary>
        /// <returns>1 if successful, -1 if an error occurs.</returns>
        public static int OpenAutoexecFolder()
        {
            try
            {
                string autoexecPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "KneeSurgery", "Autoexec");

                Process.Start("explorer.exe", autoexecPath);

                return 1;
            }
            catch
            {
                return -1;
            }
        }


        //USAGE

        int result = KneeSurgeryDll.KneeSurgery.OpenAutoexecFolder()
```
- ### int KillRobloxPlayerBeta()
```csharp
        //SOURCE

        /// <summary>
        /// Kills all running instances of RobloxPlayerBeta.
        /// </summary>
        /// <returns>1 if successful, -1 if an error occurs.</returns>
        public static int KillRobloxPlayerBeta()
        {
            try
            {
                Process[] robloxPlayerBetaProcesses = Process.GetProcessesByName("RobloxPlayerBeta");

                foreach (Process robloxPlayerBetaProcess in robloxPlayerBetaProcesses)
                {
                    robloxPlayerBetaProcess.Kill();
                    robloxPlayerBetaProcess.WaitForExit();
                }

                return 1;
            }
            catch
            {
                return -1;
            }
        }


        //USAGE

        int result = KneeSurgeryDll.KneeSurgery.KillRobloxPlayerBeta()
```
- ### int CleanRobloxPlayerBeta()
```csharp
        //SOURCE

        /// <summary>
        /// Clean RobloxPlayerBeta.
        /// </summary>
        /// <returns>1 if successful, -1 if an error occurs.</returns>
        public static int CleanRobloxPlayerBeta()
        {
            try
            {
                List<string> cmds = new List<string>
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

                foreach (string cmd in cmds)
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
                        process.WaitForExit();
                    }
                }

                return 1;
            }
            catch
            {
                return -1;
            }
        }


        //USAGE

        int result = KneeSurgeryDll.KneeSurgery.KillRobloxPlayerBeta()
```
- ### Task<int> GetInjectionState()
```csharp
        //SOURCE

        /// <summary>
        /// Checks the injection state by looking for running processes containing "RobloxPlayerBeta".
        /// </summary>
        /// <returns>
        /// A Task that resolves to:
        /// - 1 if a relevant process is found and has exited,
        /// - 0 if no relevant process is found,
        /// - -1 if an error occurs.
        /// </returns>
        public static Task<int> GetInjectionState()
        {
            try
            {
                foreach (Process process in Process.GetProcesses())
                {
                    if (process.ProcessName.Contains("RobloxPlayerBeta"))
                    {
                        process.WaitForExit();

                        return Task.FromResult(1);
                    }
                }

                return Task.FromResult(0);
            }
            catch
            {
                return Task.FromResult(-1);
            }
        }


        //USAGE

        int result = await Task.Run(() => KneeSurgeryDll.KneeSurgery.GetInjectionState())
```