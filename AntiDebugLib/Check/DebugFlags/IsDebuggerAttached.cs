using System.Diagnostics;

namespace AntiDebugLib.Check.DebugFlags
{
    /// <summary>
    /// Use <c>System.Diagnostics.Debugger.IsAttached</c> to detect debugger presence.
    /// https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiDebug.cs#L105
    /// </summary>
    public class IsDebuggerAttached : CheckBase
    {
        public override string Name => "IsDebuggerAttached";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        public override bool CheckActive() => Debugger.IsAttached;
    }
}
