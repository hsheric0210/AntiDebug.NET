using AntiDebugLib.Native;
using System.Diagnostics;

using static AntiDebugLib.Native.Kernel32;

namespace AntiDebugLib.Prevention
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// MinigamesAntiCheat :: https://github.com/AdvDebug/MinegamesAntiCheat/blob/60bc0894981cb531b8de4a085876e3503e9f79f0/MinegamesAntiCheat/MinegamesAntiCheat/AntiDebugging.cs#L75
    /// </item>
    /// </list>
    /// </summary>
    public class OverwriteDbgUiConnectToDbg : FunctionOverwrite
    {
        public override string Name => "Neutralize ntdll!DbgUiConnectToDbg";

        public override PreventionResult PreventPassive()
        {
            var ntdll = GetModuleHandleA("ntdll.dll");
            var proc = GetProcAddress(ntdll, "DbgUiConnectToDbg");
            Logger.Debug("DbgUiConnectToDbg address is {address}.", proc.ToHex());
            return OverwriteFunction(proc, new byte[] { 0xCC }); // INT3 // TODO: replace with program instant crash instruction
        }
    }
}
