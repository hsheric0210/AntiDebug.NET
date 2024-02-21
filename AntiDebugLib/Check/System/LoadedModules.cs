using System;

using static AntiDebugLib.Native.Kernel32;

namespace AntiDebugLib.Check
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// AntiCrack-DotNet (Sandboxie) :: https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiVirtualization.cs#L28
    /// </item>
    /// <item>
    /// AntiCrack-DotNet (Comodo) :: https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiVirtualization.cs#L35
    /// </item>
    /// <item>
    /// AntiCrack-DotNet (Qihoo360) :: https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiVirtualization.cs#L42
    /// </item>
    /// <item>
    /// AntiCrack-DotNet (cuckoo) :: https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiVirtualization.cs#L49
    /// </item>
    /// <item>
    /// AntiCrack-DotNet (Wine) :: https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiVirtualization.cs#L68
    /// </item>
    /// <item>
    /// al-khaser :: https://github.com/LordNoteworthy/al-khaser/blob/master/al-khaser/AntiVM/Generic.cpp
    /// </item>
    /// </list>
    /// </summary>
    internal class LoadedModules : CheckBase
    {
        public override string Name => "Loaded modules";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        private readonly string[] moduleNames = new string[]
        {
            "SbieDll.dll",      // Sandboxie
            "cmdvrt32.dll",     // Comodo Sandbox
            "cmdvrt64.dll",     // Comodo Sandbox
            "SxIn.dll",         // Qihoo 360 Sandbox
            "cuckoomon.dll",    // Cuckoo Sandbox
            "avghookx.dll",     // AVG
            "avghooka.dll",     // AVG
            "snxhk.dll",        // Avast
            "dbghelp.dll",      // WindBG
            "api_log.dll",      // iDefense Lab
            "dir_watch.dll",    // iDefense Lab
            "pstorec.dll",      // SunBelt Sandbox
            "vmcheck.dll",      // Virtual PC
            "wpespy.dll",       // WPE Pro
        };

        public override CheckResult CheckActive()
        {
            foreach (var name in moduleNames)
            {
                if (DInvoke.GetModuleHandle(name) != IntPtr.Zero)
                {
                    Logger.Information("Bad module {name} is currently loaded to this process.", name);
                    return DebuggerDetected(new { Name = name });
                }
            }

            if (DInvoke.GetProcAddress(DInvoke.GetModuleHandle("kernel32.dll"), "wine_get_unix_file_name") != IntPtr.Zero)
            {
                Logger.Information("Wine export is detected.");
                return DebuggerDetected(new { Name = "wine" });
            }

            return DebuggerNotDetected();
        }
    }
}
