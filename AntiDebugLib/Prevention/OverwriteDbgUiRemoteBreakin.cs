using AntiDebugLib.Native;
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
    public class OverwriteDbgUiRemoteBreakin : FunctionOverwrite
    {
        public override string Name => "Neutralize ntdll!DbgUiRemoteBreakin";

        public override PreventionResult PreventPassive()
        {
            var ntdll = GetModuleHandleA("ntdll.dll");
            var proc = GetProcAddress(ntdll, "DbgUiRemoteBreakin");
            Logger.Debug("DbgUiRemoteBreakin address is {address}.", proc.ToHex());
            return OverwriteFunction(proc, new byte[] { 0xCC }); // INT3
        }
    }
}
