using System.Diagnostics;
using static AntiDebugLib.NativeCalls;

namespace AntiDebugLib.Prevention
{
    /// <summary>
    /// Prevent from being debugged by patching <c>ntdll!DbgBreakPoint</c>
    /// https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiDebug.cs#L150
    /// </summary>
    public class AntiDbgBreakPoint : CheckBase
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
