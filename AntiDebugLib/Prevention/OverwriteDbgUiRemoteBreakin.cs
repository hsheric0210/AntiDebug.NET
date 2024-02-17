using System.Diagnostics;

using static AntiDebugLib.Native.Kernel32;

namespace AntiDebugLib.Prevention
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// AntiCrack-DotNet :: https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiDebug.cs#L150
    /// </item>
    /// <item>
    /// MinigamesAntiCheat :: https://github.com/AdvDebug/MinegamesAntiCheat/blob/60bc0894981cb531b8de4a085876e3503e9f79f0/MinegamesAntiCheat/MinegamesAntiCheat/AntiDebugging.cs#L75
    /// </item>
    /// </list>
    /// </summary>
    public class OverwriteDbgUiRemoteBreakin : CheckBase
    {
        public override string Name => "Neutralize ntdll!DbgUiRemoteBreakin";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        public override bool PreventPassive()
        {
            var ntdll = GetModuleHandleA("ntdll.dll");
            var proc = GetProcAddress(ntdll, "DbgUiRemoteBreakin");
            var instr = new byte[] { 0xCC }; // INT3
            return WriteProcessMemory(Process.GetCurrentProcess().SafeHandle, proc, instr, 1, 0);
        }
    }
}
