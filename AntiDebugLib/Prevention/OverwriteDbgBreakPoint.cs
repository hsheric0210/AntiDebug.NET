using AntiDebugLib.Native;
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
    public class OverwriteDbgBreakPoint : FunctionOverwrite
    {
        public override string Name => "Neutralize ntdll!DbgBreakPoint";

        public override PreventionResult PreventPassive()
        {
            var ntdll = GetModuleHandleA("ntdll.dll");
            var proc = GetProcAddress(ntdll, "DbgBreakPoint");
            Logger.Debug("DbgBreakPoint address is {address}.", proc.ToHex());
            return OverwriteFunction(proc, new byte[] { 0xC3 }); // RET
        }
    }
}
