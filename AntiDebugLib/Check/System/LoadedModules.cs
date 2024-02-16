using System;
using static AntiDebugLib.NativeCalls;

namespace AntiDebugLib.Check
{
    /// <summary>
    /// Detect some sandbox DLLs  loaded in the current process.
    /// https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiVirtualization.cs
    /// https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiVirtualization.cs#L68
    /// </summary>
    internal class LoadedModules : CheckBase
    {
        public override string Name => "Loaded modules";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        private readonly string[] moduleNames = new string[]
        {
            "SbieDll.dll", // Sandboxie
            "cmdvrt32.dll", // Comodo Sandbox
            "cmdvrt64.dll", // Comodo Sandbox
            "SxIn.dll", // Qihoo 360 Sandbox
            "cuckoomon.dll", // Cuckoo Sandbox
        };

        public override bool CheckActive()
        {
            foreach (var name in moduleNames)
            {
                if (MyGetModuleHandle(name) != IntPtr.Zero)
                {
                    Logger.Information("Bad module {name} found.", name);
                    return true;
                }
            }

            if (MyGetProcAddress(MyGetModuleHandle("kernel32.dll"), "wine_get_unix_file_name") != IntPtr.Zero)
            {
                Logger.Information("Detected wine.");
                return true;
            }

            return false;
        }
    }
}
