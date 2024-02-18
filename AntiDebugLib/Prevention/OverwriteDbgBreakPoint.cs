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
    /// The "Ultimate" Anti-Debugging Reference (by Peter Ferrie) :: 7.D.iv. DbgBreakPoint
    /// </item>
    /// </list>
    /// </summary>
    public class OverwriteDbgBreakPoint : CheckBase
    {
        public override string Name => "Neutralize ntdll!DbgBreakPoint";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        public override bool PreventPassive()
        {
            var ntdll = GetModuleHandleA("ntdll.dll");
            var proc = GetProcAddress(ntdll, "DbgBreakPoint");
            var instr = new byte[] { 0xC3 }; // RET
            return WriteProcessMemory(Process.GetCurrentProcess().SafeHandle, proc, instr, 1, 0);
        }
    }
}
