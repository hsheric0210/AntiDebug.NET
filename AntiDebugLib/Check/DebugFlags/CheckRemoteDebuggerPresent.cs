using System.Diagnostics;
using static AntiDebugLib.NativeCalls;

namespace AntiDebugLib.Check.DebugFlags
{
    /// <summary>
    /// Use <c>kernel32!CheckRemoteDebuggerPresent</c> to detect remote debugger presence.
    /// https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_DebugFlags.cpp#L11
    /// </summary>
    public class CheckRemoteDebuggerPresent : CheckBase
    {
        public override string Name => "kernel32!CheckRemoteDebuggerPresent()";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        public override bool CheckActive()
            => CheckRemoteDebuggerPresent(Process.GetCurrentProcess().SafeHandle, out var debuggerPresent) && debuggerPresent;
    }
}
