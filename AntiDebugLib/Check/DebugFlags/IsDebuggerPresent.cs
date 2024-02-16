using static AntiDebugLib.NativeCalls;

namespace AntiDebugLib.Check.DebugFlags
{
    /// <summary>
    /// Use <c>kernel32!IsDebuggerPresent</c> to detect debugger presence.
    /// https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiDebug.cs#L110
    /// </summary>
    public class IsDebuggerPresent : CheckBase
    {
        public override string Name => "kernel32!IsDebuggerPresent()";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        public override bool CheckActive() => IsDebuggerPresent();
    }
}
